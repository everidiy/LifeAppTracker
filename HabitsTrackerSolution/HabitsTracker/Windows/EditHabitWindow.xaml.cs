using HabitsTracker.Classes;
using System.Windows;
using System.Windows.Media;

namespace HabitsTracker.Windows
{
    public partial class EditHabitWindow : Window
    {
        private Habit _currentHabit;

        public EditHabitWindow(Habit habit)
        {
            InitializeComponent();

            _currentHabit = habit;
            EditHabitFromView();

        }

        public void UpdateData(Habit newHabit)
        {
            _currentHabit = newHabit;

            HabitNameInput.Text = newHabit.Name;
            RegularitySelected.Text = newHabit.Regularity;
            CategorySelected.Text = newHabit.Category;
            HabitNoteInput.Text = newHabit.Note;
        }


        private void EditHabitFromView()
        {
            HabitNameInput.Text = _currentHabit.Name;
            RegularitySelected.Text = _currentHabit.Regularity;
            CategorySelected.Text = _currentHabit.Category;
            HabitNoteInput.Text = _currentHabit.Note;
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentHabit == null) return;

            if (string.IsNullOrEmpty(HabitNameInput.Text) ||
                RegularitySelected.SelectedIndex < 0 ||
                CategorySelected.SelectedIndex < 0)
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

            _currentHabit.Name = HabitNameInput.Text;
            _currentHabit.Regularity = RegularitySelected.Text;
            _currentHabit.Category = CategorySelected.Text;
            _currentHabit.Note = HabitNoteInput.Text;

            if (Habits._ViewHabitsWindow != null && Habits._ViewHabitsWindow.HabitsTable != null)
            {
                Habits._ViewHabitsWindow.HabitsTable.Items.Refresh();
            }

            this.Close();

            Description.Text = "Прогресс требует гибкости. \nПродолжайте настраивать свои привычки ради лучшего завтра\"";
            var converter = new BrushConverter();
            Description.Foreground = (Brush)converter.ConvertFromString("#003366");
        }
    }
}
