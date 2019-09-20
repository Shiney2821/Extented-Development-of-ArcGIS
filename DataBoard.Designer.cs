namespace MapControlApplication1
{
    partial class DataBoard
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
            this.tbDataName = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.miFinish = new System.Windows.Forms.Button();
            this.miUpdate = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // tbDataName
            // 
            this.tbDataName.Location = new System.Drawing.Point(37, 68);
            this.tbDataName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbDataName.Name = "tbDataName";
            this.tbDataName.Size = new System.Drawing.Size(265, 35);
            this.tbDataName.TabIndex = 0;
            this.tbDataName.Visible = false;
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(-4, 69);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 30;
            this.dataGridView1.Size = new System.Drawing.Size(644, 756);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this.dataGridView1.CursorChanged += new System.EventHandler(this.dataGridView1_CursorChanged);
            // 
            // miFinish
            // 
            this.miFinish.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.miFinish.Enabled = false;
            this.miFinish.Location = new System.Drawing.Point(467, 16);
            this.miFinish.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.miFinish.Name = "miFinish";
            this.miFinish.Size = new System.Drawing.Size(135, 45);
            this.miFinish.TabIndex = 2;
            this.miFinish.Text = "修改完成";
            this.miFinish.UseVisualStyleBackColor = true;
            this.miFinish.Click += new System.EventHandler(this.miFinish_Click);
            // 
            // miUpdate
            // 
            this.miUpdate.Location = new System.Drawing.Point(311, 16);
            this.miUpdate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.miUpdate.Name = "miUpdate";
            this.miUpdate.Size = new System.Drawing.Size(135, 45);
            this.miUpdate.TabIndex = 3;
            this.miUpdate.Text = "开始修改";
            this.miUpdate.UseVisualStyleBackColor = true;
            this.miUpdate.Click += new System.EventHandler(this.miUpdate_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 27);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 24);
            this.label1.TabIndex = 4;
            this.label1.Text = "label1";
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(-4, 68);
            this.checkedListBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(52, 724);
            this.checkedListBox1.TabIndex = 5;
            this.checkedListBox1.Visible = false;
            // 
            // DataBoard
            // 
            this.AcceptButton = this.miFinish;
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.miFinish;
            this.ClientSize = new System.Drawing.Size(635, 809);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.miUpdate);
            this.Controls.Add(this.miFinish);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.tbDataName);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "DataBoard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数据展示台";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DataBoard_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbDataName;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button miFinish;
        private System.Windows.Forms.Button miUpdate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
    }
}