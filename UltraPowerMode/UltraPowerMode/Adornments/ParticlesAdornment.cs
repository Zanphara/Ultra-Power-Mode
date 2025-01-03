using Microsoft.VisualStudio.GraphModel.CodeSchema;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
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

        public ParticlesAdornment()
        {
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
            particles.UpdateSource(GetParticleBitmap());
            particles.Opacity = 1.0;

            layer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, particles, null);

            var left = target.X;
            var top = target.Y;

            double startAlpha = 1.0;
            //double endAlpha = 0.0;
            double alphaDecrements = 0.025;
            double frameInterval = 17.0;

            EasingFunctionBase easingFunction = new ExponentialEase()
            {
                EasingMode = EasingMode.EaseOut,
                Exponent = 2
            };

            Duration duration = CalculateDuration(startAlpha, alphaDecrements, frameInterval);
            particles.BeginAnimation(Canvas.LeftProperty, GetAnimation(spawnLocation.X, left, duration, easingFunction));
            particles.BeginAnimation(Canvas.TopProperty, GetAnimation(spawnLocation.Y, top, duration, easingFunction));

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


            //var opacityAnimation = GetAnimation(startAlpha, endAlpha, duration);
            //opacityAnimation.Completed += (sender, args) =>
            //{
            //    particles.Visibility = Visibility.Hidden;
            //    layer.RemoveAdornment(particles);
            //    particlesList.Remove(particles);
            //};
            //particles.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
        }

        private Duration CalculateDuration(double initialValue, double decrementPerInterval, double intervalMilliseconds = 17)
        {
            return  TimeSpan.FromMilliseconds(intervalMilliseconds * (initialValue / decrementPerInterval));
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


        private Bitmap GetParticleBitmap()
        {
            var color = Color.FromArgb(255, 56, 252, 253);
            var size = 3;                       
            var bitmap = new Bitmap(size, size);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(
                    new SolidBrush(Color.FromArgb(
                        color.A, color.R, color.G, color.B)),
                        0, 0,
                        size, size);
            }
            return bitmap;
        }


        public void OnSizeChanged(IAdornmentLayer layer, IWpfTextView view, int streakCount, bool backgroundColorChanged = false)
        {
            throw new NotImplementedException();
        }

        public void OnTextBufferChanged(IAdornmentLayer layer, IWpfTextView view, TextContentChangedEventArgs e)
        {

        }

        public void TextBufferPostChanged(IAdornmentLayer layer, IWpfTextView view, EventArgs e)
        {
            var spawnedAmount = 10;

            Random random = new Random();

            for (int i = 0; i < spawnedAmount; i++)
            {

                // Generate random offsets
                int targetXOffset = random.Next(-8, 8);
                int targetYOffset = random.Next(-6, 6);

                Point target = new Point
                (
                    view.Caret.Left + targetXOffset + (8 / 2) - (3 / 2), 
                    view.Caret.Top  + targetYOffset + (view.Caret.Height / 2) - (3 / 2)
                );


                // Generate random offsets
                int spawnXOffset = random.Next(-6, 6);
                int spawnYOffset = random.Next(-6, 6);



                Point spawnLocation = new Point
                (
                    target.X + spawnXOffset, 
                    target.Y + spawnYOffset
                );

                CreateVisuals(layer, view, target, spawnLocation);
            }
        }

        public void UpdateVisuals(IAdornmentLayer layer, IWpfTextView view)
        {
            throw new NotImplementedException();
        }
    }
}
