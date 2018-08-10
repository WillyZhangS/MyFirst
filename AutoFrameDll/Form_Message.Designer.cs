namespace AutoFrameDll
{
    partial class Form_Message
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Message));
            this.label_Info = new System.Windows.Forms.Label();
            this.button_yes = new System.Windows.Forms.Button();
            this.button_no = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_time = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label_Info
            // 
            this.label_Info.Location = new System.Drawing.Point(74, 9);
            this.label_Info.Name = "label_Info";
            this.label_Info.Size = new System.Drawing.Size(340, 91);
            this.label_Info.TabIndex = 0;
            this.label_Info.Text = "label1";
            // 
            // button_yes
            // 
            this.button_yes.Location = new System.Drawing.Point(132, 113);
            this.button_yes.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_yes.Name = "button_yes";
            this.button_yes.Size = new System.Drawing.Size(86, 46);
            this.button_yes.TabIndex = 1;
            this.button_yes.Text = "继续等待";
            this.button_yes.UseVisualStyleBackColor = true;
            this.button_yes.Click += new System.EventHandler(this.button_yes_Click);
            // 
            // button_no
            // 
            this.button_no.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_no.Location = new System.Drawing.Point(225, 113);
            this.button_no.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_no.Name = "button_no";
            this.button_no.Size = new System.Drawing.Size(86, 46);
            this.button_no.TabIndex = 1;
            this.button_no.Text = "退出运行";
            this.button_no.UseVisualStyleBackColor = true;
            this.button_no.Click += new System.EventHandler(this.button_cancel_Click);
            // 
            // button_cancel
            // 
            this.button_cancel.Location = new System.Drawing.Point(318, 113);
            this.button_cancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(86, 46);
            this.button_cancel.TabIndex = 1;
            this.button_cancel.Text = "继续运行";
            this.button_cancel.UseVisualStyleBackColor = true;
            this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click_1);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(7, 9);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(61, 66);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "倒计时:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(57, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "秒";
            // 
            // textBox_time
            // 
            this.textBox_time.Location = new System.Drawing.Point(7, 120);
            this.textBox_time.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox_time.Name = "textBox_time";
            this.textBox_time.ReadOnly = true;
            this.textBox_time.Size = new System.Drawing.Size(44, 26);
            this.textBox_time.TabIndex = 5;
            this.textBox_time.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // Form_Message
            // 
            this.AcceptButton = this.button_yes;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.button_no;
            this.ClientSize = new System.Drawing.Size(418, 167);
            this.Controls.Add(this.textBox_time);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_no);
            this.Controls.Add(this.button_yes);
            this.Controls.Add(this.label_Info);
            this.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_Message";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "超时提示";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form_Message_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_Info;
        private System.Windows.Forms.Button button_yes;
        private System.Windows.Forms.Button button_no;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_time;
    }
}