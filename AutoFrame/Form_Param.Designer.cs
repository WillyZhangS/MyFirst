namespace AutoFrame
{
    partial class Form_Param
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView_AllParam = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button_save = new AutoFrameUI.RoundButton();
            this.button_update = new AutoFrameUI.RoundButton();
            this.roundPanel1 = new AutoFrameUI.RoundPanel(this.components);
            this.listView1 = new System.Windows.Forms.ListView();
            this.roundButtonSetParam = new AutoFrameUI.RoundButton();
            this.button_save_as = new AutoFrameUI.RoundButton();
            this.treeView1 = new System.Windows.Forms.TreeView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_AllParam)).BeginInit();
            this.roundPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView_AllParam
            // 
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle19.BackColor = System.Drawing.Color.Gainsboro;
            dataGridViewCellStyle19.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle19.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle19.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle19.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_AllParam.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle19;
            this.dataGridView_AllParam.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView_AllParam.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView_AllParam.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView_AllParam.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView_AllParam.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleVertical;
            this.dataGridView_AllParam.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle20.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(202)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle20.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle20.ForeColor = System.Drawing.SystemColors.Info;
            dataGridViewCellStyle20.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(202)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle20.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle20.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_AllParam.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle20;
            this.dataGridView_AllParam.ColumnHeadersHeight = 40;
            this.dataGridView_AllParam.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView_AllParam.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.Column6});
            dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle21.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle21.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle21.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle21.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle21.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle21.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_AllParam.DefaultCellStyle = dataGridViewCellStyle21;
            this.dataGridView_AllParam.EnableHeadersVisualStyles = false;
            this.dataGridView_AllParam.GridColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.dataGridView_AllParam.Location = new System.Drawing.Point(287, 14);
            this.dataGridView_AllParam.MultiSelect = false;
            this.dataGridView_AllParam.Name = "dataGridView_AllParam";
            this.dataGridView_AllParam.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView_AllParam.RowHeadersVisible = false;
            this.dataGridView_AllParam.RowHeadersWidth = 35;
            this.dataGridView_AllParam.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridView_AllParam.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dataGridView_AllParam.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.dataGridView_AllParam.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridView_AllParam.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView_AllParam.RowTemplate.Height = 40;
            this.dataGridView_AllParam.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView_AllParam.Size = new System.Drawing.Size(889, 547);
            this.dataGridView_AllParam.TabIndex = 9;
            this.dataGridView_AllParam.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_param_CellValueChanged);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.FillWeight = 75F;
            this.dataGridViewTextBoxColumn1.HeaderText = "当前值";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewTextBoxColumn2.HeaderText = "单位";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn3.HeaderText = "参数描述";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewTextBoxColumn4.HeaderText = "最大值";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column6
            // 
            this.Column6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Column6.HeaderText = "最小值";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            this.Column6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // button_save
            // 
            this.button_save.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button_save.AutoEllipsis = true;
            this.button_save.BackColor = System.Drawing.Color.Transparent;
            this.button_save.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.button_save.BaseColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.button_save.FlatAppearance.BorderSize = 0;
            this.button_save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_save.ImageHeight = 80;
            this.button_save.ImageWidth = 80;
            this.button_save.Location = new System.Drawing.Point(421, 565);
            this.button_save.Name = "button_save";
            this.button_save.Radius = 10;
            this.button_save.Size = new System.Drawing.Size(102, 43);
            this.button_save.SpliteButtonWidth = 18;
            this.button_save.TabIndex = 11;
            this.button_save.Text = "保存文件";
            this.button_save.UseVisualStyleBackColor = false;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // button_update
            // 
            this.button_update.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button_update.BackColor = System.Drawing.Color.Transparent;
            this.button_update.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.button_update.BaseColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.button_update.FlatAppearance.BorderSize = 0;
            this.button_update.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_update.ImageHeight = 80;
            this.button_update.ImageWidth = 80;
            this.button_update.Location = new System.Drawing.Point(745, 565);
            this.button_update.Name = "button_update";
            this.button_update.Radius = 10;
            this.button_update.Size = new System.Drawing.Size(102, 43);
            this.button_update.SpliteButtonWidth = 18;
            this.button_update.TabIndex = 10;
            this.button_update.Text = "试用参数";
            this.button_update.UseVisualStyleBackColor = false;
            this.button_update.Click += new System.EventHandler(this.button_update_Click);
            // 
            // roundPanel1
            // 
            this.roundPanel1._setRoundRadius = 8;
            this.roundPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.roundPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(235)))));
            this.roundPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.roundPanel1.Controls.Add(this.button_update);
            this.roundPanel1.Controls.Add(this.listView1);
            this.roundPanel1.Controls.Add(this.button_save);
            this.roundPanel1.Controls.Add(this.roundButtonSetParam);
            this.roundPanel1.Controls.Add(this.button_save_as);
            this.roundPanel1.Controls.Add(this.treeView1);
            this.roundPanel1.Location = new System.Drawing.Point(2, 0);
            this.roundPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.roundPanel1.Name = "roundPanel1";
            this.roundPanel1.Size = new System.Drawing.Size(1184, 614);
            this.roundPanel1.TabIndex = 12;
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(10, 311);
            this.listView1.Name = "listView1";
            this.listView1.Scrollable = false;
            this.listView1.ShowItemToolTips = true;
            this.listView1.Size = new System.Drawing.Size(269, 297);
            this.listView1.TabIndex = 11;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            // 
            // roundButtonSetParam
            // 
            this.roundButtonSetParam.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.roundButtonSetParam.AutoEllipsis = true;
            this.roundButtonSetParam.BackColor = System.Drawing.Color.Transparent;
            this.roundButtonSetParam.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.roundButtonSetParam.BaseColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.roundButtonSetParam.FlatAppearance.BorderSize = 0;
            this.roundButtonSetParam.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.roundButtonSetParam.ImageHeight = 80;
            this.roundButtonSetParam.ImageWidth = 80;
            this.roundButtonSetParam.Location = new System.Drawing.Point(923, 565);
            this.roundButtonSetParam.Name = "roundButtonSetParam";
            this.roundButtonSetParam.Radius = 10;
            this.roundButtonSetParam.Size = new System.Drawing.Size(116, 43);
            this.roundButtonSetParam.SpliteButtonWidth = 18;
            this.roundButtonSetParam.TabIndex = 11;
            this.roundButtonSetParam.Text = "选择此配置";
            this.roundButtonSetParam.UseVisualStyleBackColor = false;
            this.roundButtonSetParam.Click += new System.EventHandler(this.button_SetParam_Click);
            // 
            // button_save_as
            // 
            this.button_save_as.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button_save_as.BackColor = System.Drawing.Color.Transparent;
            this.button_save_as.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.button_save_as.BaseColorEnd = System.Drawing.Color.FromArgb(((int)(((byte)(174)))), ((int)(((byte)(218)))), ((int)(((byte)(151)))));
            this.button_save_as.FlatAppearance.BorderSize = 0;
            this.button_save_as.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_save_as.ImageHeight = 80;
            this.button_save_as.ImageWidth = 80;
            this.button_save_as.Location = new System.Drawing.Point(588, 565);
            this.button_save_as.Name = "button_save_as";
            this.button_save_as.Radius = 10;
            this.button_save_as.Size = new System.Drawing.Size(102, 43);
            this.button_save_as.SpliteButtonWidth = 18;
            this.button_save_as.TabIndex = 10;
            this.button_save_as.Text = "另存为";
            this.button_save_as.UseVisualStyleBackColor = false;
            this.button_save_as.Click += new System.EventHandler(this.button_save_as_Click);
            // 
            // treeView1
            // 
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.treeView1.HideSelection = false;
            this.treeView1.Indent = 16;
            this.treeView1.ItemHeight = 28;
            this.treeView1.Location = new System.Drawing.Point(10, 14);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(269, 291);
            this.treeView1.TabIndex = 1;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // Form_Param
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1192, 620);
            this.Controls.Add(this.dataGridView_AllParam);
            this.Controls.Add(this.roundPanel1);
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
            this.Name = "Form_Param";
            this.Text = "系统参数设置";
            this.Load += new System.EventHandler(this.Form_Param_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_AllParam)).EndInit();
            this.roundPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView_AllParam;
        private AutoFrameUI.RoundButton button_save;
        private AutoFrameUI.RoundButton button_update;
        private AutoFrameUI.RoundPanel roundPanel1;
        private System.Windows.Forms.TreeView treeView1;
        private AutoFrameUI.RoundButton button_save_as;
        private System.Windows.Forms.ListView listView1;
        private AutoFrameUI.RoundButton roundButtonSetParam;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
    }
}