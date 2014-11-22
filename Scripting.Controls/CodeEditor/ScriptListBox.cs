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
    public class ScriptListBox : ListBox
    {
        #region Constants

        const ControlStyles style = ControlStyles.AllPaintingInWmPaint 
            | ControlStyles.OptimizedDoubleBuffer;
        const int ITEM_HEIGHT = 16;

        #endregion

        #region Variables

        // Disposed yet?
        private bool disposed;

        // New stuff
        private List<Bitmap> imageList;
        private List<AutoCompleteWord> words;
        private Color selectionBorderColor;
        private Color selectionColor;

        #endregion

        #region Properties

        /// <summary>
        /// Returns or sets the list of bitmaps
        /// </summary>
        public List<Bitmap> ImageList
        {
            get
            {
                return imageList;
            }
            set
            {
                imageList = value;
            }
        }

        /// <summary>
        /// Returns or sets the list of items
        /// </summary>
        public List<AutoCompleteWord> Words
        {
            get
            {
                return words;
            }
            set
            {
                words = value;
            }
        }

        /// <summary>
        /// Draws the selected item with a border
        /// </summary>
        public Color SelectionBorderColor
        {
            get
            {
                return selectionBorderColor;
            }
            set
            {
                selectionBorderColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Draws the selected item with a background
        /// </summary>
        public Color SelectionColor
        {
            get
            {
                return selectionColor;
            }
            set
            {
                selectionColor = value;
                Invalidate();
            }
        }

        #endregion

        #region Constructor

        // Constructs a listbox and enables various
        // styles for customizable drawing
        public ScriptListBox(Intellisense sense)
        {
            this.imageList = sense.Images;
            this.words = sense.Items;

            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.DoubleBuffered = true;
            this.SetStyle(style, true);

            this.selectionBorderColor = Color.FromArgb(244, 244, 244);
            this.selectionColor = Color.FromArgb(230, 230, 230);
            this.MaximumSize = new Size(200, ITEM_HEIGHT * 10);
            this.words = new List<AutoCompleteWord>();
            this.ItemHeight = ITEM_HEIGHT;
            this.disposed = false;
        }

        #endregion

        #region Destructor

        // Overrides the destructor with our custom code
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                base.Dispose(disposing);
                if (disposing)
                {
                    // Free our list
                    this.words.Clear();
                }

                // Set it to null
                this.words = null;
                this.disposed = true;
            }
        }

        #endregion

        #region Painting

        // Paints our custom items
        protected override void OnPaint(PaintEventArgs e)
        {
            // Draw border
            var pen = new Pen(Color.FromArgb(160, 160, 160));
            var selected = this.SelectedIndex;
            var bdrect = new Rectangle();

            bdrect.X = e.ClipRectangle.X;
            bdrect.Y = e.ClipRectangle.Y;
            bdrect.Width = e.ClipRectangle.Width - 1;
            bdrect.Height = e.ClipRectangle.Height - 1;

            e.Graphics.DrawRectangle(pen, bdrect);

            // Draw selection
            if (selected != -1)
            {
                // Predefine variables
                var bgbrush = new SolidBrush(selectionColor);
                var selpen = new Pen(selectionBorderColor);
                var rect = new Rectangle();

                rect.Y = (4 + (selected * ITEM_HEIGHT));
                rect.Height = ITEM_HEIGHT; rect.X = 1;
                rect.Width = Width - 3;

                // Draw border & bg
                e.Graphics.FillRectangle(bgbrush, rect);
                e.Graphics.DrawRectangle(selpen, rect);

                // Clean-Up
                bgbrush.Dispose();
                selpen.Dispose();
            }

            // Clean-Up
            pen.Dispose();
        }

        // Paints our custom items
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            // Draws each item with an image
            var index = e.Index;
            var item = ((AutoCompleteWord)Items[index]);

            var br = new SolidBrush(ForeColor);
            var tx = new Point(18, 2);
            var pnt = new Point(0, 0);

            var txt = item.ItemWord;
            e.Graphics.DrawString(txt, Font, br, tx);

            if (imageList.Count != 0)
            {
                var bmp = imageList[item.ImageIndex];
                e.Graphics.DrawImage(bmp, pnt);
            }

            // Clean-Up
            br.Dispose();
        }

        // Trick the measureitem-eventargs for our image
        protected override void OnMeasureItem(MeasureItemEventArgs e)
        {
            base.OnMeasureItem(e);
            e.ItemWidth += 18;
        }

        #endregion

        #region Methods

        // Adds items to normal list so
        // we do not destroy the events.
        public void AddItems()
        {
            this.Items.Clear();
            foreach (var item in words)
            {
                this.Items.Add(item);
            } this.Invalidate();
        }

        // Removes items from normal list.
        public void RemoveItems()
        {
            this.words.Clear();
            this.Items.Clear();
            this.Invalidate();
        }

        #endregion
    }
}