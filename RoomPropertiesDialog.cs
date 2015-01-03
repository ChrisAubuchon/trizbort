/*
    Copyright (c) 2010 by Genstein

    This file is (or was originally) part of Trizbort, the Interactive Fiction Mapper.

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Trizbort
{
    internal partial class RoomPropertiesDialog : Form
    {
        private const int HORIZONTAL_MARGIN = 2;
        private const int VERTICAL_MARGIN = 2;
        private const int WIDTH = 24;
        private static Tab m_lastViewedTab = Tab.Objects;
        private bool m_adjustingPosition;

        public RoomPropertiesDialog()
        {
            InitializeComponent();

            // load regions control
            cboRegion.Items.Clear();
            foreach (var region in Settings.Regions)
            {
                cboRegion.Items.Add(region.RegionName);
            }

            cboRegion.DrawMode = DrawMode.OwnerDrawFixed;
            cboRegion.DrawItem += RegionListBox_DrawItem;
            if (Settings.Regions.Count > 0)
                cboRegion.SelectedIndex = 0;

            switch (m_lastViewedTab)
            {
                case Tab.Objects:
                    m_tabControl.SelectedIndex = 0;
                    break;
                case Tab.Description:
                    m_tabControl.SelectedIndex = 1;
                    break;
                case Tab.Colors:
                    m_tabControl.SelectedIndex = 2;
                    break;
                case Tab.Regions:
                    m_tabControl.SelectedIndex = 3;
                    break;
            }
        }

        public string RoomName
        {
            get { return m_nameTextBox.Text; }
            set { m_nameTextBox.Text = value; }
        }

        public string Description
        {
            get { return m_descriptionTextBox.Text; }
            set { m_descriptionTextBox.Text = value; }
        }

        public bool IsDark
        {
            get { return m_isDarkCheckBox.Checked; }
            set { m_isDarkCheckBox.Checked = value; }
        }

        public string Objects
        {
            get { return m_objectsTextBox.Text; }
            set { m_objectsTextBox.Text = value; }
        }

        public string RoomRegion
        {
            get
            {
                return cboRegion.SelectedItem != null ? cboRegion.SelectedItem.ToString() : string.Empty;
            }
            set { cboRegion.SelectedItem = value; }
        }

        public CompassPoint ObjectsPosition
        {
            get
            {
                if (m_nCheckBox.Checked) return CompassPoint.North;
                if (m_sCheckBox.Checked) return CompassPoint.South;
                if (m_eCheckBox.Checked) return CompassPoint.East;
                if (m_wCheckBox.Checked) return CompassPoint.West;
                if (m_neCheckBox.Checked) return CompassPoint.NorthEast;
                if (m_nwCheckBox.Checked) return CompassPoint.NorthWest;
                if (m_seCheckBox.Checked) return CompassPoint.SouthEast;
                if (m_swCheckBox.Checked) return CompassPoint.SouthWest;
                return CompassPoint.WestSouthWest;
            }
            set
            {
                switch (value)
                {
                    case CompassPoint.North:
                        m_nCheckBox.Checked = true;
                        break;
                    case CompassPoint.South:
                        m_sCheckBox.Checked = true;
                        break;
                    case CompassPoint.East:
                        m_eCheckBox.Checked = true;
                        break;
                    case CompassPoint.West:
                        m_wCheckBox.Checked = true;
                        break;
                    case CompassPoint.NorthEast:
                        m_neCheckBox.Checked = true;
                        break;
                    case CompassPoint.NorthWest:
                        m_nwCheckBox.Checked = true;
                        break;
                    case CompassPoint.SouthEast:
                        m_seCheckBox.Checked = true;
                        break;
                    case CompassPoint.SouthWest:
                        m_swCheckBox.Checked = true;
                        break;
                    default:
                        m_cCheckBox.Checked = true;
                        break;
                }
            }
        }

        // Added for Room specific colors
        public Color RoomFillColor
        {
            get { return m_roomFillTextBox.BackColor; }
            set { m_roomFillTextBox.BackColor = value; }
        }

        // Added for Room specific colors
        public Color SecondFillColor
        {
            get { return m_secondFillTextBox.BackColor; }
            set { m_secondFillTextBox.BackColor = value; }
        }

        // Added for Room specific colors
        public String SecondFillLocation
        {
            get
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        return "Bottom";
                    case 1:
                        return "BottomRight";
                    case 2:
                        return "Right";
                    case 3:
                        return "TopRight";
                    default:
                        return "Bottom";
                }
            }
            set
            {
                switch (value)
                {
                    case "Bottom":
                        comboBox1.SelectedIndex = 0;
                        break;
                    case "BottomRight":
                        comboBox1.SelectedIndex = 1;
                        break;
                    case "Right":
                        comboBox1.SelectedIndex = 2;
                        break;
                    case "TopRight":
                        comboBox1.SelectedIndex = 3;
                        break;
                    default:
                        comboBox1.SelectedIndex = 0;
                        break;
                }
            }
        }

        // Added for Room specific colors
        public Color RoomBorderColor
        {
            get { return m_roomBorderTextBox.BackColor; }
            set { m_roomBorderTextBox.BackColor = value; }
        }

        // Added for Room specific colors
        public Color RoomTextColor
        {
            get { return m_roomTextTextBox.BackColor; }
            set { m_roomTextTextBox.BackColor = value; }
        }

        // Added for Room specific colors
        public Color ObjectTextColor
        {
            get { return m_objectTextTextBox.BackColor; }
            set { m_objectTextTextBox.BackColor = value; }
        }

        private void RegionListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            using (var palette = new Palette())
            {
                e.DrawBackground();

                var colorBounds = new Rectangle(e.Bounds.Left + HORIZONTAL_MARGIN, e.Bounds.Top + VERTICAL_MARGIN, WIDTH, e.Bounds.Height - VERTICAL_MARGIN*2);
                var textBounds = new Rectangle(colorBounds.Right + HORIZONTAL_MARGIN, e.Bounds.Top, e.Bounds.Width - colorBounds.Width - HORIZONTAL_MARGIN*2, e.Bounds.Height);
                e.Graphics.FillRectangle(palette.Brush(Settings.Regions.FirstOrDefault(p => p.RegionName == cboRegion.Items[e.Index].ToString()).RColor), colorBounds);
                e.Graphics.DrawRectangle(palette.Pen(e.ForeColor, 0), colorBounds);
                e.Graphics.DrawString(cboRegion.Items[e.Index].ToString(), e.Font, palette.Brush(e.ForeColor), textBounds, StringFormats.Left);
            }
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (m_tabControl.SelectedIndex)
            {
                default:
                    m_lastViewedTab = Tab.Objects;
                    break;
                case 1:
                    m_lastViewedTab = Tab.Description;
                    break;
                case 2:
                    m_lastViewedTab = Tab.Colors;
                    break;
                case 3:
                    m_lastViewedTab = Tab.Regions;
                    break;
            }
        }

        private void PositionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (m_adjustingPosition)
                return;

            m_adjustingPosition = true;
            try
            {
                var checkBox = (CheckBox) sender;
                if (checkBox.Checked)
                {
                    foreach (Control other in checkBox.Parent.Controls)
                    {
                        var box = other as CheckBox;
                        if (box != null && other != checkBox)
                        {
                            box.Checked = false;
                        }
                    }
                }
                else
                {
                    m_sCheckBox.Checked = true;
                }
            }
            finally
            {
                m_adjustingPosition = false;
            }
        }

        private void m_roomFillTextBox_TextChanged(object sender, EventArgs e)
        {
        }

        // Added for Room specific colors
        private void m_changeLargeFontButton_Click(object sender, EventArgs e)
        {
            RoomFillColor = showColorDialog(RoomFillColor);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        // Added for Room specific colors
        private void m_changeSecondFillButton_Click(object sender, EventArgs e)
        {
            SecondFillColor = showColorDialog(SecondFillColor);
        }

        // Added for Room specific colors
        private void button1_Click(object sender, EventArgs e)
        {
            RoomBorderColor = showColorDialog(RoomBorderColor);
        }

        // Added for Room specific colors
        private void button2_Click(object sender, EventArgs e)
        {
            RoomTextColor = showColorDialog(RoomTextColor);
        }

        // Added for Room specific colors
        private void button3_Click(object sender, EventArgs e)
        {
            ObjectTextColor = showColorDialog(ObjectTextColor);
        }

        // Added for Room specific colors
        private Color showColorDialog(Color color)
        {
            using (var dialog = new ColorDialog())
            {
                dialog.Color = color;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    return dialog.Color;
                }
            }
            return color;
        }

        private void label5_Click(object sender, EventArgs e)
        {
        }

        private enum Tab
        {
            Objects,
            Description,
            Colors,
            Regions
        }
    }
}