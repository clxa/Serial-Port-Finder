using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using NUnit.Framework;
using SerialPortDeviceFinder.Core.Models;
using SerialPortDeviceFinder.Core.Validation;
using SerialPortDeviceFinder.WinForms;

namespace SerialPortDeviceFinder.Core.Tests.WinForms
{
    [TestFixture]
    public sealed class DeviceProfileDraftsTests
    {
        [Test]
        public void CreateDefault_UsesAnAvailableNameAndPrefillsAllCatalogCombosWith8Enabled()
        {
            var profile = DeviceProfileDrafts.CreateDefault(new[] { "新设备", "新设备 2" });

            Assert.That(profile.Name, Is.EqualTo("新设备 3"));
            Assert.That(profile.IsEnabled, Is.True);
            Assert.That(profile.PortSettings, Has.Count.EqualTo(DeviceProfileDrafts.CommonSerialPortCombos.Count));
            Assert.That(profile.PortSettings.Count(setting => setting.Enabled), Is.EqualTo(8));
            Assert.That(profile.PortSettings.Where(setting => setting.Enabled)
                    .Select(setting => (setting.BaudRate, setting.Parity, setting.StopBits)),
                Is.EqualTo(new[]
                {
                    (9600, Parity.None, StopBits.One),
                    (19200, Parity.None, StopBits.One),
                    (38400, Parity.None, StopBits.One),
                    (57600, Parity.None, StopBits.One),
                    (115200, Parity.None, StopBits.One),
                    (9600, Parity.Even, StopBits.One),
                    (9600, Parity.Odd, StopBits.One),
                    (115200, Parity.None, StopBits.Two)
                }));
            Assert.That(profile.PortSettings.Where(setting => setting.Enabled).All(setting => setting.DataBits == 8), Is.True);
            Assert.That(profile.PortSettings.All(setting => setting.Handshake == Handshake.None), Is.True);
        }

        [Test]
        public void CommonSerialPortCombos_CatalogIsNonEmptyAndUnique()
        {
            var combos = DeviceProfileDrafts.CommonSerialPortCombos;

            Assert.That(combos.Count, Is.GreaterThan(0));
            Assert.That(combos.Select(option => (option.BaudRate, option.DataBits, option.Parity, option.StopBits)).Distinct().Count(),
                Is.EqualTo(combos.Count));
            Assert.That(combos.Select(option => option.DisplayText).Distinct().Count(),
                Is.EqualTo(combos.Count));
        }

        [Test]
        public void CommonSerialPortCombos_EveryEntryPassesValidatorRules()
        {
            foreach (var option in DeviceProfileDrafts.CommonSerialPortCombos)
            {
                var profile = new DeviceProfile
                {
                    Name = "测试",
                    CommandFormat = PayloadFormat.Text,
                    CommandContent = "AT",
                    TextTerminator = TextTerminator.CrLf,
                    ResponseFormat = PayloadFormat.Text,
                    ExpectedResponse = "OK",
                    MatchMode = ResponseMatchMode.Contains,
                    TextEncoding = TextEncodingKind.Ascii,
                    TimeoutMilliseconds = 800,
                    PortSettings = new List<SerialPortSettings> { option.CreateSettings() }
                };

                var errors = DeviceProfileValidator.Validate(profile);
                Assert.That(errors, Is.Empty, option.DisplayText);
            }
        }

