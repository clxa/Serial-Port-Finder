using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
using SerialPortDeviceFinder.Core.Models;
using SerialPortDeviceFinder.Core.Validation;

namespace SerialPortDeviceFinder.WinForms
{
    /// <summary>
    /// 只负责编辑一个设备模板的控件；扫描、配置文件和列表管理由 MainForm 负责。
    /// </summary>
    public sealed partial class DeviceProfileEditor : UserControl
    {
        private BindingList<SerialPortSettings> _settings = new BindingList<SerialPortSettings>();
        private DeviceProfile? _profile;
        private bool _isLoading;
        private bool _syncingSelection;
        private readonly List<ComboOption> _comboOptions = DeviceProfileDrafts.CommonSerialPortCombos.ToList();

        public DeviceProfileEditor()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            ConfigureSettingsGridColumns();
            BindEnumDataSources();
            WireEvents();
            SetEditorEnabled(false);
            ShowNoProfileMessage();
        }

        /// <summary>
        /// 串口参数表格的列定义（含枚举列的数据源）。
        /// </summary>
        private void ConfigureSettingsGridColumns()
        {
            _settingsGrid.AutoGenerateColumns = false;
            var enabledColumn = new DataGridViewCheckBoxColumn
            {
                Name = "Enabled",
                DataPropertyName = "Enabled",
                HeaderText = "启用",
                Width = 52
            };
            var baudRateColumn = new DataGridViewTextBoxColumn
            {
                Name = "BaudRate",
                DataPropertyName = "BaudRate",
                HeaderText = "波特率",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            var dataBitsColumn = new DataGridViewTextBoxColumn
            {
                Name = "DataBits",
                DataPropertyName = "DataBits",
                HeaderText = "数据位",
                Width = 78
            };
            var parityColumn = new DataGridViewComboBoxColumn
            {
                Name = "Parity",
                DataPropertyName = "Parity",
                HeaderText = "校验位",
                Width = 90,
                DataSource = Enum.GetValues(typeof(Parity))
            };
            var stopBitsColumn = new DataGridViewComboBoxColumn
            {
                Name = "StopBits",
                DataPropertyName = "StopBits",
                HeaderText = "停止位",
                Width = 90,
                DataSource = new[] { StopBits.One, StopBits.OnePointFive, StopBits.Two }
            };
            _settingsGrid.Columns.AddRange(new DataGridViewColumn[]
            {
                enabledColumn,
                baudRateColumn,
                dataBitsColumn,
                parityColumn,
                stopBitsColumn
            });
        }

        /// <summary>
        /// 枚举下拉框的数据源（两个枚举列的数据源随列一起定义在 ConfigureSettingsGridColumns 中）。
        /// </summary>
        private void BindEnumDataSources()
        {
            _commandFormatComboBox.DataSource = Enum.GetValues(typeof(PayloadFormat));
            _terminatorComboBox.DataSource = Enum.GetValues(typeof(TextTerminator));
            _responseFormatComboBox.DataSource = Enum.GetValues(typeof(PayloadFormat));
            _matchModeComboBox.DataSource = Enum.GetValues(typeof(ResponseMatchMode));
            _encodingComboBox.DataSource = Enum.GetValues(typeof(TextEncodingKind));
        }

        public event EventHandler? ProfileChanged;

        public void LoadProfile(DeviceProfile? profile)
        {
            _isLoading = true;
            try
            {
                _profile = profile;
                SetEditorEnabled(profile != null);
                if (profile == null)
                {
                    _settings = new BindingList<SerialPortSettings>();
                    _settingsGrid.DataSource = _settings;
                    UpdateSelectionCheckBoxStates();
                    ClearInputControls();
                    ShowNoProfileMessage();
                    return;
                }

                _nameTextBox.Text = profile.Name;
                _enabledCheckBox.Checked = profile.IsEnabled;
                _commandFormatComboBox.SelectedItem = profile.CommandFormat;
                _commandTextBox.Text = profile.CommandContent;
                _terminatorComboBox.SelectedItem = profile.TextTerminator;
                _responseFormatComboBox.SelectedItem = profile.ResponseFormat;
                _responseTextBox.Text = profile.ExpectedResponse;
                _matchModeComboBox.SelectedItem = profile.MatchMode;
                _encodingComboBox.SelectedItem = profile.TextEncoding;
                _timeoutNumeric.Value = Math.Max(_timeoutNumeric.Minimum, Math.Min(_timeoutNumeric.Maximum, profile.TimeoutMilliseconds));
                _lastScanPortTextBox.Text = ToLastScanPortDisplay(profile.LastScanPortName);

                _settings = new BindingList<SerialPortSettings>((profile.PortSettings ?? new List<SerialPortSettings>())
                    .Where(setting => setting != null)
                    .ToList());
                _settingsGrid.DataSource = _settings;
                ReorderSettingsByTier();
                UpdateSelectionCheckBoxStates();
                UpdateFormatControlState();
                ValidateCurrentProfile();
            }
            finally
            {
                _isLoading = false;
            }
        }

        public void CommitCurrentProfile()
        {
            _settingsGrid.EndEdit();
            SynchronizeProfile();
        }

        private void WireEvents()
        {
            _nameTextBox.TextChanged += (_, __) => OnInputChanged();
            _enabledCheckBox.CheckedChanged += (_, __) => OnInputChanged();
            _commandFormatComboBox.SelectedIndexChanged += (_, __) => OnInputChanged();
            _commandTextBox.TextChanged += (_, __) => OnInputChanged();
            _terminatorComboBox.SelectedIndexChanged += (_, __) => OnInputChanged();
            _responseFormatComboBox.SelectedIndexChanged += (_, __) => OnInputChanged();
            _responseTextBox.TextChanged += (_, __) => OnInputChanged();
            _matchModeComboBox.SelectedIndexChanged += (_, __) => OnInputChanged();
            _encodingComboBox.SelectedIndexChanged += (_, __) => OnInputChanged();
            _timeoutNumeric.ValueChanged += (_, __) => OnInputChanged();
            _settingsGrid.CellValueChanged += (_, e) =>
            {
                ReorderSettingsByTier();
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0
                    && _settingsGrid.Columns[e.ColumnIndex].Name == nameof(SerialPortSettings.Enabled))
                {
                    _settingsGrid.InvalidateRow(e.RowIndex);
                }
                OnInputChanged();
            };
            _settingsGrid.RowsRemoved += (_, __) => OnInputChanged();
            _settingsGrid.CellFormatting += (_, e) =>
            {
                if (e.RowIndex < 0 || _isLoading || _settings == null || e.RowIndex >= _settings.Count)
                {
                    return;
                }
                if (_settings[e.RowIndex]?.Enabled != true)
                {
                    e.CellStyle.ForeColor = SystemColors.GrayText;
                }
            };
            _addSettingButton.Click += (_, __) =>
            {
                _settings.Add(DeviceProfileDrafts.Create8N1(9600));
                ReorderSettingsByTier();
                OnInputChanged();
            };
            _removeSettingButton.Click += (_, __) =>
            {
                if (_settingsGrid.CurrentRow?.Index >= 0)
                {
                    _settings.RemoveAt(_settingsGrid.CurrentRow.Index);
                    OnInputChanged();
                }
            };
            _selectAllCheckBox.CheckedChanged += (_, __) =>
            {
                if (_isLoading || _syncingSelection || _profile == null)
                {
                    return;
                }
                ApplySelectAll(_selectAllCheckBox.Checked);
            };
            _selectCommonCheckBox.CheckedChanged += (_, __) =>
            {
                if (_isLoading || _syncingSelection || _profile == null)
                {
                    return;
                }
                ApplySelectCommon(_selectCommonCheckBox.Checked);
            };
        }

