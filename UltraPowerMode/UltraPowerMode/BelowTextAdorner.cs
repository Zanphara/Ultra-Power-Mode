﻿using EnvDTE;
using Microsoft.Build.Framework.XamlTypes;
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
    internal sealed class BelowTextAdorner
    {
        private readonly IAdornmentLayer layer;

        private readonly IAdornment screenShakeAdornment;
        private readonly IAdornment highlightAdornment;


        private readonly IWpfTextView view;



        /// <summary>
        /// Initializes a new instance of the <see cref="BelowTextAdorner"/> class.
        /// </summary>
        /// <param name="view">Text view to create the adornment for</param>
        public BelowTextAdorner(IWpfTextView view)
        {
            if (view == null) {throw new ArgumentNullException("view");}

            this.layer = view.GetAdornmentLayer("BelowTextAdorner");

            this.view = view;

            screenShakeAdornment = new ScreenShakeAdornment();
            highlightAdornment = new HighlightAdornment();

            //Todo//
            // add in the seettings

            this.view.LayoutChanged += OnLayoutChanged;
            this.view.ViewportHeightChanged += View_ViewportSizeChanged;
            this.view.ViewportWidthChanged += View_ViewportSizeChanged;
            this.view.TextBuffer.PostChanged += TextBuffer_PostChanged;
            this.view.TextBuffer.Changed += TextBuffer_Changed;
            this.view.Caret.PositionChanged += Caret_PositionChanged;
            this.view.Closed += View_Closed;
        }

        private void TextBuffer_Changed(object sender, TextContentChangedEventArgs e)
        {
        }

        private void View_Closed(object sender, EventArgs e)
        {
            highlightAdornment.Cleanup(layer, view);
        }

        private void TextBuffer_PostChanged(object sender, EventArgs e)
        {
            highlightAdornment.TextBufferPostChanged(layer, view, e);
        }

        private void Caret_PositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            highlightAdornment.CaretPositionChanged(layer, view, e);
        }

        private void View_ViewportSizeChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (view.IsClosed)
            {
                screenShakeAdornment.Cleanup(layer, view);
                highlightAdornment.Cleanup(layer, view);
            }
            //add highllight update as when the user moves the screen the highlight bugs out
        }

    }
}
