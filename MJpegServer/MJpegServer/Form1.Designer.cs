namespace MJpegServer
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.SendImg = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.ButtonMode = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SendImg
            // 
            this.SendImg.Location = new System.Drawing.Point(174, 12);
            this.SendImg.Name = "SendImg";
            this.SendImg.Size = new System.Drawing.Size(75, 23);
            this.SendImg.TabIndex = 0;
            this.SendImg.Text = "刷新文件";
            this.SendImg.UseVisualStyleBackColor = true;
            this.SendImg.Click += new System.EventHandler(this.SendImg_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(13, 12);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(96, 16);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "开机自动运行";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // ButtonMode
            // 
            this.ButtonMode.AutoSize = true;
            this.ButtonMode.Location = new System.Drawing.Point(172, 38);
            this.ButtonMode.Name = "ButtonMode";
            this.ButtonMode.Size = new System.Drawing.Size(53, 12);
            this.ButtonMode.TabIndex = 2;
            this.ButtonMode.Text = "普通模式";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(257, 52);
            this.Controls.Add(this.ButtonMode);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.SendImg);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "MJpegServer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button SendImg;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label ButtonMode;
    }
}

