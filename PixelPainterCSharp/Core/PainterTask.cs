using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PixelPainterCSharp.Core
{
    public class PainterTask
    {
        private readonly ProcessedImageInfo _imageInfo;
        private readonly Dictionary<int, MouseController.POINT> _colorMappings;
        private readonly MouseController.POINT _topLeft;
        private readonly int _delayMs;
        private volatile bool _stopRequested = false;

        public PainterTask(
            ProcessedImageInfo imageInfo, 
            Dictionary<int, MouseController.POINT> colorMappings, 
            MouseController.POINT topLeft, 
            int delayMs)
        {
            _imageInfo = imageInfo;
            _colorMappings = colorMappings;
            _topLeft = topLeft;
            _delayMs = delayMs;
            
            MouseController.OnEscPressed += Stop;
        }

        public void Stop()
        {
            _stopRequested = true;
        }

        public async Task StartPaintingAsync()
        {
            _stopRequested = false;
            MouseController.StartKeyboardHook();

            try
            {
                await Task.Run(() =>
                {
                    int width = _imageInfo.Width;
                    int height = _imageInfo.Height;

                    // Iterate over each color that has been mapped
                    foreach (var kvp in _colorMappings)
                    {
                        if (_stopRequested) break;

                        int colorIndex = kvp.Key;
                        MouseController.POINT palettePos = kvp.Value;

                        // Click the palette to select the color
                        MouseController.Click(palettePos.X, palettePos.Y);
                        PreciseDelay(100); // Wait 100ms for drawing app to register color change

                        // Draw all pixels of this color
                        for (int x = 0; x < width; x++)
                        {
                            if (_stopRequested) break;
                            for (int y = 0; y < height; y++)
                            {
                                if (_stopRequested) break;

                                if (_imageInfo.PixelColorIndices[x, y] == colorIndex)
                                {
                                    MouseController.Click(_topLeft.X + x, _topLeft.Y + y);
                                    if (_delayMs > 0)
                                    {
                                        PreciseDelay(_delayMs);
                                    }
                                }
                            }
                        }
                    }
                });
            }
            finally
            {
                MouseController.StopKeyboardHook();
                MouseController.OnEscPressed -= Stop;
            }
        }

        private static void PreciseDelay(double milliseconds)
        {
            if (milliseconds <= 0) return;
            var sw = System.Diagnostics.Stopwatch.StartNew();
            long targetTicks = (long)(milliseconds * System.Diagnostics.Stopwatch.Frequency / 1000.0);
            while (sw.ElapsedTicks < targetTicks)
            {
                // If we have a lot of time left, yield or sleep to save CPU
                long remainingTicks = targetTicks - sw.ElapsedTicks;
                if (remainingTicks > System.Diagnostics.Stopwatch.Frequency / 1000) // > 1ms
                {
                    System.Threading.Thread.Sleep(0);
                }
            }
        }
    }
}
