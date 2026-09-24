using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using SerialPortDeviceFinder.Core.Configuration;
using SerialPortDeviceFinder.Core.Models;
using SerialPortDeviceFinder.Core.Scanning;
using SerialPortDeviceFinder.Core.Tests.Scanning.Fakes;
using SerialPortDeviceFinder.WinForms;

namespace SerialPortDeviceFinder.Core.Tests.WinForms
{
    /// <summary>
    /// 用 Fake 串口目录/探测驱动 MainForm 真实扫描链路，验证汇总框与"上次扫描串口"回写。
    /// </summary>
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public sealed class MainFormScanTests
    {
        [Test]
        public void ScanWithMatch_PopulatesSummaryAndLastScanPortField()
        {
            using (var directory = new TemporaryDirectory())
            {
                var profile = TestProfiles.Text("流量计", "READ", "VALUE");
                new DeviceProfileStore(directory.Path).Save(new[] { profile });
                var scheduler = new SequentialScanScheduler(
                    new FakeSerialPortCatalog("COM3"),
                    new FakeSerialProbe(ScanStatus.Matched));

                using (var form = new MainForm(new DeviceProfileStore(directory.Path), scheduler))
                {
                    form.Show();
                    Application.DoEvents();
                    RunScan(form);

                    var summary = FindControl<TextBox>(form, "搜索到的设备汇总")!;
                    Assert.That(summary.ReadOnly, Is.True);
                    Assert.That(summary.Text, Is.EqualTo("流量计-COM3-9600-8-None"));

                    Assert.That(FindControl<TextBox>(form, "上次扫描串口号")!.Text, Is.EqualTo("COM3"));

                    FindToolStripButton(form, "保存配置")!.PerformClick();
                    Application.DoEvents();
                    Assert.That(new DeviceProfileStore(directory.Path).Load().Single().LastScanPortName,
                        Is.EqualTo("COM3"));
                }
            }
        }

        [Test]
        public void ScanWithoutMatch_KeepsLastScanPortPlaceholderAndEmptySummary()
        {
            using (var directory = new TemporaryDirectory())
            {
                var profile = TestProfiles.Text("流量计", "READ", "VALUE");
                new DeviceProfileStore(directory.Path).Save(new[] { profile });
                var scheduler = new SequentialScanScheduler(
                    new FakeSerialPortCatalog("COM3"),
                    new FakeSerialProbe(ScanStatus.Timeout));

                using (var form = new MainForm(new DeviceProfileStore(directory.Path), scheduler))
                {
                    form.Show();
                    Application.DoEvents();
                    RunScan(form);

                    Assert.That(FindControl<TextBox>(form, "搜索到的设备汇总")!.Text, Is.EqualTo(string.Empty));
                    Assert.That(FindControl<TextBox>(form, "上次扫描串口号")!.Text, Is.EqualTo("-"));
                }
            }
        }

        /// <summary>
        /// 点击"开始搜索"并泵消息循环直到扫描结束（开始按钮恢复可用）。
        /// </summary>
        private static void RunScan(Form form)
        {
            var startButton = FindToolStripButton(form, "开始搜索")!;
            Assert.That(startButton, Is.Not.Null);

            startButton.PerformClick();
            PumpUntil(() => startButton.Enabled, "扫描未在 10 秒内完成。");
        }

        private static ToolStripButton? FindToolStripButton(Form form, string accessibleName)
        {
            foreach (Control control in form.Controls)
            {
                if (control is ToolStrip strip)
                {
                    foreach (ToolStripItem item in strip.Items)
                    {
                        if (item is ToolStripButton button && button.AccessibleName == accessibleName)
                        {
                            return button;
                        }
                    }
                }
            }

            return null;
        }

        private static void PumpUntil(Func<bool> condition, string failureMessage)
        {
            var deadline = DateTime.UtcNow.AddSeconds(10);
            while (!condition() && DateTime.UtcNow < deadline)
            {
                Application.DoEvents();
                Thread.Sleep(10);
            }

            Assert.That(condition(), Is.True, failureMessage);
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

        private sealed class TemporaryDirectory : IDisposable
        {
            public TemporaryDirectory()
            {
                Path = System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(),
                    "sdf-mainform-tests-" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(Path);
            }

            public string Path { get; }

            public void Dispose()
            {
                try
                {
                    if (Directory.Exists(Path))
                    {
                        Directory.Delete(Path, recursive: true);
                    }
                }
                catch (IOException)
                {
                }
            }
        }
    }
}
