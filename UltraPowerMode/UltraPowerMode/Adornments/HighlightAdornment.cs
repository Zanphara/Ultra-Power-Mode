using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UltraPowerMode.Adornments
{
    //TODO//
    // make it so that the character moves fluidly with the caret
    //  fluidly means that it doesnt teleport from one pos to annother
    //  it means the box should streach and shrink as the caret moves
    //  and flow to whereever the caret is
    // idea:
    //  store the current position of the highlight 
    //  once a method is called to say it needs to move calculate the 
    //  travel line between the current position and the new position and
    //  fluidly move the highlight

    internal class HighlightAdornment : IAdornment
    {
        private readonly Brush brush;
        private Rectangle highlightRectangle;

        public HighlightAdornment()
        {
            // Create the pen and brush to color the box behind the character
            // Bright white background with some transparency
            this.brush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)); 
            this.brush.Freeze();

            // Create a rectangle that will serve as the highlight
            this.highlightRectangle = new Rectangle
            {
                Fill = brush,
                StrokeThickness = 0
            };
        }

        public void Cleanup(IAdornmentLayer adornmentLayer, IWpfTextView view)
        {
            adornmentLayer.RemoveAllAdornments();
        }

        public void OnSizeChanged(IAdornmentLayer adornmentLayer, IWpfTextView view, int streakCount, bool backgroundColorChanged = false)
        {
            //throw new NotImplementedException();
        }

        public void OnTextBufferChanged(IAdornmentLayer adornmentLayer, IWpfTextView view, TextContentChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void TextBufferPostChanged(IAdornmentLayer adornmentLayer, IWpfTextView view, EventArgs e)
        {
            if (!adornmentLayer.Elements.Any(adornment => adornment.Adornment == highlightRectangle))
            {
                CreateVisuals(adornmentLayer, view); // Create the visuals if they don't exist
            }
            else
            {
                UpdateVisuals(adornmentLayer, view); // Update the visuals when text is typed or deleted
            }
        }

        public void CaretPositionChanged(IAdornmentLayer adornmentLayer, IWpfTextView view, CaretPositionChangedEventArgs e)
        {
            // Ensure the rectangle is added to the adornment layer only once
            if (!adornmentLayer.Elements.Any(adornment => adornment.Adornment == highlightRectangle))
            {
                CreateVisuals(adornmentLayer, view); // Create the visuals if they don't exist
            }
            else
            {
                UpdateVisuals(adornmentLayer, view); // Update the visuals when caret position changes
            }
        }

        public void CreateVisuals(IAdornmentLayer adornmentLayer, IWpfTextView view)
        {
            // Add the adornment to the layer only if it doenst already exist
            if (!adornmentLayer.Elements.Any(adornment => adornment.Adornment == highlightRectangle))
            {
                adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, highlightRectangle, null);
            }
            UpdateVisuals(adornmentLayer, view); // Update the visuals when caret position changes
        }

        public void UpdateVisuals(IAdornmentLayer layer, IWpfTextView view)
        {
            // Update the rectangle size and position
            highlightRectangle.Width = 8;
            highlightRectangle.Height = view.Caret.Height;

            // Create a white opaque background behind the character
            highlightRectangle.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));  // Fully opaque white
            highlightRectangle.Opacity = 1.0;  // Fully opaque (no transparency)

            // Ensure the highlight box stays behind the character
            Canvas.SetLeft(highlightRectangle, view.Caret.Left);
            Canvas.SetTop(highlightRectangle, view.Caret.Top);
        }
    }
}