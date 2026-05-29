using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace WareHouseApp.Forms
{
    /// <summary>
    /// Shared colours and control builders so the login and sign up screens
    /// have a consistent, modern look.
    /// </summary>
    internal static class UiTheme
    {
        public static readonly Color DarkPanel = Color.FromArgb(33, 41, 70);
        public static readonly Color DarkPanelAccent = Color.FromArgb(48, 60, 102);
        public static readonly Color Accent = Color.FromArgb(79, 124, 255);
        public static readonly Color AccentHover = Color.FromArgb(99, 141, 255);
        public static readonly Color TextDark = Color.FromArgb(40, 44, 56);
        public static readonly Color Muted = Color.FromArgb(130, 137, 150);
        public static readonly Color Line = Color.FromArgb(214, 219, 230);

        public static readonly Color AppBg = Color.FromArgb(243, 245, 249);
        public static readonly Color Card = Color.White;
        public static readonly Color SidebarActive = Color.FromArgb(48, 60, 102);
        public static readonly Color Success = Color.FromArgb(39, 174, 96);
        public static readonly Color Danger = Color.FromArgb(231, 76, 60);
        public static readonly Color Neutral = Color.FromArgb(127, 140, 141);

        /// <summary>A flat, coloured action button used inside the management screens.</summary>
        public static Button ActionButton(string text, Color color, int x, int y, int width = 110, int height = 38)
        {
            var button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9.5F),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            return button;
        }

        /// <summary>A bordered, single-line search/input box that highlights on focus.</summary>
        public static TextBox BoxedInput(int x, int y, int width, out Panel container, int height = 34)
        {
            container = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8, 6, 8, 0)
            };
            var box = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11F)
            };
            container.Controls.Add(box);
            return box;
        }

        /// <summary>A flat dropdown used for column filters above the grids.</summary>
        public static ComboBox FilterCombo(int x, int y, int width)
        {
            return new ComboBox
            {
                Location = new Point(x, y),
                Width = width,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F)
            };
        }

        /// <summary>A white "card" surface used to group content over the grey background.</summary>
        public static Panel CardPanel(DockStyle dock, int height = 0)
        {
            var panel = new Panel
            {
                Dock = dock,
                BackColor = Card
            };
            if (dock == DockStyle.Top && height > 0)
            {
                panel.Height = height;
            }
            return panel;
        }

        /// <summary>Applies a clean, modern style to a data grid.</summary>
        public static void StyleGrid(DataGridView grid)
        {
            grid.BorderStyle = BorderStyle.None;
            grid.BackgroundColor = Color.White;
            grid.EnableHeadersVisualStyles = false;
            grid.RowHeadersVisible = false;
            grid.AllowUserToResizeRows = false;
            grid.GridColor = Line;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 42;

            grid.ColumnHeadersDefaultCellStyle.BackColor = DarkPanel;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = DarkPanel;

            grid.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            grid.DefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 232, 255);
            grid.DefaultCellStyle.SelectionForeColor = TextDark;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 249, 252);
            grid.RowTemplate.Height = 36;
        }

        /// <summary>Adds a labelled text field with an underline that highlights on focus.</summary>
        public static TextBox AddField(Control parent, string label, int x, int y, int width, bool isPassword)
        {
            var caption = new Label
            {
                Text = label,
                ForeColor = Muted,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(x, y),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var box = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 12F),
                Location = new Point(x, y + 22),
                Width = isPassword ? width - 28 : width,
                UseSystemPasswordChar = isPassword
            };

            var underline = new Panel
            {
                BackColor = Line,
                Location = new Point(x, y + 50),
                Size = new Size(width, 2)
            };

            box.Enter += (s, e) => underline.BackColor = Accent;
            box.Leave += (s, e) => underline.BackColor = Line;

            parent.Controls.Add(caption);
            parent.Controls.Add(box);
            parent.Controls.Add(underline);

            if (isPassword)
            {
                AddPasswordToggle(parent, box, x + width - 24, y + 20);
            }
            return box;
        }

        /// <summary>Adds a clickable eye icon that shows/hides the password text.</summary>
        private static void AddPasswordToggle(Control parent, TextBox box, int x, int y)
        {
            bool visible = false;
            var eye = new Panel
            {
                Size = new Size(24, 24),
                Location = new Point(x, y),
                BackColor = parent.BackColor,
                Cursor = Cursors.Hand
            };

            eye.Paint += (s, e) => PaintEye(e.Graphics, eye.ClientRectangle, visible, visible ? Accent : Muted);
            eye.Click += (s, e) =>
            {
                visible = !visible;
                box.UseSystemPasswordChar = !visible;
                eye.Invalidate();
            };

            parent.Controls.Add(eye);
            eye.BringToFront();
        }

        private static void PaintEye(Graphics g, Rectangle bounds, bool open, Color color)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (var pen = new Pen(color, 1.6f))
            using (var brush = new SolidBrush(color))
            {
                int midY = bounds.Y + bounds.Height / 2;
                var eyeRect = new Rectangle(bounds.X + 2, midY - 6, bounds.Width - 4, 12);

                // Almond shaped outline using two arcs.
                g.DrawArc(pen, eyeRect, 200, 140);
                g.DrawArc(pen, eyeRect, 20, 140);

                // Pupil.
                const int pupil = 4;
                g.FillEllipse(brush, bounds.X + bounds.Width / 2 - pupil / 2, midY - pupil / 2, pupil, pupil);

                // Slash when the password is hidden.
                if (!open)
                {
                    g.DrawLine(pen, bounds.X + 4, bounds.Y + 4, bounds.Right - 4, bounds.Bottom - 4);
                }
            }
        }

        /// <summary>Creates a flat, full-width accent button.</summary>
        public static Button PrimaryButton(string text, int x, int y, int width)
        {
            var button = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(width, 46),
                BackColor = Accent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 11F),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = AccentHover;
            return button;
        }

        /// <summary>Builds the dark branding panel shown on the left of the auth screens.</summary>
        public static Panel BrandPanel(int width, int height, Image logo, string tagline)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Left,
                Width = width,
                BackColor = DarkPanel
            };

            var stripe = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 6,
                BackColor = Accent
            };

            if (logo != null)
            {
                var picture = new PictureBox
                {
                    Image = logo,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Size = new Size(110, 110),
                    Location = new Point((width - 110) / 2, 90),
                    BackColor = Color.Transparent
                };
                panel.Controls.Add(picture);
            }

            var title = new Label
            {
                Text = "WareHouse",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 215),
                Size = new Size(width, 40),
                BackColor = Color.Transparent
            };

            var subtitle = new Label
            {
                Text = "Management System",
                ForeColor = Color.FromArgb(165, 175, 205),
                Font = new Font("Segoe UI", 11F),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 255),
                Size = new Size(width, 24),
                BackColor = Color.Transparent
            };

            var caption = new Label
            {
                Text = tagline,
                ForeColor = Color.FromArgb(140, 150, 180),
                Font = new Font("Segoe UI", 9.5F),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 300),
                Size = new Size(width - 40, 40),
                BackColor = Color.Transparent
            };

            panel.Controls.Add(title);
            panel.Controls.Add(subtitle);
            panel.Controls.Add(caption);
            panel.Controls.Add(stripe);
            return panel;
        }
    }
}
