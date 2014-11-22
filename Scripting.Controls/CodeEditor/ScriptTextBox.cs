// Copyright (C) 2014 Laz0r
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Scripting.Controls
{
    [DesignTimeVisible(false)]
    public class ScriptTextBox : RichTextBox
    {
        #region External

        // For getting the absolute caret position
        [DllImport("user32.dll")]
        static extern bool GetCaretPos(out Point pnt);

        #endregion

        #region Constants

        // Improves readability
        Point empty = Point.Empty;
        Color color = Color.FromArgb(230, 230, 230);
        ControlStyles style = ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque;

        #endregion

        #region Variables

        // Color of line between linenumbers and this
        private Color lineColor;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the absolute position
        /// of the richtextbox-caret.
        /// </summary>
        public Point CaretPoint
        {
            get
            {
                var pt = new Point();
                if (GetCaretPos(out pt))
                    return pt;
                else
                    return empty;
            }
        }

        /// <summary>
        /// Gets or sets the color of the line
        /// between linenumbers and textbox.
        /// </summary>
        public Color LineColor
        {
            get
            {
                return lineColor;
            }
            set
            {
                lineColor = value;
            }
        }

        #endregion

        #region Constructor

        // Enables double-buffering and other stuff
        public ScriptTextBox()
        {
            SetStyle(style, true);
        }

        #endregion

        #region Overriding

        // We need to hack WM_PAINT to draw our line
        protected override void WndProc(ref Message m)
        {
            // Calls the original wndproc
            base.WndProc(ref m);

            // Draws our custom line
            if (m.Msg == 0xF)
            {
                var gr = Graphics.FromHwnd(this.Handle);
                var pn = new Pen(lineColor);

                var hg = this.Parent.Height;
                gr.DrawLine(pn, 0, 0, 0, hg);

                pn.Dispose();
                gr.Dispose();
            }
        }

        #endregion
    }
}