using System.Drawing;
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
                Width = width,
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
            return box;
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
