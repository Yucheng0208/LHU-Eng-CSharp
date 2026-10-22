namespace Bonus_01
{
    partial class FormMerit
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
            fishView = new WoodenFishView();
            btnBack = new Button();
            SuspendLayout();
            //
            // fishView
            //
            fishView.Dock = DockStyle.Fill;
            fishView.Location = new Point(0, 0);
            fishView.Name = "fishView";
            fishView.Size = new Size(520, 560);
            fishView.TabIndex = 0;
            //
            // btnBack
            //
            btnBack.Dock = DockStyle.Bottom;
            btnBack.Font = new Font("Microsoft JhengHei UI", 11F);
            btnBack.Location = new Point(0, 560);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(520, 40);
            btnBack.TabIndex = 1;
            btnBack.TabStop = false;
            btnBack.Text = "◀ 返回燈泡控制";
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += btnBack_Click;
            //
            // FormMerit
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(520, 600);
            Controls.Add(fishView);
            Controls.Add(btnBack);
            KeyPreview = true;
            MinimumSize = new Size(420, 480);
            Name = "FormMerit";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "功德 +1";
            KeyDown += FormMerit_KeyDown;
            ResumeLayout(false);
        }

        #endregion

        private WoodenFishView fishView;
        private Button btnBack;
    }
}
