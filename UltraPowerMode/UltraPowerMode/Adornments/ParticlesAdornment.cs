using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
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
        private readonly List<Image> _particlesList;

        private EditTag _editTag;
        private Color _color;
        private float _particleSize;
        private int _spawnedAmount;

        public ParticlesAdornment()
        {
            _color = Color.FromArgb(155, 56, 252, 253);
            _particleSize = 3;
            _particlesList = new List<Image>();
        }

        public void AddAnimationToStoryboard()
        {

        }

        public void CaretPositionChanged(IAdornmentLayer layer, IWpfTextView view, CaretPositionChangedEventArgs e)
        {

        }

        public void Cleanup(IAdornmentLayer layer, IWpfTextView view)
        {
            layer.RemoveAllAdornments();
        }

        public void CreateVisuals(IAdornmentLayer layer, IWpfTextView view, Point target, Point spawnLocation, Storyboard storyboard)
        {
            var particle = new Image();
            particle.UpdateSource(GetParticleBitmap(_color, _particleSize));
            particle.Opacity = 1.0;

            // Apply a glow effect to the particle (consider using a ShaderEffect or batch rendering)
            var glowEffect = new DropShadowEffect
            {
                Color = System.Windows.Media.Color.FromArgb(255, _color.R, _color.G, _color.B),
                BlurRadius = 10,
                ShadowDepth = 0,
                Opacity = 0.8
            };
            particle.Effect = glowEffect;

            layer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, particle, null);

            double startAlpha = 1.0;
            double alphaDecrements = 0.025;
            double frameInterval = 17.0;

            EasingFunctionBase easingFunction = new ExponentialEase()
            {
                EasingMode = EasingMode.EaseOut,
                Exponent = 4
            };
            Duration duration = CalculateDuration(startAlpha, alphaDecrements, frameInterval);

            DoubleAnimation moveXAnimation = GetAnimation(spawnLocation.X, target.X, duration, easingFunction);
            DoubleAnimation moveYAnimation = GetAnimation(spawnLocation.Y, target.Y, duration, easingFunction);

            double initialSize = 3;
            double finalSize = 0;

            var sizeAnimation = GetAnimation(initialSize, finalSize, duration);

            Storyboard.SetTarget(moveXAnimation, particle);
            Storyboard.SetTargetProperty(moveXAnimation, new PropertyPath(Canvas.LeftProperty));

            Storyboard.SetTarget(moveYAnimation, particle);
            Storyboard.SetTargetProperty(moveYAnimation, new PropertyPath(Canvas.TopProperty));

            storyboard.Children.Add(moveXAnimation);
            storyboard.Children.Add(moveYAnimation);

            Storyboard.SetTarget(sizeAnimation, particle);
            Storyboard.SetTargetProperty(sizeAnimation, new PropertyPath(Image.WidthProperty));
            storyboard.Children.Add(sizeAnimation);

            Storyboard.SetTarget(sizeAnimation, particle);
            Storyboard.SetTargetProperty(sizeAnimation, new PropertyPath(Image.HeightProperty));
            storyboard.Children.Add(sizeAnimation);

            sizeAnimation.Completed += (sender, args) =>
            {
                particle.Visibility = Visibility.Hidden;
                layer.RemoveAdornment(particle);
                _particlesList.Remove(particle);
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
            var particleColor = _color;
            var particleSize = _particleSize;
            Bitmap bitmap = new Bitmap(100, 100);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

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
            int spawnedAmount =  _spawnedAmount = 10;

            Random random = new Random();
            Point target = new Point();
            Point spawnLocation = new Point();

            var offsetX = (8 / 2) - (3 / 2);
            var offsetY = (view.Caret.Height / 2) - (3 / 2);


            for (int i = 0; i < spawnedAmount; i++)
            {
                // (Visuals) need a more robust method of picking s random spawn and target

                Storyboard storyboard = new Storyboard();

                if (_editTag == EditTag.Insert)
                {
                    target = new Point
                    (
                        view.Caret.Left + offsetX + random.Next(-8, 10),
                        view.Caret.Top + offsetY + random.Next(-6, 6)
                    );

                    spawnLocation = new Point
                    (
                        view.Caret.Left + offsetX + random.Next(-6, 6),
                        view.Caret.Top  + offsetY + random.Next(-6, 6)
                    );

                    CreateVisuals(layer, view, target, spawnLocation, storyboard);
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

                    CreateVisuals(layer, view, target, spawnLocation, storyboard);
                }

                storyboard.Begin();
            }
        }

        public void UpdateVisuals(IAdornmentLayer layer, IWpfTextView view)
        {
            throw new NotImplementedException();
        }
    }
}
