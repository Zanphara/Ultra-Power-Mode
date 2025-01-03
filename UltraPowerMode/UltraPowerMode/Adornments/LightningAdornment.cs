using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraPowerMode.Adornments
{
    internal class LightningAdornment : IAdornment
    {
        public void CaretPositionChanged(IAdornmentLayer layer, IWpfTextView view, CaretPositionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Cleanup(IAdornmentLayer adornmentLayer, IWpfTextView view)
        {
            throw new NotImplementedException();
        }

        public void CreateVisuals(IAdornmentLayer layer, IWpfTextView view)
        {
            throw new NotImplementedException();
        }

        public void OnSizeChanged(IAdornmentLayer adornmentLayer, IWpfTextView view, int streakCount, bool backgroundColorChanged = false)
        {
            throw new NotImplementedException();
        }

        public void OnTextBufferChanged(IAdornmentLayer adornmentLayer, IWpfTextView view, TextContentChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void TextBufferPostChanged(IAdornmentLayer layer, IWpfTextView view, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void UpdateVisuals(IAdornmentLayer layer, IWpfTextView view)
        {
            throw new NotImplementedException();
        }
    }
}
