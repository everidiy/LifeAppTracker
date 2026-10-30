using SuccessDiary.Classes;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SuccessDiary.Windows
{
    public partial class AddPage : Window
    {

        private readonly Random _rand = new();

        private readonly List<string> _cachedImageUrls = new();
        private List<string> _shuffledImages = new();
        private int _currentImage = 0;

        string imagesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");

        public AddPage()
        {
            InitializeComponent();
        }

        private void ShuffleImages()
        {
            _shuffledImages = _cachedImageUrls;

            for (int i = _shuffledImages.Count - 1; i > 0; i--)
            {
                int j = _rand.Next(i + 1);
                (_shuffledImages[i], _shuffledImages[j]) = (_shuffledImages[j], _shuffledImages[i]);
            }

            _currentImage = 0;
        }

        string rndImg = "";
        private void GetRandomPin_Click(object sender, RoutedEventArgs e)
        {
            _cachedImageUrls.Clear();
            foreach (var image in Directory.GetFiles(imagesFolderPath))
            {
                _cachedImageUrls.Add(image);
            }

            ShuffleImages();

            int attempts = 0;
            bool foundUnique = false;

            while (!foundUnique && attempts < 200)
            {
                rndImg = _shuffledImages[_rand.Next(100)];
                bool isImageUsed = false;

                foreach (var page in Diary.pages)
                {
                    if (page.ImageUrl == rndImg)
                    {
                        isImageUsed = true;
                        break;
                    }
                }

                if (!isImageUsed)
                {
                    foundUnique = true;
                }

                attempts++;
            }

            if (foundUnique)
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(rndImg, UriKind.RelativeOrAbsolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                RandomDiaryImage.Source = bitmap;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TtileInput.Text) ||
            ResultInput.SelectedIndex < 0 ||
            string.IsNullOrEmpty(NoteInput.Text))
            {
                Description.Text = $"Присутствуют пустые строки! \n(Заметка необязательна) ";
                Description.Foreground = Brushes.Red;
                return;
            }

            if (TtileInput.Text.Length > 40)
            {
                Description.Text = $"Слишком большое название! \n(нужно 40 символов) \nУберите {TtileInput.Text.Length - 40} символов!";
                Description.Foreground = Brushes.Red;
                return;
            }

            DiaryPage page = new DiaryPage
            {
                PageId = Diary.pages.Count + 1,
                Title = TtileInput.Text.Trim(),
                Result = ResultInput.Text,
                ImageUrl = rndImg,
                NoteText = NoteInput.Text.Trim()
            };
            Diary.pages.Add(page);

            Diary.diaryPages = new DiaryPages(Diary.pages);
            Diary.diaryPages.Close();

            TtileInput.Text = "";
            ResultInput.SelectedIndex = 0;
            NoteInput.Text = "";
            RandomDiaryImage.Source = null;
        }
    }
}