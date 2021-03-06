﻿// Copyright (C) 2014 Laz0r
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
    public class SyntaxHighlighting : IDisposable
    {
        #region Constants

        Color empty = Color.Empty;

        #endregion

        #region Variables

        // If this SH is disposed
        private bool disposed;

        // The pattern of the wanted chars
        private string regex;

        // All stuff for transforming a text
        private Font font;
        private Color forecolor;
        private Color backcolor;

        #endregion

        #region Properties

        /// <summary>
        /// Pattern of wanted chars
        /// </summary>
        public string Regex
        {
            get
            {
                return regex;
            }
        }

        /// <summary>
        /// The Selection-Font
        /// </summary>
        public Font Font
        {
            get
            {
                return font;
            }
        }

        /// <summary>
        /// The Selection-Color
        /// </summary>
        public Color ForeColor
        {
            get
            {
                return forecolor;
            }
        }

        /// <summary>
        /// The Selection-BackColor
        /// </summary>
        public Color BackColor
        {
            get
            {
                return backcolor;
            }
        }

        #endregion

        #region Constructor

        // Constructs a new SH, basically we just copy objects
        public SyntaxHighlighting(string str, Font fnt, Color fc, Color bc)
        {
            this.forecolor = fc;
            this.backcolor = bc;
            this.regex = str;
            this.font = fnt;
            this.disposed = false;
        }

        #endregion

        #region Destructor

        // The standard destructor
        ~SyntaxHighlighting()
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
                    this.font.Dispose();
                }

                // Set objects to null
                this.regex = null;
                this.forecolor = empty;
                this.backcolor = empty;
                this.disposed = true;
            }
        }

        #endregion
    }
}