        [Test]
        public void Clone_CreatesAnIndependentCopyWithAnAvailableName()
        {
            var original = DeviceProfileDrafts.CreateDefault(System.Array.Empty<string>());
            original.CommandContent = "AT";
            original.LastScanPortName = "COM9";

            var clone = DeviceProfileDrafts.Clone(original, new[] { original.Name });
            clone.PortSettings[0].BaudRate = 4800;
            clone.PortSettings[3].Enabled = false;

            Assert.That(clone.Name, Is.EqualTo("新设备 2"));
            Assert.That(clone.CommandContent, Is.EqualTo("AT"));
            Assert.That(clone.LastScanPortName, Is.EqualTo("COM9"));
            Assert.That(original.PortSettings[0].BaudRate, Is.EqualTo(1200));
            Assert.That(original.PortSettings[3].Enabled, Is.True);
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void MainForm_CanBeCreatedWithoutLayoutException()
        {
            using (var form = new MainForm())
            {
                Assert.That(form.Text, Is.EqualTo("串口设备搜索器"));
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void DeviceProfileEditor_ReservesEnoughHeightForSerialParameterRows()
        {
            using (var editor = new DeviceProfileEditor())
            {
                Assert.That(editor.MinimumSize.Height, Is.GreaterThanOrEqualTo(430));
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void MainForm_DefaultLayout_KeepsSerialParameterGridTallEnoughToEdit()
        {
            using (var form = new MainForm { Size = new Size(1280, 840) })
            {
                form.Show();
                Application.DoEvents();

                var grid = FindControl<DataGridView>(form, "串口参数表格");

                Assert.That(grid, Is.Not.Null);
                Assert.That(grid!.ClientSize.Height, Is.GreaterThanOrEqualTo(200));

                var log = FindControl<TextBox>(form, "扫描日志");
                Assert.That(log!.ClientSize.Height, Is.GreaterThanOrEqualTo(25));
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void MainForm_MinimumSize_DoesNotThrowOrOverlap()
        {
            using (var form = new MainForm { Size = new Size(1050, 700) })
            {
                Assert.DoesNotThrow(() =>
                {
                    form.Show();
                    Application.DoEvents();

                    var grid = FindControl<DataGridView>(form, "串口参数表格");
                    Assert.That(grid!.ClientSize.Height, Is.GreaterThanOrEqualTo(86));

                    var gridRect = grid.RectangleToScreen(grid.ClientRectangle);
                    var labelRect = FindControl<Label>(form, "所有设备模板校验结果")!.RectangleToScreen(
                        FindControl<Label>(form, "所有设备模板校验结果")!.ClientRectangle);
                    Assert.That(gridRect.IntersectsWith(labelRect), Is.False);
                });
            }
        }

        [Test]
        public void ProfileValidationFeedback_DuplicateNames_ReportsGlobalErrorInsteadOfLocalSuccess()
        {
            var first = DeviceProfileDrafts.CreateDefault(System.Array.Empty<string>());
            first.Name = "重复设备";
            first.CommandContent = "AT";
            first.ExpectedResponse = "OK";
            var second = DeviceProfileDrafts.Clone(first, System.Array.Empty<string>());

            var feedback = ProfileValidationFeedback.Create(new[] { first, second });

            Assert.That(feedback.IsValid, Is.False);
            Assert.That(feedback.CanStartSearch, Is.False);
            Assert.That(feedback.Message, Does.Contain("设备模板名称不能重复：重复设备。"));
        }

        [Test]
        public void ProfileValidationFeedback_LoadedValidProfile_AllowsStartingSearch()
        {
            var profile = DeviceProfileDrafts.CreateDefault(System.Array.Empty<string>());
            profile.CommandContent = "AT";
            profile.ExpectedResponse = "OK";

            var feedback = ProfileValidationFeedback.Create(new[] { profile });

            Assert.That(feedback.IsValid, Is.True);
            Assert.That(feedback.CanStartSearch, Is.True);
        }

        [Test]
        public void ScanSummary_FormatMatchedDevices_ListsOnlyMatchedInNamePortBaudDataParityFormat()
        {
            var results = new List<ScanResult>
            {
                new ScanResult
                {
                    PortName = "COM3",
                    ProfileName = "流量计",
                    Status = ScanStatus.Matched,
                    PortSettings = new SerialPortSettings { BaudRate = 9600, DataBits = 8, Parity = Parity.None }
                },
                new ScanResult { PortName = "COM4", ProfileName = "流量计", Status = ScanStatus.Timeout },
                new ScanResult
                {
                    PortName = "COM5",
                    ProfileName = "温控器",
                    Status = ScanStatus.Matched,
                    PortSettings = new SerialPortSettings { BaudRate = 19200, DataBits = 7, Parity = Parity.Even }
                }
            };

            Assert.That(ScanSummary.FormatMatchedDevices(results), Is.EqualTo(
                "流量计-COM3-9600-8-None" + System.Environment.NewLine + "温控器-COM5-19200-7-Even"));
        }

        [Test]
        public void ScanSummary_FormatMatchedDevices_NoMatches_ReturnsEmpty()
        {
            var results = new List<ScanResult>
            {
                new ScanResult { PortName = "COM3", Status = ScanStatus.Timeout }
            };

            Assert.That(ScanSummary.FormatMatchedDevices(results), Is.EqualTo(string.Empty));
        }

        [Test]
        public void ScanSummary_MixedResults_ReportsMatchedUnmatchedPortsAndErrorAttempts()
        {
            var results = new List<ScanResult>
            {
                CreateResult("COM1", ScanStatus.Matched),
                CreateResult("COM2", ScanStatus.Timeout),
                CreateResult("COM2", ScanStatus.PortUnavailable),
                CreateResult("COM3", ScanStatus.CommunicationError)
            };

            var summary = ScanSummary.From(results);

            Assert.That(summary.MatchedPortCount, Is.EqualTo(1));
            Assert.That(summary.UnmatchedPortCount, Is.EqualTo(2));
            Assert.That(summary.ErrorCount, Is.EqualTo(2));
            Assert.That(summary.ToDisplayText(), Is.EqualTo("搜索完成：匹配 1 个端口；未匹配 2 个端口；端口/通信错误 2 次。"));
        }

        [Test]
        public void ScanCompletionFeedback_CancellationWithPartialResults_NeverUsesCompletedWording()
        {
            var feedback = ScanCompletionFeedback.Create(
                new[] { CreateResult("COM1", ScanStatus.Matched), CreateResult("COM2", ScanStatus.Cancelled) },
                cancellationRequested: true);

            Assert.That(feedback.IsStopped, Is.True);
            Assert.That(feedback.Message, Does.Contain("搜索已停止"));
            Assert.That(feedback.Message, Does.Contain("部分结果"));
            Assert.That(feedback.Message, Does.Not.Contain("搜索完成"));
        }

        [Test]
        public void ScanCompletionFeedback_CancellationWithZeroResults_NeverClaimsNoMatchCompletion()
        {
            var feedback = ScanCompletionFeedback.Create(System.Array.Empty<ScanResult>(), cancellationRequested: true);

            Assert.That(feedback.IsStopped, Is.True);
            Assert.That(feedback.ShouldShowNoMatchState, Is.False);
            Assert.That(feedback.Message, Does.Contain("搜索已停止"));
            Assert.That(feedback.Message, Does.Not.Contain("未发现匹配设备"));
        }

        private static ScanResult CreateResult(string portName, ScanStatus status)
        {
            return new ScanResult { PortName = portName, Status = status };
        }

        private static TControl? FindControl<TControl>(Control parent, string accessibleName)
            where TControl : Control
        {
            foreach (Control child in parent.Controls)
            {
                if (child is TControl typed && child.AccessibleName == accessibleName)
                {
                    return typed;
                }

                var nested = FindControl<TControl>(child, accessibleName);
                if (nested != null)
                {
                    return nested;
                }
            }

            return null;
        }
    }
}
