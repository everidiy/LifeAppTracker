using System.Windows;
using FinanceTracker;
using SuccessDiary.Windows;
using HabitsTracker.Windows;

namespace LifeAppWindow
{
    public partial class Menu : Window
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void Habits_Click(object sender, RoutedEventArgs e)
        {
            Habits habitsWindow = new Habits();
            habitsWindow.Owner = this;
            habitsWindow.Show();
            this.Hide();
        }

        private void Finace_Click(object sender, RoutedEventArgs e)
        {
            Finance financeWindow = new Finance();
            financeWindow.Owner = this;
            financeWindow.Show();
            this.Hide();
        }

        private void Diary_Click(object sender, RoutedEventArgs e)
        {
            Diary diaryWindow = new Diary();
            diaryWindow.Owner = this;
            diaryWindow.Show();
            this.Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}