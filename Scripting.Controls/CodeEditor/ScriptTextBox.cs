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

        /// <summary>
        /// Absolute displayed text
        /// </summary>
        public string DisplayedText
        {
            get
            {
                int startpos;
                int endpos;

                startpos = GetStartCharPos();
                endpos = GetLastCharPos();

                return Text.Substring(startpos, (endpos - startpos) + 1);
            }
        }

        private int GetStartCharPos()
        {
            var upperleft = new Point(ClientRectangle.Left, 
                ClientRectangle.Top + (FontHeight / 2));
            return GetCharIndexFromPosition(upperleft);
        }

        private int GetLastCharPos()
        {
            var lowerright = new Point(ClientRectangle.Right,
                ClientRectangle.Bottom);
            return GetCharIndexFromPosition(lowerright);
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

    #region Native Methods

    public class NativeMethods
    {

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Explicit, Size = 4)]
        public struct MakeAParam
        {

            [FieldOffset(0)]
            public Int32 Number;

            [FieldOffset(0)]
            public UInt16 LowWord;

            [FieldOffset(2)]
            public UInt16 HighWord;
        }

        public const int WM_VSCROLL = 0x115;
        public const int SB_HORZ = 0;
        public const int SB_VERT = 1;
        public const int SB_PAGEDOWN = 3;
        public const int SB_PAGEUP = 2;
        public const int SB_LINEDOWN = 1;
        public const int SB_LINEUP = 0;
        public const int SB_TOP = 6;

        public const int SB_ENDSCROLL = 8;
        public const int WM_HSCROLL = 0x114;
        public const int SB_LEFT = 6;
        public const int SB_LINELEFT = 0;
        public const int SB_LINERIGHT = 1;
        public const int SB_PAGERIGHT = 3;
        public const int SB_PAGELEFT = 2;

        public const int SB_RIGHT = 7;
    }

    public class ScrollBarInfo
    {
        public enum VScrollBarCommands : uint
        {
            SB_LINEUP = 0,
            SB_LINEDOWN = 1,
            SB_PAGEUP = 2,
            SB_PAGEDOWN = 3,
            SB_TOP = 6,
            SB_BOTTOM = 7,
            SB_THUMBPOSITION = 4,
            SB_THUMBTRACK = 5,
            SB_ENDSCROLL = 8
        }

        public enum HScrollBarCommands : uint
        {
            SB_LINELEFT = 0,
            SB_LINERIGHT = 1,
            SB_PAGELEFT = 2,
            SB_PAGERIGHT = 3,
            SB_LEFT = 6,
            SB_RIGHT = 7,
            SB_THUMBPOSITION = 4,
            SB_THUMBTRACK = 5,
            SB_ENDSCROLL = 8
        }

        public enum ScrollBarType : int
        {
            SbHorz = 0,
            SbVert = 1,
            SbCtl = 2,
            SbBoth = 3
        }

        [Flags]
        public enum ScrollBarInfoFmask : int
        {
            Range = 0x1,
            Page = 0x2,
            Pos = 0x4,
            DisableNoScroll = 0x8,
            TrackPos = 0x10,
            All = (Range | Page | Pos | TrackPos)
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ScrollInfo
        {
            public int cbSize;
            public int fMask;
            public int nMin;
            public int nMax;
            public int nPage;
            public int nPos;
            public int nTrackPos;
        }
        
        [DllImport("user32.dll")]
        static extern int GetScrollInfo(IntPtr hWnd, int bar, ScrollInfo si);

        [DllImport("user32.dll")]
        static extern int SetScrollInfo(IntPtr hWnd, int bar, ScrollInfo si, bool redraw);
    }

    #endregion
}