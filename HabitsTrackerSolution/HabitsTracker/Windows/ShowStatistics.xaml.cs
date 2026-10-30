using HabitsTracker.Classes;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HabitsTracker.Windows
{
    public partial class ShowStatistics : Window
    {
        public ObservableCollection<HabitForStats> HabitsWithStats { get; set; }
        public ObservableCollection<Habit> Stats { get; set; }

        bool saved = false;

        public ShowStatistics(ObservableCollection<Habit> habits,
            ObservableCollection<Habit> stats,
            bool saves = true)
        {
            saved = saves;

            Stats = habits ?? throw new Exception(nameof(habits));

            InitializeComponent();

            HabitsWithStats = new ObservableCollection<HabitForStats>();

            CheckFiles();
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
                    Stats.Clear();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void CheckFiles()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string directoryPath = Path.Combine(baseDirectory, "stats");
            string fullPath = Path.Combine(directoryPath, "stats.json");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            Dictionary<string, Dictionary<string, bool>> loadedStats =
                new Dictionary<string, Dictionary<string, bool>>();

            if (File.Exists(fullPath))
            {
                try
                {
                    string jsonStr = File.ReadAllText(fullPath);
                    var deserializedItems = JsonSerializer.Deserialize<List<HabitForStats>>(jsonStr);

                    if (deserializedItems != null)
                    {
                        foreach (var item in deserializedItems)
                        {
                            if (!string.IsNullOrEmpty(item.Name) && item.Stats != null)
                            {
                                loadedStats[item.Name] = item.Stats;
                            }
                        }
                    }
                }
                catch (JsonException ex)
                {
                    throw new Exception("Ошибка чтения файла статистики. " +
                        "Данные могут быть повреждены.");
                }
                catch (IOException)
                {
                    throw new Exception("Не удалось получить доступ к файлу статистики.");
                }
            }

            HabitsWithStats.Clear();
            foreach (Habit forStats in Stats)
            {
                if (forStats == null || string.IsNullOrEmpty(forStats.Name))
                {
                    continue;
                }

                HabitForStats newHabitWithStats = new HabitForStats
                {
                    Name = forStats.Name,
                    Regularity = forStats.Regularity,
                    Category = forStats.Category,
                    Note = forStats.Note
                };

                if (loadedStats.ContainsKey(forStats.Name))
                {
                    newHabitWithStats.Stats = loadedStats[forStats.Name];
                }
                else
                {
                    newHabitWithStats.Stats = new Dictionary<string, bool>();
                }

                HabitsWithStats.Add(newHabitWithStats);
            }

            HabitsGrid.ItemsSource = HabitsWithStats;

            StartDatePicker.SelectedDate = DateTime.Today;
        }

        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StartDatePicker.SelectedDate == null) return;
            DateTime startDate = StartDatePicker.SelectedDate.Value;

            while (HabitsGrid.Columns.Count > 1)
            {
                HabitsGrid.Columns.RemoveAt(1);
            }

            Style headerStyle = HabitsGrid.FindResource("HeaderStyle") as Style;

            int daysCount = 7;
            for (int i = 0; i < daysCount; i++)
            {
                DateTime current = startDate.AddDays(i);
                int dayNum = current.Day;

                DataGridTemplateColumn checkboxColumn = new DataGridTemplateColumn
                {
                    Header = dayNum.ToString(),
                    Width = new DataGridLength(45),
                    HeaderStyle = headerStyle
                };

                FrameworkElementFactory checkboxFactory = new FrameworkElementFactory(typeof(CheckBox));

                string dateKey = current.ToString("yyyy-MM-dd");

                Binding binding = new Binding($"Stats[{dateKey}]")
                {
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                checkboxFactory.SetBinding(CheckBox.IsCheckedProperty, binding);

                checkboxFactory.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                checkboxFactory.SetValue(CheckBox.VerticalAlignmentProperty, VerticalAlignment.Center);

                checkboxColumn.CellTemplate = new DataTemplate { VisualTree = checkboxFactory };

                HabitsGrid.Columns.Add(checkboxColumn);
            }
        }

        private void Today_Click(object sender, RoutedEventArgs e)
        {
            StartDatePicker.SelectedDate = DateTime.Today;
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
                    string directoryPath = Path.Combine(baseDirectory, "stats");
                    string fullPath = Path.Combine(directoryPath, "stats.json");

                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    };

                    string jsonStr = JsonSerializer.Serialize(HabitsWithStats, options);

                    File.WriteAllText(fullPath, jsonStr);
                    saved = true;
                }
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (Stats == null) return;

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string fullPath = Path.Combine(baseDirectory, "stats", "stats.json");

            Dictionary<string, Dictionary<string, bool>> loadedStats = new Dictionary<string, Dictionary<string, bool>>();
            if (File.Exists(fullPath))
            {
                try
                {
                    string jsonStr = File.ReadAllText(fullPath);
                    var deserialeziedItems = JsonSerializer.Deserialize<List<HabitForStats>>(jsonStr);
                    if (deserialeziedItems != null)
                    {
                        foreach (var item in deserialeziedItems)
                        {
                            if (!string.IsNullOrEmpty(item.Name) && item.Stats != null)
                            {
                                loadedStats[item.Name] = item.Stats;
                            }
                        }
                    }
                }
                catch { }
            }

            HabitsWithStats.Clear();
            foreach (Habit forStats in Stats)
            {
                if (forStats == null || string.IsNullOrEmpty(forStats.Name)) continue;

                HabitForStats newHabitWithStats = new HabitForStats
                {
                    Name = forStats.Name,
                    Regularity = forStats.Regularity,
                    Category = forStats.Category,
                    Note = forStats.Note
                };

                if (loadedStats.ContainsKey(forStats.Name))
                {
                    newHabitWithStats.Stats = loadedStats[forStats.Name];
                }
                else
                {
                    newHabitWithStats.Stats = new Dictionary<string, bool>();
                }

                HabitsWithStats.Add(newHabitWithStats);
            }

            StartDatePicker_SelectedDateChanged(null, null);

            if (Stats.Count == 0)
            {
                saved = true;
            }
        }

        private void SeeInfo_Click(object sender, RoutedEventArgs e)
        {
            if (HabitsGrid.SelectedItem is not HabitForStats habitForStats) return;

            Habit checkinHabit = new Habit 
            {
                Name = habitForStats.Name,
                Regularity = habitForStats.Regularity,
                Category = habitForStats.Category,
                Note = habitForStats.Note
            };

            if (Habits._EditHabitWindow == null || !Habits._EditHabitWindow.IsLoaded)
            {
                Habits._EditHabitWindow = new EditHabitWindow(checkinHabit);
                Habits._EditHabitWindow.Owner = Application.Current.MainWindow;

                UpdateEditWindowPosition();

                Habits._EditHabitWindow.Show();
            }
            else
            {
                Habits._EditHabitWindow.UpdateData(checkinHabit);

                UpdateEditWindowPosition();

                Habits._EditHabitWindow.Activate();
            }
        }

        private void UpdateEditWindowPosition()
        {
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
        }
    }
}
