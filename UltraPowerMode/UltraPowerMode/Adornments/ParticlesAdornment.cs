using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using UltraPowerMode.Enums;
using UltraPowerMode.Utils;
using Color = System.Drawing.Color;
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;



namespace UltraPowerMode.Adornments
{
    //TODO//
    // need to get color from settings
    // need to get size from settings

    //ideas
    // while hold ctrl create red and blue particls spewing out the top and bottom of the highlight 
    // and when a large deletion or adition the particles spew out of the correct color over the area which was affected
    // the particles originate from where the caret originaly was
    internal class ParticlesAdornment : IAdornment
    {
        private readonly List<Image> particlesList;
        private EditTag _editTag;
        private Color _color;
        private float _particleSize;

        public ParticlesAdornment()
        {
            _color = Color.FromArgb(155, 56, 252, 253);
            _particleSize = 100;
            particlesList = new List<Image>();
        }

        public void CaretPositionChanged(IAdornmentLayer layer, IWpfTextView view, CaretPositionChangedEventArgs e)
        {

        }

        public void Cleanup(IAdornmentLayer layer, IWpfTextView view)
        {
            layer.RemoveAllAdornments();
        }

        public void CreateVisuals(IAdornmentLayer layer, IWpfTextView view, Point target, Point spawnLocation)
        {
            var particles = new Image();

            particles.UpdateSource(GetParticleBitmap(_color, _particleSize));
            particles.Opacity = 1.0;

            layer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, particles, null);

            double startAlpha = 1.0;
            double alphaDecrements = 0.025;
            double frameInterval = 17.0;

            EasingFunctionBase easingFunction = new ExponentialEase()
            {
                EasingMode = EasingMode.EaseOut,
                Exponent = 4
            };

            Duration duration = CalculateDuration(startAlpha, alphaDecrements, frameInterval);
            particles.BeginAnimation(Canvas.LeftProperty, GetAnimation(spawnLocation.X, target.X, duration, easingFunction));
            particles.BeginAnimation(Canvas.TopProperty, GetAnimation(spawnLocation.Y, target.Y, duration, easingFunction));

            double initialSize = 3;
            double finalSize = 0;

            var sizeAnimation = GetAnimation(initialSize, finalSize, duration);
            particles.BeginAnimation(Image.WidthProperty, sizeAnimation);
            particles.BeginAnimation(Image.HeightProperty, sizeAnimation);

            sizeAnimation.Completed += (sender, args) =>
            {
                particles.Visibility = Visibility.Hidden;
                layer.RemoveAdornment(particles);
                particlesList.Remove(particles);
            };
        }

        private Duration CalculateDuration(double initialValue, double decrementPerInterval, double intervalMilliseconds = 17)
        {
            return TimeSpan.FromMilliseconds(intervalMilliseconds * (initialValue / decrementPerInterval));
        }

        private DoubleAnimation GetAnimation(double start, double end, Duration duration)
        {
            return new DoubleAnimation()
            {
                From = start,
                To = end,
                Duration = duration
            };
        }

        private DoubleAnimation GetAnimation(double start, double end, Duration duration, EasingFunctionBase easingFunction)
        {
            return new DoubleAnimation()
            {
                From = start,
                To = end,
                Duration = duration,
                EasingFunction = easingFunction

            };
        }


        private Bitmap GetParticleBitmap(Color color, float size)
        {
            Color particleColor = color;
            float particleSize = size;
            Bitmap bitmap = new Bitmap(100, 100);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(
                    new SolidBrush(Color.FromArgb(
                        particleColor.A, particleColor.R, particleColor.G, particleColor.B)),
                        0, 0,
                        particleSize, particleSize);
            }

            return bitmap;
        }


        public void OnSizeChanged(IAdornmentLayer layer, IWpfTextView view, int streakCount, bool backgroundColorChanged = false)
        {
            throw new NotImplementedException();
        }

        public void OnTextBufferChanged(IAdornmentLayer layer, IWpfTextView view, TextContentChangedEventArgs e)
        {
            _particleSize = 100;

            if (e.EditTag == null)
            {
                _editTag = EditTag.Delete;
                _color = Color.FromArgb(255, 255, 252, 82);
            }
            else
            {
                _editTag = EditTag.Insert;
                _color = Color.FromArgb(255, 56, 252, 253);
            }
        }

        public void TextBufferPostChanged(IAdornmentLayer layer, IWpfTextView view, EventArgs e)
        {
            var spawnedAmount = 10;

            Random random = new Random();
            Point target = new Point();
            Point spawnLocation = new Point();

            var offsetX = (8 / 2) - (3 / 2);
            var offsetY = (view.Caret.Height / 2) - (3 / 2);


            for (int i = 0; i < spawnedAmount; i++)
            {
                // (Visuals) need a more robust method of picking s random spawn and target
                // (Performance) need to add in storyboards so that these get done togerther and not one at a time


                if (_editTag == EditTag.Insert)
                {
                    target = new Point
                    (
                        view.Caret.Left + offsetX + random.Next(-8, 10) ,
                        view.Caret.Top  + offsetY + random.Next(-6, 6)
                    );

                    spawnLocation = new Point
                    (
                        view.Caret.Left + offsetX + random.Next(-6, 6),
                        view.Caret.Top  + offsetY + random.Next(-6, 6)
                    );

                    CreateVisuals(layer, view, target, spawnLocation);
                }
                else if (_editTag == EditTag.Delete)
                {
                    target = new Point
                    (
                        view.Caret.Left + offsetX + random.Next(0, 16), // (WIP) needs to not move in the direction the caret is moving
                        view.Caret.Top + offsetY + random.Next(-6, 6)
                    );

                    spawnLocation = new Point
                    (
                        view.Caret.Left + offsetX,
                        view.Caret.Top + offsetY
                    );

                    CreateVisuals(layer, view, target, spawnLocation);
                }



            }
        }

        public void UpdateVisuals(IAdornmentLayer layer, IWpfTextView view)
        {
            throw new NotImplementedException();
        }
    }
}
