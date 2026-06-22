using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace PixelPainterCSharp.Core
{
    public class ProcessedImageInfo
    {
        public BitmapSource PreviewImage { get; set; } = null!;
        public List<System.Windows.Media.Color> Palette { get; set; } = new();
        public int[,] PixelColorIndices { get; set; } = new int[0,0];
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public static class ImageProcessor
    {
        public static ProcessedImageInfo ProcessImage(string path, int targetWidth, int targetHeight, int maxColors)
        {
            using var image = Image.Load<Rgba32>(path);
            
            if (targetWidth > 0 && targetHeight > 0)
            {
                image.Mutate(x => x.Resize(targetWidth, targetHeight, KnownResamplers.NearestNeighbor));
            }

            var quantizer = new WuQuantizer(new QuantizerOptions { MaxColors = maxColors });
            image.Mutate(x => x.Quantize(quantizer));

            var palette = new List<System.Windows.Media.Color>();
            var colorToIndex = new Dictionary<Rgba32, int>();
            
            int width = image.Width;
            int height = image.Height;
            var indices = new int[width, height];

            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<Rgba32> pixelRow = accessor.GetRowSpan(y);
                    for (int x = 0; x < pixelRow.Length; x++)
                    {
                        var pixel = pixelRow[x];
                        if (!colorToIndex.TryGetValue(pixel, out int index))
                        {
                            index = palette.Count;
                            palette.Add(System.Windows.Media.Color.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B));
                            colorToIndex[pixel] = index;
                        }
                        indices[x, y] = index;
                    }
                }
            });

            using var ms = new MemoryStream();
            image.SaveAsBmp(ms);
            ms.Seek(0, SeekOrigin.Begin);
            
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = ms;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return new ProcessedImageInfo
            {
                PreviewImage = bitmapImage,
                Palette = palette,
                PixelColorIndices = indices,
                Width = width,
                Height = height
            };
        }
    }
}
