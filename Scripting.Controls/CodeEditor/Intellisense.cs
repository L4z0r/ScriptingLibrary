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
    public class Intellisense : IDisposable
    {
        #region Variables

        // Is this disposed?
        private bool disposed;

        // Memorysaving via imageList
        private List<Bitmap> images;
        private List<AutoCompleteWord> items;

        #endregion

        #region Properties

        /// <summary>
        /// Returns the list of images
        /// </summary>
        public List<Bitmap> Images
        {
            get
            {
                return images;
            }
        }

        /// <summary>
        /// Returns the list of items
        /// </summary>
        public List<AutoCompleteWord> Items
        {
            get
            {
                return items;
            }
        }

        #endregion

        #region Constructor

        // Creates an empty intellisense
        public Intellisense()
        {
            this.images = new List<Bitmap>();
            this.items = new List<AutoCompleteWord>();
            this.disposed = false;
        }

        #endregion

        #region Destructor

        // The standard destructor
        ~Intellisense()
        {
            this.Dispose(false);
        }

        // Implementation of IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Clean-Up of all resources
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Free other inheritors of IDisposable
                    foreach (var bmp in images)
                    {
                        bmp.Dispose();
                    }
                }

                // Set objects to null
                this.images.Clear();
                this.items.Clear();
                this.images = null;
                this.items = null;
                this.disposed = true;
            }
        }

        #endregion

        #region Methods

        // Adds items from stringlist
        public void ParseItems(List<string> items, int index)
        {
            this.items.Clear();
            foreach (string itemword in items)
            {
                var word = new AutoCompleteWord();
                word.ImageIndex = index;
                word.ItemWord = itemword;
                this.items.Add(word);
            }
        }

        #endregion
    }

    public struct AutoCompleteWord
    {
        // We should not waste Bitmaps...
        // so we have a struct with an index
        public int ImageIndex;
        public string ItemWord;
    }
}