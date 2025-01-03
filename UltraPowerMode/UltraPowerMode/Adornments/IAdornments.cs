﻿using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;


namespace UltraPowerMode.Adornments
{ 
    public interface IAdornment
    {
        void OnSizeChanged(IAdornmentLayer adornmentLayer, IWpfTextView view, int streakCount, bool backgroundColorChanged = false);

        void OnTextBufferChanged(IAdornmentLayer adornmentLayer, IWpfTextView view, TextContentChangedEventArgs e);

        void Cleanup(IAdornmentLayer adornmentLayer, IWpfTextView view);

        void CaretPositionChanged(IAdornmentLayer layer, IWpfTextView view, CaretPositionChangedEventArgs e);
        void TextBufferPostChanged(IAdornmentLayer layer, IWpfTextView view, EventArgs e);
    }
}