// <copyright file="MainForm.cs" company="www.PublicDomain.tech">All rights waived.</copyright>

// Programmed by Victor L. Senior (VLS) <support@publicdomain.tech>, 2016
//
// Web: http://publicdomain.tech
//
// Sources: http://github.com/publicdomaintech/
//
// This software and associated documentation files (the "Software") is
// released under the CC0 Public Domain Dedication, version 1.0, as
// published by Creative Commons. To the extent possible under law, the
// author(s) have dedicated all copyright and related and neighboring
// rights to the Software to the public domain worldwide. The Software is
// distributed WITHOUT ANY WARRANTY.
//
// If you did not receive a copy of the CC0 Public Domain Dedication
// along with the Software, see
// <http://creativecommons.org/publicdomain/zero/1.0/>

/// <summary>
/// History List display module.
/// </summary>
namespace History_32_List
{
    // Directives
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using PdBets;

    /// <summary>
    /// Main form.
    /// </summary>
    [Export(typeof(IPdBets))]
    public partial class MainForm : Form, IPdBets
    {
        /// <summary>
        /// The roulette class instance.
        /// </summary>
        private Roulette roulette = new Roulette();

        /// <summary>
        /// Initializes a new instance of the <see cref="History_32_List.MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            // The InitializeComponent() call is required for Windows Forms designer support.
            this.InitializeComponent();
        }

        /// <summary>
        /// Processes incoming input and bet strings.
        /// </summary>
        /// <param name="inputString">Input string.</param>
        /// <param name="betString">Bet string.</param>
        /// <returns>>The processed input string.</returns>
        public string Input(string inputString, string betString)
        {
            // Check if must undo
            if (inputString == "-U")
            {
                // Remove
                if (this.historyListBox.Items.Count > 0)
                {
                    this.historyListBox.Items.RemoveAt(0);
                }
            }
            else
            {
                // Add number
                this.historyListBox.Items.Insert(0, inputString);
            }

            // Update history count
            this.UpdateHistoryCount();

            // Return bet string "as is"
            return betString;
        }

        /// <summary>
        /// Updates the history count.
        /// </summary>
        private void UpdateHistoryCount()
        {
            // Check history list box items count
            if (this.historyListBox.Items.Count > 0)
            {
                // Set main tool strip status label text to current count 
                this.mainToolStripStatusLabel.Text = this.historyListBox.Items.Count.ToString();
            }
            else
            {
                // Reset to default text
                this.mainToolStripStatusLabel.Text = "Count";
            }
        }

        /// <summary>
        /// Raises the history list box draw item event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnHistoryListBoxDrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                // Set sender's ItemHeight
                ((ListBox)sender).ItemHeight = e.Font.Height + 3;

                // Draw background
                e.DrawBackground();

                // Set brush
                SolidBrush solidBrush = new SolidBrush(this.roulette.GetNumberColor(Convert.ToByte(((ListBox)sender).Items[e.Index])));

                // Set number string
                string numberString = ((ListBox)sender).Items[e.Index].ToString();

                // Resolve double zero display
                if (numberString == "37")
                {
                    // Change it to 00
                    numberString = "00";
                }

                // Draw roulette number string
                e.Graphics.DrawString(numberString, e.Font, solidBrush, e.Bounds, StringFormat.GenericDefault);

                // Draw focus rectangle
                e.DrawFocusRectangle();
            }
            catch (Exception /*ex*/)
            {
                // TODO Log
            }
        }

        /// <summary>
        /// Raises the copy tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnCopyToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Check there's something to work with
            if (this.historyListBox.Items.Count == 0)
            {
                // Halt flow
                return;
            }

            // Clear clipboard
            Clipboard.Clear();

            // History list string
            string historyList = string.Empty;

            // Add items in reversed order
            for (int h = this.historyListBox.Items.Count - 1; h > -1; h--)
            {
                // Add current item
                historyList += this.historyListBox.Items[h] + Environment.NewLine;
            }

            // Trim last newline element
            historyList.TrimEnd();

            // Copy to clipboard
            Clipboard.SetText(historyList);

            // Advice user
            MessageBox.Show("History copied to clipboard.", "Copy", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Raises the save tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSaveToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Check there's something to work with
            if (this.historyListBox.Items.Count == 0)
            {
                // Halt flow
                return;
            }

            // Declare save file dialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            // Set filter
            saveFileDialog.Filter = "Text File | *.txt";

            // TODO try-catch to handle exceptions

            // Check a valid file was entered
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            { 
                // Write via StreamWriter
                using (StreamWriter writer = new StreamWriter(saveFileDialog.OpenFile()))
                { 
                    // Add items in reversed order
                    for (int h = this.historyListBox.Items.Count - 1; h > -1; h--)
                    { 
                        // Write current line
                        writer.WriteLine(this.historyListBox.Items[h]);
                    } 
                } 
            }

            // Advice user
            MessageBox.Show("History saved to file:" + Environment.NewLine + saveFileDialog.FileName, "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
