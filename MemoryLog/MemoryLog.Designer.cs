namespace MemoryLog
{
    partial class MemoryLog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MemoryLog));
            this.tab1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.userMsgs = new System.Windows.Forms.Label();
            this.pbxBusy = new System.Windows.Forms.PictureBox();
            this.lstPrecinct = new System.Windows.Forms.ListBox();
            this.lblCount = new System.Windows.Forms.Label();
            this.lblStatus1 = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.userMsg = new System.Windows.Forms.Label();
            this.lblElection = new System.Windows.Forms.Label();
            this.lblElec = new System.Windows.Forms.Label();
            this.lblCopiedPrecints = new System.Windows.Forms.Label();
            this.btnCopyStick = new System.Windows.Forms.Button();
            this.SourceBtn = new System.Windows.Forms.Button();
            this.txtSource = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.cmbBoxElection = new System.Windows.Forms.ComboBox();
            this.chkFilter = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblvedate = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.startDate = new System.Windows.Forms.DateTimePicker();
            this.lblvsdate = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.endDate = new System.Windows.Forms.DateTimePicker();
            this.lblRuserMsg = new System.Windows.Forms.Label();
            this.lblvlog = new System.Windows.Forms.Label();
            this.lblvelection = new System.Windows.Forms.Label();
            this.lblvdestn = new System.Windows.Forms.Label();
            this.btnReport = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.btnRepDestn = new System.Windows.Forms.Button();
            this.txtRepDestn = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioSystem = new System.Windows.Forms.RadioButton();
            this.radioElection = new System.Windows.Forms.RadioButton();
            this.filterToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tab1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxBusy)).BeginInit();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.Controls.Add(this.tabPage1);
            this.tab1.Controls.Add(this.tabPage2);
            this.tab1.Font = new System.Drawing.Font("Verdana", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tab1.Location = new System.Drawing.Point(-1, -1);
            this.tab1.Margin = new System.Windows.Forms.Padding(2);
            this.tab1.Name = "tab1";
            this.tab1.SelectedIndex = 0;
            this.tab1.Size = new System.Drawing.Size(551, 286);
            this.tab1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.userMsgs);
            this.tabPage1.Controls.Add(this.pbxBusy);
            this.tabPage1.Controls.Add(this.lstPrecinct);
            this.tabPage1.Controls.Add(this.lblCount);
            this.tabPage1.Controls.Add(this.lblStatus1);
            this.tabPage1.Controls.Add(this.lblStatus);
            this.tabPage1.Controls.Add(this.userMsg);
            this.tabPage1.Controls.Add(this.lblElection);
            this.tabPage1.Controls.Add(this.lblElec);
            this.tabPage1.Controls.Add(this.lblCopiedPrecints);
            this.tabPage1.Controls.Add(this.btnCopyStick);
            this.tabPage1.Controls.Add(this.SourceBtn);
            this.tabPage1.Controls.Add(this.txtSource);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage1.Location = new System.Drawing.Point(4, 21);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage1.Size = new System.Drawing.Size(543, 261);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Log";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // userMsgs
            // 
            this.userMsgs.AutoSize = true;
            this.userMsgs.Location = new System.Drawing.Point(12, 300);
            this.userMsgs.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.userMsgs.Name = "userMsgs";
            this.userMsgs.Size = new System.Drawing.Size(61, 13);
            this.userMsgs.TabIndex = 15;
            this.userMsgs.Text = "userMsgs";
            // 
            // pbxBusy
            // 
            this.pbxBusy.Image = ((System.Drawing.Image)(resources.GetObject("pbxBusy.Image")));
            this.pbxBusy.Location = new System.Drawing.Point(436, 64);
            this.pbxBusy.Margin = new System.Windows.Forms.Padding(2);
            this.pbxBusy.Name = "pbxBusy";
            this.pbxBusy.Size = new System.Drawing.Size(100, 100);
            this.pbxBusy.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbxBusy.TabIndex = 14;
            this.pbxBusy.TabStop = false;
            this.pbxBusy.Visible = false;
            // 
            // lstPrecinct
            // 
            this.lstPrecinct.FormattingEnabled = true;
            this.lstPrecinct.ItemHeight = 12;
            this.lstPrecinct.Location = new System.Drawing.Point(24, 178);
            this.lstPrecinct.Margin = new System.Windows.Forms.Padding(2);
            this.lstPrecinct.Name = "lstPrecinct";
            this.lstPrecinct.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstPrecinct.Size = new System.Drawing.Size(229, 52);
            this.lstPrecinct.TabIndex = 13;
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCount.Location = new System.Drawing.Point(407, 211);
            this.lblCount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(12, 17);
            this.lblCount.TabIndex = 12;
            this.lblCount.Text = ".";
            this.lblCount.Visible = false;
            // 
            // lblStatus1
            // 
            this.lblStatus1.AutoSize = true;
            this.lblStatus1.Location = new System.Drawing.Point(205, 123);
            this.lblStatus1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStatus1.Name = "lblStatus1";
            this.lblStatus1.Size = new System.Drawing.Size(78, 13);
            this.lblStatus1.TabIndex = 11;
            this.lblStatus1.Text = "copyStatus1";
            this.lblStatus1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(113, 96);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(71, 13);
            this.lblStatus.TabIndex = 10;
            this.lblStatus.Text = "copyStatus";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // userMsg
            // 
            this.userMsg.AutoSize = true;
            this.userMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.userMsg.ForeColor = System.Drawing.Color.Navy;
            this.userMsg.Location = new System.Drawing.Point(105, 119);
            this.userMsg.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.userMsg.Name = "userMsg";
            this.userMsg.Size = new System.Drawing.Size(0, 17);
            this.userMsg.TabIndex = 9;
            // 
            // lblElection
            // 
            this.lblElection.AutoSize = true;
            this.lblElection.ForeColor = System.Drawing.Color.Black;
            this.lblElection.Location = new System.Drawing.Point(88, 151);
            this.lblElection.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblElection.Name = "lblElection";
            this.lblElection.Size = new System.Drawing.Size(52, 13);
            this.lblElection.TabIndex = 8;
            this.lblElection.Text = "election";
            // 
            // lblElec
            // 
            this.lblElec.AutoSize = true;
            this.lblElec.Location = new System.Drawing.Point(20, 152);
            this.lblElec.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblElec.Name = "lblElec";
            this.lblElec.Size = new System.Drawing.Size(54, 13);
            this.lblElec.TabIndex = 6;
            this.lblElec.Text = "Election:";
            // 
            // lblCopiedPrecints
            // 
            this.lblCopiedPrecints.AutoSize = true;
            this.lblCopiedPrecints.Location = new System.Drawing.Point(257, 214);
            this.lblCopiedPrecints.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCopiedPrecints.Name = "lblCopiedPrecints";
            this.lblCopiedPrecints.Size = new System.Drawing.Size(140, 13);
            this.lblCopiedPrecints.TabIndex = 5;
            this.lblCopiedPrecints.Text = "Total  Locations Copied:";
            this.lblCopiedPrecints.Visible = false;
            // 
            // btnCopyStick
            // 
            this.btnCopyStick.Location = new System.Drawing.Point(90, 55);
            this.btnCopyStick.Margin = new System.Windows.Forms.Padding(2);
            this.btnCopyStick.Name = "btnCopyStick";
            this.btnCopyStick.Size = new System.Drawing.Size(338, 26);
            this.btnCopyStick.TabIndex = 3;
            this.btnCopyStick.Text = "Start Copy";
            this.btnCopyStick.UseVisualStyleBackColor = true;
            this.btnCopyStick.Click += new System.EventHandler(this.btnCopyStick_Click_1);
            // 
            // SourceBtn
            // 
            this.SourceBtn.Location = new System.Drawing.Point(436, 25);
            this.SourceBtn.Margin = new System.Windows.Forms.Padding(2);
            this.SourceBtn.Name = "SourceBtn";
            this.SourceBtn.Size = new System.Drawing.Size(56, 24);
            this.SourceBtn.TabIndex = 2;
            this.SourceBtn.Text = "Browse";
            this.SourceBtn.UseVisualStyleBackColor = true;
            this.SourceBtn.Click += new System.EventHandler(this.SourceBtn_Click_1);
            // 
            // txtSource
            // 
            this.txtSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSource.Location = new System.Drawing.Point(90, 25);
            this.txtSource.Margin = new System.Windows.Forms.Padding(2);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(338, 26);
            this.txtSource.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 30);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.cmbBoxElection);
            this.tabPage2.Controls.Add(this.chkFilter);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.lblRuserMsg);
            this.tabPage2.Controls.Add(this.lblvlog);
            this.tabPage2.Controls.Add(this.lblvelection);
            this.tabPage2.Controls.Add(this.lblvdestn);
            this.tabPage2.Controls.Add(this.btnReport);
            this.tabPage2.Controls.Add(this.label6);
            this.tabPage2.Controls.Add(this.btnRepDestn);
            this.tabPage2.Controls.Add(this.txtRepDestn);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Font = new System.Drawing.Font("Tahoma", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage2.Location = new System.Drawing.Point(4, 21);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage2.Size = new System.Drawing.Size(543, 261);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Report";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // cmbBoxElection
            // 
            this.cmbBoxElection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoxElection.FormattingEnabled = true;
            this.cmbBoxElection.Location = new System.Drawing.Point(111, 72);
            this.cmbBoxElection.Margin = new System.Windows.Forms.Padding(2);
            this.cmbBoxElection.Name = "cmbBoxElection";
            this.cmbBoxElection.Size = new System.Drawing.Size(165, 20);
            this.cmbBoxElection.TabIndex = 18;
            this.cmbBoxElection.SelectedIndexChanged += new System.EventHandler(this.cmbBoxElection_SelectedIndexChanged);
            // 
            // chkFilter
            // 
            this.chkFilter.AutoSize = true;
            this.chkFilter.Location = new System.Drawing.Point(111, 132);
            this.chkFilter.Margin = new System.Windows.Forms.Padding(2);
            this.chkFilter.Name = "chkFilter";
            this.chkFilter.Size = new System.Drawing.Size(55, 17);
            this.chkFilter.TabIndex = 17;
            this.chkFilter.Text = "Filter";
            this.filterToolTip.SetToolTip(this.chkFilter, "@\"Created report based on start date and end date\"");
            this.chkFilter.UseVisualStyleBackColor = true;
            this.chkFilter.MouseHover += new System.EventHandler(this.chkFilter_MouseHover);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblvedate);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.startDate);
            this.groupBox2.Controls.Add(this.lblvsdate);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.endDate);
            this.groupBox2.Location = new System.Drawing.Point(111, 150);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(334, 55);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            // 
            // lblvedate
            // 
            this.lblvedate.AutoSize = true;
            this.lblvedate.ForeColor = System.Drawing.Color.Red;
            this.lblvedate.Location = new System.Drawing.Point(77, 37);
            this.lblvedate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblvedate.Name = "lblvedate";
            this.lblvedate.Size = new System.Drawing.Size(0, 13);
            this.lblvedate.TabIndex = 10;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 15);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Start Date";
            // 
            // startDate
            // 
            this.startDate.CustomFormat = "MM/dd/yyyy";
            this.startDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startDate.Location = new System.Drawing.Point(71, 15);
            this.startDate.Margin = new System.Windows.Forms.Padding(2);
            this.startDate.Name = "startDate";
            this.startDate.Size = new System.Drawing.Size(96, 20);
            this.startDate.TabIndex = 7;
            this.startDate.Value = new System.DateTime(2015, 2, 6, 0, 0, 0, 0);
            this.startDate.ValueChanged += new System.EventHandler(this.startDate_ValueChanged);
            this.startDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.startDate_KeyDown);
            // 
            // lblvsdate
            // 
            this.lblvsdate.AutoSize = true;
            this.lblvsdate.ForeColor = System.Drawing.Color.Red;
            this.lblvsdate.Location = new System.Drawing.Point(4, 37);
            this.lblvsdate.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblvsdate.Name = "lblvsdate";
            this.lblvsdate.Size = new System.Drawing.Size(0, 13);
            this.lblvsdate.TabIndex = 14;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(176, 15);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "End Date";
            // 
            // endDate
            // 
            this.endDate.CustomFormat = "MM/dd/yyyy";
            this.endDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endDate.Location = new System.Drawing.Point(233, 15);
            this.endDate.Margin = new System.Windows.Forms.Padding(2);
            this.endDate.Name = "endDate";
            this.endDate.Size = new System.Drawing.Size(97, 20);
            this.endDate.TabIndex = 6;
            this.endDate.Value = new System.DateTime(2015, 2, 6, 0, 0, 0, 0);
            this.endDate.ValueChanged += new System.EventHandler(this.endDate_ValueChanged);
            this.endDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.endDate_KeyDown);
            // 
            // lblRuserMsg
            // 
            this.lblRuserMsg.AutoSize = true;
            this.lblRuserMsg.ForeColor = System.Drawing.Color.Lime;
            this.lblRuserMsg.Location = new System.Drawing.Point(239, 266);
            this.lblRuserMsg.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblRuserMsg.Name = "lblRuserMsg";
            this.lblRuserMsg.Size = new System.Drawing.Size(0, 13);
            this.lblRuserMsg.TabIndex = 15;
            // 
            // lblvlog
            // 
            this.lblvlog.AutoSize = true;
            this.lblvlog.ForeColor = System.Drawing.Color.Red;
            this.lblvlog.Location = new System.Drawing.Point(287, 102);
            this.lblvlog.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblvlog.Name = "lblvlog";
            this.lblvlog.Size = new System.Drawing.Size(0, 13);
            this.lblvlog.TabIndex = 13;
            // 
            // lblvelection
            // 
            this.lblvelection.AutoSize = true;
            this.lblvelection.ForeColor = System.Drawing.Color.Red;
            this.lblvelection.Location = new System.Drawing.Point(115, 98);
            this.lblvelection.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblvelection.Name = "lblvelection";
            this.lblvelection.Size = new System.Drawing.Size(0, 13);
            this.lblvelection.TabIndex = 12;
            // 
            // lblvdestn
            // 
            this.lblvdestn.AutoSize = true;
            this.lblvdestn.ForeColor = System.Drawing.Color.Red;
            this.lblvdestn.Location = new System.Drawing.Point(111, 57);
            this.lblvdestn.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblvdestn.Name = "lblvdestn";
            this.lblvdestn.Size = new System.Drawing.Size(0, 13);
            this.lblvdestn.TabIndex = 11;
            // 
            // btnReport
            // 
            this.btnReport.Location = new System.Drawing.Point(129, 217);
            this.btnReport.Margin = new System.Windows.Forms.Padding(2);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(290, 24);
            this.btnReport.TabIndex = 10;
            this.btnReport.Text = "Generate Excel Report";
            this.btnReport.UseVisualStyleBackColor = true;
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click_1);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(53, 73);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Election";
            // 
            // btnRepDestn
            // 
            this.btnRepDestn.Location = new System.Drawing.Point(443, 30);
            this.btnRepDestn.Margin = new System.Windows.Forms.Padding(2);
            this.btnRepDestn.Name = "btnRepDestn";
            this.btnRepDestn.Size = new System.Drawing.Size(56, 24);
            this.btnRepDestn.TabIndex = 3;
            this.btnRepDestn.Text = "Browse";
            this.btnRepDestn.UseVisualStyleBackColor = true;
            this.btnRepDestn.Click += new System.EventHandler(this.btnRepDestn_Click);
            // 
            // txtRepDestn
            // 
            this.txtRepDestn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.txtRepDestn.Location = new System.Drawing.Point(111, 30);
            this.txtRepDestn.Margin = new System.Windows.Forms.Padding(2);
            this.txtRepDestn.Name = "txtRepDestn";
            this.txtRepDestn.ReadOnly = true;
            this.txtRepDestn.Size = new System.Drawing.Size(330, 26);
            this.txtRepDestn.TabIndex = 2;
            this.txtRepDestn.TextChanged += new System.EventHandler(this.txtRepDestn_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(4, 33);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Destination Folder";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioSystem);
            this.groupBox1.Controls.Add(this.radioElection);
            this.groupBox1.Location = new System.Drawing.Point(290, 64);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(152, 36);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // radioSystem
            // 
            this.radioSystem.AutoSize = true;
            this.radioSystem.Location = new System.Drawing.Point(86, 12);
            this.radioSystem.Margin = new System.Windows.Forms.Padding(2);
            this.radioSystem.Name = "radioSystem";
            this.radioSystem.Size = new System.Drawing.Size(68, 17);
            this.radioSystem.TabIndex = 1;
            this.radioSystem.TabStop = true;
            this.radioSystem.Text = "System";
            this.radioSystem.UseVisualStyleBackColor = true;
            this.radioSystem.CheckedChanged += new System.EventHandler(this.radioSystem_CheckedChanged);
            // 
            // radioElection
            // 
            this.radioElection.AutoSize = true;
            this.radioElection.Location = new System.Drawing.Point(12, 12);
            this.radioElection.Margin = new System.Windows.Forms.Padding(2);
            this.radioElection.Name = "radioElection";
            this.radioElection.Size = new System.Drawing.Size(69, 17);
            this.radioElection.TabIndex = 0;
            this.radioElection.TabStop = true;
            this.radioElection.Text = "Election";
            this.radioElection.UseVisualStyleBackColor = true;
            this.radioElection.CheckedChanged += new System.EventHandler(this.radioElection_CheckedChanged);
            // 
            // MemoryLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 286);
            this.Controls.Add(this.tab1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MemoryLog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Memory Log";
            this.tab1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxBusy)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tab1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnCopyStick;
        private System.Windows.Forms.Button SourceBtn;
        private System.Windows.Forms.TextBox txtSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label userMsg;
        private System.Windows.Forms.Label lblElection;
        private System.Windows.Forms.Label lblElec;
        private System.Windows.Forms.Label lblCopiedPrecints;
        private System.Windows.Forms.Button btnRepDestn;
        private System.Windows.Forms.TextBox txtRepDestn;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnReport;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker startDate;
        private System.Windows.Forms.DateTimePicker endDate;
        private System.Windows.Forms.ListBox cmbElection;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.RadioButton radioSystem;
        private System.Windows.Forms.RadioButton radioElection;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblStatus1;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Label lblvdestn;
        private System.Windows.Forms.Label lblvsdate;
        private System.Windows.Forms.Label lblvlog;
        private System.Windows.Forms.Label lblvelection;
        private System.Windows.Forms.Label lblRuserMsg;
        private System.Windows.Forms.ListBox lstPrecinct;
        private System.Windows.Forms.CheckBox chkFilter;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblvedate;
        private System.Windows.Forms.ToolTip filterToolTip;
        private System.Windows.Forms.ComboBox cmbBoxElection;
        private System.Windows.Forms.PictureBox pbxBusy;
        private System.Windows.Forms.Label userMsgs;
    }
}

