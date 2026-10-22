namespace Bonus_02
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
            components = new System.ComponentModel.Container();
            grpConvert = new GroupBox();
            lblAmount = new Label();
            txtAmount = new TextBox();
            lblFrom = new Label();
            cboFrom = new ComboBox();
            btnSwap = new Button();
            lblTo = new Label();
            cboTo = new ComboBox();
            lblResult = new Label();
            txtResult = new TextBox();
            lblRate = new Label();
            lblUpdated = new Label();
            tabMain = new TabControl();
            tabLine = new TabPage();
            chartLine = new System.Windows.Forms.DataVisualization.Charting.Chart();
            tabBar = new TabPage();
            chartBar = new System.Windows.Forms.DataVisualization.Charting.Chart();
            tabTable = new TabPage();
            gridRates = new DataGridView();
            tabML = new TabPage();
            txtStrategy = new TextBox();
            chartForecast = new System.Windows.Forms.DataVisualization.Charting.Chart();
            statusStrip = new StatusStrip();
            lblStatus = new ToolStripStatusLabel();
            refreshTimer = new System.Windows.Forms.Timer(components);
            grpConvert.SuspendLayout();
            tabMain.SuspendLayout();
            tabLine.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)chartLine).BeginInit();
            tabBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)chartBar).BeginInit();
            tabTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridRates).BeginInit();
            tabML.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)chartForecast).BeginInit();
            statusStrip.SuspendLayout();
            SuspendLayout();
            //
            // grpConvert
            //
            grpConvert.Controls.Add(lblAmount);
            grpConvert.Controls.Add(txtAmount);
            grpConvert.Controls.Add(lblFrom);
            grpConvert.Controls.Add(cboFrom);
            grpConvert.Controls.Add(btnSwap);
            grpConvert.Controls.Add(lblTo);
            grpConvert.Controls.Add(cboTo);
            grpConvert.Controls.Add(lblResult);
            grpConvert.Controls.Add(txtResult);
            grpConvert.Controls.Add(lblRate);
            grpConvert.Controls.Add(lblUpdated);
            grpConvert.Dock = DockStyle.Top;
            grpConvert.Location = new Point(0, 0);
            grpConvert.Name = "grpConvert";
            grpConvert.Padding = new Padding(10);
            grpConvert.Size = new Size(1000, 160);
            grpConvert.TabIndex = 0;
            grpConvert.TabStop = false;
            grpConvert.Text = "換匯";
            //
            // lblAmount
            //
            lblAmount.AutoSize = true;
            lblAmount.Location = new Point(20, 40);
            lblAmount.Name = "lblAmount";
            lblAmount.Size = new Size(37, 18);
            lblAmount.TabIndex = 0;
            lblAmount.Text = "金額";
            //
            // txtAmount
            //
            txtAmount.Font = new Font("Microsoft JhengHei UI", 12F);
            txtAmount.Location = new Point(70, 34);
            txtAmount.Name = "txtAmount";
            txtAmount.Size = new Size(180, 28);
            txtAmount.TabIndex = 1;
            txtAmount.Text = "100";
            txtAmount.TextAlign = HorizontalAlignment.Right;
            txtAmount.TextChanged += txtAmount_TextChanged;
            //
            // lblFrom
            //
            lblFrom.AutoSize = true;
            lblFrom.Location = new Point(270, 40);
            lblFrom.Name = "lblFrom";
            lblFrom.Size = new Size(22, 18);
            lblFrom.TabIndex = 2;
            lblFrom.Text = "從";
            //
            // cboFrom
            //
            cboFrom.DropDownStyle = ComboBoxStyle.DropDownList;
            cboFrom.Font = new Font("Microsoft JhengHei UI", 12F);
            cboFrom.Location = new Point(300, 34);
            cboFrom.Name = "cboFrom";
            cboFrom.Size = new Size(180, 28);
            cboFrom.TabIndex = 3;
            cboFrom.SelectedIndexChanged += Currency_SelectedIndexChanged;
            //
            // btnSwap
            //
            btnSwap.Font = new Font("Microsoft JhengHei UI", 11F, FontStyle.Bold);
            btnSwap.Location = new Point(495, 31);
            btnSwap.Name = "btnSwap";
            btnSwap.Size = new Size(100, 34);
            btnSwap.TabIndex = 4;
            btnSwap.Text = "⇄ 對轉";
            btnSwap.UseVisualStyleBackColor = true;
            btnSwap.Click += btnSwap_Click;
            //
            // lblTo
            //
            lblTo.AutoSize = true;
            lblTo.Location = new Point(610, 40);
            lblTo.Name = "lblTo";
            lblTo.Size = new Size(37, 18);
            lblTo.TabIndex = 5;
            lblTo.Text = "換成";
            //
            // cboTo
            //
            cboTo.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTo.Font = new Font("Microsoft JhengHei UI", 12F);
            cboTo.Location = new Point(655, 34);
            cboTo.Name = "cboTo";
            cboTo.Size = new Size(180, 28);
            cboTo.TabIndex = 6;
            cboTo.SelectedIndexChanged += Currency_SelectedIndexChanged;
            //
            // lblResult
            //
            lblResult.AutoSize = true;
            lblResult.Location = new Point(20, 88);
            lblResult.Name = "lblResult";
            lblResult.Size = new Size(37, 18);
            lblResult.TabIndex = 7;
            lblResult.Text = "結果";
            //
            // txtResult
            //
            txtResult.Font = new Font("Microsoft JhengHei UI", 14F, FontStyle.Bold);
            txtResult.ForeColor = Color.FromArgb(0, 102, 204);
            txtResult.Location = new Point(70, 80);
            txtResult.Name = "txtResult";
            txtResult.ReadOnly = true;
            txtResult.Size = new Size(410, 32);
            txtResult.TabIndex = 8;
            txtResult.TabStop = false;
            txtResult.TextAlign = HorizontalAlignment.Right;
            //
            // lblRate
            //
            lblRate.AutoSize = true;
            lblRate.Font = new Font("Microsoft JhengHei UI", 11F);
            lblRate.Location = new Point(495, 87);
            lblRate.Name = "lblRate";
            lblRate.Size = new Size(0, 19);
            lblRate.TabIndex = 9;
            //
            // lblUpdated
            //
            lblUpdated.AutoSize = true;
            lblUpdated.ForeColor = Color.Gray;
            lblUpdated.Location = new Point(70, 124);
            lblUpdated.Name = "lblUpdated";
            lblUpdated.Size = new Size(0, 18);
            lblUpdated.TabIndex = 10;
            //
            // tabMain
            //
            tabMain.Controls.Add(tabLine);
            tabMain.Controls.Add(tabBar);
            tabMain.Controls.Add(tabTable);
            tabMain.Controls.Add(tabML);
            tabMain.Dock = DockStyle.Fill;
            tabMain.Location = new Point(0, 160);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(1000, 518);
            tabMain.TabIndex = 1;
            //
            // tabLine
            //
            tabLine.Controls.Add(chartLine);
            tabLine.Location = new Point(4, 27);
            tabLine.Name = "tabLine";
            tabLine.Size = new Size(992, 487);
            tabLine.TabIndex = 0;
            tabLine.Text = "折線圖";
            tabLine.UseVisualStyleBackColor = true;
            //
            // chartLine
            //
            chartLine.Dock = DockStyle.Fill;
            chartLine.Location = new Point(0, 0);
            chartLine.Name = "chartLine";
            chartLine.Size = new Size(992, 487);
            chartLine.TabIndex = 0;
            //
            // tabBar
            //
            tabBar.Controls.Add(chartBar);
            tabBar.Location = new Point(4, 27);
            tabBar.Name = "tabBar";
            tabBar.Size = new Size(992, 487);
            tabBar.TabIndex = 1;
            tabBar.Text = "長條圖";
            tabBar.UseVisualStyleBackColor = true;
            //
            // chartBar
            //
            chartBar.Dock = DockStyle.Fill;
            chartBar.Location = new Point(0, 0);
            chartBar.Name = "chartBar";
            chartBar.Size = new Size(992, 487);
            chartBar.TabIndex = 0;
            //
            // tabTable
            //
            tabTable.Controls.Add(gridRates);
            tabTable.Location = new Point(4, 27);
            tabTable.Name = "tabTable";
            tabTable.Size = new Size(992, 487);
            tabTable.TabIndex = 2;
            tabTable.Text = "表格";
            tabTable.UseVisualStyleBackColor = true;
            //
            // gridRates
            //
            gridRates.AllowUserToAddRows = false;
            gridRates.AllowUserToDeleteRows = false;
            gridRates.AllowUserToResizeRows = false;
            gridRates.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridRates.BackgroundColor = Color.White;
            gridRates.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridRates.Dock = DockStyle.Fill;
            gridRates.Location = new Point(0, 0);
            gridRates.Name = "gridRates";
            gridRates.ReadOnly = true;
            gridRates.RowHeadersVisible = false;
            gridRates.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridRates.Size = new Size(992, 487);
            gridRates.TabIndex = 0;
            //
            // tabML
            //
            tabML.Controls.Add(txtStrategy);
            tabML.Controls.Add(chartForecast);
            tabML.Location = new Point(4, 27);
            tabML.Name = "tabML";
            tabML.Size = new Size(992, 487);
            tabML.TabIndex = 3;
            tabML.Text = "機器學習預測";
            tabML.UseVisualStyleBackColor = true;
            //
            // txtStrategy
            //
            txtStrategy.BackColor = Color.White;
            txtStrategy.BorderStyle = BorderStyle.None;
            txtStrategy.Dock = DockStyle.Fill;
            txtStrategy.Font = new Font("Microsoft JhengHei UI", 10.5F);
            txtStrategy.Location = new Point(520, 0);
            txtStrategy.Multiline = true;
            txtStrategy.Name = "txtStrategy";
            txtStrategy.ReadOnly = true;
            txtStrategy.ScrollBars = ScrollBars.Vertical;
            txtStrategy.Size = new Size(472, 487);
            txtStrategy.TabIndex = 1;
            txtStrategy.TabStop = false;
            //
            // chartForecast
            //
            chartForecast.Dock = DockStyle.Left;
            chartForecast.Location = new Point(0, 0);
            chartForecast.Name = "chartForecast";
            chartForecast.Size = new Size(520, 487);
            chartForecast.TabIndex = 0;
            //
            // statusStrip
            //
            statusStrip.Items.AddRange(new ToolStripItem[] { lblStatus });
            statusStrip.Location = new Point(0, 678);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(1000, 22);
            statusStrip.TabIndex = 2;
            //
            // lblStatus
            //
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 17);
            //
            // refreshTimer
            //
            refreshTimer.Interval = 600000;
            refreshTimer.Tick += refreshTimer_Tick;
            //
            // Form1
            //
            AutoScaleDimensions = new SizeF(8F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 700);
            Controls.Add(tabMain);
            Controls.Add(grpConvert);
            Controls.Add(statusStrip);
            Font = new Font("Microsoft JhengHei UI", 10F);
            MinimumSize = new Size(900, 650);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "換匯系統";
            Load += Form1_Load;
            grpConvert.ResumeLayout(false);
            grpConvert.PerformLayout();
            tabMain.ResumeLayout(false);
            tabLine.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)chartLine).EndInit();
            tabBar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)chartBar).EndInit();
            tabTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridRates).EndInit();
            tabML.ResumeLayout(false);
            tabML.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)chartForecast).EndInit();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox grpConvert;
        private Label lblAmount;
        private TextBox txtAmount;
        private Label lblFrom;
        private ComboBox cboFrom;
        private Button btnSwap;
        private Label lblTo;
        private ComboBox cboTo;
        private Label lblResult;
        private TextBox txtResult;
        private Label lblRate;
        private Label lblUpdated;
        private TabControl tabMain;
        private TabPage tabLine;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartLine;
        private TabPage tabBar;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartBar;
        private TabPage tabTable;
        private DataGridView gridRates;
        private TabPage tabML;
        private TextBox txtStrategy;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartForecast;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Timer refreshTimer;
    }
}
