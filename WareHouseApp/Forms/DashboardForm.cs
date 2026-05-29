using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WareHouseApp.People;
using WareHouseApp.Properties;

namespace WareHouseApp.Forms
{
    /// <summary>
    /// Main application shell: a branded sidebar, a header bar showing the
    /// current page and signed-in user, and a content area that hosts each
    /// management screen.
    /// </summary>
    public class DashboardForm : Form
    {
        private readonly Person currentUser;
        private readonly List<Panel> navItems = new List<Panel>();

        private FlowLayoutPanel navFlow;
        private Panel contentHost;
        private Label lblPageTitle;

        public DashboardForm(Person user)
        {
            currentUser = user;
            BuildUi();
        }

        private void BuildUi()
        {
            Text = "Warehouse Management System";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            ClientSize = new Size(1100, 650);
            BackColor = UiTheme.AppBg;
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(960, 600);

            Controls.Add(BuildRightContainer());
            Controls.Add(BuildSidebar());

            // Select the first nav item (Home) once everything is wired up.
            if (navItems.Count > 0)
            {
                ((Button)FindButton(navItems[0])).PerformClick();
            }
        }

        private Panel BuildSidebar()
        {
            var sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 240,
                BackColor = UiTheme.DarkPanel
            };

            var brand = new Panel { Dock = DockStyle.Top, Height = 90, BackColor = UiTheme.DarkPanel };
            var logo = new PictureBox
            {
                Image = new Bitmap(Resources.wholesale, new Size(34, 34)),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(34, 34),
                Location = new Point(22, 28),
                BackColor = Color.Transparent
            };
            var brandText = new Label
            {
                Text = "WareHouse",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                Location = new Point(64, 30),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            brand.Controls.Add(brandText);
            brand.Controls.Add(logo);

            navFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = false,
                BackColor = UiTheme.DarkPanel,
                Padding = new Padding(0, 10, 0, 0)
            };

            AddNavItem("Dashboard", Resources.reliability, () => Navigate("Dashboard", new HomeControl(currentUser)));
            AddNavItem("Inventory", Resources.menu, () => Navigate("Inventory Management", new InventoryControl()));
            AddNavItem("Stock In / Out", Resources.wholesale, () => Navigate("Stock In / Out", new StockControl(currentUser)));
            AddNavItem("Customers", Resources.user, () => Navigate("Customer Management", new CustomerControl()));
            if (currentUser.CanManageEmployees)
            {
                AddNavItem("Employees", Resources.administrator, () => Navigate("Employee Management", new EmployeeControl()));
            }
            AddNavItem("Profile", Resources.user, () => Navigate("My Profile", new ProfileControl(currentUser)));

            var logoutArea = new Panel { Dock = DockStyle.Bottom, Height = 64, BackColor = UiTheme.DarkPanel };
            var btnLogout = new Button
            {
                Text = "   Logout",
                Image = new Bitmap(Resources.logout, new Size(20, 20)),
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                BackColor = UiTheme.Danger,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 10F),
                Padding = new Padding(24, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += BtnLogout_Click;
            logoutArea.Controls.Add(btnLogout);

            sidebar.Controls.Add(navFlow);
            sidebar.Controls.Add(brand);
            sidebar.Controls.Add(logoutArea);
            return sidebar;
        }

        private void AddNavItem(string text, Image icon, Action onClick)
        {
            var item = new Panel
            {
                Width = 240,
                Height = 48,
                Margin = new Padding(0),
                BackColor = UiTheme.DarkPanel
            };

            var stripe = new Panel
            {
                Dock = DockStyle.Left,
                Width = 4,
                BackColor = UiTheme.Accent,
                Visible = false
            };

            var button = new Button
            {
                Text = "    " + text,
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                BackColor = UiTheme.DarkPanel,
                ForeColor = Color.FromArgb(206, 212, 230),
                Font = new Font("Segoe UI", 10.5F),
                TextAlign = ContentAlignment.MiddleLeft,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                Padding = new Padding(18, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = UiTheme.SidebarActive;
            if (icon != null)
            {
                button.Image = new Bitmap(icon, new Size(22, 22));
            }

            item.Tag = stripe;
            button.Click += (s, e) =>
            {
                SetActive(item);
                onClick();
            };

            item.Controls.Add(button);
            item.Controls.Add(stripe);
            navFlow.Controls.Add(item);
            navItems.Add(item);
        }

        private void SetActive(Panel activeItem)
        {
            foreach (Panel item in navItems)
            {
                bool active = item == activeItem;
                Color back = active ? UiTheme.SidebarActive : UiTheme.DarkPanel;
                item.BackColor = back;
                ((Panel)item.Tag).Visible = active;
                Button button = FindButton(item);
                button.BackColor = back;
                button.ForeColor = active ? Color.White : Color.FromArgb(206, 212, 230);
            }
        }

        private static Button FindButton(Panel item)
        {
            foreach (Control control in item.Controls)
            {
                if (control is Button button)
                {
                    return button;
                }
            }
            return null;
        }

        private Panel BuildRightContainer()
        {
            var rightContainer = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.AppBg };

            contentHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = UiTheme.AppBg,
                Padding = new Padding(20)
            };

            var header = new Panel { Dock = DockStyle.Top, Height = 66, BackColor = Color.White };

            lblPageTitle = new Label
            {
                Text = "Dashboard",
                ForeColor = UiTheme.TextDark,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                Location = new Point(24, 18),
                AutoSize = true
            };

            var userChip = new Panel { Dock = DockStyle.Right, Width = 260, BackColor = Color.White };
            var avatar = new PictureBox
            {
                Image = new Bitmap(Resources.user, new Size(38, 38)),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(38, 38),
                Location = new Point(14, 14),
                BackColor = Color.Transparent
            };
            var lblName = new Label
            {
                Text = string.IsNullOrEmpty(currentUser.FullName) ? currentUser.UserName : currentUser.FullName,
                ForeColor = UiTheme.TextDark,
                Font = new Font("Segoe UI Semibold", 10F),
                Location = new Point(62, 16),
                AutoSize = true
            };
            var lblRole = new Label
            {
                Text = currentUser.Role,
                ForeColor = UiTheme.Muted,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(62, 36),
                AutoSize = true
            };
            userChip.Controls.Add(avatar);
            userChip.Controls.Add(lblName);
            userChip.Controls.Add(lblRole);

            var divider = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = UiTheme.Line };

            header.Controls.Add(lblPageTitle);
            header.Controls.Add(userChip);
            header.Controls.Add(divider);

            rightContainer.Controls.Add(contentHost);
            rightContainer.Controls.Add(header);
            return rightContainer;
        }

        private void Navigate(string title, UserControl control)
        {
            lblPageTitle.Text = title;
            control.Dock = DockStyle.Fill;
            contentHost.Controls.Clear();
            contentHost.Controls.Add(control);
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Log out?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                Close();
            }
        }
    }
}
