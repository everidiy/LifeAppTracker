using SuccessDiary.Classes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace SuccessDiary.Windows
{
    public partial class Diary : Window
    {

        public static DiaryPages diaryPages;
        public AddPage addPage;

        public static List<DiaryPage> pages = new();
        private Random _rnd = new();
        private DispatcherTimer timer;

        public Diary()
        {
            InitializeComponent();

            GetRandomCard();
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.Show();
            }

            this.Close();
        }

        private void GetRandomCard()
        {
            if (pages.Count != 0)
            {
                RandomCard.Visibility = Visibility.Visible;

                if (timer != null)
                {
                    timer.Stop();
                }

                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(3);
                timer.Tick += Timer_Tick;
                timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            int rndCard = _rnd.Next(pages.Count);

            DoubleAnimation fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500));

            fadeOut.Completed += (s, a) =>
            {
                CardTitle.Text = pages[rndCard].Title;
                CardResult.Text = pages[rndCard].Result;
                CardNote.Text = pages[rndCard].NoteText;

                DoubleAnimation fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(500));
                RandomCard.BeginAnimation(TextBlock.OpacityProperty, fadeIn);
            };

            RandomCard.BeginAnimation(TextBlock.OpacityProperty, fadeOut);
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (addPage == null || !addPage.IsLoaded)
            {
                addPage = new AddPage();
                addPage.Owner = this;
                addPage.Left = this.Left + this.Width;
                addPage.Top = this.Top;
                addPage.Show();
            }
            else
            {
                addPage.Activate();
                addPage.Left = this.Left - 800;
                addPage.Top = this.Top;
            }
        }

        private void Read_Click(object sender, RoutedEventArgs e)
        {
            if (diaryPages == null || !diaryPages.IsLoaded)
            {
                diaryPages = new DiaryPages(pages);
                diaryPages.Owner = this;
                diaryPages.Left = this.Left - 800;
                diaryPages.Top = this.Top;
                diaryPages.Show();
            }
            else
            {
                diaryPages.Activate();
                diaryPages.Left = this.Left - 800;
                diaryPages.Top = this.Top;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}