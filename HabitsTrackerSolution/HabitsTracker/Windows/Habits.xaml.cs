using HabitsTracker.Classes;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace HabitsTracker.Windows
{
    public partial class Habits : Window
    {

        private string[] phrases =
        {
            "Добро пожаловать в чистое начало",
            "Поймайте свой ритм",
            "Вдохните жизнь в свои привычки",
            "Ваш день, ясный как кристалл",
            "Привет! Давайте начнем",
            "Рады видеть вас снова!",
            "Оставайтесь в фокусе, сохраняйте свежесть",
            "Готовы достичь большего сегодня!",
            "Соединяем вас с вашими целями...",
            "Сегодня будущее выглядит светлым!",
            "Давайте построим что-то великое",
            "Ваша персональная экосистема готова"
        };

        private string[] musicHabits =
        {
            "overscorn — keep ahold of my eyes.mp3",
            "Secret Potion.mp3",
            "Takeshi Abo - Lease.mp3",
            "Toyota Corolla.mp3",
            "Tsundere Twintails — Butterflies.mp3",
            "Tsundere Twintails — Cute Circus.mp3",
            "Xploshi — Bliss Boutique.mp3",
            "Xploshi — Demo Booth.mp3"
        };

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

        public ObservableCollection<Habit> MyHabits = new ObservableCollection<Habit>();
        public ObservableCollection<Habit> MyArchivedHabits = new ObservableCollection<Habit>();
        public ObservableCollection<Habit> MyStatsHabits = new ObservableCollection<Habit>();

        private Random random = new Random();
        private DispatcherTimer timer;
        private MediaPlayer player;

        private bool savedMusicSetting = Properties.Settings.Default.isMusicWillBePlaying;
        private bool savedWordsSetting = Properties.Settings.Default.isWordsExist;
        private bool savedRulesSetting = Properties.Settings.Default.isRulesEnable;

        public static AddNewHabitWindow _AddNewHabitWindow;
        public static EditHabitWindow _EditHabitWindow;
        public static ViewHabitsWindow _ViewHabitsWindow;
        public static ShowStatistics _ShowStatistics;

        private bool isPlaying = false;
        private DispatcherTimer fadeTimer;
        private bool isFirstLaunch = true;

        public Habits()
        {
            InitializeComponent();

            CheckFilesBeforeLoad(MyHabits, MyArchivedHabits);

            player = new MediaPlayer();
            player.MediaEnded += (s, e) => PlayMusic();

            CheckBoxisMusicWillBePlaying.IsChecked = savedMusicSetting;
            CheckBoxIsWordsExist.IsChecked = savedWordsSetting;
            CheckBoxIsRulesEnable.IsChecked = savedRulesSetting;

            if (savedMusicSetting)
            {
                SetUpMusic();
            }
            else
            {
                isFirstLaunch = true;
                SongSettingText.Text = "Музыка выключена";
            }

            if (savedWordsSetting)
            {
                TimerForWords();
            }
            else
            {
                WelcomeText.Text = "";
            }

            if (savedRulesSetting)
            {
                CheckRulesInterface();
            }
        }

        private void CheckRulesInterface()
        {
            if (savedRulesSetting)
            {
                SettingsBtn.Content = "ПРОЧИТАТЬ";
                DoubleAnimation blinkAnimation = new DoubleAnimation
                {
                    From = 1.0,                                  
                    To = 0.2,                                        
                    Duration = new Duration(TimeSpan.FromSeconds(1)),
                    AutoReverse = true,                              
                    RepeatBehavior = RepeatBehavior.Forever          
                };

                SettingsBtn.BeginAnimation(Button.OpacityProperty, blinkAnimation);
            }
            else
            {
                SettingsBtn.BeginAnimation(Button.OpacityProperty, null);
                SettingsBtn.Content = "Настройки";
            }
        }

        private void CheckFilesBeforeLoad(
            ObservableCollection<Habit> habits, 
            ObservableCollection<Habit> archiveHabits)
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string directoryPath = Path.Combine(baseDirectory, "habits");
            string fullPath = Path.Combine(directoryPath, "habits.json");

            string directoryPath2 = Path.Combine(baseDirectory, "archive");
            string fullPath2 = Path.Combine(directoryPath2, "archive.json");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!Directory.Exists(directoryPath2))
            {
                Directory.CreateDirectory(directoryPath2);
            }

            if (File.Exists(fullPath))
            {
                var file = new FileInfo(fullPath);
                if (file.Length > 0)
                {
                    try
                    {
                        string jsonString = File.ReadAllText(fullPath);
                        var loadedList = JsonSerializer.Deserialize<List<Habit>>(jsonString);

                        if (loadedList != null)
                        {
                            foreach (Habit habit in loadedList)
                            {
                                bool isCategoryValid = false;
                                bool isRegularityValid = false;

                                foreach(string category in validCategories)
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

                            habits.Clear();
                            foreach (var habit in loadedList)
                            {
                                habits.Add(habit);
                            }
                        }
                    }
                    catch (JsonException) { }
                    catch (IOException) { }
                }
            }

            if (File.Exists(fullPath2))
            {
                var file2 = new FileInfo(fullPath2);
                if (file2.Length > 0)
                {
                    try
                    {
                        string jsonString2 = File.ReadAllText(fullPath2);
                        var loadedArchiveList = JsonSerializer.Deserialize<List<Habit>>(jsonString2);

                        if (loadedArchiveList != null)
                        {
                            archiveHabits.Clear();
                            foreach (var habit in loadedArchiveList)
                            {
                                archiveHabits.Add(habit);
                            }
                        }
                    }
                    catch (JsonException) { return; }
                    catch (IOException) { return; }
                }
            }
        }

        private void PushCheckBoxIsWordsExist_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxIsWordsExist.IsChecked == true)
            {
                Properties.Settings.Default.isWordsExist = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.isWordsExist = false;
                Properties.Settings.Default.Save();
            }
        }

        private void PushCheckBoxIsRulesEnable_Click(object sender, RoutedEventArgs e)
        {
            if (CheckBoxIsRulesEnable.IsChecked == true)
            {
                Properties.Settings.Default.isRulesEnable = true;
                Properties.Settings.Default.Save();

            }
            else
            {
                Properties.Settings.Default.isRulesEnable = false;
                Properties.Settings.Default.Save();

            }
        }

        private void PushCheckBoxisMusicWillBePlaying_Click(object sender, EventArgs e)
        {
            if (CheckBoxisMusicWillBePlaying.IsChecked == true)
            {
                Properties.Settings.Default.isMusicWillBePlaying = true;
                Properties.Settings.Default.Save();

                SongSettingText.Text = "Музыка включена";
            }
            else
            {
                Properties.Settings.Default.isMusicWillBePlaying = false;
                Properties.Settings.Default.Save();

                isPlaying = false;
                isFirstLaunch = true;

                if (fadeTimer != null && fadeTimer.IsEnabled) fadeTimer.Stop();
                player.Stop();

                SongSettingText.Text = "Музыка выключена";
            }
        }

        private void SetUpMusic()
        {
            player.Open(new Uri($"musicHabits/{musicHabits[2]}", UriKind.Relative));
            player.Volume = 1.0;
            player.MediaOpened += (s, e) => player.Play();
            isPlaying = true;

            fadeTimer = new DispatcherTimer();
            fadeTimer.Interval = TimeSpan.FromSeconds(0.1);
            fadeTimer.Tick += (s, e) =>
            {
                if (player.Position >= TimeSpan.FromSeconds(10))
                {
                    if (player.Volume > 0)
                        player.Volume -= 0.033;
                }

                if (player.Position >= TimeSpan.FromSeconds(15))
                {
                    player.Stop();
                    isPlaying = false;
                    fadeTimer.Stop();
                }
            };
            fadeTimer.Start();
        }

        private void MusicButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;

            if (fadeTimer != null && fadeTimer.IsEnabled) fadeTimer.Stop();

            if (!isPlaying)
            {
                if (isFirstLaunch)
                {
                    PlayMusic();
                    isFirstLaunch = false;
                }
                else
                {
                    player.Volume = 1.0;
                    player.Play();
                }
                btn.Content = "■";
                isPlaying = true;
            }
            else
            {
                player.Pause();
                btn.Content = "▶";
                isPlaying = false;
            }
        }

        int currentTrackIndex = 0;
        private void PlayMusic()
        {
            int index = random.Next(musicHabits.Length);
            currentTrackIndex = index;
            string randomMusic = musicHabits[currentTrackIndex];

            player.Open(new Uri($"musicHabits/{randomMusic}", UriKind.Relative));
            player.Volume = 1.0;
            player.MediaOpened += (s, e) => player.Play();

            SongTitleText.Text = "Сейчас играет: " + randomMusic.Replace(".mp3", "");

            isPlaying = true;
        }

        private void PrevMusicButton_Click(object sender, RoutedEventArgs e)
        {
            isFirstLaunch = false;

            Button btn = (Button)sender;

            player.Stop();
            currentTrackIndex--;

            if (currentTrackIndex < 0)
            {
                currentTrackIndex = musicHabits.Length - 1;
            }

            string trackName = musicHabits[currentTrackIndex];

            player.Open(new Uri($"musicHabits/{trackName}", UriKind.Relative));
            player.Volume = 1.0;
            player.MediaOpened += (s, e) => player.Play();

            SongTitleText.Text = "Сейчас играет: " + trackName.Replace(".mp3", "");
            PlayMusicBtn.Content = "■";
            isPlaying = true;
        }

        private void NextMusicButton_Click(object sender, RoutedEventArgs e)
        {
            isFirstLaunch = false;

            Button btn = (Button)sender;

            player.Stop();
            currentTrackIndex++;

            if (currentTrackIndex >= musicHabits.Length)
            {
                currentTrackIndex = 0;
            }

            string trackName = musicHabits[currentTrackIndex];

            player.Open(new Uri($"musicHabits/{trackName}", UriKind.Relative));
            player.Volume = 1.0;
            player.MediaOpened += (s, e) => player.Play();

            SongTitleText.Text = "Сейчас играет: " + trackName.Replace(".mp3", "");
            PlayMusicBtn.Content = "■";
            isPlaying = true;
        }

        private void TimerForWords()
        {
            if (timer != null)
            {
                timer.Stop();
            }

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += Timer_Tick;
            timer.Start();

            WelcomeText.Text = phrases[random.Next(phrases.Length)];
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500));

            fadeOut.Completed += (s, a) =>
            {
                WelcomeText.Text = phrases[random.Next(phrases.Length)];

                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
                WelcomeText.BeginAnimation(TextBlock.OpacityProperty, fadeIn);
            };

            WelcomeText.BeginAnimation(TextBlock.OpacityProperty, fadeOut);
        }



        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (_AddNewHabitWindow == null || !_AddNewHabitWindow.IsLoaded)
            {
                _AddNewHabitWindow = new AddNewHabitWindow(MyHabits, MyArchivedHabits, MyStatsHabits);
                _AddNewHabitWindow.Owner = this;
                _AddNewHabitWindow.Left = this.Left + this.Width + 5;
                _AddNewHabitWindow.Top = this.Top;
                _AddNewHabitWindow.Show();
            }
            else
            {
                _AddNewHabitWindow.Left = this.Left + this.Width + 5;
                _AddNewHabitWindow.Top = this.Top;
                _AddNewHabitWindow.Activate();
            }
        }

        private void View_Click(object sender, RoutedEventArgs e)
        {
            if (_ViewHabitsWindow == null || !_ViewHabitsWindow.IsLoaded)
            {
                _ViewHabitsWindow = new ViewHabitsWindow(MyHabits, MyArchivedHabits, MyStatsHabits);
                _ViewHabitsWindow.Owner = this;
                _ViewHabitsWindow.Top = this.Top + this.Height + 10;
                _ViewHabitsWindow.Left = this.Left;
                _ViewHabitsWindow.Show();
            }
            else
            {
                _ViewHabitsWindow.Top = this.Top + this.Height + 10;
                _ViewHabitsWindow.Left = this.Left;
                _ViewHabitsWindow.Activate();
            }
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            if (_ShowStatistics == null || !_ShowStatistics.IsLoaded)
            {
                _ShowStatistics = new ShowStatistics(MyHabits, MyStatsHabits);
                _ShowStatistics.Owner = this;
                _ShowStatistics.Top = this.Top;
                _ShowStatistics.Left = Application.Current.MainWindow.Width - 325;

                _ShowStatistics.Show();
            }
            else
            {
                _ShowStatistics.Top = this.Top - this.Height + 40;
                _ShowStatistics.Left = this.Left + 75;
                _ShowStatistics.Activate();
            }
        }

        private void ShowSettings_Click(object sender, RoutedEventArgs e)
        {
            WelcomeText.Visibility = Visibility.Collapsed;
            InstructionPanel.Visibility = Visibility.Visible;
        }

        private void HideSettings_Click(object sender, RoutedEventArgs e)
        {
            WelcomeText.Visibility = Visibility.Visible;
            InstructionPanel.Visibility = Visibility.Collapsed;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D1:
                case Key.NumPad1:
                    Add_Click(null, null);
                    break;
                case Key.D2:
                case Key.NumPad2:
                    View_Click(null, null);
                    break;
                case Key.D3:
                case Key.NumPad3:
                    Show_Click(null, null);
                    break;
                case Key.D4:
                case Key.NumPad4:
                    ShowSettings_Click(null, null);
                    break;
                case Key.Escape:
                    Environment.Exit(0);
                    break;
            }
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.Show();
            }

            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}