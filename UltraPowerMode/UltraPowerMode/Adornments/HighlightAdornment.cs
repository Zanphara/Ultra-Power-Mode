using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Differencing;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UltraPowerMode.Adornments
{
    //TODO//
    //when there is nothing on a line pressin enter messes up the highligh position
    //pasting in a specific way casues the code to run twice and the highlight to be put in the wrong place

    internal class HighlightAdornment : IAdornment
    {
        private readonly Brush brush;
        private readonly Pen pen;
        private Rectangle highlightRectangle;

        public HighlightAdornment()
        {
            // Create the pen and brush to color the box behind the character
            // Bright white background with some transparency
            this.brush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));  // Semi-transparent white (120 is opacity)
            this.brush.Freeze();

            var penBrush = new SolidColorBrush(Colors.Transparent);  // No border (just the highlight box)
            penBrush.Freeze();
            this.pen = new Pen(penBrush, 0.5);
            this.pen.Freeze();

            // Create a rectangle that will serve as the highlight
            this.highlightRectangle = new Rectangle
            {
                Fill = brush,
                Stroke = penBrush,  // Optional, you can add a border if needed
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
            var newPosition = e.Changes[0].NewEnd;

            //int offset = view.Caret.Position.BufferPosition.Position - e.Changes[0].NewEnd;

            if (!adornmentLayer.Elements.Any(adornment => adornment.Adornment == highlightRectangle))
            {
                CreateVisuals(adornmentLayer, view); // Create the visuals if they don't exist
            }
            else
            {
                UpdateVisuals(adornmentLayer, view, newPosition); // Update the visuals when text is typed or deleted
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

        private void CreateVisuals(IAdornmentLayer adornmentLayer, IWpfTextView view)
        {

            // Add the adornment to the layer with a valid SnapshotSpan
            adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, highlightRectangle, null);

            // Now update the visuals after adding the adornment
            UpdateVisuals(adornmentLayer, view);
        }

        private void UpdateVisuals(IAdornmentLayer adornmentLayer, IWpfTextView view, int exactposition = -1)
        {
            try
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

                if (exactposition != -1)
                {
                    // Move the highlight box to the exact position in the text editor
                    SnapshotPoint caretPosition = new SnapshotPoint(view.TextSnapshot, exactposition); //out of bounds here sometimes
                    var textViewLine = view.TextViewLines.GetTextViewLineContainingBufferPosition(caretPosition);
                    var characterBounds = textViewLine.GetCharacterBounds(caretPosition);
                    Debug.WriteLine("exactposito: " + exactposition);
                    Debug.WriteLine("boundingpos: " + characterBounds.Left);
                    Debug.WriteLine("caretsposit: " + view.Caret.Position.BufferPosition.Position);
                    Debug.WriteLine(" ");

                    Canvas.SetLeft(highlightRectangle, characterBounds.Left);
                }

            }
            catch (ArgumentOutOfRangeException)
            {
                // Safely handle cases where the buffer position is invalid
                Debug.WriteLine("Caret position out of range, skipping update.");
            }
        }
    }
}