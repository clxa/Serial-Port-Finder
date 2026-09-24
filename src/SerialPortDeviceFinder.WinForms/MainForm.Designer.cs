namespace SerialPortDeviceFinder.WinForms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.ListBox _profileListBox;
        private System.Windows.Forms.FlowLayoutPanel _profileCommandPanel;
        private System.Windows.Forms.Button _addProfileButton;
        private System.Windows.Forms.Button _copyProfileButton;
        private System.Windows.Forms.Button _deleteProfileButton;
        private DeviceProfileEditor _editor;
        private System.Windows.Forms.DataGridView _resultsGrid;
        private System.Windows.Forms.Label _resultsEmptyLabel;
        private System.Windows.Forms.TextBox _logTextBox;
        private System.Windows.Forms.Label _globalValidationLabel;
        private System.Windows.Forms.ToolStrip _toolStrip;
        private System.Windows.Forms.ToolStripButton _startButton;
        private System.Windows.Forms.ToolStripButton _stopButton;
        private System.Windows.Forms.ToolStripSeparator _toolStripSeparator;
        private System.Windows.Forms.ToolStripButton _saveButton;
        private System.Windows.Forms.ToolStripButton _loadButton;
        private System.Windows.Forms.StatusStrip _statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel _statusLabel;
        private System.Windows.Forms.SplitContainer _rootSplit;
        private System.Windows.Forms.SplitContainer _verticalSplit;
        private System.Windows.Forms.SplitContainer _lowerSplit;
        private System.Windows.Forms.SplitContainer _summarySplit;
        private System.Windows.Forms.Panel _profileListPanel;
        private System.Windows.Forms.Panel _contentPanel;
        private System.Windows.Forms.Label _profileListLabel;
        private System.Windows.Forms.GroupBox _resultsGroup;
        private System.Windows.Forms.GroupBox _logGroup;
        private System.Windows.Forms.GroupBox _summaryGroup;
        private System.Windows.Forms.TextBox _summaryTextBox;

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
            this._profileListBox = new System.Windows.Forms.ListBox();
            this._profileCommandPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._addProfileButton = new System.Windows.Forms.Button();
            this._copyProfileButton = new System.Windows.Forms.Button();
            this._deleteProfileButton = new System.Windows.Forms.Button();
            this._editor = new SerialPortDeviceFinder.WinForms.DeviceProfileEditor();
            this._resultsGrid = new System.Windows.Forms.DataGridView();
            this._resultsEmptyLabel = new System.Windows.Forms.Label();
            this._logTextBox = new System.Windows.Forms.TextBox();
            this._globalValidationLabel = new System.Windows.Forms.Label();
            this._toolStrip = new System.Windows.Forms.ToolStrip();
            this._startButton = new System.Windows.Forms.ToolStripButton();
            this._stopButton = new System.Windows.Forms.ToolStripButton();
            this._toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this._saveButton = new System.Windows.Forms.ToolStripButton();
            this._loadButton = new System.Windows.Forms.ToolStripButton();
            this._statusStrip = new System.Windows.Forms.StatusStrip();
            this._statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._rootSplit = new System.Windows.Forms.SplitContainer();
            this._profileListPanel = new System.Windows.Forms.Panel();
            this._profileListLabel = new System.Windows.Forms.Label();
            this._contentPanel = new System.Windows.Forms.Panel();
            this._verticalSplit = new System.Windows.Forms.SplitContainer();
            this._summarySplit = new System.Windows.Forms.SplitContainer();
            this._lowerSplit = new System.Windows.Forms.SplitContainer();
            this._resultsGroup = new System.Windows.Forms.GroupBox();
            this._logGroup = new System.Windows.Forms.GroupBox();
            this._summaryGroup = new System.Windows.Forms.GroupBox();
            this._summaryTextBox = new System.Windows.Forms.TextBox();
            this._profileCommandPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._resultsGrid)).BeginInit();
            this._toolStrip.SuspendLayout();
            this._statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._rootSplit)).BeginInit();
            this._rootSplit.Panel1.SuspendLayout();
            this._rootSplit.Panel2.SuspendLayout();
            this._rootSplit.SuspendLayout();
            this._profileListPanel.SuspendLayout();
            this._contentPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._verticalSplit)).BeginInit();
            this._verticalSplit.Panel1.SuspendLayout();
            this._verticalSplit.Panel2.SuspendLayout();
            this._verticalSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._summarySplit)).BeginInit();
            this._summarySplit.Panel1.SuspendLayout();
            this._summarySplit.Panel2.SuspendLayout();
            this._summarySplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._lowerSplit)).BeginInit();
            this._lowerSplit.Panel1.SuspendLayout();
            this._lowerSplit.Panel2.SuspendLayout();
            this._lowerSplit.SuspendLayout();
            this._resultsGroup.SuspendLayout();
            this._logGroup.SuspendLayout();
            this._summaryGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // _profileListBox
            // 
            this._profileListBox.AccessibleName = "设备模板列表";
            this._profileListBox.DisplayMember = "Name";
            this._profileListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._profileListBox.ItemHeight = 24;
            this._profileListBox.Location = new System.Drawing.Point(8, 32);
            this._profileListBox.Name = "_profileListBox";
            this._profileListBox.Size = new System.Drawing.Size(402, 607);
            this._profileListBox.TabIndex = 0;
            // 
            // _profileCommandPanel
            // 
            this._profileCommandPanel.AutoSize = true;
            this._profileCommandPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._profileCommandPanel.Controls.Add(this._addProfileButton);
            this._profileCommandPanel.Controls.Add(this._copyProfileButton);
            this._profileCommandPanel.Controls.Add(this._deleteProfileButton);
            this._profileCommandPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._profileCommandPanel.Location = new System.Drawing.Point(8, 639);
            this._profileCommandPanel.Name = "_profileCommandPanel";
            this._profileCommandPanel.Size = new System.Drawing.Size(402, 40);
            this._profileCommandPanel.TabIndex = 1;
            // 
            // _addProfileButton
            // 
            this._addProfileButton.AccessibleName = "新增设备模板";
            this._addProfileButton.AutoSize = true;
            this._addProfileButton.Location = new System.Drawing.Point(3, 3);
            this._addProfileButton.Name = "_addProfileButton";
            this._addProfileButton.Size = new System.Drawing.Size(75, 34);
            this._addProfileButton.TabIndex = 0;
            this._addProfileButton.Text = "新增";
            // 
            // _copyProfileButton
            // 
            this._copyProfileButton.AccessibleName = "复制当前设备模板";
            this._copyProfileButton.AutoSize = true;
            this._copyProfileButton.Location = new System.Drawing.Point(84, 3);
            this._copyProfileButton.Name = "_copyProfileButton";
            this._copyProfileButton.Size = new System.Drawing.Size(75, 34);
            this._copyProfileButton.TabIndex = 1;
            this._copyProfileButton.Text = "复制";
            // 
            // _deleteProfileButton
            // 
            this._deleteProfileButton.AccessibleName = "删除当前设备模板";
            this._deleteProfileButton.AutoSize = true;
            this._deleteProfileButton.Location = new System.Drawing.Point(165, 3);
            this._deleteProfileButton.Name = "_deleteProfileButton";
            this._deleteProfileButton.Size = new System.Drawing.Size(75, 34);
            this._deleteProfileButton.TabIndex = 2;
            this._deleteProfileButton.Text = "删除";
            // 
            // _editor
            // 
            this._editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this._editor.Location = new System.Drawing.Point(0, 0);
            this._editor.MinimumSize = new System.Drawing.Size(0, 430);
            this._editor.Name = "_editor";
            this._editor.Padding = new System.Windows.Forms.Padding(8);
            this._editor.Size = new System.Drawing.Size(832, 430);
            this._editor.TabIndex = 0;
            // 
            // _resultsGrid
            // 
            this._resultsGrid.AccessibleName = "匹配结果表格";
            this._resultsGrid.AllowUserToAddRows = false;
            this._resultsGrid.AllowUserToDeleteRows = false;
            this._resultsGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._resultsGrid.ColumnHeadersHeight = 46;
            this._resultsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._resultsGrid.Location = new System.Drawing.Point(8, 36);
            this._resultsGrid.Name = "_resultsGrid";
            this._resultsGrid.ReadOnly = true;
            this._resultsGrid.RowHeadersVisible = false;
            this._resultsGrid.RowHeadersWidth = 82;
            this._resultsGrid.Size = new System.Drawing.Size(261, 114);
            this._resultsGrid.TabIndex = 1;
            // 
            // _resultsEmptyLabel
            // 
            this._resultsEmptyLabel.AccessibleName = "匹配结果为空提示";
            this._resultsEmptyLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._resultsEmptyLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this._resultsEmptyLabel.Location = new System.Drawing.Point(8, 36);
            this._resultsEmptyLabel.Name = "_resultsEmptyLabel";
            this._resultsEmptyLabel.Size = new System.Drawing.Size(261, 114);
            this._resultsEmptyLabel.TabIndex = 0;
            this._resultsEmptyLabel.Text = "尚未开始搜索。";
            this._resultsEmptyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _logTextBox
            // 
            this._logTextBox.AccessibleName = "扫描日志";
            this._logTextBox.BackColor = System.Drawing.SystemColors.Window;
            this._logTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._logTextBox.Location = new System.Drawing.Point(8, 36);
            this._logTextBox.Multiline = true;
            this._logTextBox.Name = "_logTextBox";
            this._logTextBox.ReadOnly = true;
            this._logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._logTextBox.Size = new System.Drawing.Size(261, 113);
            this._logTextBox.TabIndex = 0;
            // 
            // _globalValidationLabel
            // 
            this._globalValidationLabel.AccessibleName = "所有设备模板校验结果";
            this._globalValidationLabel.AutoEllipsis = true;
            this._globalValidationLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._globalValidationLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._globalValidationLabel.Location = new System.Drawing.Point(0, 0);
            this._globalValidationLabel.Name = "_globalValidationLabel";
            this._globalValidationLabel.Padding = new System.Windows.Forms.Padding(8);
            this._globalValidationLabel.Size = new System.Drawing.Size(832, 42);
            this._globalValidationLabel.TabIndex = 1;
            // 
            // _toolStrip
            // 
            this._toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._toolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._startButton,
            this._stopButton,
            this._toolStripSeparator,
            this._saveButton,
            this._loadButton});
            this._toolStrip.Location = new System.Drawing.Point(0, 0);
            this._toolStrip.Name = "_toolStrip";
            this._toolStrip.Size = new System.Drawing.Size(1254, 41);
            this._toolStrip.TabIndex = 2;
            // 
            // _startButton
            // 
            this._startButton.AccessibleName = "开始搜索";
            this._startButton.Name = "_startButton";
            this._startButton.Size = new System.Drawing.Size(114, 35);
            this._startButton.Text = "开始搜索";
            this._startButton.ToolTipText = "用所有启用模板顺序搜索当前 COM 端口";
            // 
            // _stopButton
            // 
            this._stopButton.AccessibleName = "停止搜索";
            this._stopButton.Name = "_stopButton";
            this._stopButton.Size = new System.Drawing.Size(114, 35);
            this._stopButton.Text = "停止搜索";
            this._stopButton.ToolTipText = "取消尚未开始的搜索任务";
            // 
            // _toolStripSeparator
            // 
            this._toolStripSeparator.Name = "_toolStripSeparator";
            this._toolStripSeparator.Size = new System.Drawing.Size(6, 41);
            // 
            // _saveButton
            // 
            this._saveButton.AccessibleName = "保存配置";
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new System.Drawing.Size(114, 35);
            this._saveButton.Text = "保存配置";
            this._saveButton.ToolTipText = "保存模板到程序目录的 devices.json";
            // 
            // _loadButton
            // 
            this._loadButton.AccessibleName = "加载配置";
            this._loadButton.Name = "_loadButton";
            this._loadButton.Size = new System.Drawing.Size(114, 35);
            this._loadButton.Text = "加载配置";
            this._loadButton.ToolTipText = "从程序目录的 devices.json 重新加载模板";
            // 
            // _statusStrip
            // 
            this._statusStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._statusLabel});
            this._statusStrip.Location = new System.Drawing.Point(0, 728);
            this._statusStrip.Name = "_statusStrip";
            this._statusStrip.Size = new System.Drawing.Size(1254, 41);
            this._statusStrip.TabIndex = 1;
            // 
            // _statusLabel
            // 
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(278, 31);
            this._statusLabel.Text = "请新增或加载设备模板。";
            // 
            // _rootSplit
            // 
            this._rootSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rootSplit.Location = new System.Drawing.Point(0, 41);
            this._rootSplit.Name = "_rootSplit";
            // 
            // _rootSplit.Panel1
            // 
            this._rootSplit.Panel1.Controls.Add(this._profileListPanel);
            // 
            // _rootSplit.Panel2
            // 
            this._rootSplit.Panel2.Controls.Add(this._contentPanel);
            this._rootSplit.Size = new System.Drawing.Size(1254, 687);
            this._rootSplit.SplitterDistance = 418;
            this._rootSplit.TabIndex = 0;
            // 
            // _profileListPanel
            // 
            this._profileListPanel.Controls.Add(this._profileListBox);
            this._profileListPanel.Controls.Add(this._profileCommandPanel);
            this._profileListPanel.Controls.Add(this._profileListLabel);
            this._profileListPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._profileListPanel.Location = new System.Drawing.Point(0, 0);
            this._profileListPanel.Name = "_profileListPanel";
            this._profileListPanel.Padding = new System.Windows.Forms.Padding(8);
            this._profileListPanel.Size = new System.Drawing.Size(418, 687);
            this._profileListPanel.TabIndex = 0;
            // 
            // _profileListLabel
            // 
            this._profileListLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._profileListLabel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this._profileListLabel.Location = new System.Drawing.Point(8, 8);
            this._profileListLabel.Name = "_profileListLabel";
            this._profileListLabel.Size = new System.Drawing.Size(402, 24);
            this._profileListLabel.TabIndex = 2;
            this._profileListLabel.Text = "设备模板";
            // 
            // _contentPanel
            // 
            this._contentPanel.Controls.Add(this._verticalSplit);
            this._contentPanel.Controls.Add(this._globalValidationLabel);
            this._contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._contentPanel.Location = new System.Drawing.Point(0, 0);
            this._contentPanel.Name = "_contentPanel";
            this._contentPanel.Size = new System.Drawing.Size(832, 687);
            this._contentPanel.TabIndex = 0;
            // 
            // _verticalSplit
            // 
            this._verticalSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this._verticalSplit.Location = new System.Drawing.Point(0, 42);
            this._verticalSplit.Name = "_verticalSplit";
            this._verticalSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _verticalSplit.Panel1
            // 
            this._verticalSplit.Panel1.Controls.Add(this._editor);
            // 
            // _verticalSplit.Panel2
            // 
            this._verticalSplit.Panel2.Controls.Add(this._summarySplit);
            this._verticalSplit.Size = new System.Drawing.Size(832, 645);
            this._verticalSplit.SplitterDistance = 322;
            this._verticalSplit.TabIndex = 0;
            // 
            // _summarySplit
            // 
            this._summarySplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this._summarySplit.Location = new System.Drawing.Point(0, 0);
            this._summarySplit.Name = "_summarySplit";
            // 
            // _summarySplit.Panel1
            // 
            this._summarySplit.Panel1.Controls.Add(this._lowerSplit);
            // 
            // _summarySplit.Panel2
            // 
            this._summarySplit.Panel2.Controls.Add(this._summaryGroup);
            this._summarySplit.Size = new System.Drawing.Size(832, 319);
            this._summarySplit.SplitterDistance = 277;
            this._summarySplit.TabIndex = 0;
            // 
            // _lowerSplit
            // 
            this._lowerSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lowerSplit.Location = new System.Drawing.Point(0, 0);
            this._lowerSplit.Name = "_lowerSplit";
            this._lowerSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _lowerSplit.Panel1
            // 
            this._lowerSplit.Panel1.Controls.Add(this._resultsGroup);
            // 
            // _lowerSplit.Panel2
            // 
            this._lowerSplit.Panel2.Controls.Add(this._logGroup);
            this._lowerSplit.Size = new System.Drawing.Size(277, 319);
            this._lowerSplit.SplitterDistance = 158;
            this._lowerSplit.TabIndex = 0;
            // 
            // _resultsGroup
            // 
            this._resultsGroup.Controls.Add(this._resultsEmptyLabel);
            this._resultsGroup.Controls.Add(this._resultsGrid);
            this._resultsGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._resultsGroup.Location = new System.Drawing.Point(0, 0);
            this._resultsGroup.Name = "_resultsGroup";
            this._resultsGroup.Padding = new System.Windows.Forms.Padding(8);
            this._resultsGroup.Size = new System.Drawing.Size(277, 158);
            this._resultsGroup.TabIndex = 0;
            this._resultsGroup.TabStop = false;
            this._resultsGroup.Text = "匹配结果";
            // 
            // _logGroup
            // 
            this._logGroup.Controls.Add(this._logTextBox);
            this._logGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._logGroup.Location = new System.Drawing.Point(0, 0);
            this._logGroup.Name = "_logGroup";
            this._logGroup.Padding = new System.Windows.Forms.Padding(8);
            this._logGroup.Size = new System.Drawing.Size(277, 157);
            this._logGroup.TabIndex = 0;
            this._logGroup.TabStop = false;
            this._logGroup.Text = "扫描日志";
            // 
            // _summaryGroup
            // 
            this._summaryGroup.Controls.Add(this._summaryTextBox);
            this._summaryGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this._summaryGroup.Location = new System.Drawing.Point(0, 0);
            this._summaryGroup.Name = "_summaryGroup";
            this._summaryGroup.Padding = new System.Windows.Forms.Padding(8);
            this._summaryGroup.Size = new System.Drawing.Size(551, 319);
            this._summaryGroup.TabIndex = 0;
            this._summaryGroup.TabStop = false;
            this._summaryGroup.Text = "搜索到的设备";
            // 
            // _summaryTextBox
            // 
            this._summaryTextBox.AccessibleName = "搜索到的设备汇总";
            this._summaryTextBox.BackColor = System.Drawing.SystemColors.Window;
            this._summaryTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._summaryTextBox.Location = new System.Drawing.Point(8, 36);
            this._summaryTextBox.Multiline = true;
            this._summaryTextBox.Name = "_summaryTextBox";
            this._summaryTextBox.ReadOnly = true;
            this._summaryTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._summaryTextBox.Size = new System.Drawing.Size(535, 275);
            this._summaryTextBox.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1254, 769);
            this.Controls.Add(this._rootSplit);
            this.Controls.Add(this._statusStrip);
            this.Controls.Add(this._toolStrip);
            this.MinimumSize = new System.Drawing.Size(1050, 700);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "串口设备搜索器";
            this._profileCommandPanel.ResumeLayout(false);
            this._profileCommandPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._resultsGrid)).EndInit();
            this._toolStrip.ResumeLayout(false);
            this._toolStrip.PerformLayout();
            this._statusStrip.ResumeLayout(false);
            this._statusStrip.PerformLayout();
            this._rootSplit.Panel1.ResumeLayout(false);
            this._rootSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._rootSplit)).EndInit();
            this._rootSplit.ResumeLayout(false);
            this._profileListPanel.ResumeLayout(false);
            this._profileListPanel.PerformLayout();
            this._contentPanel.ResumeLayout(false);
            this._verticalSplit.Panel1.ResumeLayout(false);
            this._verticalSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._verticalSplit)).EndInit();
            this._verticalSplit.ResumeLayout(false);
            this._summarySplit.Panel1.ResumeLayout(false);
            this._summarySplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._summarySplit)).EndInit();
            this._summarySplit.ResumeLayout(false);
            this._lowerSplit.Panel1.ResumeLayout(false);
            this._lowerSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._lowerSplit)).EndInit();
            this._lowerSplit.ResumeLayout(false);
            this._resultsGroup.ResumeLayout(false);
            this._logGroup.ResumeLayout(false);
            this._logGroup.PerformLayout();
            this._summaryGroup.ResumeLayout(false);
            this._summaryGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
