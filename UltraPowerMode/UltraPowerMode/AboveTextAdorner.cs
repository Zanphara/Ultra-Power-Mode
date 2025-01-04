using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using UltraPowerMode.Adornments;

namespace UltraPowerMode
{
    /// <summary>
    /// AboveTextAdorner places red boxes behind all the "a"s in the editor window
    /// </summary>
    internal sealed class AboveTextAdorner
    {
        private readonly IAdornmentLayer layer;

        private readonly IAdornment particlesAdornment;
        private readonly IAdornment lightningAdornment;

        private readonly IWpfTextView view;


        public AboveTextAdorner(IWpfTextView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            this.layer = view.GetAdornmentLayer("AboveTextAdorner");

            this.view = view;

            particlesAdornment = new ParticlesAdornment();
            lightningAdornment = new LightningAdornment();

            this.view.TextBuffer.PostChanged += TextBuffer_PostChanged;
            this.view.TextBuffer.Changed += TextBuffer_Changed;
            this.view.Closed += View_Closed;
        }

        private void View_Closed(object sender, EventArgs e)
        {
            if (view.IsClosed)
            {
                particlesAdornment.Cleanup(layer, view);
            }
        }

        private void TextBuffer_Changed(object sender, TextContentChangedEventArgs e)
        {
            particlesAdornment.OnTextBufferChanged(layer, view, e);
        }

        private void TextBuffer_PostChanged(object sender, EventArgs e)
        {
            particlesAdornment.TextBufferPostChanged(layer, view, e);
        }
    }
}
