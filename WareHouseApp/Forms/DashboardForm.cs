using System;
using System.Drawing;
using System.Windows.Forms;
using WareHouseApp.People;

namespace WareHouseApp.Forms
{
    /// <summary>
    /// Main application shell. A sidebar switches the content panel between the
    /// inventory, customer and employee management controls.
    /// </summary>
    public class DashboardForm : Form
    {
        private readonly Person currentUser;

        private Panel sidebar;
        private Panel content;
        private Label lblWelcome;

        public DashboardForm(Person user)
        {
            currentUser = user;
            BuildUi();
            ShowControl(new InventoryControl());
        }

        private void BuildUi()
        {
            Text = "Warehouse Management System";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            ClientSize = new Size(1000, 600);

            sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.RoyalBlue
            };

            var appTitle = new Label
            {
                Text = "WareHouse",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblWelcome = new Label
            {
                Text = currentUser.WelcomeMessage(),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var btnInventory = MakeNavButton("Inventory");
            btnInventory.Click += (s, e) => ShowControl(new InventoryControl());

            var btnCustomers = MakeNavButton("Customers");
            btnCustomers.Click += (s, e) => ShowControl(new CustomerControl());

            var btnEmployees = MakeNavButton("Employees");
            btnEmployees.Enabled = currentUser.CanManageEmployees;
            btnEmployees.Click += (s, e) => ShowControl(new EmployeeControl());

            var btnLogout = MakeNavButton("Logout");
            btnLogout.Dock = DockStyle.Bottom;
            btnLogout.BackColor = Color.Firebrick;
            btnLogout.Click += BtnLogout_Click;

            // Added bottom-to-top because each docks to the top of the remaining space.
            sidebar.Controls.Add(btnEmployees);
            sidebar.Controls.Add(btnCustomers);
            sidebar.Controls.Add(btnInventory);
            sidebar.Controls.Add(lblWelcome);
            sidebar.Controls.Add(appTitle);
            sidebar.Controls.Add(btnLogout);

            content = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(10)
            };

            Controls.Add(content);
            Controls.Add(sidebar);
        }

        private Button MakeNavButton(string text)
        {
            return new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 50,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.RoyalBlue,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Font = new Font("Segoe UI", 10F)
            };
        }

        private void ShowControl(UserControl control)
        {
            control.Dock = DockStyle.Fill;
            content.Controls.Clear();
            content.Controls.Add(control);
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