        private void OnInputChanged()
        {
            if (_isLoading || _profile == null)
            {
                return;
            }

            SynchronizeProfile();
            UpdateFormatControlState();
            ValidateCurrentProfile();
            UpdateSelectionCheckBoxStates();
            ProfileChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 按"勾选的 → 常用的 → 不常用的"排序（行序 = 扫描序）：
        /// 启用行在最上；8 位数据位的常用组合次之；7 位数据位的不常用组合最后。
        /// 段内按目录顺序；目录外的自定义行归入所在段末尾（稳定序，保持原相对顺序）。
        /// </summary>
        private void ReorderSettingsByTier()
        {
            var sorted = _settings
                .OrderBy(setting => setting?.Enabled != true)
                .ThenBy(ComboTierIndex)
                .ToList();

            if (sorted.SequenceEqual(_settings))
            {
                return;
            }

            _settings.Clear();
            foreach (var setting in sorted)
            {
                _settings.Add(setting);
            }
        }

        private int ComboTierIndex(SerialPortSettings? setting)
        {
            if (setting == null)
            {
                return int.MaxValue;
            }

            var commonOffset = setting.DataBits == 8 ? 0 : _comboOptions.Count;
            for (var index = 0; index < _comboOptions.Count; index++)
            {
                if (Matches(setting, _comboOptions[index]))
                {
                    return commonOffset + index;
                }
            }

            return commonOffset + _comboOptions.Count;
        }

        private static bool Matches(SerialPortSettings setting, ComboOption option)
        {
            return setting.BaudRate == option.BaudRate
                && setting.DataBits == option.DataBits
                && setting.Parity == option.Parity
                && setting.StopBits == option.StopBits;
        }

        private void ApplySelectAll(bool enableAll)
        {
            foreach (var setting in _settings)
            {
                if (setting != null)
                {
                    setting.Enabled = enableAll;
                }
            }
            RefreshSettingsGridFromModel();
        }

        /// <summary>
        /// "选择常用的" = 只保留 8 位数据位的常用组合（22 个），7 位数据位的禁用；
        /// 取消勾选 = 撤销常用选择（8 位全部禁用，7 位保持原状）。
        /// </summary>
        private void ApplySelectCommon(bool enableCommon)
        {
            foreach (var setting in _settings)
            {
                if (setting == null)
                {
                    continue;
                }
                if (setting.DataBits == 8)
                {
                    setting.Enabled = enableCommon;
                }
                else if (enableCommon)
                {
                    setting.Enabled = false;
                }
            }
            RefreshSettingsGridFromModel();
        }

        /// <summary>
        /// 重新绑定数据源并重排网格。
        /// </summary>
        private void RefreshSettingsGridFromModel()
        {
            _settingsGrid.DataSource = _settings;
            ReorderSettingsByTier();
            OnInputChanged();
        }

        /// <summary>
        /// 同步"全选/选择常用的"勾选状态（派生状态，不触发 CheckedChanged 应用逻辑）：
        /// 全选 = 全部行启用；选择常用的 = 恰好 8 位数据位启用、7 位禁用。
        /// </summary>
        private void UpdateSelectionCheckBoxStates()
        {
            _syncingSelection = true;
            try
            {
                if (_settings.Count == 0)
                {
                    _selectAllCheckBox.Checked = false;
                    _selectCommonCheckBox.Checked = false;
                    return;
                }

                _selectAllCheckBox.Checked = _settings.All(setting => setting?.Enabled == true);
                _selectCommonCheckBox.Checked = _settings.All(setting => setting?.Enabled == (setting?.DataBits == 8));
            }
            finally
            {
                _syncingSelection = false;
            }
        }

        private void SynchronizeProfile()
        {
            if (_profile == null)
            {
                return;
            }

            _profile.Name = _nameTextBox.Text;
            _profile.IsEnabled = _enabledCheckBox.Checked;
            _profile.CommandFormat = ReadComboValue(_commandFormatComboBox, PayloadFormat.Text);
            _profile.CommandContent = _commandTextBox.Text;
            _profile.TextTerminator = ReadComboValue(_terminatorComboBox, TextTerminator.CrLf);
            _profile.ResponseFormat = ReadComboValue(_responseFormatComboBox, PayloadFormat.Text);
            _profile.ExpectedResponse = _responseTextBox.Text;
            _profile.MatchMode = ReadComboValue(_matchModeComboBox, ResponseMatchMode.Contains);
            _profile.TextEncoding = ReadComboValue(_encodingComboBox, TextEncodingKind.Ascii);
            _profile.TimeoutMilliseconds = Decimal.ToInt32(_timeoutNumeric.Value);
            _profile.PortSettings = _settings.Where(setting => setting != null).ToList();
            foreach (var setting in _profile.PortSettings)
            {
                setting.Handshake = Handshake.None;
            }
        }

        private void ValidateCurrentProfile()
        {
            if (_profile == null)
            {
                ShowNoProfileMessage();
                return;
            }

            var errors = DeviceProfileValidator.Validate(_profile);
            if (errors.Count == 0)
            {
                _validationLabel.ForeColor = SystemColors.ControlText;
                _validationLabel.Text = "当前模板字段有效；仍需通过所有模板的全局检查。";
                return;
            }

            _validationLabel.ForeColor = Color.DarkRed;
            _validationLabel.Text = "需要修正：" + string.Join("  ", errors);
        }

        private void UpdateFormatControlState()
        {
            var commandIsText = ReadComboValue(_commandFormatComboBox, PayloadFormat.Text) == PayloadFormat.Text;
            var responseIsText = ReadComboValue(_responseFormatComboBox, PayloadFormat.Text) == PayloadFormat.Text;
            _terminatorComboBox.Enabled = commandIsText;
            _encodingComboBox.Enabled = commandIsText || responseIsText;
        }

        private void ShowNoProfileMessage()
        {
            _validationLabel.ForeColor = SystemColors.ControlText;
            _validationLabel.Text = "请先在左侧新增或选择一个设备模板。";
        }

        private void SetEditorEnabled(bool enabled)
        {
            foreach (Control control in Controls)
            {
                control.Enabled = enabled;
            }
            _validationLabel.Enabled = true;
        }

        private void ClearInputControls()
        {
            _nameTextBox.Clear();
            _commandTextBox.Clear();
            _responseTextBox.Clear();
            _lastScanPortTextBox.Text = ToLastScanPortDisplay(null);
        }

        /// <summary>
        /// 上次扫描串口只读展示：从未搜索到过时显示占位符 "-"。
        /// </summary>
        private static string ToLastScanPortDisplay(string? lastScanPortName)
        {
            return string.IsNullOrWhiteSpace(lastScanPortName) ? "-" : lastScanPortName;
        }

        private static TEnum ReadComboValue<TEnum>(ComboBox comboBox, TEnum fallback) where TEnum : struct
        {
            return comboBox.SelectedItem is TEnum value ? value : fallback;
        }
    }
}
