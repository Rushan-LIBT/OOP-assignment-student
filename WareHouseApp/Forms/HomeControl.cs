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
    /// <summary>Landing screen: summary counts and a filterable inventory overview.</summary>
    public class HomeControl : UserControl
    {
        private const string AllCategories = "All categories";

        private readonly Person currentUser;
        private readonly MaterialRepository materials = new MaterialRepository();
        private readonly CustomerRepository customers = new CustomerRepository();
        private readonly EmployeeRepository employees = new EmployeeRepository();

        private bool refreshingFilter;
        private FlowLayoutPanel cards;
        private DataGridView grid;
        private ComboBox cmbCategoryFilter;

        public HomeControl(Person user)
        {
            currentUser = user;
            BuildUi();
            LoadData();
        }

        private void BuildUi()
        {
            BackColor = UiTheme.AppBg;

            Controls.Add(BuildInventoryCard());
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

        private Panel BuildInventoryCard()
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Card, Padding = new Padding(16) };

            var header = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = UiTheme.Card };
            var headerLabel = new Label
            {
                Text = "Inventory Overview",
                Font = new Font("Segoe UI Semibold", 11F),
                ForeColor = UiTheme.TextDark,
                Location = new Point(2, 10),
                AutoSize = true
            };
            var lblFilter = new Label
            {
                Text = "Category",
                ForeColor = UiTheme.Muted,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(348, 12),
                AutoSize = true
            };
            cmbCategoryFilter = UiTheme.FilterCombo(420, 6, 200);
            cmbCategoryFilter.SelectedIndexChanged += (s, e) =>
            {
                if (!refreshingFilter)
                {
                    BindGrid();
                }
            };

            header.Controls.Add(headerLabel);
            header.Controls.Add(cmbCategoryFilter);
            header.Controls.Add(lblFilter);

            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            UiTheme.StyleGrid(grid);

            card.Controls.Add(grid);
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

                RefreshCategoryFilter(allMaterials);
                BindGrid(allMaterials);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BindGrid(List<Material> source = null)
        {
            try
            {
                List<Material> list = source ?? materials.GetAll();

                string category = cmbCategoryFilter.SelectedItem as string;
                if (!string.IsNullOrEmpty(category) && category != AllCategories)
                {
                    list = list.Where(m => (m.Category ?? string.Empty) == category).ToList();
                }

                grid.DataSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshCategoryFilter(List<Material> source)
        {
            try
            {
                refreshingFilter = true;
                string current = cmbCategoryFilter.SelectedItem as string;

                cmbCategoryFilter.Items.Clear();
                cmbCategoryFilter.Items.Add(AllCategories);
                foreach (string category in source
                    .Select(m => m.Category)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct()
                    .OrderBy(c => c))
                {
                    cmbCategoryFilter.Items.Add(category);
                }

                cmbCategoryFilter.SelectedItem = current != null && cmbCategoryFilter.Items.Contains(current)
                    ? current
                    : AllCategories;
            }
            finally
            {
                refreshingFilter = false;
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
