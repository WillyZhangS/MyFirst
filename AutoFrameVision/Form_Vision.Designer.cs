namespace AutoFrameVision
{
    partial class Form_Vision
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
            this.roundPanel1 = new AutoFrameUI.RoundPanel(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button_config = new System.Windows.Forms.Button();
            this.listbox_log = new System.Windows.Forms.ListBox();
            this.roundPanel_button = new AutoFrameUI.RoundPanel(this.components);
            this.T1_Add = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonT1_calib2 = new System.Windows.Forms.Button();
            this.button_T1_1 = new System.Windows.Forms.Button();
            this.button_T1_2 = new System.Windows.Forms.Button();
            this.button_debug = new System.Windows.Forms.Button();
            this.button_delete = new System.Windows.Forms.Button();
            this.button_cali = new System.Windows.Forms.Button();
            this.buttonT1_calib = new System.Windows.Forms.Button();
            this.button_T1_3 = new System.Windows.Forms.Button();
            this.button_clear = new System.Windows.Forms.Button();
            this.button_T1_4 = new System.Windows.Forms.Button();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.visionControl1 = new AutoFrameVision.VisionControl();
            this.visionControl2 = new AutoFrameVision.VisionControl();
            this.visionControl3 = new AutoFrameVision.VisionControl();
            this.roundPanel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.roundPanel_button.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.visionControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.visionControl2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.visionControl3)).BeginInit();
            this.SuspendLayout();
            // 
            // roundPanel1
            // 
            this.roundPanel1._setRoundRadius = 12;
            this.roundPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.roundPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(235)))));
            this.roundPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.roundPanel1.Controls.Add(this.tableLayoutPanel1);
            this.roundPanel1.Controls.Add(this.listbox_log);
            this.roundPanel1.Controls.Add(this.roundPanel_button);
            this.roundPanel1.Location = new System.Drawing.Point(3, 3);
            this.roundPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.roundPanel1.Name = "roundPanel1";
            this.roundPanel1.Size = new System.Drawing.Size(1344, 622);
            this.roundPanel1.TabIndex = 12;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.visionControl1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.visionControl2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.button_config, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.visionControl3, 2, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1065, 616);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // button_config
            // 
            this.button_config.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_config.Location = new System.Drawing.Point(952, 555);
            this.button_config.Name = "button_config";
            this.button_config.Size = new System.Drawing.Size(110, 58);
            this.button_config.TabIndex = 7;
            this.button_config.Text = "Config";
            this.button_config.UseVisualStyleBackColor = true;
            this.button_config.Click += new System.EventHandler(this.button_config_Click);
            // 
            // listbox_log
            // 
            this.listbox_log.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listbox_log.IntegralHeight = false;
            this.listbox_log.ItemHeight = 21;
            this.listbox_log.Location = new System.Drawing.Point(1074, 3);
            this.listbox_log.Name = "listbox_log";
            this.listbox_log.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.listbox_log.Size = new System.Drawing.Size(267, 420);
            this.listbox_log.TabIndex = 8;
            // 
            // roundPanel_button
            // 
            this.roundPanel_button._setRoundRadius = 8;
            this.roundPanel_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.roundPanel_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.roundPanel_button.Controls.Add(this.T1_Add);
            this.roundPanel_button.Controls.Add(this.label1);
            this.roundPanel_button.Controls.Add(this.buttonT1_calib2);
            this.roundPanel_button.Controls.Add(this.button_T1_1);
            this.roundPanel_button.Controls.Add(this.button_T1_2);
            this.roundPanel_button.Controls.Add(this.button_debug);
            this.roundPanel_button.Controls.Add(this.button_delete);
            this.roundPanel_button.Controls.Add(this.button_cali);
            this.roundPanel_button.Controls.Add(this.buttonT1_calib);
            this.roundPanel_button.Controls.Add(this.button_T1_3);
            this.roundPanel_button.Controls.Add(this.button_clear);
            this.roundPanel_button.Controls.Add(this.button_T1_4);
            this.roundPanel_button.Location = new System.Drawing.Point(1074, 426);
            this.roundPanel_button.Margin = new System.Windows.Forms.Padding(0);
            this.roundPanel_button.Name = "roundPanel_button";
            this.roundPanel_button.Size = new System.Drawing.Size(274, 193);
            this.roundPanel_button.TabIndex = 7;
            // 
            // T1_Add
            // 
            this.T1_Add.FormattingEnabled = true;
            this.T1_Add.Items.AddRange(new object[] {
            "新增模块1",
            "新增模块2",
            "新增模块3",
            "新增模块4",
            "新增模块5",
            "新增模块6",
            "新增模块7",
            "新增模块8",
            "新增模块9"});
            this.T1_Add.Location = new System.Drawing.Point(61, 5);
            this.T1_Add.Name = "T1_Add";
            this.T1_Add.Size = new System.Drawing.Size(135, 29);
            this.T1_Add.TabIndex = 7;
            this.T1_Add.SelectedIndexChanged += new System.EventHandler(this.T1_Add_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 21);
            this.label1.TabIndex = 7;
            this.label1.Text = "T1_n:";
            // 
            // buttonT1_calib2
            // 
            this.buttonT1_calib2.Font = new System.Drawing.Font("微软雅黑", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonT1_calib2.Location = new System.Drawing.Point(103, 137);
            this.buttonT1_calib2.Name = "buttonT1_calib2";
            this.buttonT1_calib2.Size = new System.Drawing.Size(78, 40);
            this.buttonT1_calib2.TabIndex = 6;
            this.buttonT1_calib2.Text = "T1_calib2";
            this.buttonT1_calib2.UseVisualStyleBackColor = true;
            this.buttonT1_calib2.Click += new System.EventHandler(this.buttonT1_calib2_Click);
            // 
            // button_T1_1
            // 
            this.button_T1_1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_T1_1.Location = new System.Drawing.Point(6, 41);
            this.button_T1_1.Name = "button_T1_1";
            this.button_T1_1.Size = new System.Drawing.Size(78, 40);
            this.button_T1_1.TabIndex = 4;
            this.button_T1_1.Text = "T1_1";
            this.button_T1_1.UseVisualStyleBackColor = true;
            this.button_T1_1.Click += new System.EventHandler(this.button_T1_Click);
            // 
            // button_T1_2
            // 
            this.button_T1_2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_T1_2.Location = new System.Drawing.Point(103, 43);
            this.button_T1_2.Name = "button_T1_2";
            this.button_T1_2.Size = new System.Drawing.Size(78, 40);
            this.button_T1_2.TabIndex = 4;
            this.button_T1_2.Text = "T1_2";
            this.button_T1_2.UseVisualStyleBackColor = true;
            this.button_T1_2.Click += new System.EventHandler(this.button_T1_2_Click);
            // 
            // button_debug
            // 
            this.button_debug.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_debug.Location = new System.Drawing.Point(202, 138);
            this.button_debug.Name = "button_debug";
            this.button_debug.Size = new System.Drawing.Size(59, 40);
            this.button_debug.TabIndex = 5;
            this.button_debug.Text = "调试";
            this.button_debug.UseVisualStyleBackColor = true;
            this.button_debug.Click += new System.EventHandler(this.button_debug_Click);
            // 
            // button_delete
            // 
            this.button_delete.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_delete.Location = new System.Drawing.Point(202, 5);
            this.button_delete.Name = "button_delete";
            this.button_delete.Size = new System.Drawing.Size(59, 40);
            this.button_delete.TabIndex = 4;
            this.button_delete.Text = "Delete";
            this.button_delete.UseVisualStyleBackColor = true;
            this.button_delete.Click += new System.EventHandler(this.button_delete_Click);
            // 
            // button_cali
            // 
            this.button_cali.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_cali.Location = new System.Drawing.Point(202, 93);
            this.button_cali.Name = "button_cali";
            this.button_cali.Size = new System.Drawing.Size(59, 40);
            this.button_cali.TabIndex = 4;
            this.button_cali.Text = "Cali";
            this.button_cali.UseVisualStyleBackColor = true;
            this.button_cali.Click += new System.EventHandler(this.button_cali_Click);
            // 
            // buttonT1_calib
            // 
            this.buttonT1_calib.Font = new System.Drawing.Font("微软雅黑", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonT1_calib.Location = new System.Drawing.Point(6, 134);
            this.buttonT1_calib.Name = "buttonT1_calib";
            this.buttonT1_calib.Size = new System.Drawing.Size(78, 40);
            this.buttonT1_calib.TabIndex = 4;
            this.buttonT1_calib.Text = "T1_calib";
            this.buttonT1_calib.UseVisualStyleBackColor = true;
            this.buttonT1_calib.Click += new System.EventHandler(this.buttonT1_calib_Click);
            // 
            // button_T1_3
            // 
            this.button_T1_3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_T1_3.Location = new System.Drawing.Point(6, 87);
            this.button_T1_3.Name = "button_T1_3";
            this.button_T1_3.Size = new System.Drawing.Size(78, 40);
            this.button_T1_3.TabIndex = 4;
            this.button_T1_3.Text = "T1_3";
            this.button_T1_3.UseVisualStyleBackColor = true;
            this.button_T1_3.Click += new System.EventHandler(this.button_T1_3_Click);
            // 
            // button_clear
            // 
            this.button_clear.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_clear.Location = new System.Drawing.Point(202, 48);
            this.button_clear.Name = "button_clear";
            this.button_clear.Size = new System.Drawing.Size(59, 40);
            this.button_clear.TabIndex = 4;
            this.button_clear.Text = "Clear";
            this.button_clear.UseVisualStyleBackColor = true;
            this.button_clear.Click += new System.EventHandler(this.button_clear_Click);
            // 
            // button_T1_4
            // 
            this.button_T1_4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_T1_4.Location = new System.Drawing.Point(103, 88);
            this.button_T1_4.Name = "button_T1_4";
            this.button_T1_4.Size = new System.Drawing.Size(78, 40);
            this.button_T1_4.TabIndex = 4;
            this.button_T1_4.Text = "T1_4";
            this.button_T1_4.UseVisualStyleBackColor = true;
            this.button_T1_4.Click += new System.EventHandler(this.button_T1_4_Click);
            // 
            // visionControl1
            // 
            this.visionControl1.BackColor = System.Drawing.Color.MidnightBlue;
            this.visionControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.visionControl1.Location = new System.Drawing.Point(3, 3);
            this.visionControl1.Name = "visionControl1";
            this.visionControl1.Size = new System.Drawing.Size(349, 302);
            this.visionControl1.TabIndex = 6;
            this.visionControl1.TabStop = false;
            // 
            // visionControl2
            // 
            this.visionControl2.BackColor = System.Drawing.Color.MidnightBlue;
            this.visionControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.visionControl2.Location = new System.Drawing.Point(358, 3);
            this.visionControl2.Name = "visionControl2";
            this.visionControl2.Size = new System.Drawing.Size(349, 302);
            this.visionControl2.TabIndex = 6;
            this.visionControl2.TabStop = false;
            // 
            // visionControl3
            // 
            this.visionControl3.BackColor = System.Drawing.Color.MidnightBlue;
            this.visionControl3.Location = new System.Drawing.Point(713, 3);
            this.visionControl3.Name = "visionControl3";
            this.visionControl3.Size = new System.Drawing.Size(347, 301);
            this.visionControl3.TabIndex = 8;
            this.visionControl3.TabStop = false;
            // 
            // Form_Vision
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1350, 628);
            this.Controls.Add(this.roundPanel1);
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "Form_Vision";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AutoFrameVision";
            this.Load += new System.EventHandler(this.Form_Vision_Load);
            this.roundPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.roundPanel_button.ResumeLayout(false);
            this.roundPanel_button.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.visionControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.visionControl2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.visionControl3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AutoFrameUI.RoundPanel roundPanel1;
        private System.Windows.Forms.Button button_debug;
        private System.Windows.Forms.Button button_delete;
        private VisionControl visionControl1;
        private System.Windows.Forms.Button button_T1_3;
        private System.Windows.Forms.Button button_T1_4;
        private System.Windows.Forms.Button button_T1_1;
        private VisionControl visionControl2;
        private System.Windows.Forms.Button button_clear;
        private System.Windows.Forms.Button button_cali;
        private System.Windows.Forms.ListBox listbox_log;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button button_T1_2;
        private System.Windows.Forms.Button buttonT1_calib;
        private AutoFrameUI.RoundPanel roundPanel_button;
        private System.Windows.Forms.Button button_config;
        private System.Windows.Forms.Button buttonT1_calib2;
        private VisionControl visionControl3;
        private System.Windows.Forms.ComboBox T1_Add;
        private System.Windows.Forms.Label label1;
    }
}