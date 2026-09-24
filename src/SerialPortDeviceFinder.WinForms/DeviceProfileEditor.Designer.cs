namespace SerialPortDeviceFinder.WinForms
{
    partial class DeviceProfileEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TextBox _nameTextBox;
        private System.Windows.Forms.CheckBox _enabledCheckBox;
        private System.Windows.Forms.ComboBox _commandFormatComboBox;
        private System.Windows.Forms.TextBox _commandTextBox;
        private System.Windows.Forms.ComboBox _terminatorComboBox;
        private System.Windows.Forms.ComboBox _responseFormatComboBox;
        private System.Windows.Forms.TextBox _responseTextBox;
        private System.Windows.Forms.ComboBox _matchModeComboBox;
        private System.Windows.Forms.ComboBox _encodingComboBox;
        private System.Windows.Forms.NumericUpDown _timeoutNumeric;
        private System.Windows.Forms.TextBox _lastScanPortTextBox;
        private System.Windows.Forms.DataGridView _settingsGrid;
        private System.Windows.Forms.Button _addSettingButton;
        private System.Windows.Forms.Button _removeSettingButton;
        private System.Windows.Forms.CheckBox _selectAllCheckBox;
        private System.Windows.Forms.CheckBox _selectCommonCheckBox;
        private System.Windows.Forms.Label _validationLabel;
        private System.Windows.Forms.GroupBox _protocolGroup;
        private System.Windows.Forms.GroupBox _settingsGroup;
        private System.Windows.Forms.TableLayoutPanel _protocolTable;
        private System.Windows.Forms.Panel _settingsPanel;
        private System.Windows.Forms.FlowLayoutPanel _settingsCommandPanel;
        private System.Windows.Forms.Label _nameLabel;
        private System.Windows.Forms.Label _commandFormatLabel;
        private System.Windows.Forms.Label _terminatorLabel;
        private System.Windows.Forms.Label _commandLabel;
        private System.Windows.Forms.Label _encodingLabel;
        private System.Windows.Forms.Label _responseFormatLabel;
        private System.Windows.Forms.Label _matchModeLabel;
        private System.Windows.Forms.Label _responseLabel;
        private System.Windows.Forms.Label _timeoutLabel;
        private System.Windows.Forms.Label _lastScanPortLabel;
        private System.Windows.Forms.FlowLayoutPanel _timeoutPanel;
        private System.Windows.Forms.Label _timeoutUnitLabel;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._nameTextBox = new System.Windows.Forms.TextBox();
            this._enabledCheckBox = new System.Windows.Forms.CheckBox();
            this._commandFormatComboBox = new System.Windows.Forms.ComboBox();
            this._commandTextBox = new System.Windows.Forms.TextBox();
            this._terminatorComboBox = new System.Windows.Forms.ComboBox();
            this._responseFormatComboBox = new System.Windows.Forms.ComboBox();
            this._responseTextBox = new System.Windows.Forms.TextBox();
            this._matchModeComboBox = new System.Windows.Forms.ComboBox();
            this._encodingComboBox = new System.Windows.Forms.ComboBox();
            this._timeoutNumeric = new System.Windows.Forms.NumericUpDown();
            this._lastScanPortTextBox = new System.Windows.Forms.TextBox();
            this._settingsGrid = new System.Windows.Forms.DataGridView();
            this._addSettingButton = new System.Windows.Forms.Button();
            this._removeSettingButton = new System.Windows.Forms.Button();
            this._selectAllCheckBox = new System.Windows.Forms.CheckBox();
            this._selectCommonCheckBox = new System.Windows.Forms.CheckBox();
            this._validationLabel = new System.Windows.Forms.Label();
            this._protocolGroup = new System.Windows.Forms.GroupBox();
            this._protocolTable = new System.Windows.Forms.TableLayoutPanel();
            this._nameLabel = new System.Windows.Forms.Label();
            this._commandFormatLabel = new System.Windows.Forms.Label();
            this._terminatorLabel = new System.Windows.Forms.Label();
            this._commandLabel = new System.Windows.Forms.Label();
            this._encodingLabel = new System.Windows.Forms.Label();
            this._responseFormatLabel = new System.Windows.Forms.Label();
            this._matchModeLabel = new System.Windows.Forms.Label();
            this._responseLabel = new System.Windows.Forms.Label();
            this._timeoutLabel = new System.Windows.Forms.Label();
            this._timeoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._timeoutUnitLabel = new System.Windows.Forms.Label();
            this._lastScanPortLabel = new System.Windows.Forms.Label();
            this._settingsGroup = new System.Windows.Forms.GroupBox();
            this._settingsPanel = new System.Windows.Forms.Panel();
            this._settingsCommandPanel = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this._timeoutNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._settingsGrid)).BeginInit();
            this._protocolGroup.SuspendLayout();
            this._protocolTable.SuspendLayout();
            this._timeoutPanel.SuspendLayout();
            this._settingsGroup.SuspendLayout();
            this._settingsPanel.SuspendLayout();
            this._settingsCommandPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _nameTextBox
            // 
            this._nameTextBox.AccessibleName = "设备名称";
            this._nameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._nameTextBox.Location = new System.Drawing.Point(187, 3);
            this._nameTextBox.Name = "_nameTextBox";
            this._nameTextBox.Size = new System.Drawing.Size(853, 35);
            this._nameTextBox.TabIndex = 1;
            // 
            // _enabledCheckBox
            // 
            this._enabledCheckBox.AccessibleName = "启用此模板";
            this._protocolTable.SetColumnSpan(this._enabledCheckBox, 2);
            this._enabledCheckBox.Location = new System.Drawing.Point(1046, 3);
            this._enabledCheckBox.Name = "_enabledCheckBox";
            this._enabledCheckBox.Size = new System.Drawing.Size(104, 24);
            this._enabledCheckBox.TabIndex = 2;
            this._enabledCheckBox.Text = "启用此模板";
            // 
            // _commandFormatComboBox
            // 
            this._commandFormatComboBox.AccessibleName = "命令格式";
            this._commandFormatComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._commandFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._commandFormatComboBox.Location = new System.Drawing.Point(187, 44);
            this._commandFormatComboBox.Name = "_commandFormatComboBox";
            this._commandFormatComboBox.Size = new System.Drawing.Size(853, 32);
            this._commandFormatComboBox.TabIndex = 4;
            // 
            // _commandTextBox
            // 
            this._commandTextBox.AccessibleName = "命令内容";
            this._commandTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._commandTextBox.Location = new System.Drawing.Point(187, 82);
            this._commandTextBox.Name = "_commandTextBox";
            this._commandTextBox.Size = new System.Drawing.Size(853, 35);
            this._commandTextBox.TabIndex = 8;
            // 
            // _terminatorComboBox
            // 
            this._terminatorComboBox.AccessibleName = "文本结束符";
            this._terminatorComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._terminatorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._terminatorComboBox.Location = new System.Drawing.Point(1206, 44);
            this._terminatorComboBox.Name = "_terminatorComboBox";
            this._terminatorComboBox.Size = new System.Drawing.Size(853, 32);
            this._terminatorComboBox.TabIndex = 6;
            // 
            // _responseFormatComboBox
            // 
            this._responseFormatComboBox.AccessibleName = "响应格式";
            this._responseFormatComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._responseFormatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._responseFormatComboBox.Location = new System.Drawing.Point(187, 123);
            this._responseFormatComboBox.Name = "_responseFormatComboBox";
            this._responseFormatComboBox.Size = new System.Drawing.Size(853, 32);
            this._responseFormatComboBox.TabIndex = 12;
            // 
            // _responseTextBox
            // 
            this._responseTextBox.AccessibleName = "预期响应";
            this._responseTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._responseTextBox.Location = new System.Drawing.Point(187, 161);
            this._responseTextBox.Name = "_responseTextBox";
            this._responseTextBox.Size = new System.Drawing.Size(853, 35);
            this._responseTextBox.TabIndex = 16;
            // 
            // _matchModeComboBox
            // 
            this._matchModeComboBox.AccessibleName = "匹配方式";
            this._matchModeComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._matchModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._matchModeComboBox.Location = new System.Drawing.Point(1206, 123);
            this._matchModeComboBox.Name = "_matchModeComboBox";
            this._matchModeComboBox.Size = new System.Drawing.Size(853, 32);
            this._matchModeComboBox.TabIndex = 14;
            // 
            // _encodingComboBox
            // 
            this._encodingComboBox.AccessibleName = "文本编码";
            this._encodingComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._encodingComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._encodingComboBox.Location = new System.Drawing.Point(1206, 82);
            this._encodingComboBox.Name = "_encodingComboBox";
            this._encodingComboBox.Size = new System.Drawing.Size(853, 32);
            this._encodingComboBox.TabIndex = 10;
            // 
            // _timeoutNumeric
            // 
            this._timeoutNumeric.AccessibleName = "超时时间毫秒";
            this._timeoutNumeric.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this._timeoutNumeric.Location = new System.Drawing.Point(3, 3);
            this._timeoutNumeric.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this._timeoutNumeric.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._timeoutNumeric.Name = "_timeoutNumeric";
            this._timeoutNumeric.Size = new System.Drawing.Size(110, 35);
            this._timeoutNumeric.TabIndex = 0;
            this._timeoutNumeric.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // _lastScanPortTextBox
            // 
            this._lastScanPortTextBox.AccessibleName = "上次扫描串口号";
            this._lastScanPortTextBox.BackColor = System.Drawing.SystemColors.Window;
            this._protocolTable.SetColumnSpan(this._lastScanPortTextBox, 3);
            this._lastScanPortTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lastScanPortTextBox.Location = new System.Drawing.Point(187, 208);
            this._lastScanPortTextBox.Name = "_lastScanPortTextBox";
            this._lastScanPortTextBox.ReadOnly = true;
            this._lastScanPortTextBox.Size = new System.Drawing.Size(1872, 35);
            this._lastScanPortTextBox.TabIndex = 20;
            // 
            // _settingsGrid
            // 
            this._settingsGrid.AccessibleName = "串口参数表格";
            this._settingsGrid.AllowUserToAddRows = false;
            this._settingsGrid.AllowUserToDeleteRows = false;
            this._settingsGrid.AllowUserToResizeRows = false;
            this._settingsGrid.ColumnHeadersHeight = 28;
            this._settingsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._settingsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._settingsGrid.Location = new System.Drawing.Point(0, 40);
            this._settingsGrid.MinimumSize = new System.Drawing.Size(0, 86);
            this._settingsGrid.MultiSelect = false;
            this._settingsGrid.Name = "_settingsGrid";
            this._settingsGrid.RowHeadersVisible = false;
            this._settingsGrid.RowHeadersWidth = 82;
            this._settingsGrid.RowTemplate.Height = 26;
            this._settingsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._settingsGrid.Size = new System.Drawing.Size(2062, 504);
            this._settingsGrid.TabIndex = 0;
            // 
            // _addSettingButton
            // 
            this._addSettingButton.AccessibleName = "添加串口参数";
            this._addSettingButton.AutoSize = true;
            this._addSettingButton.Location = new System.Drawing.Point(3, 3);
            this._addSettingButton.Name = "_addSettingButton";
            this._addSettingButton.Size = new System.Drawing.Size(176, 34);
            this._addSettingButton.TabIndex = 0;
            this._addSettingButton.Text = "添加 8N1 参数";
            // 
            // _removeSettingButton
            // 
            this._removeSettingButton.AccessibleName = "删除选中串口参数";
            this._removeSettingButton.AutoSize = true;
            this._removeSettingButton.Location = new System.Drawing.Point(185, 3);
            this._removeSettingButton.Name = "_removeSettingButton";
            this._removeSettingButton.Size = new System.Drawing.Size(164, 34);
            this._removeSettingButton.TabIndex = 1;
            this._removeSettingButton.Text = "删除选中参数";
            // 
            // _selectAllCheckBox
            // 
            this._selectAllCheckBox.AccessibleName = "全选串口参数";
            this._selectAllCheckBox.AutoSize = true;
            this._selectAllCheckBox.Location = new System.Drawing.Point(355, 3);
            this._selectAllCheckBox.Name = "_selectAllCheckBox";
            this._selectAllCheckBox.Size = new System.Drawing.Size(90, 28);
            this._selectAllCheckBox.TabIndex = 2;
            this._selectAllCheckBox.Text = "全选";
            // 
            // _selectCommonCheckBox
            // 
            this._selectCommonCheckBox.AccessibleName = "选择常用串口参数";
            this._selectCommonCheckBox.AutoSize = true;
            this._selectCommonCheckBox.Location = new System.Drawing.Point(451, 3);
            this._selectCommonCheckBox.Name = "_selectCommonCheckBox";
            this._selectCommonCheckBox.Size = new System.Drawing.Size(162, 28);
            this._selectCommonCheckBox.TabIndex = 3;
            this._selectCommonCheckBox.Text = "选择常用的";
            // 
            // _validationLabel
            // 
            this._validationLabel.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
            this._validationLabel.AutoEllipsis = true;
            this._validationLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._validationLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._validationLabel.Location = new System.Drawing.Point(8, 832);
            this._validationLabel.Name = "_validationLabel";
            this._validationLabel.Padding = new System.Windows.Forms.Padding(6);
            this._validationLabel.Size = new System.Drawing.Size(2078, 42);
            this._validationLabel.TabIndex = 1;
            // 
            // _protocolGroup
            // 
            this._protocolGroup.Controls.Add(this._protocolTable);
            this._protocolGroup.Dock = System.Windows.Forms.DockStyle.Top;
            this._protocolGroup.Location = new System.Drawing.Point(8, 8);
            this._protocolGroup.Name = "_protocolGroup";
            this._protocolGroup.Padding = new System.Windows.Forms.Padding(8);
            this._protocolGroup.Size = new System.Drawing.Size(2078, 236);
            this._protocolGroup.TabIndex = 2;
            this._protocolGroup.TabStop = false;
            this._protocolGroup.Text = "设备模板";
            // 
            // _protocolTable
            // 
            this._protocolTable.ColumnCount = 4;
            this._protocolTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._protocolTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._protocolTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this._protocolTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._protocolTable.Controls.Add(this._nameLabel, 0, 0);
            this._protocolTable.Controls.Add(this._nameTextBox, 1, 0);
            this._protocolTable.Controls.Add(this._enabledCheckBox, 2, 0);
            this._protocolTable.Controls.Add(this._commandFormatLabel, 0, 1);
            this._protocolTable.Controls.Add(this._commandFormatComboBox, 1, 1);
            this._protocolTable.Controls.Add(this._terminatorLabel, 2, 1);
            this._protocolTable.Controls.Add(this._terminatorComboBox, 3, 1);
            this._protocolTable.Controls.Add(this._commandLabel, 0, 2);
            this._protocolTable.Controls.Add(this._commandTextBox, 1, 2);
            this._protocolTable.Controls.Add(this._encodingLabel, 2, 2);
            this._protocolTable.Controls.Add(this._encodingComboBox, 3, 2);
            this._protocolTable.Controls.Add(this._responseFormatLabel, 0, 3);
            this._protocolTable.Controls.Add(this._responseFormatComboBox, 1, 3);
            this._protocolTable.Controls.Add(this._matchModeLabel, 2, 3);
            this._protocolTable.Controls.Add(this._matchModeComboBox, 3, 3);
            this._protocolTable.Controls.Add(this._responseLabel, 0, 4);
            this._protocolTable.Controls.Add(this._responseTextBox, 1, 4);
            this._protocolTable.Controls.Add(this._timeoutLabel, 2, 4);
            this._protocolTable.Controls.Add(this._timeoutPanel, 3, 4);
            this._protocolTable.Controls.Add(this._lastScanPortLabel, 0, 5);
            this._protocolTable.Controls.Add(this._lastScanPortTextBox, 1, 5);
            this._protocolTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this._protocolTable.Location = new System.Drawing.Point(8, 36);
            this._protocolTable.Name = "_protocolTable";
            this._protocolTable.RowCount = 7;
            this._protocolTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._protocolTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._protocolTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._protocolTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._protocolTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._protocolTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._protocolTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._protocolTable.Size = new System.Drawing.Size(2062, 192);
            this._protocolTable.TabIndex = 0;
            // 
            // _nameLabel
            // 
            this._nameLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._nameLabel.AutoSize = true;
            this._nameLabel.Location = new System.Drawing.Point(51, 10);
            this._nameLabel.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this._nameLabel.Name = "_nameLabel";
            this._nameLabel.Size = new System.Drawing.Size(130, 24);
            this._nameLabel.TabIndex = 0;
            this._nameLabel.Text = "设备名称：";
            // 
            // _commandFormatLabel
            // 
            this._commandFormatLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._commandFormatLabel.AutoSize = true;
            this._commandFormatLabel.Location = new System.Drawing.Point(51, 50);
            this._commandFormatLabel.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this._commandFormatLabel.Name = "_commandFormatLabel";
            this._commandFormatLabel.Size = new System.Drawing.Size(130, 24);
            this._commandFormatLabel.TabIndex = 3;
            this._commandFormatLabel.Text = "命令格式：";
            // 
            // _terminatorLabel
            // 
            this._terminatorLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._terminatorLabel.AutoSize = true;
            this._terminatorLabel.Location = new System.Drawing.Point(1046, 50);
            this._terminatorLabel.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this._terminatorLabel.Name = "_terminatorLabel";
            this._terminatorLabel.Size = new System.Drawing.Size(154, 24);
            this._terminatorLabel.TabIndex = 5;
            this._terminatorLabel.Text = "文本结束符：";
            // 
            // _commandLabel
            // 
            this._commandLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._commandLabel.AutoSize = true;
            this._commandLabel.Location = new System.Drawing.Point(51, 89);
            this._commandLabel.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this._commandLabel.Name = "_commandLabel";
            this._commandLabel.Size = new System.Drawing.Size(130, 24);
            this._commandLabel.TabIndex = 7;
            this._commandLabel.Text = "命令内容：";
            // 
            // _encodingLabel
            // 
            this._encodingLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._encodingLabel.AutoSize = true;
            this._encodingLabel.Location = new System.Drawing.Point(1070, 89);
            this._encodingLabel.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this._encodingLabel.Name = "_encodingLabel";
            this._encodingLabel.Size = new System.Drawing.Size(130, 24);
            this._encodingLabel.TabIndex = 9;
            this._encodingLabel.Text = "文本编码：";
            // 
            // _responseFormatLabel
            // 
            this._responseFormatLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._responseFormatLabel.AutoSize = true;
            this._responseFormatLabel.Location = new System.Drawing.Point(51, 129);
            this._responseFormatLabel.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this._responseFormatLabel.Name = "_responseFormatLabel";
            this._responseFormatLabel.Size = new System.Drawing.Size(130, 24);
            this._responseFormatLabel.TabIndex = 11;
            this._responseFormatLabel.Text = "响应格式：";
            // 
            // _matchModeLabel
            // 
            this._matchModeLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._matchModeLabel.AutoSize = true;
            this._matchModeLabel.Location = new System.Drawing.Point(1070, 129);
            this._matchModeLabel.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this._matchModeLabel.Name = "_matchModeLabel";
            this._matchModeLabel.Size = new System.Drawing.Size(130, 24);
            this._matchModeLabel.TabIndex = 13;
            this._matchModeLabel.Text = "匹配方式：";
            // 
            // _responseLabel
            // 
            this._responseLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._responseLabel.AutoSize = true;
            this._responseLabel.Location = new System.Drawing.Point(51, 171);
            this._responseLabel.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this._responseLabel.Name = "_responseLabel";
            this._responseLabel.Size = new System.Drawing.Size(130, 24);
            this._responseLabel.TabIndex = 15;
            this._responseLabel.Text = "预期响应：";
            // 
            // _timeoutLabel
            // 
            this._timeoutLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._timeoutLabel.AutoSize = true;
            this._timeoutLabel.Location = new System.Drawing.Point(1070, 171);
            this._timeoutLabel.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this._timeoutLabel.Name = "_timeoutLabel";
            this._timeoutLabel.Size = new System.Drawing.Size(130, 24);
            this._timeoutLabel.TabIndex = 17;
            this._timeoutLabel.Text = "超时时间：";
            // 
            // _timeoutPanel
            // 
            this._timeoutPanel.AutoSize = true;
            this._timeoutPanel.Controls.Add(this._timeoutNumeric);
            this._timeoutPanel.Controls.Add(this._timeoutUnitLabel);
            this._timeoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._timeoutPanel.Location = new System.Drawing.Point(1206, 161);
            this._timeoutPanel.Name = "_timeoutPanel";
            this._timeoutPanel.Size = new System.Drawing.Size(853, 41);
            this._timeoutPanel.TabIndex = 18;
            this._timeoutPanel.WrapContents = false;
            // 
            // _timeoutUnitLabel
            // 
            this._timeoutUnitLabel.AutoSize = true;
            this._timeoutUnitLabel.Location = new System.Drawing.Point(120, 7);
            this._timeoutUnitLabel.Margin = new System.Windows.Forms.Padding(4, 7, 3, 3);
            this._timeoutUnitLabel.Name = "_timeoutUnitLabel";
            this._timeoutUnitLabel.Size = new System.Drawing.Size(58, 24);
            this._timeoutUnitLabel.TabIndex = 1;
            this._timeoutUnitLabel.Text = "毫秒";
            // 
            // _lastScanPortLabel
            // 
            this._lastScanPortLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._lastScanPortLabel.AutoSize = true;
            this._lastScanPortLabel.Location = new System.Drawing.Point(3, 215);
            this._lastScanPortLabel.Margin = new System.Windows.Forms.Padding(3, 7, 3, 3);
            this._lastScanPortLabel.Name = "_lastScanPortLabel";
            this._lastScanPortLabel.Size = new System.Drawing.Size(178, 24);
            this._lastScanPortLabel.TabIndex = 19;
            this._lastScanPortLabel.Text = "上次扫描串口：";
            // 
            // _settingsGroup
            // 
            this._settingsGroup.Controls.Add(this._settingsPanel);
            this._settingsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._settingsGroup.Location = new System.Drawing.Point(8, 244);
            this._settingsGroup.Name = "_settingsGroup";
            this._settingsGroup.Padding = new System.Windows.Forms.Padding(8);
            this._settingsGroup.Size = new System.Drawing.Size(2078, 588);
            this._settingsGroup.TabIndex = 0;
            this._settingsGroup.TabStop = false;
            this._settingsGroup.Text = "串口参数（握手方式固定为 None）";
            // 
            // _settingsPanel
            // 
            this._settingsPanel.Controls.Add(this._settingsGrid);
            this._settingsPanel.Controls.Add(this._settingsCommandPanel);
            this._settingsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._settingsPanel.Location = new System.Drawing.Point(8, 36);
            this._settingsPanel.Name = "_settingsPanel";
            this._settingsPanel.Size = new System.Drawing.Size(2062, 544);
            this._settingsPanel.TabIndex = 0;
            // 
            // _settingsCommandPanel
            // 
            this._settingsCommandPanel.AutoSize = true;
            this._settingsCommandPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._settingsCommandPanel.Controls.Add(this._addSettingButton);
            this._settingsCommandPanel.Controls.Add(this._removeSettingButton);
            this._settingsCommandPanel.Controls.Add(this._selectAllCheckBox);
            this._settingsCommandPanel.Controls.Add(this._selectCommonCheckBox);
            this._settingsCommandPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._settingsCommandPanel.Location = new System.Drawing.Point(0, 0);
            this._settingsCommandPanel.Name = "_settingsCommandPanel";
            this._settingsCommandPanel.Size = new System.Drawing.Size(2062, 40);
            this._settingsCommandPanel.TabIndex = 1;
            // 
            // DeviceProfileEditor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this._settingsGroup);
            this.Controls.Add(this._validationLabel);
            this.Controls.Add(this._protocolGroup);
            this.MinimumSize = new System.Drawing.Size(0, 430);
            this.Name = "DeviceProfileEditor";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.Size = new System.Drawing.Size(2094, 882);
            ((System.ComponentModel.ISupportInitialize)(this._timeoutNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._settingsGrid)).EndInit();
            this._protocolGroup.ResumeLayout(false);
            this._protocolTable.ResumeLayout(false);
            this._protocolTable.PerformLayout();
            this._timeoutPanel.ResumeLayout(false);
            this._timeoutPanel.PerformLayout();
            this._settingsGroup.ResumeLayout(false);
            this._settingsPanel.ResumeLayout(false);
            this._settingsPanel.PerformLayout();
            this._settingsCommandPanel.ResumeLayout(false);
            this._settingsCommandPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
