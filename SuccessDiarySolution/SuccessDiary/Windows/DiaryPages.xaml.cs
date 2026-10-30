using SuccessDiary.Classes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SuccessDiary.Windows
{
    /// <summary>
    /// Логика взаимодействия для DiaryPages.xaml
    /// </summary>
    public partial class DiaryPages : Window
    {
        public DiaryPages(List<DiaryPage> pages)
        {
            InitializeComponent();
        }
    }
}
