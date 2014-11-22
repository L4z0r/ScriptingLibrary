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
                AddItems();
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

            this.selectionBorderColor = Color.FromArgb(216, 216, 216);
            this.selectionColor = Color.FromArgb(230, 230, 230);
            this.MaximumSize = new Size(200, ITEM_HEIGHT * 10);
            this.ItemHeight = ITEM_HEIGHT;
            this.disposed = false;
            this.AddItems();
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
                    if (words != null)
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
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            // Draws each item with an image
            var index = e.Index;

            if (index != -1)
            {
                var item = ((AutoCompleteWord)Items[index]);
                var br = new SolidBrush(ForeColor);

                var tx = new Point(18, index * ITEM_HEIGHT);
                var pnt = new Point(0, 0);
                var txt = item.ItemWord;

                // Draw selection
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    // Predefine variables
                    var bgbrush = new SolidBrush(selectionColor);
                    var selpen = new Pen(selectionBorderColor);

                    var sz = new Size(e.Bounds.Width, ITEM_HEIGHT);
                    var rect = new Rectangle(tx, sz); rect.X = 0;
                    rect.Height--; rect.Width--;

                    // Draw border & bg
                    e.Graphics.FillRectangle(bgbrush, rect);
                    e.Graphics.DrawRectangle(selpen, rect);

                    // Clean-Up
                    bgbrush.Dispose();
                    selpen.Dispose();
                }
                else
                {
                    // Draw background
                    // Draw border
                    var bk = new SolidBrush(BackColor);
                    var sz = new Size(e.Bounds.Width, ITEM_HEIGHT);
                    var rect = new Rectangle(tx, sz); rect.X = 0;

                    // Draw background
                    e.Graphics.FillRectangle(bk, rect);

                    // Clean-Up
                    bk.Dispose();
                }

                e.Graphics.DrawString(txt, Font, br, tx);

                if (imageList.Count != 0)
                {
                    var bmp = imageList[item.ImageIndex];
                    e.Graphics.DrawImage(bmp, pnt);
                }

                // Clean-Up
                br.Dispose();
            }
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
            }
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