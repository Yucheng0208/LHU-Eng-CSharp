namespace Bonus_01
{
    public partial class FormMerit : Form
    {
        public FormMerit()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Close();
        }

        // 空白鍵也能敲木魚
        private void FormMerit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Space) return;

            fishView.Hit();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }
}
