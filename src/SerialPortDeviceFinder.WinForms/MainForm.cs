using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SerialPortDeviceFinder.Core.Configuration;
using SerialPortDeviceFinder.Core.Models;
using SerialPortDeviceFinder.Core.Scanning;
using SerialPortDeviceFinder.Core.Validation;

namespace SerialPortDeviceFinder.WinForms
{
    /// <summary>
    /// 应用程序协调窗体：维护模板列表、配置文件、扫描生命周期和结果展示。
    /// 具体模板字段由 DeviceProfileEditor 管理。
    /// </summary>
    public sealed partial class MainForm : Form
    {
        private readonly BindingList<DeviceProfile> _profiles = new BindingList<DeviceProfile>();
        private readonly BindingSource _profileBinding = new BindingSource();
        private readonly BindingList<ScanResultDisplay> _matchedResults = new BindingList<ScanResultDisplay>();
        private readonly DeviceProfileStore _store;
        private readonly IScanScheduler _scheduler;
        private CancellationTokenSource? _scanCancellation;
        private Task<IReadOnlyList<ScanResult>>? _scanTask;
        private bool _closeWhenScanCompletes;

        public MainForm()
            : this(
                new DeviceProfileStore(AppDomain.CurrentDomain.BaseDirectory),
                new SequentialScanScheduler(new SystemSerialPortCatalog(), new SerialProbe()))
        {
        }

        internal MainForm(DeviceProfileStore store, IScanScheduler scheduler)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            InitializeComponent();
            ConfigureResultGridColumns();
            WireEvents();
            LoadProfiles(showDialogOnFailure: false);
            UpdateCommandState();
        }

