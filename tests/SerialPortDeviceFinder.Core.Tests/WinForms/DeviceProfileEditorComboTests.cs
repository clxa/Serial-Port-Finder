using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using SerialPortDeviceFinder.Core.Models;
using SerialPortDeviceFinder.Core.Validation;
using SerialPortDeviceFinder.WinForms;

namespace SerialPortDeviceFinder.Core.Tests.WinForms
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public sealed class DeviceProfileEditorComboTests
    {
        [Test]
        public void DefaultProfile_GridSortedByEnabledThenCommonThenUncommon()
        {
            using (var editor = CreateEditorWithDefaultProfile())
            {
                var grid = FindControl<DataGridView>(editor, "串口参数表格")!;

                Assert.That(grid.RowCount, Is.EqualTo(DeviceProfileDrafts.CommonSerialPortCombos.Count));
                Assert.That(grid.Rows.Cast<DataGridViewRow>().Take(8).All(row => Equals(row.Cells["Enabled"].Value, true)), Is.True);
                Assert.That(grid.Rows[0].Cells["BaudRate"].Value, Is.EqualTo(9600));
                Assert.That(grid.Rows[7].Cells["BaudRate"].Value, Is.EqualTo(115200));
                Assert.That(grid.Rows[7].Cells["StopBits"].Value, Is.EqualTo(StopBits.Two));
                Assert.That(grid.Rows[8].Cells["Enabled"].Value, Is.EqualTo(false));
                Assert.That(grid.Rows[8].Cells["BaudRate"].Value, Is.EqualTo(1200));
                Assert.That(grid.Rows[8].Cells["DataBits"].Value, Is.EqualTo(8));
                Assert.That(grid.Rows[22].Cells["DataBits"].Value, Is.EqualTo(7));
                Assert.That(grid.Rows[25].Cells["DataBits"].Value, Is.EqualTo(7));
                Assert.That(grid.Rows[22].Cells["Enabled"].Value, Is.EqualTo(false));
            }
        }

        [Test]
        public void TogglingEnabledCell_UpdatesProfileKeepsRowAndResorts()
        {
            var profile = DeviceProfileDrafts.CreateDefault(Array.Empty<string>());
            using (var editor = new DeviceProfileEditor())
            {
                editor.LoadProfile(profile);
                var grid = FindControl<DataGridView>(editor, "串口参数表格")!;

                grid.Rows[0].Cells["Enabled"].Value = false;
                grid.Rows[7].Cells["Enabled"].Value = true;
                editor.CommitCurrentProfile();

                Assert.That(grid.RowCount, Is.EqualTo(26));
                Assert.That(profile.PortSettings.Count(setting => setting.Enabled), Is.EqualTo(8));
                Assert.That(profile.PortSettings.Take(8).All(setting => setting.Enabled), Is.True);
                Assert.That(profile.PortSettings.First(setting => setting.BaudRate == 9600
                        && setting.Parity == Parity.None && setting.StopBits == StopBits.One).Enabled,
                    Is.False);
                Assert.That(profile.PortSettings.Any(setting => setting.BaudRate == 1200 && setting.Enabled), Is.True);
            }
        }

        [Test]
        public void DisablingAllRows_MakesProfileInvalid()
        {
            var profile = DeviceProfileDrafts.CreateDefault(Array.Empty<string>());
            foreach (var setting in profile.PortSettings)
            {
                setting.Enabled = false;
            }

            using (var editor = new DeviceProfileEditor())
            {
                editor.LoadProfile(profile);
                editor.CommitCurrentProfile();
            }

            var errors = DeviceProfileValidator.Validate(profile);
            Assert.That(errors, Does.Contain("至少需要一组启用的串口参数。"));
        }

        [Test]
        public void AddingCustomRow_IsEnabledByDefaultAndSortsIntoEnabledSection()
        {
            using (var editor = CreateEditorWithDefaultProfile())
            {
                var grid = FindControl<DataGridView>(editor, "串口参数表格")!;

                FindControl<Button>(editor, "添加串口参数")!.PerformClick();

                Assert.That(grid.RowCount, Is.EqualTo(27));
                Assert.That(grid.Rows[0].Cells["BaudRate"].Value, Is.EqualTo(9600));
                Assert.That(grid.Rows[1].Cells["Enabled"].Value, Is.EqualTo(true));
                Assert.That(grid.Rows[1].Cells["BaudRate"].Value, Is.EqualTo(9600));
            }
        }

        [Test]
        public void LoadingProfile_DisabledRowsStayVisible()
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
                PortSettings = new List<SerialPortSettings>
                {
                    DeviceProfileDrafts.Create8N1(9600),
                    new SerialPortSettings { BaudRate = 4800, Enabled = false }
                }
            };

            using (var editor = new DeviceProfileEditor())
            {
                editor.LoadProfile(profile);
                var grid = FindControl<DataGridView>(editor, "串口参数表格")!;

                Assert.That(grid.RowCount, Is.EqualTo(2));
                Assert.That(grid.Rows[0].Cells["Enabled"].Value, Is.EqualTo(true));
                Assert.That(grid.Rows[1].Cells["Enabled"].Value, Is.EqualTo(false));
                Assert.That(grid.Rows[1].Cells["BaudRate"].Value, Is.EqualTo(4800));
            }
        }

        [Test]
        public void SelectAllCheckBox_ChecksAllRowsAndUncheckDisablesAll()
        {
            var profile = DeviceProfileDrafts.CreateDefault(Array.Empty<string>());
            using (var editor = new DeviceProfileEditor())
            {
                editor.LoadProfile(profile);
                var selectAll = FindControl<CheckBox>(editor, "全选串口参数")!;
                var selectCommon = FindControl<CheckBox>(editor, "选择常用串口参数")!;

                Assert.That(selectAll.Checked, Is.False);
                Assert.That(selectCommon.Checked, Is.False);

                selectAll.Checked = true;

                Assert.That(profile.PortSettings.All(setting => setting.Enabled), Is.True);
                Assert.That(selectAll.Checked, Is.True);
                Assert.That(selectCommon.Checked, Is.False);

                selectAll.Checked = false;

                Assert.That(profile.PortSettings.All(setting => !setting.Enabled), Is.True);
                Assert.That(DeviceProfileValidator.Validate(profile),
                    Does.Contain("至少需要一组启用的串口参数。"));
            }
        }

        [Test]
        public void SelectCommonCheckBox_ChecksExactlyTheCommonEightBitRows()
        {
            var profile = DeviceProfileDrafts.CreateDefault(Array.Empty<string>());
            using (var editor = new DeviceProfileEditor())
            {
                editor.LoadProfile(profile);
                var selectAll = FindControl<CheckBox>(editor, "全选串口参数")!;
                var selectCommon = FindControl<CheckBox>(editor, "选择常用串口参数")!;

                selectCommon.Checked = true;

                Assert.That(profile.PortSettings.Where(setting => setting.DataBits == 8).All(setting => setting.Enabled),
                    Is.True);
                Assert.That(profile.PortSettings.Where(setting => setting.DataBits == 7).All(setting => !setting.Enabled),
                    Is.True);
                Assert.That(selectCommon.Checked, Is.True);
                Assert.That(selectAll.Checked, Is.False);
            }
        }

        [Test]
        public void ManualRowToggle_UnchecksDerivedSelectionCheckBoxes()
        {
            var profile = DeviceProfileDrafts.CreateDefault(Array.Empty<string>());
            using (var editor = new DeviceProfileEditor())
            {
                editor.LoadProfile(profile);
                var grid = FindControl<DataGridView>(editor, "串口参数表格")!;
                var selectAll = FindControl<CheckBox>(editor, "全选串口参数")!;
                var selectCommon = FindControl<CheckBox>(editor, "选择常用串口参数")!;

                selectAll.Checked = true;
                grid.Rows[25].Cells["Enabled"].Value = false;
                Assert.That(selectAll.Checked, Is.False);

                selectCommon.Checked = true;
                grid.Rows[25].Cells["Enabled"].Value = true;
                Assert.That(selectCommon.Checked, Is.False);
                Assert.That(selectAll.Checked, Is.False);
            }
        }

        [Test]
        public void LoadingProfile_NeverScanned_ShowsDashPlaceholderInLastScanPortField()
        {
            using (var editor = new DeviceProfileEditor())
            {
                editor.LoadProfile(CreateValidTextProfile());

                var lastScanPort = FindControl<TextBox>(editor, "上次扫描串口号")!;
                Assert.That(lastScanPort.Text, Is.EqualTo("-"));
                Assert.That(lastScanPort.ReadOnly, Is.True);
            }
        }

        [Test]
        public void LoadingProfile_ScannedBefore_ShowsLastScanPort()
        {
            var profile = CreateValidTextProfile();
            profile.LastScanPortName = "COM7";

            using (var editor = new DeviceProfileEditor())
            {
                editor.LoadProfile(profile);

                Assert.That(FindControl<TextBox>(editor, "上次扫描串口号")!.Text, Is.EqualTo("COM7"));
            }
        }

        [Test]
        public void RemovingSelectedRow_RemovesRowFromGrid()
        {
            using (var editor = CreateEditorWithDefaultProfile())
            {
                var grid = FindControl<DataGridView>(editor, "串口参数表格")!;

                grid.CurrentCell = grid.Rows[0].Cells[1];
                FindControl<Button>(editor, "删除选中串口参数")!.PerformClick();

                Assert.That(grid.RowCount, Is.EqualTo(25));
                Assert.That(grid.Rows[0].Cells["BaudRate"].Value, Is.EqualTo(19200));
                Assert.That(grid.Rows[0].Cells["Enabled"].Value, Is.EqualTo(true));
            }
        }

        private static DeviceProfileEditor CreateEditorWithDefaultProfile()
        {
            var editor = new DeviceProfileEditor();
            editor.LoadProfile(DeviceProfileDrafts.CreateDefault(Array.Empty<string>()));
            return editor;
        }

        private static DeviceProfile CreateValidTextProfile()
        {
            return new DeviceProfile
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
                PortSettings = new List<SerialPortSettings> { DeviceProfileDrafts.Create8N1(9600) }
            };
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
