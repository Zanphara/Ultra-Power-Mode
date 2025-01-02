using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

using UltraPowerMode.Adornments;

namespace UltraPowerMode
{
    internal sealed class UltraPowerModeAdornment
    {
        private readonly IAdornmentLayer layer;

        private readonly IAdornment screenShakeAdornment;
        private readonly IAdornment lightningAdornment;
        private readonly IAdornment particlesAdornment;
        private readonly IAdornment highlightAdornment;


        private readonly IWpfTextView view;



        /// <summary>
        /// Initializes a new instance of the <see cref="UltraPowerModeAdornment"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        public UltraPowerModeAdornment(IWpfTextView view)
        {
            if (view == null) {throw new ArgumentNullException("view");}

            this.layer = view.GetAdornmentLayer("UltraPowerModeAdornment");

            this.view = view;

            screenShakeAdornment = new ScreenShakeAdornment();
            particlesAdornment = new ParticlesAdornment();
            lightningAdornment = new LightningAdornment();
            highlightAdornment = new HighlightAdornment();

            //Todo//
            // add in the seettings

            this.view.LayoutChanged += OnLayoutChanged;
            this.view.ViewportHeightChanged += View_ViewportSizeChanged;
            this.view.ViewportWidthChanged += View_ViewportSizeChanged;
            this.view.TextBuffer.Changed += TextBuffer_Changed;
            this.view.Caret.PositionChanged += Caret_PositionChanged;

        }

        private void Caret_PositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            highlightAdornment.CaretPositionChanged(layer, view, e);
        }

        private void View_ViewportSizeChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void TextBuffer_Changed(object sender, TextContentChangedEventArgs e)
        {
            highlightAdornment.OnTextBufferChanged(layer, view, e);
        }

        /// <summary>
        /// Handles whenever the text displayed in the view changes by adding the adornment to any reformatted lines
        /// </summary>
        /// <remarks><para>This event is raised whenever the rendered text displayed in the <see cref="ITextView"/> changes.</para>
        /// <para>It is raised whenever the view does a layout (which happens when DisplayTextLineContainingBufferPosition is called or in response to text or classification changes).</para>
        /// <para>It is also raised whenever the view scrolls horizontally or when its size changes.</para>
        /// </remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if(view.IsClosed)
            {
                screenShakeAdornment.Cleanup(layer, view);
                particlesAdornment.Cleanup(layer, view);
            }
        }

    }
}