        /// <summary>
        /// 匹配结果表格的列定义。
        /// </summary>
        private void ConfigureResultGridColumns()
        {
            _resultsGrid.AutoGenerateColumns = false;
            _resultsGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                CreateTextColumn(nameof(ScanResultDisplay.DeviceName), "设备"),
                CreateTextColumn(nameof(ScanResultDisplay.PortName), "串口"),
                CreateTextColumn(nameof(ScanResultDisplay.Parameters), "实际参数"),
                CreateTextColumn(nameof(ScanResultDisplay.Response), "原始响应（十六进制）"),
                CreateTextColumn(nameof(ScanResultDisplay.Time), "识别时间")
            });
        }

        private void WireEvents()
        {
            _profileBinding.DataSource = _profiles;
            _profileListBox.DataSource = _profileBinding;
            _resultsGrid.DataSource = _matchedResults;
            _resultsGrid.Visible = false;
            _profileListBox.SelectedIndexChanged += (_, __) => _editor.LoadProfile(_profileListBox.SelectedItem as DeviceProfile);
            _addProfileButton.Click += (_, __) => AddProfile();
            _copyProfileButton.Click += (_, __) => CopySelectedProfile();
            _deleteProfileButton.Click += (_, __) => DeleteSelectedProfile();
            _editor.ProfileChanged += (_, __) =>
            {
                _profileBinding.ResetBindings(false);
                UpdateCommandState();
            };
            _startButton.Click += async (_, __) => await StartScanAsync();
            _stopButton.Click += (_, __) => RequestStop();
            _saveButton.Click += (_, __) => SaveProfiles();
            _loadButton.Click += (_, __) => LoadProfiles(showDialogOnFailure: true);
            FormClosing += MainForm_FormClosing;
            Shown += (_, __) =>
            {
                ConfigureSplit(_rootSplit, 190, 650, 210);
                ConfigureSplit(_verticalSplit, 430, 140, 590);
                ConfigureSplit(_lowerSplit, 55, 40, 63);
                ConfigureSplit(_summarySplit, 300, 200, 545);
            };
        }

        private void AddProfile()
        {
            var profile = DeviceProfileDrafts.CreateDefault(_profiles.Select(item => item.Name));
            _profiles.Add(profile);
            _profileListBox.SelectedItem = profile;
            UpdateCommandState();
        }

        private void CopySelectedProfile()
        {
            if (!(_profileListBox.SelectedItem is DeviceProfile selected))
            {
                return;
            }

            var copy = DeviceProfileDrafts.Clone(selected, _profiles.Select(item => item.Name));
            _profiles.Add(copy);
            _profileListBox.SelectedItem = copy;
            UpdateCommandState();
        }

        private void DeleteSelectedProfile()
        {
            if (!(_profileListBox.SelectedItem is DeviceProfile selected))
            {
                return;
            }

            _profiles.Remove(selected);
            if (_profiles.Count > 0)
            {
                _profileListBox.SelectedIndex = Math.Min(_profileListBox.SelectedIndex, _profiles.Count - 1);
            }
            UpdateCommandState();
        }

        private void SaveProfiles()
        {
            try
            {
                _editor.CommitCurrentProfile();
                _store.Save(_profiles.ToList());
                _statusLabel.Text = "配置已保存到 devices.json。";
                AppendLog("配置已保存。");
            }
            catch (Exception exception)
            {
                ShowOperationError("保存配置失败", exception);
            }
        }

        private void LoadProfiles(bool showDialogOnFailure)
        {
            try
            {
                var loaded = _store.Load();
                _profiles.RaiseListChangedEvents = false;
                _profiles.Clear();
                foreach (var profile in loaded)
                {
                    _profiles.Add(profile);
                }
                _profiles.RaiseListChangedEvents = true;
                _profileBinding.ResetBindings(false);
                _profileListBox.SelectedIndex = _profiles.Count > 0 ? 0 : -1;
                _editor.LoadProfile(_profileListBox.SelectedItem as DeviceProfile);
                UpdateCommandState();
                _statusLabel.Text = _profiles.Count == 0 ? "尚未保存设备模板，请新增模板。" : $"已加载 {_profiles.Count} 个设备模板。";
                if (showDialogOnFailure)
                {
                    AppendLog($"已加载 {_profiles.Count} 个设备模板。");
                }
            }
            catch (Exception exception)
            {
                _statusLabel.Text = "加载配置失败。";
                AppendLog("加载配置失败：" + exception.Message);
                if (showDialogOnFailure)
                {
                    ShowOperationError("加载配置失败", exception);
                }
            }
        }

        private async Task StartScanAsync()
        {
            _editor.CommitCurrentProfile();
            var errors = DeviceProfileValidator.ValidateProfiles(_profiles.ToList());
            if (errors.Count > 0)
            {
                _statusLabel.Text = "模板配置无效，无法开始搜索。";
                MessageBox.Show(this, string.Join(Environment.NewLine, errors), "无法开始搜索", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _matchedResults.Clear();
            _summaryTextBox.Clear();
            _logTextBox.Clear();
            ShowResultsEmptyState("搜索进行中，等待匹配结果。");
            _scanCancellation = new CancellationTokenSource();
            SetScanningState(true);
            AppendLog("开始顺序搜索当前可见的 COM 端口。");
            var progress = new Progress<ScanResult>(ReportScanResult);
            try
            {
                _scanTask = _scheduler.ScanAsync(_profiles.ToList(), _scanCancellation.Token, progress);
                var results = await _scanTask.ConfigureAwait(true);
                _summaryTextBox.Text = ScanSummary.FormatMatchedDevices(results);
                UpdateLastScanPorts(results);
                var feedback = ScanCompletionFeedback.Create(results, _scanCancellation.IsCancellationRequested);
                AppendLog(feedback.Message);
                _statusLabel.Text = feedback.Message;
                if (feedback.ShouldShowNoMatchState)
                {
                    ShowResultsEmptyState("本轮搜索未发现匹配设备。请检查模板、连接和串口参数。");
                }
                else if (feedback.IsStopped && _matchedResults.Count == 0)
                {
                    ShowResultsEmptyState("搜索已停止，已保留部分结果。可以调整模板后重新搜索。");
                }
            }
            catch (Exception exception)
            {
                _statusLabel.Text = "搜索异常结束。";
                ShowOperationError("搜索异常", exception);
            }
            finally
            {
                _scanCancellation?.Dispose();
                _scanCancellation = null;
                _scanTask = null;
                SetScanningState(false);
                if (_closeWhenScanCompletes && !IsDisposed)
                {
                    BeginInvoke(new Action(Close));
                }
            }
        }

        private void ReportScanResult(ScanResult result)
        {
            if (result.Status == ScanStatus.Matched)
            {
                _matchedResults.Add(ScanResultDisplay.From(result));
                _resultsGrid.Visible = true;
                _resultsEmptyLabel.Visible = false;
            }

            AppendLog($"{result.OccurredAt.LocalDateTime:HH:mm:ss}  {result.PortName} / {result.ProfileName} / {result.Status}：{result.Message}；响应 {ScanResultDisplay.ToHex(result.ResponseBytes)}");
        }

        /// <summary>
        /// 把匹配结果回写到各模板的"上次扫描串口号"（只读展示字段）。
        /// 只写匹配到的模板，未匹配的保持原值；选中模板有更新时刷新编辑器显示。
        /// </summary>
        private void UpdateLastScanPorts(IReadOnlyList<ScanResult> results)
        {
            var selectedProfile = _profileListBox.SelectedItem as DeviceProfile;
            var selectedUpdated = false;
            foreach (var result in results.Where(result => result?.Status == ScanStatus.Matched))
            {
                var profile = _profiles.FirstOrDefault(item => item.Name == result.ProfileName);
                if (profile == null || profile.LastScanPortName == result.PortName)
                {
                    continue;
                }

                profile.LastScanPortName = result.PortName;
                if (ReferenceEquals(profile, selectedProfile))
                {
                    selectedUpdated = true;
                }
            }

            if (selectedUpdated)
            {
                _editor.LoadProfile(selectedProfile);
            }
        }

        private void RequestStop()
        {
            _scanCancellation?.Cancel();
            _stopButton.Enabled = false;
            _statusLabel.Text = "已请求停止，正在安全关闭当前串口。";
            AppendLog("已请求停止搜索。");
        }

        private void SetScanningState(bool isScanning)
        {
            _profileListBox.Enabled = !isScanning;
            _profileCommandPanel.Enabled = !isScanning;
            _editor.Enabled = !isScanning;
            _saveButton.Enabled = !isScanning;
            _loadButton.Enabled = !isScanning;
            _startButton.Enabled = !isScanning && CanStartScan();
            _stopButton.Enabled = isScanning;
        }

        private void UpdateCommandState()
        {
            var feedback = ProfileValidationFeedback.Create(_profiles.ToList());
            _globalValidationLabel.ForeColor = feedback.IsValid ? SystemColors.ControlText : Color.DarkRed;
            _globalValidationLabel.Text = feedback.Message;
            if (_scanCancellation == null)
            {
                _startButton.Enabled = feedback.CanStartSearch;
            }
        }

        private bool CanStartScan()
        {
            return ProfileValidationFeedback.Create(_profiles.ToList()).CanStartSearch;
        }

        private void AppendLog(string message)
        {
            _logTextBox.AppendText(message + Environment.NewLine);
        }

        private void ShowResultsEmptyState(string message)
        {
            _resultsGrid.Visible = false;
            _resultsEmptyLabel.Text = message;
            _resultsEmptyLabel.Visible = true;
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_scanTask?.IsCompleted == false)
            {
                e.Cancel = true;
                _closeWhenScanCompletes = true;
                RequestStop();
            }
        }

        private void ShowOperationError(string caption, Exception exception)
        {
            AppendLog(caption + "：" + exception.Message);
            MessageBox.Show(this, exception.Message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static DataGridViewTextBoxColumn CreateTextColumn(string propertyName, string headerText)
        {
            return new DataGridViewTextBoxColumn { DataPropertyName = propertyName, HeaderText = headerText };
        }

        private static void ConfigureSplit(SplitContainer split, int panel1Minimum, int panel2Minimum, int desiredDistance)
        {
            split.Panel1MinSize = panel1Minimum;
            split.Panel2MinSize = panel2Minimum;
            var availableLength = split.Orientation == Orientation.Vertical ? split.ClientSize.Width : split.ClientSize.Height;
            var maximumDistance = Math.Max(panel1Minimum, availableLength - split.SplitterWidth - panel2Minimum);
            split.SplitterDistance = Math.Min(
                maximumDistance,
                Math.Max(panel1Minimum, Math.Min(desiredDistance, maximumDistance)));
        }
    }
}
