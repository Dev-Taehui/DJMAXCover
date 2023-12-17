using DJMAXCover.Properties;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DJMAXCover
{
    public partial class MainWindow : Window
    {
        DispatcherTimer dispatcherTimer;

        public MainWindow()
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(Settings.Default.CoverImage))
            {
                SaveCoverImage(Settings.Default.CoverImage);
            }
            CoverImage.Stretch = (Stretch)Settings.Default.Stretch;
            CoverImage.VerticalAlignment = (VerticalAlignment)Settings.Default.VerticalAlignment;
            CoverImage.HorizontalAlignment = (HorizontalAlignment)Settings.Default.HorizontalAlignment;
            Opacity = Settings.Default.Opacity;
            Left = Settings.Default.WindowX;
            Top = Settings.Default.WindowY;
            if (Settings.Default.WindowWidth > 0.0)
            {
                Width = Settings.Default.WindowWidth;
            }
            if (Settings.Default.WindowHeight > 0.0)
            {
                Height = Settings.Default.WindowHeight;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (CoverImage.Source != null)
            {
                Settings.Default.CoverImage = string.Empty;
                CoverImage.Source = null;
            }
            else
            {
                var ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == true)
                {
                    var coverImage = ofd.FileName;
                    Settings.Default.CoverImage = coverImage;
                    SaveCoverImage(coverImage);
                }
            }
        }

        void SaveCoverImage(string coverImage)
        {
            try
            {
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.UriSource = new Uri(coverImage);
                bi.EndInit();
                bi.Freeze();
                CoverImage.Source = bi;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"이미지 파일을 불러올 수 없습니다. ({ex.Message})", "DJMAXCover", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F2:
                    CoverImage.Stretch = (Stretch)(((int)CoverImage.Stretch + 1) % 4);
                    Settings.Default.Stretch = (int)CoverImage.Stretch;
                    SetStatusText($"Stretch: {CoverImage.Stretch}");
                    break;
                case Key.F3:
                    CoverImage.VerticalAlignment = (VerticalAlignment)(((int)CoverImage.VerticalAlignment + 1) % 4);
                    Settings.Default.VerticalAlignment = (int)CoverImage.VerticalAlignment;
                    SetStatusText($"VerticalAlignment: {CoverImage.VerticalAlignment}");
                    break;
                case Key.F4:
                    CoverImage.HorizontalAlignment = (HorizontalAlignment)(((int)CoverImage.HorizontalAlignment + 1) % 4);
                    Settings.Default.HorizontalAlignment = (int)CoverImage.HorizontalAlignment;
                    SetStatusText($"HorizontalAlignment: {CoverImage.HorizontalAlignment}");
                    break;
                case Key.F5:
                    var opacity = Opacity;
                    opacity += 0.1;
                    if (opacity > 1.0)
                    {
                        opacity = 0.1;
                    }
                    opacity = Math.Round(opacity, 1);
                    Opacity = opacity;
                    Settings.Default.Opacity = Opacity;
                    SetStatusText($"Opacity: {100 * Opacity}%");
                    break;
                case Key.Escape:
                    Close();
                    break;
            }
        }

        void SetStatusText(string statusText)
        {
            StatusTextBlock.Text = statusText;
            dispatcherTimer?.Stop();
            dispatcherTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.Background, (sender, e) =>
            {
                (sender as DispatcherTimer)?.Stop();
                StatusTextBlock.Text = string.Empty;
            }, Dispatcher);
        }

        void OnClosed(object sender, EventArgs e)
        {
            Settings.Default.WindowX = Left;
            Settings.Default.WindowY = Top;
            Settings.Default.WindowWidth = Width;
            Settings.Default.WindowHeight = Height;
            Settings.Default.Save();
        }

        void OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
        }
    }
}