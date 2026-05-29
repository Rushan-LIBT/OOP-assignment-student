using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WareHouseApp.Data;
using WareHouseApp.Models;
using WareHouseApp.People;

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
            BackColor = Color.WhiteSmoke;

            var title = new Label
            {
                Text = currentUser.WelcomeMessage(),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 50
            };

            cards = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 130,
                Padding = new Padding(0, 10, 0, 10)
            };

            var lowStockTitle = new Label
            {
                Text = "Low stock (quantity below " + LowStockThreshold + ")",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 30
            };

            lowStockGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };

            Controls.Add(lowStockGrid);
            Controls.Add(lowStockTitle);
            Controls.Add(cards);
            Controls.Add(title);
        }

        private void LoadData()
        {
            try
            {
                List<Material> allMaterials = materials.GetAll();

                cards.Controls.Clear();
                cards.Controls.Add(MakeCard("Materials", allMaterials.Count, Color.RoyalBlue));
                cards.Controls.Add(MakeCard("Customers", customers.GetAll().Count, Color.SeaGreen));
                cards.Controls.Add(MakeCard("Employees", employees.GetAll().Count, Color.DarkOrange));

                lowStockGrid.DataSource = allMaterials
                    .Where(m => m.Quantity < LowStockThreshold)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel MakeCard(string caption, int value, Color color)
        {
            var panel = new Panel
            {
                Width = 180,
                Height = 100,
                BackColor = color,
                Margin = new Padding(10)
            };
            var lblValue = new Label
            {
                Text = value.ToString(),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            var lblCaption = new Label
            {
                Text = caption,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F),
                Dock = DockStyle.Bottom,
                Height = 28,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panel.Controls.Add(lblValue);
            panel.Controls.Add(lblCaption);
            return panel;
        }
    }
}
