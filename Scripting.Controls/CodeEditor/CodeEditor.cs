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
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Scripting.Controls
{
    public class CodeEditor : UserControl
    {
        #region Override

        [Browsable(false)]
        public new string AccessibleDescription
        {
            get { return base.AccessibleDescription; }
        }

        [Browsable(false)]
        public new string AccessibleName
        {
            get { return base.AccessibleName; }
        }

        [Browsable(false)]
        public new AccessibleRole AccessibleRole
        {
            get { return base.AccessibleRole; }
        }

        [Browsable(false)]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        [Browsable(false)]
        public override Image BackgroundImage
        {
            get { return base.BackgroundImage; }
            set { base.BackgroundImage = value; }
        }

        [Browsable(false)]
        public override ImageLayout BackgroundImageLayout
        {
            get { return base.BackgroundImageLayout; }
            set { base.BackgroundImageLayout = value; }
        }

        [Browsable(false)]
        public new BorderStyle BorderStyle
        {
            get { return base.BorderStyle; }
            set { base.BorderStyle = value; }
        }

        [Browsable(false)]
        public override Cursor Cursor
        {
            get { return base.Cursor; }
            set { base.Cursor = value; }
        }

        [Category("Layout")]
        public override Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }

        [Browsable(false)]
        public override Color ForeColor
        {
            get { return base.ForeColor; }
            set { base.ForeColor = value; }
        }

        [Browsable(false)]
        public override RightToLeft RightToLeft
        {
            get { return base.RightToLeft; }
            set { base.RightToLeft = value; }
        }

        [Browsable(false)]
        public new bool UseWaitCursor
        {
            get { return base.UseWaitCursor; }
        }

        [Browsable(false)]
        public new ControlBindingsCollection DataBindings
        {
            get { return base.DataBindings; }
        }

        [Browsable(false)]
        public new Object Tag
        {
            get { return base.Tag; }
        }

        [Browsable(false)]
        public new bool CausesValidation
        {
            get { return base.CausesValidation; }
        }

        [Browsable(false)]
        public new bool AutoScroll
        {
            get { return base.AutoScroll; }
        }

        [Browsable(false)]
        public new Size AutoScrollMargin
        {
            get { return base.AutoScrollMargin; }
            set { base.AutoScrollMargin = value; }
        }

        [Browsable(false)]
        public new Size AutoScrollMinSize
        {
            get { return base.AutoScrollMinSize; }
            set { base.AutoScrollMinSize = value; }
        }

        [Browsable(false)]
        public override bool AutoSize
        {
            get { return base.AutoSize; }
            set { base.AutoSize = value; }
        }

        [Browsable(false)]
        public new AutoSizeMode AutoSizeMode
        {
            get { return base.AutoSizeMode; }
        }

        [Browsable(false)]
        public new Padding Margin
        {
            get { return base.Margin; }
        }

        [Browsable(false)]
        public new Size MaximumSize
        {
            get { return base.MaximumSize; }
        }

        [Browsable(false)]
        public new Size MinimumSize
        {
            get { return base.MinimumSize; }
        }

        [Browsable(false)]
        public new Padding Padding
        {
            get { return base.Padding; }
        }

        [Browsable(false)]
        public override bool AllowDrop
        {
            get { return base.AllowDrop; }
            set { base.AllowDrop = value; }
        }

        [Browsable(false)]
        public new AutoValidate AutoValidate
        {
            get { return base.AutoValidate; }
        }

        [Browsable(false)]
        public override ContextMenuStrip ContextMenuStrip
        {
            get { return base.ContextMenuStrip; }
            set { base.ContextMenuStrip = value; }
        }

        [Browsable(false)]
        public new ImeMode ImeMode
        {
            get { return base.ImeMode; }
        }

        [Browsable(false)]
        public new int TabIndex
        {
            get { return base.TabIndex; }
            set { base.TabIndex = value; }
        }

        [Browsable(false)]
        public new bool TabStop
        {
            get { return base.TabStop; }
        }

        #endregion

        #region External

        // Disables update for smoother drawing
        [DllImport("user32.dll")]
        static extern bool LockWindowUpdate(IntPtr hWndLock);

        #endregion

        #region Constants

        Color line_color = Color.FromArgb(230, 230, 230);
        const RegexOptions option = RegexOptions.Multiline;
        const AnchorStyles anchor = AnchorStyles.Bottom | AnchorStyles.Top
                | AnchorStyles.Left | AnchorStyles.Right;
        const ControlStyles style = ControlStyles.ResizeRedraw;
        const string whitespace = " ";

        #endregion

        #region Variables

        // Determines if a listbox is opened currently
        private bool intellisenseActive;

        // Subcontrols of the codeeditor
        private ScriptListBox intellisenseBox;
        private ScriptTextBox scriptBox;

        // For syntaxhighlighting and intellisense
        private List<SyntaxHighlighting> ruleSets;
        private Intellisense autocompleteWords;

        // Current line selection
        private Color lineBorderColor;
        private Color lineBackColor;

        // Various color defs
        private Color codeBackColor;
        private Color codeBorderColor;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the regex rules
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<SyntaxHighlighting> RuleSets
        {
            get
            {
                return ruleSets;
            }
            set
            {
                ruleSets = value;
            }
        }

        /// <summary>
        /// Gets or sets the intellisense
        /// </summary>
        [Browsable(false)]
        public Intellisense AutoComplete
        {
            get
            {
                return autocompleteWords;
            }
            set
            {
                autocompleteWords = value;
            }
        }

        [Category("CodeEditor-Properties")]
        [Description("Bordercolor of selected line.")]
        public Color LineBorderColor
        {
            get
            {
                return lineBorderColor;
            }
            set
            {
                lineBorderColor = value;
                this.Invalidate();
            }
        }

        [Category("CodeEditor-Properties")]
        [Description("Backcolor of selected line.")]
        public Color LineBackColor
        {
            get
            {
                return lineBackColor;
            }
            set
            {
                lineBackColor = value;
                this.Invalidate();
            }
        }

        [Category("CodeEditor-Properties")]
        [Description("Color of line between numbers and textbox.")]
        public Color LineSeperator
        {
            get
            {
                return scriptBox.LineColor;
            }
            set
            {
                scriptBox.LineColor = value;
                scriptBox.Invalidate();
            }
        }

        [Category("CodeEditor-Properties")]
        [Description("Backcolor of the editor.")]
        public Color CodeBackColor
        {
            get
            {
                return codeBackColor;
            }
            set
            {
                codeBackColor = value;
                this.Invalidate();
            }
        }

        [Category("CodeEditor-Properties")]
        [Description("Bordercolor of the editor.")]
        public Color CodeBorderColor
        {
            get
            {
                return codeBorderColor;
            }
            set
            {
                codeBorderColor = value;
                this.Invalidate();
            }
        }

        #endregion

        #region Constructor

        // Constructs a new CodeEditor and sets various properties
        public CodeEditor()
        {
            // CodeEditor properties
            this.intellisenseActive = false;
            this.SetStyle(style, true);
            this.DoubleBuffered = true;
            this.Anchor = anchor;

            this.codeBorderColor = Color.FromArgb(160, 160, 160);
            this.codeBackColor = Color.FromArgb(244, 244, 244);
            this.lineBorderColor = Color.FromArgb(51, 153, 255);
            this.lineBackColor = Color.FromArgb(158, 206, 255);

            this.scriptBox = new ScriptTextBox();
            this.ruleSets = new List<SyntaxHighlighting>();

            // Predefines some variables
            var sze = new Size(Width - 49, Height -2);
            var fnt = new Font("Consolas", 9.75f);
            var pnt = new Point(48, 1);
            var bds = BorderStyle.None;

            // ScriptBox properties
            this.scriptBox.WordWrap = false;
            this.scriptBox.AcceptsTab = true;
            this.scriptBox.HideSelection = false;

            this.scriptBox.Anchor = anchor;
            this.scriptBox.Location = pnt;
            this.scriptBox.Size = sze;

            this.scriptBox.LineColor = line_color;
            this.scriptBox.BorderStyle = bds;
            this.scriptBox.Font = fnt;

            // ScriptTextBox events
            this.scriptBox.PreviewKeyDown += PreviewOnKey;
            this.scriptBox.TextChanged += OnTextChange;
            this.scriptBox.VScroll += OnTextScroll;
            this.scriptBox.GotFocus += OnTextFocus;
            this.scriptBox.Click += OnMouseClick;
            this.scriptBox.KeyDown += OnKeyDown;

            // Other properties
            this.Controls.Add(scriptBox);
            this.Font = fnt;
        }

        #endregion

        #region Codeevents

        // Hides the autocompletelist if you press escape or enter a new line.
        private void PreviewOnKey(object obj, PreviewKeyDownEventArgs arg)
        {
            // Variables (readability) =D
            var data = arg.KeyData;
            var enter = Keys.Enter;
            var esc = Keys.Escape;

            if (data == enter || data == esc)
            {
                // Hide the intellibox
                if (this.intellisenseActive == true)
                {
                    this.DisposeIntellisense();
                }
            }
        }

        // Updates and highlights the text, shows intellisense if word matches.
        private void OnTextChange(object obj, EventArgs arg)
        {
            try
            {
                int textlength = scriptBox.TextLength;
                if (textlength != 0)
                {

                    // Locks the update for smoothness purposes
                    LockWindowUpdate(this.scriptBox.Handle);

                    // Saves the current caret position and other vars
                    var caretpos = this.scriptBox.SelectionStart;
                    var length = this.scriptBox.TextLength;
                    var foreclr = this.scriptBox.ForeColor;
                    var scfont = this.scriptBox.Font;

                    // Only edits the visible text, for performance improvements
                    string text = this.scriptBox.DisplayedText;

                    // Resets the whole selection colors
                    this.scriptBox.Select(0, length);
                    this.scriptBox.SelectionColor = foreclr;
                    this.scriptBox.SelectionFont = scfont;

                    // Selects all the regex stuff
                    foreach (var sh in ruleSets)
                    {
                        // Saves all the properties so we 
                        // don't have to access them all the time
                        var fore = sh.ForeColor;
                        var back = sh.BackColor;
                        var font = sh.Font;
                        var str = sh.Regex;

                        // Looks for any matches
                        var match = Regex.Match(text, str, option);
                        if (match.Success == true)
                        {
                            do
                            {
                                // Loops as long as there are matches
                                // and selects them via match.Index.
                                this.scriptBox.Select(match.Index, match.Length);
                                this.scriptBox.SelectionBackColor = back;
                                this.scriptBox.SelectionColor = fore;
                                this.scriptBox.SelectionFont = font;
                                match = match.NextMatch();
                            } while (match.Success == true);
                        }
                    }

                    // Now we reset the caret position,
                    // in preparation for intellisense.
                    this.scriptBox.Select(caretpos, 0);

                    // Now we see if the current line is a PART
                    // or is the WHOLE of an autocomplete-word. 
                    if (this.scriptBox.Lines.Length > 0)
                    {
                        // Receive the line number and gets the whole line as string
                        int linenumber = this.scriptBox.GetLineFromCharIndex(caretpos);
                        var sublinetext = this.scriptBox.Lines[linenumber];

                        // Check if this line even contains something ... performance =)
                        if (sublinetext.Length > 0)
                        {
                            // We need a list of string for items in the intellibox
                            // and to escape the line to prevent errors
                            var founditems = new List<string>();
                            var escape = Regex.Escape(sublinetext);

                            // Loop through all the intelliwords and
                            // add to founditems if there is a match
                            foreach (var word in autocompleteWords.Items)
                            {
                                var itemword = word.ItemWord;
                                if (Regex.IsMatch(itemword, escape))
                                {
                                    founditems.Add(itemword);
                                }
                            }

                            // If no items found, overjump this part
                            var foundcount = founditems.Count;
                            if (foundcount != 0)
                            {
                                if (intellisenseActive == false)
                                {
                                    // There is no intellibox yet - let's create one!
                                    this.intellisenseBox = new ScriptListBox(ParseItems(founditems, 0));

                                    // Predefines some variables
                                    var caretpoint = this.scriptBox.CaretPoint;
                                    var point = new Point(caretpoint.X,
                                        caretpoint.Y + MeasureScriptBoxHeight());
                                    var size = new Size(200, (8 + (16 * foundcount)));

                                    // Sets properties for intellibox
                                    this.intellisenseBox.Size = size;
                                    this.intellisenseBox.Location = point;
                                    this.intellisenseBox.KeyDown += OnMenuKey;

                                    // Finally adds it to our textbox
                                    this.scriptBox.Controls.Add(intellisenseBox);
                                    this.intellisenseActive = true;
                                    founditems.Clear();
                                }
                                else
                                {
                                    // Update the intellibox
                                    this.intellisenseBox.RemoveItems();
                                    this.intellisenseBox.Words = ParseWords(founditems, 0);

                                    // Update the position & size
                                    var caretpoint = this.scriptBox.CaretPoint;
                                    var point = new Point(caretpoint.X,
                                        caretpoint.Y + MeasureScriptBoxHeight());
                                    var size = new Size(200, (8 + (16 * foundcount)));

                                    this.intellisenseBox.Size = size;
                                    this.intellisenseBox.Location = point;
                                    founditems.Clear();
                                }
                            }
                            else
                            {
                                // There are no items, so hide intellibox
                                if (this.intellisenseActive == true)
                                {
                                    this.DisposeIntellisense();
                                }
                            }
                        }
                        else
                        {
                            // Hides the intellibox because no words can be displayed
                            if (this.intellisenseActive == true)
                            {
                                this.DisposeIntellisense();
                            }
                        }
                    }
                    else
                    {
                        // No lines, no words, no intellisense =)
                        if (this.intellisenseActive == true)
                        {
                            this.DisposeIntellisense();
                        }
                    }
                }
            }
            finally
            {
                // Unlock update for proper eventlogic
                // and redraw the whole CodeEditor
                LockWindowUpdate(IntPtr.Zero);
                Invalidate(true); Regex.CacheSize = 0x0;
            }
        }

        // As this funtion is used quite often...
        private void DisposeIntellisense()
        {
            this.intellisenseBox.Dispose();
            this.intellisenseActive = false;
        }

        // Gets the height of one char in scriptbox
        private int MeasureScriptBoxHeight()
        {
            return TextRenderer.MeasureText(whitespace, Font).Height;
        }

        // If scrolled, must update all the line-numbers.
        private void OnTextScroll(object obj, EventArgs arg)
        {
            this.Invalidate();
        }

        // We focus the intellibox, as soon as we press the down arrow.
        private void OnKeyDown(object obj, KeyEventArgs arg)
        {
            // Variables (readability) =D
            var data = arg.KeyData;
            var down = Keys.Down;
            var tab = Keys.Tab;
            var txt = ("    ");

            // Checks also for tab key - Why?
            // Because the indent of the normal tab
            // is too big imo, thus reducing it
            if (this.intellisenseActive && data == down)
            {
                this.intellisenseBox.SelectedIndex = 0;
                this.intellisenseBox.Focus();
            }
            else if (data == tab)
            {
                arg.SuppressKeyPress = true;
                this.scriptBox.SelectedText = txt;
            }
        }

        // If the scriptbox receives focus, hide the intellibox.
        private void OnTextFocus(object obj, EventArgs arg)
        {
            if (this.intellisenseActive == true)
            {
                this.DisposeIntellisense();
            }
        }

        // If you click on the scriptbox, hide the intellibox.
        private void OnMouseClick(object obj, EventArgs arg)
        {
            if (this.intellisenseActive == true)
            {
                this.DisposeIntellisense();
            }
        }

        // Fires if a key on the intellibox is pressed
        private void OnMenuKey(object obj, KeyEventArgs arg)
        {
            // Variables (Readability) =D
            var data = arg.KeyData;
            var back = Keys.Back;
            var esc = Keys.Escape;
            var ent = Keys.Enter;
            var ups = Keys.Up;

            // Checks for various stuff and does some useful actions
            if (data == esc || data == back)
            {
                // Simply hide the intellibox
                this.scriptBox.Focus();
            }
            else if (data == ups)
            {
                // Only hide the intellibox, if the FIRST
                // item is selected -> else act normal
                if (this.intellisenseBox.SelectedIndex == 0)
                {
                    this.scriptBox.Focus();
                }
            }
            else if (data == ent)
            {
                // We need some variables again =)
                var item = (((AutoCompleteWord)intellisenseBox.SelectedItem).ItemWord);
                var array = this.scriptBox.Lines;

                // If no lines, create one line
                if (array.Length == 0)
                {
                    scriptBox.SelectionStart = 0;
                    array = new string[1];
                }

                // Get the current line number
                int selection = scriptBox.SelectionStart;
                int linenumber = scriptBox.GetLineFromCharIndex(selection);
                selection += (item.Length - array[linenumber].Length);
                array[linenumber] = item;

                // Now copy all the lines back to the textbox
                this.scriptBox.Lines = array;
                this.scriptBox.SelectionStart = selection;
                this.scriptBox.Focus();
            }
        }

        // Creates a new intellisense struct from found strings
        private Intellisense ParseItems(List<string> found, int index)
        {
            var sense = new Intellisense();

            foreach (string itemword in found)
            {
                var word = new AutoCompleteWord();
                word.ImageIndex = index;
                word.ItemWord = itemword;
                sense.Items.Add(word);
            } return sense;
        }

        // Creates a new list of intelliwords from found strings
        private List<AutoCompleteWord> ParseWords(List<string> found, int index)
        {
            var list = new List<AutoCompleteWord>();

            foreach (string itemword in found)
            {
                var word = new AutoCompleteWord();
                word.ImageIndex = index;
                word.ItemWord = itemword;
                list.Add(word);
            } return list;
        }

        #endregion

        #region Codedrawing

        // Paints the background plus the border of the CodeEditor
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            var rect = new Rectangle(0, 0, base.Width - 1, base.Height - 1);
            var rect2 = new Rectangle(1, 1, base.Width - 2, base.Height - 2);
            var bru = new SolidBrush(codeBackColor);
            var pen = new Pen(codeBorderColor);

            e.Graphics.FillRectangle(bru, rect2);
            e.Graphics.DrawRectangle(pen, rect);

            bru.Dispose();
            pen.Dispose();
        }

        // Paints the linenumbers, their selection & more
        protected override void OnPaint(PaintEventArgs e)
        {
            // Defines needed variables
            int scriptheight = MeasureScriptBoxHeight();
            int visibleindex = 0; var pos = new Point(0, 0);
            int textlength = scriptBox.TextLength;
            int selection = scriptBox.SelectionStart;

            int firstIndex = scriptBox.GetCharIndexFromPosition(pos);
            int firstLine = scriptBox.GetLineFromCharIndex(firstIndex);
            int lastLine = scriptBox.GetLineFromCharIndex(textlength);
            int currLine = scriptBox.GetLineFromCharIndex(selection);

            // Draws the line-selection
            var bdrect = new Rectangle(1, (currLine * scriptheight + 1), 46, scriptheight - 1);
            var inrect = new Rectangle(2, (currLine * scriptheight + 2), 46, scriptheight - 2);

            var pen = new Pen(this.lineBorderColor);
            var bru = new SolidBrush(this.lineBackColor);
            var txt = new SolidBrush(this.scriptBox.ForeColor);

            e.Graphics.FillRectangle(bru, inrect);
            e.Graphics.DrawRectangle(pen, bdrect);

            for (int line = firstLine; line <= lastLine; line++)
            {
                // Variables for each line
                var pnt = new Point(8, visibleindex * scriptheight);
                var numb = line.ToString();

                // Loop until we have e.g "000<line>"
                while (numb.Length < 0x4)
                {
                    numb = numb.Insert(0, "0");
                }

                // Draw the string
                e.Graphics.DrawString(numb, Font, txt, pnt);

                // Increase the visible lineindex
                visibleindex += 1;
            }

            // Clean-Up
            bru.Dispose();
            pen.Dispose();
            txt.Dispose();
        }

        #endregion
    }
}