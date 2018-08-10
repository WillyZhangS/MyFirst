namespace AutoFrame
{
    partial class Form_Machine
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Machine));
            this.roundPanel1 = new AutoFrameUI.RoundPanel(this.components);
            this.label_software_life = new System.Windows.Forms.Label();
            this.label_machine_life = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label_last_date = new System.Windows.Forms.Label();
            this.label_version = new System.Windows.Forms.Label();
            this.label_build_date = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.roundPanel2 = new AutoFrameUI.RoundPanel(this.components);
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.roundPanel3 = new AutoFrameUI.RoundPanel(this.components);
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.roundPanel4 = new AutoFrameUI.RoundPanel(this.components);
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.roundPanel1.SuspendLayout();
            this.roundPanel2.SuspendLayout();
            this.roundPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.roundPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // roundPanel1
            // 
            this.roundPanel1._setRoundRadius = 8;
            this.roundPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.roundPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(235)))));
            this.roundPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.roundPanel1.Controls.Add(this.label_software_life);
            this.roundPanel1.Controls.Add(this.label_machine_life);
            this.roundPanel1.Controls.Add(this.label6);
            this.roundPanel1.Controls.Add(this.label7);
            this.roundPanel1.Controls.Add(this.label_last_date);
            this.roundPanel1.Controls.Add(this.label_version);
            this.roundPanel1.Controls.Add(this.label_build_date);
            this.roundPanel1.Controls.Add(this.label3);
            this.roundPanel1.Controls.Add(this.label2);
            this.roundPanel1.Controls.Add(this.label1);
            this.roundPanel1.Location = new System.Drawing.Point(5, 5);
            this.roundPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.roundPanel1.Name = "roundPanel1";
            this.roundPanel1.Size = new System.Drawing.Size(702, 144);
            this.roundPanel1.TabIndex = 0;
            // 
            // label_software_life
            // 
            this.label_software_life.AutoSize = true;
            this.label_software_life.Location = new System.Drawing.Point(193, 109);
            this.label_software_life.Name = "label_software_life";
            this.label_software_life.Size = new System.Drawing.Size(37, 21);
            this.label_software_life.TabIndex = 6;
            this.label_software_life.Text = "134";
            // 
            // label_machine_life
            // 
            this.label_machine_life.AutoSize = true;
            this.label_machine_life.Location = new System.Drawing.Point(193, 86);
            this.label_machine_life.Name = "label_machine_life";
            this.label_machine_life.Size = new System.Drawing.Size(46, 21);
            this.label_machine_life.TabIndex = 5;
            this.label_machine_life.Text = "5784";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(27, 109);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(157, 21);
            this.label6.TabIndex = 4;
            this.label6.Text = "Total Software Life:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(27, 87);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(160, 21);
            this.label7.TabIndex = 4;
            this.label7.Text = "Total Machine Life :";
            // 
            // label_last_date
            // 
            this.label_last_date.AutoSize = true;
            this.label_last_date.Location = new System.Drawing.Point(143, 65);
            this.label_last_date.Name = "label_last_date";
            this.label_last_date.Size = new System.Drawing.Size(69, 21);
            this.label_last_date.TabIndex = 3;
            this.label_last_date.Text = "7/15/14";
            // 
            // label_version
            // 
            this.label_version.AutoSize = true;
            this.label_version.Location = new System.Drawing.Point(118, 43);
            this.label_version.Name = "label_version";
            this.label_version.Size = new System.Drawing.Size(63, 21);
            this.label_version.TabIndex = 2;
            this.label_version.Text = "1.001.1";
            // 
            // label_build_date
            // 
            this.label_build_date.AutoSize = true;
            this.label_build_date.Location = new System.Drawing.Point(118, 21);
            this.label_build_date.Name = "label_build_date";
            this.label_build_date.Size = new System.Drawing.Size(69, 21);
            this.label_build_date.TabIndex = 1;
            this.label_build_date.Text = "3/12/14";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 21);
            this.label3.TabIndex = 0;
            this.label3.Text = "SW Updated:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 21);
            this.label2.TabIndex = 0;
            this.label2.Text = "SW Rev:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Build Data:";
            // 
            // roundPanel2
            // 
            this.roundPanel2._setRoundRadius = 8;
            this.roundPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.roundPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(235)))));
            this.roundPanel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.roundPanel2.Controls.Add(this.label16);
            this.roundPanel2.Controls.Add(this.label15);
            this.roundPanel2.Controls.Add(this.label14);
            this.roundPanel2.Controls.Add(this.label11);
            this.roundPanel2.Controls.Add(this.label12);
            this.roundPanel2.Controls.Add(this.label13);
            this.roundPanel2.Location = new System.Drawing.Point(725, 5);
            this.roundPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.roundPanel2.Name = "roundPanel2";
            this.roundPanel2.Size = new System.Drawing.Size(616, 144);
            this.roundPanel2.TabIndex = 0;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.label16.Location = new System.Drawing.Point(190, 88);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(70, 21);
            this.label16.TabIndex = 1;
            this.label16.Text = "62.4 PSI";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.label15.Location = new System.Drawing.Point(212, 66);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(70, 21);
            this.label15.TabIndex = 1;
            this.label15.Text = "25.2 PSI";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.label14.Location = new System.Drawing.Point(178, 44);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(70, 21);
            this.label14.TabIndex = 1;
            this.label14.Text = "94.2 PSI";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(73, 43);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(99, 21);
            this.label11.TabIndex = 0;
            this.label11.Text = "Air Press In:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(73, 65);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(133, 21);
            this.label12.TabIndex = 0;
            this.label12.Text = "Air Press Clamp:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(73, 86);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(111, 21);
            this.label13.TabIndex = 0;
            this.label13.Text = "Air Press Rail:";
            // 
            // roundPanel3
            // 
            this.roundPanel3._setRoundRadius = 8;
            this.roundPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.roundPanel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(235)))));
            this.roundPanel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.roundPanel3.Controls.Add(this.dataGridView1);
            this.roundPanel3.Location = new System.Drawing.Point(5, 165);
            this.roundPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.roundPanel3.Name = "roundPanel3";
            this.roundPanel3.Size = new System.Drawing.Size(702, 454);
            this.roundPanel3.TabIndex = 0;
            // 
            // dataGridView1
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.LightGray;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleVertical;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(202)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.Info;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(202)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridView1.ColumnHeadersHeight = 50;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView1.EnableHeadersVisualStyles = false;
            this.dataGridView1.GridColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.dataGridView1.Location = new System.Drawing.Point(14, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridView1.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dataGridView1.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.dataGridView1.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridView1.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowTemplate.Height = 40;
            this.dataGridView1.Size = new System.Drawing.Size(671, 205);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dataGridView1_DataBindingComplete);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "MOTION";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "LIGHT";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "SCAN";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "VISION";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // roundPanel4
            // 
            this.roundPanel4._setRoundRadius = 8;
            this.roundPanel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.roundPanel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(235)))));
            this.roundPanel4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.roundPanel4.Controls.Add(this.label23);
            this.roundPanel4.Controls.Add(this.label24);
            this.roundPanel4.Controls.Add(this.label28);
            this.roundPanel4.Controls.Add(this.label27);
            this.roundPanel4.Controls.Add(this.label26);
            this.roundPanel4.Controls.Add(this.label25);
            this.roundPanel4.Controls.Add(this.label22);
            this.roundPanel4.Controls.Add(this.label21);
            this.roundPanel4.Controls.Add(this.label20);
            this.roundPanel4.Controls.Add(this.label19);
            this.roundPanel4.Controls.Add(this.label18);
            this.roundPanel4.Controls.Add(this.label17);
            this.roundPanel4.Location = new System.Drawing.Point(725, 165);
            this.roundPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.roundPanel4.Name = "roundPanel4";
            this.roundPanel4.Size = new System.Drawing.Size(616, 454);
            this.roundPanel4.TabIndex = 1;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label23.Location = new System.Drawing.Point(32, 130);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(108, 27);
            this.label23.TabIndex = 2;
            this.label23.Text = "-BC SCAN";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label24.Location = new System.Drawing.Point(424, 99);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(92, 27);
            this.label24.TabIndex = 2;
            this.label24.Text = "OUTPUT";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label28.Location = new System.Drawing.Point(232, 130);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(167, 27);
            this.label28.TabIndex = 2;
            this.label28.Text = "-TURRET IMAGE";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label27.Location = new System.Drawing.Point(232, 161);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(169, 27);
            this.label27.TabIndex = 2;
            this.label27.Text = "-POCKET IMAGE";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label26.Location = new System.Drawing.Point(232, 192);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(104, 27);
            this.label26.TabIndex = 2;
            this.label26.Text = "RC PLACE";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label25.Location = new System.Drawing.Point(232, 99);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(74, 27);
            this.label25.TabIndex = 2;
            this.label25.Text = "WORK";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label22.Location = new System.Drawing.Point(32, 99);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(72, 27);
            this.label22.TabIndex = 2;
            this.label22.Text = "INPUT";
            // 
            // label21
            // 
            this.label21.Image = ((System.Drawing.Image)(resources.GetObject("label21.Image")));
            this.label21.Location = new System.Drawing.Point(387, 39);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(35, 35);
            this.label21.TabIndex = 1;
            this.label21.Text = "     ";
            // 
            // label20
            // 
            this.label20.Image = ((System.Drawing.Image)(resources.GetObject("label20.Image")));
            this.label20.Location = new System.Drawing.Point(190, 39);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(35, 35);
            this.label20.TabIndex = 1;
            this.label20.Text = "     ";
            // 
            // label19
            // 
            this.label19.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(253)))), ((int)(((byte)(191)))));
            this.label19.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label19.Location = new System.Drawing.Point(423, 24);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(153, 64);
            this.label19.TabIndex = 0;
            this.label19.Text = "WAITING";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label18
            // 
            this.label18.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.label18.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label18.Location = new System.Drawing.Point(231, 24);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(153, 64);
            this.label18.TabIndex = 0;
            this.label18.Text = "ACTIVE";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label17
            // 
            this.label17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(253)))), ((int)(((byte)(253)))), ((int)(((byte)(191)))));
            this.label17.Font = new System.Drawing.Font("微软雅黑", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label17.Location = new System.Drawing.Point(31, 24);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(153, 64);
            this.label17.TabIndex = 0;
            this.label17.Text = "WAITING";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form_Machine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1350, 628);
            this.Controls.Add(this.roundPanel4);
            this.Controls.Add(this.roundPanel2);
            this.Controls.Add(this.roundPanel3);
            this.Controls.Add(this.roundPanel1);
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "Form_Machine";
            this.Text = "Form_machine";
            this.Load += new System.EventHandler(this.Form_Machine_Load);
            this.roundPanel1.ResumeLayout(false);
            this.roundPanel1.PerformLayout();
            this.roundPanel2.ResumeLayout(false);
            this.roundPanel2.PerformLayout();
            this.roundPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.roundPanel4.ResumeLayout(false);
            this.roundPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private AutoFrameUI.RoundPanel roundPanel1;
        private AutoFrameUI.RoundPanel roundPanel2;
        private AutoFrameUI.RoundPanel roundPanel3;
        private System.Windows.Forms.Label label_software_life;
        private System.Windows.Forms.Label label_machine_life;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label_last_date;
        private System.Windows.Forms.Label label_version;
        private System.Windows.Forms.Label label_build_date;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private AutoFrameUI.RoundPanel roundPanel4;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
    }
}