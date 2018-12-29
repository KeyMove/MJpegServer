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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.ButtonMode = new System.Windows.Forms.Label();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PortLable = new System.Windows.Forms.Label();
            this.HIDCheck = new System.Windows.Forms.CheckBox();
            this.OutputBox = new System.Windows.Forms.TextBox();
            this.InputBOX = new System.Windows.Forms.TextBox();
            this.InputButton = new System.Windows.Forms.Button();
            this.TestPic = new System.Windows.Forms.PictureBox();
            this.LoadLua = new System.Windows.Forms.Button();
            this.SendImg = new System.Windows.Forms.Button();
            this.DrawTest = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.valV = new System.Windows.Forms.Label();
            this.valB = new System.Windows.Forms.Label();
            this.valS = new System.Windows.Forms.Label();
            this.valG = new System.Windows.Forms.Label();
            this.valH = new System.Windows.Forms.Label();
            this.valR = new System.Windows.Forms.Label();
            this.getposbutton = new System.Windows.Forms.Button();
            this.DataImage = new System.Windows.Forms.PictureBox();
            this.copyscreenbutton = new System.Windows.Forms.Button();
            this.clearposbutton = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TestPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DrawTest)).BeginInit();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataImage)).BeginInit();
            this.SuspendLayout();
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
            this.ButtonMode.Location = new System.Drawing.Point(192, 38);
            this.ButtonMode.Name = "ButtonMode";
            this.ButtonMode.Size = new System.Drawing.Size(53, 12);
            this.ButtonMode.TabIndex = 2;
            this.ButtonMode.Text = "普通模式";
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "MJpegServer";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.退出ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 26);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.退出ToolStripMenuItem.Text = "退出";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // PortLable
            // 
            this.PortLable.AutoSize = true;
            this.PortLable.Location = new System.Drawing.Point(2, 51);
            this.PortLable.Name = "PortLable";
            this.PortLable.Size = new System.Drawing.Size(35, 12);
            this.PortLable.TabIndex = 3;
            this.PortLable.Text = "Port:";
            // 
            // HIDCheck
            // 
            this.HIDCheck.AutoSize = true;
            this.HIDCheck.Location = new System.Drawing.Point(13, 30);
            this.HIDCheck.Name = "HIDCheck";
            this.HIDCheck.Size = new System.Drawing.Size(42, 16);
            this.HIDCheck.TabIndex = 1;
            this.HIDCheck.Text = "HID";
            this.HIDCheck.UseVisualStyleBackColor = true;
            this.HIDCheck.CheckedChanged += new System.EventHandler(this.HIDCheck_CheckedChanged);
            // 
            // OutputBox
            // 
            this.OutputBox.Location = new System.Drawing.Point(4, 66);
            this.OutputBox.Multiline = true;
            this.OutputBox.Name = "OutputBox";
            this.OutputBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.OutputBox.Size = new System.Drawing.Size(245, 51);
            this.OutputBox.TabIndex = 5;
            // 
            // InputBOX
            // 
            this.InputBOX.Location = new System.Drawing.Point(4, 123);
            this.InputBOX.Multiline = true;
            this.InputBOX.Name = "InputBOX";
            this.InputBOX.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.InputBOX.Size = new System.Drawing.Size(245, 51);
            this.InputBOX.TabIndex = 5;
            // 
            // InputButton
            // 
            this.InputButton.Location = new System.Drawing.Point(4, 180);
            this.InputButton.Name = "InputButton";
            this.InputButton.Size = new System.Drawing.Size(75, 23);
            this.InputButton.TabIndex = 6;
            this.InputButton.Text = "Input";
            this.InputButton.UseVisualStyleBackColor = true;
            this.InputButton.Click += new System.EventHandler(this.InputButton_Click);
            // 
            // TestPic
            // 
            this.TestPic.Location = new System.Drawing.Point(85, 180);
            this.TestPic.Name = "TestPic";
            this.TestPic.Size = new System.Drawing.Size(24, 23);
            this.TestPic.TabIndex = 7;
            this.TestPic.TabStop = false;
            // 
            // LoadLua
            // 
            this.LoadLua.FlatAppearance.BorderSize = 0;
            this.LoadLua.Image = global::MJpegServer.Properties.Resources.prog_lua_24px_1097210_easyicon_net;
            this.LoadLua.Location = new System.Drawing.Point(182, 3);
            this.LoadLua.Name = "LoadLua";
            this.LoadLua.Size = new System.Drawing.Size(32, 32);
            this.LoadLua.TabIndex = 4;
            this.LoadLua.UseVisualStyleBackColor = true;
            this.LoadLua.Visible = false;
            this.LoadLua.Click += new System.EventHandler(this.LoadLua_Click);
            // 
            // SendImg
            // 
            this.SendImg.Image = global::MJpegServer.Properties.Resources.update_24px_1160368_easyicon_net;
            this.SendImg.Location = new System.Drawing.Point(220, 3);
            this.SendImg.Name = "SendImg";
            this.SendImg.Size = new System.Drawing.Size(32, 32);
            this.SendImg.TabIndex = 0;
            this.SendImg.UseVisualStyleBackColor = true;
            this.SendImg.Click += new System.EventHandler(this.SendImg_Click);
            // 
            // DrawTest
            // 
            this.DrawTest.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DrawTest.Location = new System.Drawing.Point(4, 2);
            this.DrawTest.Name = "DrawTest";
            this.DrawTest.Size = new System.Drawing.Size(239, 165);
            this.DrawTest.TabIndex = 8;
            this.DrawTest.TabStop = false;
            this.DrawTest.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.DrawTest_MouseDoubleClick);
            this.DrawTest.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DrawTest_MouseDown);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Location = new System.Drawing.Point(258, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(260, 200);
            this.panel1.TabIndex = 9;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(257, 197);
            this.tabControl1.TabIndex = 9;
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.Controls.Add(this.DrawTest);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(249, 171);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.valV);
            this.tabPage2.Controls.Add(this.valB);
            this.tabPage2.Controls.Add(this.valS);
            this.tabPage2.Controls.Add(this.valG);
            this.tabPage2.Controls.Add(this.valH);
            this.tabPage2.Controls.Add(this.valR);
            this.tabPage2.Controls.Add(this.clearposbutton);
            this.tabPage2.Controls.Add(this.getposbutton);
            this.tabPage2.Controls.Add(this.DataImage);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(249, 171);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // valV
            // 
            this.valV.AutoSize = true;
            this.valV.Location = new System.Drawing.Point(174, 110);
            this.valV.Name = "valV";
            this.valV.Size = new System.Drawing.Size(41, 12);
            this.valV.TabIndex = 4;
            this.valV.Text = "label1";
            // 
            // valB
            // 
            this.valB.AutoSize = true;
            this.valB.Location = new System.Drawing.Point(174, 152);
            this.valB.Name = "valB";
            this.valB.Size = new System.Drawing.Size(41, 12);
            this.valB.TabIndex = 4;
            this.valB.Text = "label1";
            // 
            // valS
            // 
            this.valS.AutoSize = true;
            this.valS.Location = new System.Drawing.Point(174, 98);
            this.valS.Name = "valS";
            this.valS.Size = new System.Drawing.Size(41, 12);
            this.valS.TabIndex = 3;
            this.valS.Text = "label1";
            // 
            // valG
            // 
            this.valG.AutoSize = true;
            this.valG.Location = new System.Drawing.Point(174, 140);
            this.valG.Name = "valG";
            this.valG.Size = new System.Drawing.Size(41, 12);
            this.valG.TabIndex = 3;
            this.valG.Text = "label1";
            // 
            // valH
            // 
            this.valH.AutoSize = true;
            this.valH.Location = new System.Drawing.Point(174, 86);
            this.valH.Name = "valH";
            this.valH.Size = new System.Drawing.Size(41, 12);
            this.valH.TabIndex = 2;
            this.valH.Text = "label1";
            // 
            // valR
            // 
            this.valR.AutoSize = true;
            this.valR.Location = new System.Drawing.Point(174, 128);
            this.valR.Name = "valR";
            this.valR.Size = new System.Drawing.Size(41, 12);
            this.valR.TabIndex = 2;
            this.valR.Text = "label1";
            // 
            // getposbutton
            // 
            this.getposbutton.Location = new System.Drawing.Point(171, 3);
            this.getposbutton.Name = "getposbutton";
            this.getposbutton.Size = new System.Drawing.Size(75, 23);
            this.getposbutton.TabIndex = 1;
            this.getposbutton.Text = "生成坐标";
            this.getposbutton.UseVisualStyleBackColor = true;
            this.getposbutton.Click += new System.EventHandler(this.getposbutton_Click);
            // 
            // DataImage
            // 
            this.DataImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DataImage.Location = new System.Drawing.Point(3, 3);
            this.DataImage.Name = "DataImage";
            this.DataImage.Size = new System.Drawing.Size(165, 165);
            this.DataImage.TabIndex = 0;
            this.DataImage.TabStop = false;
            this.DataImage.MouseClick += new System.Windows.Forms.MouseEventHandler(this.DataImage_MouseClick);
            // 
            // copyscreenbutton
            // 
            this.copyscreenbutton.Location = new System.Drawing.Point(210, 180);
            this.copyscreenbutton.Name = "copyscreenbutton";
            this.copyscreenbutton.Size = new System.Drawing.Size(42, 23);
            this.copyscreenbutton.TabIndex = 10;
            this.copyscreenbutton.Text = "copy";
            this.copyscreenbutton.UseVisualStyleBackColor = true;
            this.copyscreenbutton.Click += new System.EventHandler(this.copyscreenbutton_Click);
            // 
            // clearposbutton
            // 
            this.clearposbutton.Location = new System.Drawing.Point(171, 32);
            this.clearposbutton.Name = "clearposbutton";
            this.clearposbutton.Size = new System.Drawing.Size(75, 23);
            this.clearposbutton.TabIndex = 1;
            this.clearposbutton.Text = "清空选择";
            this.clearposbutton.UseVisualStyleBackColor = true;
            this.clearposbutton.Click += new System.EventHandler(this.clearposbutton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(256, 63);
            this.Controls.Add(this.copyscreenbutton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.TestPic);
            this.Controls.Add(this.InputButton);
            this.Controls.Add(this.InputBOX);
            this.Controls.Add(this.OutputBox);
            this.Controls.Add(this.LoadLua);
            this.Controls.Add(this.PortLable);
            this.Controls.Add(this.ButtonMode);
            this.Controls.Add(this.HIDCheck);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.SendImg);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "MJpegServer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TestPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DrawTest)).EndInit();
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button SendImg;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label ButtonMode;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Label PortLable;
        private System.Windows.Forms.CheckBox HIDCheck;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.Button LoadLua;
        private System.Windows.Forms.TextBox OutputBox;
        private System.Windows.Forms.TextBox InputBOX;
        private System.Windows.Forms.Button InputButton;
        private System.Windows.Forms.PictureBox TestPic;
        private System.Windows.Forms.PictureBox DrawTest;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.PictureBox DataImage;
        private System.Windows.Forms.Button copyscreenbutton;
        private System.Windows.Forms.Button getposbutton;
        private System.Windows.Forms.Label valB;
        private System.Windows.Forms.Label valG;
        private System.Windows.Forms.Label valR;
        private System.Windows.Forms.Label valV;
        private System.Windows.Forms.Label valS;
        private System.Windows.Forms.Label valH;
        private System.Windows.Forms.Button clearposbutton;
    }
}

