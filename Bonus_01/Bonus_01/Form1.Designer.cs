namespace Bonus_01
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            grp1to1 = new GroupBox();
            picLight11 = new PictureBox();
            picSwitch11 = new PictureBox();
            lblSwitch11 = new Label();
            grp2to1 = new GroupBox();
            picLight21 = new PictureBox();
            picSwitch21 = new PictureBox();
            picSwitch22 = new PictureBox();
            lblSwitch21 = new Label();
            lblSwitch22 = new Label();
            grp3to2 = new GroupBox();
            btnGoMerit = new Button();
            picLight31 = new PictureBox();
            picLight32 = new PictureBox();
            lblLight31 = new Label();
            lblLight32 = new Label();
            picSwitch31 = new PictureBox();
            picSwitch32 = new PictureBox();
            picSwitch33 = new PictureBox();
            lblSwitch31 = new Label();
            lblSwitch32 = new Label();
            lblSwitch33 = new Label();
            grp1to1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLight11).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picSwitch11).BeginInit();
            grp2to1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLight21).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picSwitch21).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picSwitch22).BeginInit();
            grp3to2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLight31).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picLight32).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picSwitch31).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picSwitch32).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picSwitch33).BeginInit();
            SuspendLayout();
            //
            // grp1to1
            //
            grp1to1.Controls.Add(picLight11);
            grp1to1.Controls.Add(picSwitch11);
            grp1to1.Controls.Add(lblSwitch11);
            grp1to1.Location = new Point(12, 12);
            grp1to1.Name = "grp1to1";
            grp1to1.Size = new Size(200, 260);
            grp1to1.TabIndex = 0;
            grp1to1.TabStop = false;
            grp1to1.Text = "一對一控制：L = S1";
            //
            // picLight11
            //
            picLight11.Location = new Point(50, 30);
            picLight11.Name = "picLight11";
            picLight11.Size = new Size(100, 100);
            picLight11.SizeMode = PictureBoxSizeMode.Zoom;
            picLight11.TabIndex = 0;
            picLight11.TabStop = false;
            //
            // picSwitch11
            //
            picSwitch11.Cursor = Cursors.Hand;
            picSwitch11.Location = new Point(68, 150);
            picSwitch11.Name = "picSwitch11";
            picSwitch11.Size = new Size(64, 64);
            picSwitch11.SizeMode = PictureBoxSizeMode.Zoom;
            picSwitch11.TabIndex = 1;
            picSwitch11.TabStop = false;
            picSwitch11.Click += Switch_Click;
            //
            // lblSwitch11
            //
            lblSwitch11.Location = new Point(68, 218);
            lblSwitch11.Name = "lblSwitch11";
            lblSwitch11.Size = new Size(64, 23);
            lblSwitch11.TabIndex = 2;
            lblSwitch11.Text = "S1";
            lblSwitch11.TextAlign = ContentAlignment.MiddleCenter;
            //
            // grp2to1
            //
            grp2to1.Controls.Add(picLight21);
            grp2to1.Controls.Add(picSwitch21);
            grp2to1.Controls.Add(picSwitch22);
            grp2to1.Controls.Add(lblSwitch21);
            grp2to1.Controls.Add(lblSwitch22);
            grp2to1.Location = new Point(224, 12);
            grp2to1.Name = "grp2to1";
            grp2to1.Size = new Size(240, 260);
            grp2to1.TabIndex = 1;
            grp2to1.TabStop = false;
            grp2to1.Text = "二對一控制：L = S1 ⊕ S2";
            //
            // picLight21
            //
            picLight21.Location = new Point(70, 30);
            picLight21.Name = "picLight21";
            picLight21.Size = new Size(100, 100);
            picLight21.SizeMode = PictureBoxSizeMode.Zoom;
            picLight21.TabIndex = 0;
            picLight21.TabStop = false;
            //
            // picSwitch21
            //
            picSwitch21.Cursor = Cursors.Hand;
            picSwitch21.Location = new Point(36, 150);
            picSwitch21.Name = "picSwitch21";
            picSwitch21.Size = new Size(64, 64);
            picSwitch21.SizeMode = PictureBoxSizeMode.Zoom;
            picSwitch21.TabIndex = 1;
            picSwitch21.TabStop = false;
            picSwitch21.Click += Switch_Click;
            //
            // picSwitch22
            //
            picSwitch22.Cursor = Cursors.Hand;
            picSwitch22.Location = new Point(140, 150);
            picSwitch22.Name = "picSwitch22";
            picSwitch22.Size = new Size(64, 64);
            picSwitch22.SizeMode = PictureBoxSizeMode.Zoom;
            picSwitch22.TabIndex = 2;
            picSwitch22.TabStop = false;
            picSwitch22.Click += Switch_Click;
            //
            // lblSwitch21
            //
            lblSwitch21.Location = new Point(36, 218);
            lblSwitch21.Name = "lblSwitch21";
            lblSwitch21.Size = new Size(64, 23);
            lblSwitch21.TabIndex = 3;
            lblSwitch21.Text = "S1";
            lblSwitch21.TextAlign = ContentAlignment.MiddleCenter;
            //
            // lblSwitch22
            //
            lblSwitch22.Location = new Point(140, 218);
            lblSwitch22.Name = "lblSwitch22";
            lblSwitch22.Size = new Size(64, 23);
            lblSwitch22.TabIndex = 4;
            lblSwitch22.Text = "S2";
            lblSwitch22.TextAlign = ContentAlignment.MiddleCenter;
            //
            // grp3to2
            //
            grp3to2.Controls.Add(picLight31);
            grp3to2.Controls.Add(picLight32);
            grp3to2.Controls.Add(lblLight31);
            grp3to2.Controls.Add(lblLight32);
            grp3to2.Controls.Add(picSwitch31);
            grp3to2.Controls.Add(picSwitch32);
            grp3to2.Controls.Add(picSwitch33);
            grp3to2.Controls.Add(lblSwitch31);
            grp3to2.Controls.Add(lblSwitch32);
            grp3to2.Controls.Add(lblSwitch33);
            grp3to2.Location = new Point(476, 12);
            grp3to2.Name = "grp3to2";
            grp3to2.Size = new Size(340, 260);
            grp3to2.TabIndex = 2;
            grp3to2.TabStop = false;
            grp3to2.Text = "三對二控制：L1 = S1 ⊕ S2，L2 = S2 ⊕ S3";
            //
            // picLight31
            //
            picLight31.Location = new Point(60, 24);
            picLight31.Name = "picLight31";
            picLight31.Size = new Size(100, 100);
            picLight31.SizeMode = PictureBoxSizeMode.Zoom;
            picLight31.TabIndex = 0;
            picLight31.TabStop = false;
            //
            // picLight32
            //
            picLight32.Location = new Point(180, 24);
            picLight32.Name = "picLight32";
            picLight32.Size = new Size(100, 100);
            picLight32.SizeMode = PictureBoxSizeMode.Zoom;
            picLight32.TabIndex = 1;
            picLight32.TabStop = false;
            //
            // lblLight31
            //
            lblLight31.Location = new Point(60, 124);
            lblLight31.Name = "lblLight31";
            lblLight31.Size = new Size(100, 20);
            lblLight31.TabIndex = 2;
            lblLight31.Text = "L1";
            lblLight31.TextAlign = ContentAlignment.MiddleCenter;
            //
            // lblLight32
            //
            lblLight32.Location = new Point(180, 124);
            lblLight32.Name = "lblLight32";
            lblLight32.Size = new Size(100, 20);
            lblLight32.TabIndex = 3;
            lblLight32.Text = "L2";
            lblLight32.TextAlign = ContentAlignment.MiddleCenter;
            //
            // picSwitch31
            //
            picSwitch31.Cursor = Cursors.Hand;
            picSwitch31.Location = new Point(30, 150);
            picSwitch31.Name = "picSwitch31";
            picSwitch31.Size = new Size(64, 64);
            picSwitch31.SizeMode = PictureBoxSizeMode.Zoom;
            picSwitch31.TabIndex = 4;
            picSwitch31.TabStop = false;
            picSwitch31.Click += Switch_Click;
            //
            // picSwitch32
            //
            picSwitch32.Cursor = Cursors.Hand;
            picSwitch32.Location = new Point(138, 150);
            picSwitch32.Name = "picSwitch32";
            picSwitch32.Size = new Size(64, 64);
            picSwitch32.SizeMode = PictureBoxSizeMode.Zoom;
            picSwitch32.TabIndex = 5;
            picSwitch32.TabStop = false;
            picSwitch32.Click += Switch_Click;
            //
            // picSwitch33
            //
            picSwitch33.Cursor = Cursors.Hand;
            picSwitch33.Location = new Point(246, 150);
            picSwitch33.Name = "picSwitch33";
            picSwitch33.Size = new Size(64, 64);
            picSwitch33.SizeMode = PictureBoxSizeMode.Zoom;
            picSwitch33.TabIndex = 6;
            picSwitch33.TabStop = false;
            picSwitch33.Click += Switch_Click;
            //
            // lblSwitch31
            //
            lblSwitch31.Location = new Point(30, 218);
            lblSwitch31.Name = "lblSwitch31";
            lblSwitch31.Size = new Size(64, 23);
            lblSwitch31.TabIndex = 7;
            lblSwitch31.Text = "S1";
            lblSwitch31.TextAlign = ContentAlignment.MiddleCenter;
            //
            // lblSwitch32
            //
            lblSwitch32.Location = new Point(138, 218);
            lblSwitch32.Name = "lblSwitch32";
            lblSwitch32.Size = new Size(64, 23);
            lblSwitch32.TabIndex = 8;
            lblSwitch32.Text = "S2";
            lblSwitch32.TextAlign = ContentAlignment.MiddleCenter;
            //
            // lblSwitch33
            //
            lblSwitch33.Location = new Point(246, 218);
            lblSwitch33.Name = "lblSwitch33";
            lblSwitch33.Size = new Size(64, 23);
            lblSwitch33.TabIndex = 9;
            lblSwitch33.Text = "S3";
            lblSwitch33.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnGoMerit
            // 
            btnGoMerit.Dock = DockStyle.Bottom;
            btnGoMerit.Font = new Font("Microsoft JhengHei UI", 11F);
            btnGoMerit.Location = new Point(0, 284);
            btnGoMerit.Name = "btnGoMerit";
            btnGoMerit.Size = new Size(828, 40);
            btnGoMerit.TabIndex = 3;
            btnGoMerit.TabStop = false;
            btnGoMerit.Text = "切換到 功德 +1 ▶";
            btnGoMerit.UseVisualStyleBackColor = true;
            btnGoMerit.Click += btnGoMerit_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(828, 324);
            Controls.Add(grp1to1);
            Controls.Add(grp2to1);
            Controls.Add(grp3to2);
            Controls.Add(btnGoMerit);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "燈泡控制";
            Load += Form1_Load;
            grp1to1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picLight11).EndInit();
            ((System.ComponentModel.ISupportInitialize)picSwitch11).EndInit();
            grp2to1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picLight21).EndInit();
            ((System.ComponentModel.ISupportInitialize)picSwitch21).EndInit();
            ((System.ComponentModel.ISupportInitialize)picSwitch22).EndInit();
            grp3to2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picLight31).EndInit();
            ((System.ComponentModel.ISupportInitialize)picLight32).EndInit();
            ((System.ComponentModel.ISupportInitialize)picSwitch31).EndInit();
            ((System.ComponentModel.ISupportInitialize)picSwitch32).EndInit();
            ((System.ComponentModel.ISupportInitialize)picSwitch33).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox grp1to1;
        private PictureBox picLight11;
        private PictureBox picSwitch11;
        private Label lblSwitch11;
        private GroupBox grp2to1;
        private PictureBox picLight21;
        private PictureBox picSwitch21;
        private PictureBox picSwitch22;
        private Label lblSwitch21;
        private Label lblSwitch22;
        private GroupBox grp3to2;
        private PictureBox picLight31;
        private PictureBox picLight32;
        private Label lblLight31;
        private Label lblLight32;
        private PictureBox picSwitch31;
        private PictureBox picSwitch32;
        private PictureBox picSwitch33;
        private Label lblSwitch31;
        private Label lblSwitch32;
        private Label lblSwitch33;
        private Button btnGoMerit;
    }
}
