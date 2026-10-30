using HabitsTracker.Classes;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HabitsTracker.Windows
{
    public partial class ViewHabitsWindow : Window
    {
        private ObservableCollection<Habit> _HABITS;
        private ObservableCollection<Habit> _ARCHIVE;
        private ObservableCollection<Habit> _STATS;
        bool saved = false;

        public readonly string[] validCategories =
        {
            "Здоровье и спорт",
            "Разум и образование",
            "Рутина и дом",
            "Работа и финансы",
            "Творчество и хобби"
        };

        public readonly string[] validRegularities =
        {
            "Каждый день",
            "Через день",
            "По будням",
            "По выходным",
            "Раз в неделю"
        };

        public ViewHabitsWindow(
            ObservableCollection<Habit> habits, 
            ObservableCollection<Habit> archiveHabits,
            ObservableCollection<Habit> stats,
            bool saves = true)
        {
            InitializeComponent();

            _HABITS = habits;
            _ARCHIVE = archiveHabits;
            _STATS = stats;

            HabitsTable.ItemsSource = habits;

            saved = saves;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!saved)
            {
                ConfirmCloseWindow confirm = new ConfirmCloseWindow();
                confirm.Owner = this;

                bool? result = confirm.ShowDialog();
                if (result == true)
                {
                    _HABITS.Clear();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (OptionViewHabitWindow.Visibility == Visibility.Visible)
                {
                    OptionViewHabitWindow.Visibility = Visibility.Collapsed;
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                if (OptionViewHabitWindow.Visibility == Visibility.Collapsed)
                {
                    OptionViewHabitWindow.Visibility = Visibility.Visible;
                }
            }

        }

        private void EditHabit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OptionViewHabitWindow.Visibility = Visibility.Collapsed;
            e.Handled = true;

            if (HabitsTable.SelectedItem is not Habit habit)
            {
                return;
            }

            if (Habits._EditHabitWindow == null || !Habits._EditHabitWindow.IsLoaded)
            {
                Habits._EditHabitWindow = new EditHabitWindow(habit);
                Habits._EditHabitWindow.Owner = Application.Current.MainWindow;

                if (Habits._AddNewHabitWindow != null && Habits._AddNewHabitWindow.IsLoaded)
                {
                    Habits._EditHabitWindow.Left = Habits._AddNewHabitWindow.Left + Habits._AddNewHabitWindow.Width + 5;
                    Habits._EditHabitWindow.Top = Habits._AddNewHabitWindow.Top;
                }
                else
                {
                    Habits._EditHabitWindow.Top = Application.Current.MainWindow.Top;
                    Habits._EditHabitWindow.Left = Application.Current.MainWindow.Left + Application.Current.MainWindow.Width + 5;
                }

                Habits._EditHabitWindow.Show();
            }
            else
            {
                Habits._EditHabitWindow.Activate();
            }
            saved = false;
        }

        private void HabitsTable_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while (dep != null && dep is not DataGridRow)
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep is DataGridRow row)
            {
                HabitsTable.SelectedItem = row.Item;
                Point clickPoint = e.GetPosition(this);

                double leftMargin = clickPoint.X;
                if (leftMargin + OptionViewHabitWindow.Width > this.ActualWidth)
                {
                    leftMargin = this.ActualWidth - OptionViewHabitWindow.Width - 25;
                }
                OptionViewHabitWindow.Margin = new Thickness(leftMargin, clickPoint.Y + 15, 0, 0);
                OptionViewHabitWindow.Visibility = Visibility.Visible;

                e.Handled = true;
            }
        }

        private void SaveToJson_Click(object sender, RoutedEventArgs e)
        {
            if (saved == false)
            {
                ConfirmSave save = new ConfirmSave();
                save.Owner = this;

                bool? result = save.ShowDialog();

                if (result == true)
                {
                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string directoryPath = Path.Combine(baseDirectory, "habits");
                    string fullPath = Path.Combine(directoryPath, "habits.json");

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    var habits = HabitsTable.ItemsSource as IEnumerable<Habit>;

                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    };

                    string jsonStr = JsonSerializer.Serialize(habits ?? new List<Habit>(), options);

                    File.WriteAllText(fullPath, jsonStr);
                    saved = true;
                }
            }
        }

        private void DownloadHabits_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Файлы JSON (*.json)|*.json|Все файлы (*.*)|*.*"
            };
            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                if (Path.GetExtension(openFileDialog.FileName).ToLower() != ".json") return;

                string filePath = openFileDialog.FileName;

                var file = new FileInfo(filePath);
                if (file.Length > 0)
                {
                    try
                    {
                        string jsonString = File.ReadAllText(filePath);

                        var loadedList = JsonSerializer.Deserialize<List<Habit>>(jsonString);

                        if (loadedList != null)
                        {
                            foreach (Habit habit in loadedList)
                            {
                                bool isCategoryValid = false;
                                bool isRegularityValid = false;

                                foreach (string category in validCategories)
                                {
                                    if (habit.Category == category)
                                    {
                                        isCategoryValid = true;
                                        break;
                                    }
                                }

                                if (!isCategoryValid)
                                {
                                    throw new Exception($"Некорректная категория '{habit.Category}' в файле: {file.Name}!");
                                }

                                foreach (string regularity in validRegularities)
                                {
                                    if (habit.Regularity == regularity)
                                    {
                                        isRegularityValid = true;
                                        break;
                                    }
                                }

                                if (!isRegularityValid)
                                {
                                    throw new Exception($"Некорректная регулярность '{habit.Regularity}' в файле: {file.Name}!");
                                }
                            }

                            foreach (var habit in loadedList)
                            {
                                _HABITS.Add(habit);
                            }
                        }
                    }
                    catch (JsonException)
                    {
                        return;
                    }
                    catch (IOException)
                    {
                        return;
                    }
                }
            }
            saved = false;
        }

        private void Delete_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (HabitsTable.SelectedItem is not Habit selectedHabit) return;

            _HABITS.Remove(selectedHabit);
            saved = false;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ConfirmClear clear = new ConfirmClear();
            clear.Owner = this;

            bool? result = clear.ShowDialog();

            if (result == true)
            {
                _HABITS.Clear();
                saved = true;
            }

            SaveToJson_Click(null, null);
        }
    }
}
