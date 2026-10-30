using HabitsTracker.Classes;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace HabitsTracker.Windows
{
    public partial class AddNewHabitWindow : Window
    {
        private ObservableCollection<Habit> _mainHabitsList;
        private ObservableCollection<Habit> _archiveHabitsList;
        private ObservableCollection<Habit> _statsHabitsList;

        public AddNewHabitWindow(
            ObservableCollection<Habit> mainHabitsList,
            ObservableCollection<Habit> archiveHabitsList,
            ObservableCollection<Habit> statsHabitsList)
        {
            InitializeComponent();

            _mainHabitsList = mainHabitsList;
            _archiveHabitsList = archiveHabitsList;
            _statsHabitsList = statsHabitsList;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(HabitNameInput.Text) ||
            RegularitySelect.SelectedIndex < 0 ||
            CategorySelect.SelectedIndex < 0)
            {
                Description.Text = $"Присутствуют пустые строки! \n(Заметка необязательна) ";
                Description.Foreground = Brushes.Red;
                return;
            }

            if (HabitNameInput.Text.Length > 40)
            {
                Description.Text = $"Слишком большое название! \n(нужно 40 символов) \nУберите {HabitNameInput.Text.Length - 40} символов!";
                Description.Foreground = Brushes.Red;
                return;
            }

            Habit habit = new Habit()
            {
                Name = HabitNameInput.Text,
                Regularity = RegularitySelect.Text,
                Category = CategorySelect.Text,
                Note = HabitNoteInput.Text
            };

            _mainHabitsList.Add(habit);
            _statsHabitsList.Add(habit);

            if (Habits._ViewHabitsWindow == null || !Habits._ViewHabitsWindow.IsLoaded)
            {
                Habits._ViewHabitsWindow = new ViewHabitsWindow(_mainHabitsList, _archiveHabitsList, _statsHabitsList, false);
                Habits._ViewHabitsWindow.Owner = Application.Current.MainWindow;
                Habits._ViewHabitsWindow.Top = Application.Current.MainWindow.Top + Application.Current.MainWindow.Height + 10;
                Habits._ViewHabitsWindow.Left = Application.Current.MainWindow.Left;
                Habits._ViewHabitsWindow.Show();
            }
            else
            {
                Habits._ViewHabitsWindow.Top = Application.Current.MainWindow.Top + Application.Current.MainWindow.Height + 10;
                Habits._ViewHabitsWindow.Left = Application.Current.MainWindow.Left;
                Habits._ViewHabitsWindow.Activate();
            }

            if (Habits._ShowStatistics == null || !Habits._ShowStatistics.IsLoaded)
            {
                Habits._ShowStatistics = new ShowStatistics(_mainHabitsList, _statsHabitsList, false);
                Habits._ShowStatistics.Owner = Application.Current.MainWindow;
                Habits._ShowStatistics.Top = Application.Current.MainWindow.Top;
                Habits._ShowStatistics.Left = Application.Current.MainWindow.Width - 325;
                Habits._ShowStatistics.Show();
            }
            else
            {
                Habits._ShowStatistics.Top = Application.Current.MainWindow.Top;
                Habits._ShowStatistics.Left = Application.Current.MainWindow.Width - 325;
                Habits._ShowStatistics.Activate();
            }

            HabitNameInput.Text = "";
            RegularitySelect.SelectedIndex = 0;
            CategorySelect.SelectedIndex = 0;
            HabitNoteInput.Text = "";

            Description.Text = "Поразмышляйте о своих целях. \n Маленькие действия сегодня создают круги на воде вашего будущего";
            var converter = new BrushConverter();
            Description.Foreground = (Brush)converter.ConvertFromString("#003366");
        }
    }
}
