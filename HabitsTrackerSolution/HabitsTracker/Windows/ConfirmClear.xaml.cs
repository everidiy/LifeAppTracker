using System.Windows;

namespace HabitsTracker.Windows
{
    public partial class ConfirmClear : Window
    {
        public ConfirmClear()
        {
            InitializeComponent();
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
