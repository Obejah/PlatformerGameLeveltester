using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PlatformerGame.Core
{
    public class SpriteAnimation
    {
        private List<BitmapSource> _frames = new List<BitmapSource>();
        private int _currentFrame = 0;
        private double _timePerFrame;
        private double _timer;

        public bool Loop { get; set; } = true;
        public BitmapSource CurrentFrame => _frames[_currentFrame];

        public SpriteAnimation(string imagePath, int frameWidth, int frameHeight, int frameCount, int startX, int startY, double fps = 10, bool loop = true)
        {
            BitmapImage spriteSheet = new BitmapImage(new Uri($"pack://application:,,,/{imagePath}", UriKind.Absolute));
            _timePerFrame = 1.0 / fps;
            Loop = loop;

            for (int i = 0; i < frameCount; i++)
            {
                int x = startX + (i * frameWidth);

                if (x + frameWidth > spriteSheet.PixelWidth || startY + frameHeight > spriteSheet.PixelHeight)
                {
                    Console.WriteLine($"[Animation Warning] Tried to crop outside image bounds at X:{x}, Y:{startY}");
                    break;
                }

                Int32Rect rect = new Int32Rect(x, startY, frameWidth, frameHeight);
                CroppedBitmap frame = new CroppedBitmap(spriteSheet, rect);
                _frames.Add(frame);
            }

            // Failsafe: If no frames were loaded, just load the whole image
            if (_frames.Count == 0)
            {
                _frames.Add(spriteSheet);
            }
        }
        public void Update(double dt)
        {
            _timer += dt;
            if (_timer >= _timePerFrame)
            {
                _timer = 0;
                _currentFrame++;
                if (_currentFrame >= _frames.Count)
                {
                    if (Loop) _currentFrame = 0;
                    else _currentFrame = _frames.Count - 1;
                }
            }
        }
    }
}