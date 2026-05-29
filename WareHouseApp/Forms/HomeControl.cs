using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WareHouseApp.Data;
using WareHouseApp.Models;
using WareHouseApp.People;
using WareHouseApp.Properties;

namespace WareHouseApp.Forms
{
    /// <summary>Landing screen: summary counts and a low-stock alert list.</summary>
    public class HomeControl : UserControl
    {
        private const int LowStockThreshold = 10;

        private readonly Person currentUser;
        private readonly MaterialRepository materials = new MaterialRepository();
        private readonly CustomerRepository customers = new CustomerRepository();
        private readonly EmployeeRepository employees = new EmployeeRepository();

        private FlowLayoutPanel cards;
        private DataGridView lowStockGrid;

        public HomeControl(Person user)
        {
            currentUser = user;
            BuildUi();
            LoadData();
        }

        private void BuildUi()
        {
            BackColor = UiTheme.AppBg;

            Controls.Add(BuildLowStockCard());
            Controls.Add(new Panel { Dock = DockStyle.Top, Height = 16, BackColor = UiTheme.AppBg });
            Controls.Add(BuildCardsRow());
            Controls.Add(BuildGreeting());
        }

        private Panel BuildGreeting()
        {
            var panel = new Panel { Dock = DockStyle.Top, Height = 54, BackColor = UiTheme.AppBg };
            var greeting = new Label
            {
                Text = currentUser.WelcomeMessage(),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = UiTheme.TextDark,
                Location = new Point(2, 6),
                AutoSize = true
            };
            var sub = new Label
            {
                Text = "Here is what is happening in your warehouse today.",
                Font = new Font("Segoe UI", 10F),
                ForeColor = UiTheme.Muted,
                Location = new Point(4, 36),
                AutoSize = true
            };
            panel.Controls.Add(greeting);
            panel.Controls.Add(sub);
            return panel;
        }

        private FlowLayoutPanel BuildCardsRow()
        {
            cards = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 130,
                BackColor = UiTheme.AppBg,
                WrapContents = false
            };
            return cards;
        }

        private Panel BuildLowStockCard()
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Card, Padding = new Padding(16) };

            var header = new Label
            {
                Text = "Low Stock Alert  (quantity below " + LowStockThreshold + ")",
                Font = new Font("Segoe UI Semibold", 11F),
                ForeColor = UiTheme.Danger,
                Dock = DockStyle.Top,
                Height = 34
            };

            lowStockGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            UiTheme.StyleGrid(lowStockGrid);

            card.Controls.Add(lowStockGrid);
            card.Controls.Add(header);
            return card;
        }

        private void LoadData()
        {
            try
            {
                List<Material> allMaterials = materials.GetAll();

                cards.Controls.Clear();
                cards.Controls.Add(MakeCard("Materials", allMaterials.Count, UiTheme.Accent, Resources.menu));
                cards.Controls.Add(MakeCard("Customers", customers.GetAll().Count, UiTheme.Success, Resources.user));
                cards.Controls.Add(MakeCard("Employees", employees.GetAll().Count, Color.FromArgb(230, 126, 34), Resources.administrator));

                lowStockGrid.DataSource = allMaterials
                    .Where(m => m.Quantity < LowStockThreshold)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel MakeCard(string caption, int value, Color accent, Image icon)
        {
            var panel = new Panel
            {
                Width = 240,
                Height = 110,
                BackColor = UiTheme.Card,
                Margin = new Padding(0, 0, 16, 0)
            };

            var stripe = new Panel { Dock = DockStyle.Left, Width = 5, BackColor = accent };

            var iconBox = new PictureBox
            {
                Image = new Bitmap(icon, new Size(40, 40)),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(40, 40),
                Location = new Point(24, 35),
                BackColor = Color.Transparent
            };

            var lblValue = new Label
            {
                Text = value.ToString(),
                ForeColor = UiTheme.TextDark,
                Font = new Font("Segoe UI", 26F, FontStyle.Bold),
                Location = new Point(86, 22),
                AutoSize = true
            };

            var lblCaption = new Label
            {
                Text = caption,
                ForeColor = UiTheme.Muted,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(90, 72),
                AutoSize = true
            };

            panel.Controls.Add(lblValue);
            panel.Controls.Add(lblCaption);
            panel.Controls.Add(iconBox);
            panel.Controls.Add(stripe);
            return panel;
        }
    }
}
