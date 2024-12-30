using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace UltraPowerMode.Adornments
{
    internal class HighlightAdornment : IAdornment
    {
        private readonly Brush brush;
        private readonly Pen pen;

        public HighlightAdornment()
        {
            // Create the pen and brush to color the box behind the a's
            this.brush = new SolidColorBrush(Color.FromArgb(0x20, 0x00, 0x00, 0xff));
            this.brush.Freeze();

            var penBrush = new SolidColorBrush(Colors.Red);
            penBrush.Freeze();
            this.pen = new Pen(penBrush, 0.5);
            this.pen.Freeze();
        }

        public void Cleanup(IAdornmentLayer adornmentLayer, IWpfTextView view)
        {
            //throw new NotImplementedException();
        }

        public void OnSizeChanged(IAdornmentLayer adornmentLayer, IWpfTextView view, int streakCount, bool backgroundColorChanged = false)
        {
            //throw new NotImplementedException();
        }

        public void OnTextBufferChanged(IAdornmentLayer adornmentLayer, IWpfTextView view, TextContentChangedEventArgs e)
        {
            foreach (ITextChange change in e.Changes)
            {
                this.CreateVisuals(adornmentLayer, view, change);
            }
        }



        /// <summary>
        /// Adds the scarlet box behind the characters within the given line
        /// </summary>
        /// <param name="adornmentLayer"></param>
        /// <param name="view"></param>
        /// <param name="change"></param>
        private void CreateVisuals(IAdornmentLayer adornmentLayer, IWpfTextView view, ITextChange change)
        {
            IWpfTextViewLineCollection textViewLines = view.TextViewLines;

            SnapshotSpan span = new SnapshotSpan(view.TextSnapshot, Span.FromBounds(0, change.NewEnd-1));

            //SnapshotSpan span = new SnapshotSpan(view.TextSnapshot, change.OldPosition, 1);

            Geometry geometry = textViewLines.GetMarkerGeometry(span);

            Debug.WriteLine(change.NewEnd);
            Debug.WriteLine(change.NewPosition);
            Debug.WriteLine(change.NewLength);
            Debug.WriteLine(change.OldEnd);
            Debug.WriteLine(change.OldPosition);
            Debug.WriteLine(change.OldLength);

            if (geometry != null)
            {
                var drawing = new GeometryDrawing(this.brush, this.pen, geometry);
                drawing.Freeze();

                var drawingImage = new DrawingImage(drawing);
                drawingImage.Freeze();

                var image = new Image
                {
                    Source = drawingImage,
                };

                // Align the image with the top of the bounds of the text geometry
                Canvas.SetLeft(image, geometry.Bounds.Left);
                Canvas.SetTop(image, geometry.Bounds.Top);

                adornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, image, null);
            }

        }
    }
}
