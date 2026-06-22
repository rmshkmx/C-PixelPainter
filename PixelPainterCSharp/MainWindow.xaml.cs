using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using PixelPainterCSharp.Core;

namespace PixelPainterCSharp
{
    public partial class MainWindow : Window
    {
        private string _currentImagePath = string.Empty;
        private MouseController.POINT _topLeft;
        private MouseController.POINT _bottomRight;
        private ProcessedImageInfo? _lastProcessedInfo;
        
        public ObservableCollection<PaletteItem> PaletteItems { get; set; } = new();

        public MainWindow()
        {
            InitializeComponent();
            IcPalette.ItemsSource = PaletteItems;
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog { Filter = "Images|*.jpg;*.jpeg;*.png" };
            if (ofd.ShowDialog() == true)
            {
                _currentImagePath = ofd.FileName;
                TxtFilePath.Text = System.IO.Path.GetFileName(_currentImagePath);
                UpdatePreview();
            }
        }

        private void Settings_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (TxtColorsCount != null)
            {
                TxtColorsCount.Text = SldColors.Value.ToString("0");
                UpdatePreview();
            }
        }

        private void UpdatePreview()
        {
            if (string.IsNullOrEmpty(_currentImagePath)) return;

            int width = _bottomRight.X - _topLeft.X;
            int height = _bottomRight.Y - _topLeft.Y;

            if (width <= 0 || height <= 0)
            {
                // Default fallback if area is not selected
                width = 100;
                height = 100;
            }

            try
            {
                _lastProcessedInfo = ImageProcessor.ProcessImage(_currentImagePath, width, height, (int)SldColors.Value);
                ImgPreview.Source = _lastProcessedInfo.PreviewImage;
                
                UpdatePaletteList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing image: " + ex.Message);
            }
        }

        private void UpdatePaletteList()
        {
            PaletteItems.Clear();
            if (_lastProcessedInfo == null) return;

            for (int i = 0; i < _lastProcessedInfo.Palette.Count; i++)
            {
                var color = _lastProcessedInfo.Palette[i];
                PaletteItems.Add(new PaletteItem
                {
                    Index = i,
                    ColorBrush = new SolidColorBrush(color),
                    PosText = "Not Mapped",
                    IsMapped = false
                });
            }
        }

        private async void SetTopLeft_Click(object sender, RoutedEventArgs e)
        {
            await WaitForClickAndSet(pos =>
            {
                _topLeft = pos;
                TxtTopLeft.Text = $"X: {pos.X}, Y: {pos.Y}";
                UpdatePreview();
            });
        }

        private async void SetBottomRight_Click(object sender, RoutedEventArgs e)
        {
            await WaitForClickAndSet(pos =>
            {
                _bottomRight = pos;
                TxtBottomRight.Text = $"X: {pos.X}, Y: {pos.Y}";
                UpdatePreview();
            });
        }

        private async void MapColor_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button btn && btn.CommandParameter is PaletteItem item)
            {
                await WaitForClickAndSet(pos =>
                {
                    item.MappedPosition = pos;
                    item.IsMapped = true;
                    item.PosText = $"X: {pos.X}, Y: {pos.Y}";
                });
            }
        }

        private void PaletteColor_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.Tag is PaletteItem item && item.ColorBrush is SolidColorBrush brush)
            {
                var color = brush.Color;
                string hex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
                Clipboard.SetText(hex);
                System.Media.SystemSounds.Beep.Play();
            }
        }

        private async Task WaitForClickAndSet(Action<MouseController.POINT> onPositionSelected)
        {
            MessageBox.Show("Hover your mouse where you want to set the coordinate and WAIT 3 SECONDS.\nDo not click, just hold the mouse there.", "Map Coordinate", MessageBoxButton.OK, MessageBoxImage.Information);
            await Task.Delay(3000);
            onPositionSelected(MouseController.GetPosition());
            System.Media.SystemSounds.Beep.Play(); // Notify user that coordinate was captured
        }

        private async void StartPainting_Click(object sender, RoutedEventArgs e)
        {
            if (_lastProcessedInfo == null)
            {
                MessageBox.Show("Load an image first.");
                return;
            }

            int width = _bottomRight.X - _topLeft.X;
            int height = _bottomRight.Y - _topLeft.Y;

            if (width <= 0 || height <= 0)
            {
                MessageBox.Show("Please select a valid drawing area first.");
                return;
            }

            var mappings = new System.Collections.Generic.Dictionary<int, MouseController.POINT>();
            foreach (var item in PaletteItems)
            {
                if (!item.IsMapped)
                {
                    MessageBox.Show("Please map all colors to palette coordinates before starting.");
                    return;
                }
                mappings[item.Index] = item.MappedPosition;
            }

            if (!int.TryParse(TxtDelay.Text, out int delay))
            {
                delay = 3;
            }

            var task = new PainterTask(_lastProcessedInfo, mappings, _topLeft, delay);
            MessageBox.Show("Painting will start in 2 seconds. Press ESC to stop at any time.", "Starting", MessageBoxButton.OK, MessageBoxImage.Information);
            await Task.Delay(2000);
            
            await task.StartPaintingAsync();
            MessageBox.Show("Painting finished or stopped.", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class PaletteItem : INotifyPropertyChanged
    {
        public int Index { get; set; }
        public Brush ColorBrush { get; set; } = Brushes.Transparent;
        
        private string _posText = "";
        public string PosText
        {
            get => _posText;
            set { _posText = value; OnPropertyChanged(); }
        }

        public bool IsMapped { get; set; }
        public MouseController.POINT MappedPosition { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
