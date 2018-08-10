namespace AutoFrameVision
{
    partial class Form_Vision_debug
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
            this.button_cancel = new System.Windows.Forms.Button();
            this.button_cali = new System.Windows.Forms.Button();
            this.groupBox_func = new System.Windows.Forms.GroupBox();
            this.checkBox_auto = new System.Windows.Forms.CheckBox();
            this.textBox_span = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button_batch_stop = new System.Windows.Forms.Button();
            this.button_batch_pause = new System.Windows.Forms.Button();
            this.button_path = new System.Windows.Forms.Button();
            this.textBox_dir = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox_step = new System.Windows.Forms.ComboBox();
            this.button_test = new System.Windows.Forms.Button();
            this.button_batch_test = new System.Windows.Forms.Button();
            this.groupBox_cam = new System.Windows.Forms.GroupBox();
            this.comboBox_cam = new System.Windows.Forms.ComboBox();
            this.button_stop_grab = new System.Windows.Forms.Button();
            this.button_snap = new System.Windows.Forms.Button();
            this.button_save = new System.Windows.Forms.Button();
            this.button_open = new System.Windows.Forms.Button();
            this.button_grab = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.textBox_exposure = new System.Windows.Forms.TextBox();
            this.button_setExposure = new System.Windows.Forms.Button();
            this.visionControl1 = new AutoFrameVision.VisionControl();
            this.groupBox_func.SuspendLayout();
            this.groupBox_cam.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.visionControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // button_cancel
            // 
            this.button_cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_cancel.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_cancel.Location = new System.Drawing.Point(710, 443);
            this.button_cancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(91, 39);
            this.button_cancel.TabIndex = 7;
            this.button_cancel.Text = "退出";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // button_cali
            // 
            this.button_cali.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_cali.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_cali.Location = new System.Drawing.Point(607, 443);
            this.button_cali.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_cali.Name = "button_cali";
            this.button_cali.Size = new System.Drawing.Size(91, 39);
            this.button_cali.TabIndex = 7;
            this.button_cali.Text = "标定配置";
            this.button_cali.UseVisualStyleBackColor = true;
            this.button_cali.Click += new System.EventHandler(this.button_light_Click);
            // 
            // groupBox_func
            // 
            this.groupBox_func.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_func.Controls.Add(this.checkBox_auto);
            this.groupBox_func.Controls.Add(this.textBox_span);
            this.groupBox_func.Controls.Add(this.label3);
            this.groupBox_func.Controls.Add(this.label2);
            this.groupBox_func.Controls.Add(this.button_batch_stop);
            this.groupBox_func.Controls.Add(this.button_batch_pause);
            this.groupBox_func.Controls.Add(this.button_path);
            this.groupBox_func.Controls.Add(this.textBox_dir);
            this.groupBox_func.Controls.Add(this.label1);
            this.groupBox_func.Controls.Add(this.comboBox_step);
            this.groupBox_func.Controls.Add(this.button_test);
            this.groupBox_func.Controls.Add(this.button_batch_test);
            this.groupBox_func.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox_func.Location = new System.Drawing.Point(607, 182);
            this.groupBox_func.Name = "groupBox_func";
            this.groupBox_func.Size = new System.Drawing.Size(202, 255);
            this.groupBox_func.TabIndex = 9;
            this.groupBox_func.TabStop = false;
            this.groupBox_func.Text = "功能选择";
            // 
            // checkBox_auto
            // 
            this.checkBox_auto.AutoSize = true;
            this.checkBox_auto.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_auto.Location = new System.Drawing.Point(6, 55);
            this.checkBox_auto.Name = "checkBox_auto";
            this.checkBox_auto.Size = new System.Drawing.Size(126, 24);
            this.checkBox_auto.TabIndex = 14;
            this.checkBox_auto.Text = "更新后自动测试";
            this.checkBox_auto.UseVisualStyleBackColor = true;
            this.checkBox_auto.Click += new System.EventHandler(this.checkBox_auto_Click);
            // 
            // textBox_span
            // 
            this.textBox_span.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.textBox_span.Location = new System.Drawing.Point(109, 223);
            this.textBox_span.Name = "textBox_span";
            this.textBox_span.Size = new System.Drawing.Size(50, 26);
            this.textBox_span.TabIndex = 11;
            this.textBox_span.Text = "500";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.label3.Location = new System.Drawing.Point(160, 224);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 20);
            this.label3.TabIndex = 13;
            this.label3.Text = "ms";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.5F);
            this.label2.Location = new System.Drawing.Point(7, 224);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 20);
            this.label2.TabIndex = 12;
            this.label2.Text = "图像间隔时间:";
            // 
            // button_batch_stop
            // 
            this.button_batch_stop.Enabled = false;
            this.button_batch_stop.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_batch_stop.Location = new System.Drawing.Point(108, 181);
            this.button_batch_stop.Name = "button_batch_stop";
            this.button_batch_stop.Size = new System.Drawing.Size(86, 34);
            this.button_batch_stop.TabIndex = 10;
            this.button_batch_stop.Text = "停止测试";
            this.button_batch_stop.UseVisualStyleBackColor = true;
            this.button_batch_stop.Click += new System.EventHandler(this.button_batch_stop_Click);
            // 
            // button_batch_pause
            // 
            this.button_batch_pause.Enabled = false;
            this.button_batch_pause.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_batch_pause.Location = new System.Drawing.Point(5, 181);
            this.button_batch_pause.Name = "button_batch_pause";
            this.button_batch_pause.Size = new System.Drawing.Size(86, 34);
            this.button_batch_pause.TabIndex = 10;
            this.button_batch_pause.Text = "暂停测试";
            this.button_batch_pause.UseVisualStyleBackColor = true;
            this.button_batch_pause.Click += new System.EventHandler(this.button_batch_pause_Click);
            // 
            // button_path
            // 
            this.button_path.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_path.Location = new System.Drawing.Point(5, 144);
            this.button_path.Name = "button_path";
            this.button_path.Size = new System.Drawing.Size(86, 34);
            this.button_path.TabIndex = 10;
            this.button_path.Text = "目录选择";
            this.button_path.UseVisualStyleBackColor = true;
            this.button_path.Click += new System.EventHandler(this.button_path_Click);
            // 
            // textBox_dir
            // 
            this.textBox_dir.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_dir.Location = new System.Drawing.Point(6, 112);
            this.textBox_dir.Name = "textBox_dir";
            this.textBox_dir.Size = new System.Drawing.Size(188, 26);
            this.textBox_dir.TabIndex = 9;
            this.textBox_dir.Text = "D:\\exe\\Image_test";
            this.textBox_dir.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(7, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "批量路径指定:";
            // 
            // comboBox_step
            // 
            this.comboBox_step.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox_step.FormattingEnabled = true;
            this.comboBox_step.Location = new System.Drawing.Point(7, 20);
            this.comboBox_step.Name = "comboBox_step";
            this.comboBox_step.Size = new System.Drawing.Size(187, 28);
            this.comboBox_step.TabIndex = 0;
            this.comboBox_step.SelectedIndexChanged += new System.EventHandler(this.comboBox_step_SelectedIndexChanged);
            // 
            // button_test
            // 
            this.button_test.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_test.Location = new System.Drawing.Point(131, 49);
            this.button_test.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_test.Name = "button_test";
            this.button_test.Size = new System.Drawing.Size(62, 34);
            this.button_test.TabIndex = 7;
            this.button_test.Text = "测试";
            this.button_test.UseVisualStyleBackColor = true;
            this.button_test.Click += new System.EventHandler(this.button_test_Click);
            // 
            // button_batch_test
            // 
            this.button_batch_test.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_batch_test.Location = new System.Drawing.Point(108, 144);
            this.button_batch_test.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_batch_test.Name = "button_batch_test";
            this.button_batch_test.Size = new System.Drawing.Size(86, 34);
            this.button_batch_test.TabIndex = 7;
            this.button_batch_test.Text = "批量测试";
            this.button_batch_test.UseVisualStyleBackColor = true;
            this.button_batch_test.Click += new System.EventHandler(this.button_test_batch_Click);
            // 
            // groupBox_cam
            // 
            this.groupBox_cam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_cam.Controls.Add(this.comboBox_cam);
            this.groupBox_cam.Controls.Add(this.button_stop_grab);
            this.groupBox_cam.Controls.Add(this.button_snap);
            this.groupBox_cam.Controls.Add(this.button_save);
            this.groupBox_cam.Controls.Add(this.button_open);
            this.groupBox_cam.Controls.Add(this.button_grab);
            this.groupBox_cam.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox_cam.Location = new System.Drawing.Point(607, 3);
            this.groupBox_cam.Name = "groupBox_cam";
            this.groupBox_cam.Size = new System.Drawing.Size(202, 173);
            this.groupBox_cam.TabIndex = 9;
            this.groupBox_cam.TabStop = false;
            this.groupBox_cam.Text = "图像选择";
            // 
            // comboBox_cam
            // 
            this.comboBox_cam.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox_cam.FormattingEnabled = true;
            this.comboBox_cam.Location = new System.Drawing.Point(7, 21);
            this.comboBox_cam.Name = "comboBox_cam";
            this.comboBox_cam.Size = new System.Drawing.Size(187, 28);
            this.comboBox_cam.TabIndex = 0;
            this.comboBox_cam.SelectedIndexChanged += new System.EventHandler(this.comboBox_cam_SelectedIndexChanged);
            // 
            // button_stop_grab
            // 
            this.button_stop_grab.Enabled = false;
            this.button_stop_grab.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_stop_grab.Location = new System.Drawing.Point(107, 92);
            this.button_stop_grab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_stop_grab.Name = "button_stop_grab";
            this.button_stop_grab.Size = new System.Drawing.Size(86, 34);
            this.button_stop_grab.TabIndex = 7;
            this.button_stop_grab.Text = "停止采集";
            this.button_stop_grab.UseVisualStyleBackColor = true;
            this.button_stop_grab.Click += new System.EventHandler(this.button_stop_Click);
            // 
            // button_snap
            // 
            this.button_snap.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_snap.Location = new System.Drawing.Point(7, 55);
            this.button_snap.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_snap.Name = "button_snap";
            this.button_snap.Size = new System.Drawing.Size(86, 73);
            this.button_snap.TabIndex = 7;
            this.button_snap.Text = "单帧采集";
            this.button_snap.UseVisualStyleBackColor = true;
            this.button_snap.Click += new System.EventHandler(this.button_snap_Click);
            // 
            // button_save
            // 
            this.button_save.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_save.Location = new System.Drawing.Point(107, 132);
            this.button_save.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(86, 34);
            this.button_save.TabIndex = 7;
            this.button_save.Text = "保存图片";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_open
            // 
            this.button_open.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_open.Location = new System.Drawing.Point(7, 132);
            this.button_open.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_open.Name = "button_open";
            this.button_open.Size = new System.Drawing.Size(86, 34);
            this.button_open.TabIndex = 7;
            this.button_open.Text = "加载图片";
            this.button_open.UseVisualStyleBackColor = true;
            this.button_open.Click += new System.EventHandler(this.button_open_Click);
            // 
            // button_grab
            // 
            this.button_grab.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_grab.Location = new System.Drawing.Point(107, 53);
            this.button_grab.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button_grab.Name = "button_grab";
            this.button_grab.Size = new System.Drawing.Size(86, 34);
            this.button_grab.TabIndex = 7;
            this.button_grab.Text = "连续采集";
            this.button_grab.UseVisualStyleBackColor = true;
            this.button_grab.Click += new System.EventHandler(this.button_grab_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "sample";
            this.openFileDialog1.Filter = "bmp文件|*.bmp|png文件|*.png|所有文件|*.*";
            // 
            // textBox_exposure
            // 
            this.textBox_exposure.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_exposure.Location = new System.Drawing.Point(610, 500);
            this.textBox_exposure.Name = "textBox_exposure";
            this.textBox_exposure.Size = new System.Drawing.Size(100, 23);
            this.textBox_exposure.TabIndex = 11;
            this.textBox_exposure.Visible = false;
            this.textBox_exposure.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_exposure_KeyPress);
            // 
            // button_setExposure
            // 
            this.button_setExposure.Location = new System.Drawing.Point(715, 500);
            this.button_setExposure.Name = "button_setExposure";
            this.button_setExposure.Size = new System.Drawing.Size(91, 39);
            this.button_setExposure.TabIndex = 12;
            this.button_setExposure.Text = "设置曝光";
            this.button_setExposure.UseVisualStyleBackColor = true;
            this.button_setExposure.Visible = false;
            this.button_setExposure.Click += new System.EventHandler(this.button_setExposure_Click);
            // 
            // visionControl1
            // 
            this.visionControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.visionControl1.BackColor = System.Drawing.Color.MidnightBlue;
            this.visionControl1.Location = new System.Drawing.Point(3, 3);
            this.visionControl1.Name = "visionControl1";
            this.visionControl1.Size = new System.Drawing.Size(598, 530);
            this.visionControl1.TabIndex = 10;
            this.visionControl1.TabStop = false;
            // 
            // Form_Vision_debug
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(811, 545);
            this.Controls.Add(this.button_setExposure);
            this.Controls.Add(this.textBox_exposure);
            this.Controls.Add(this.visionControl1);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_cali);
            this.Controls.Add(this.groupBox_func);
            this.Controls.Add(this.groupBox_cam);
            this.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_Vision_debug";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "相机视觉";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Vision_debug_FormClosing);
            this.Load += new System.EventHandler(this.Form_Vision_debug_Load);
            this.groupBox_func.ResumeLayout(false);
            this.groupBox_func.PerformLayout();
            this.groupBox_cam.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.visionControl1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.Button button_cali;
        private System.Windows.Forms.GroupBox groupBox_func;
        private System.Windows.Forms.Button button_path;
        private System.Windows.Forms.TextBox textBox_dir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_step;
        private System.Windows.Forms.Button button_test;
        private System.Windows.Forms.Button button_batch_test;
        private System.Windows.Forms.GroupBox groupBox_cam;
        private System.Windows.Forms.ComboBox comboBox_cam;
        private System.Windows.Forms.Button button_stop_grab;
        private System.Windows.Forms.Button button_snap;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.Button button_open;
        private System.Windows.Forms.Button button_grab;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox textBox_span;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox_auto;
        private System.Windows.Forms.Button button_batch_stop;
        private System.Windows.Forms.Button button_batch_pause;
        private VisionControl visionControl1;
        private System.Windows.Forms.TextBox textBox_exposure;
        private System.Windows.Forms.Button button_setExposure;
    }
}