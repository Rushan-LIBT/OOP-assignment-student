using System;
using System.Drawing;
using System.Windows.Forms;
using WareHouseApp.Data;
using WareHouseApp.Exceptions;
using WareHouseApp.Models;
using WareHouseApp.People;

namespace WareHouseApp.Forms
{
    /// <summary>Stock in / stock out screen with a movement history.</summary>
    public class StockControl : UserControl
    {
        private readonly Person currentUser;
        private readonly MaterialRepository materials = new MaterialRepository();
        private readonly StockMovementRepository movements = new StockMovementRepository();

        private ComboBox cmbMaterial;
        private TextBox txtQuantity;
        private DataGridView grid;

        public StockControl(Person user)
        {
            currentUser = user;
            BuildUi();
            LoadMaterials();
            LoadHistory();
        }

        private void BuildUi()
        {
            BackColor = Color.WhiteSmoke;

            var title = new Label
            {
                Text = "Stock In / Out",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 40
            };

            var panel = new Panel { Dock = DockStyle.Top, Height = 100 };

            var lblMaterial = new Label { Text = "Material", Location = new Point(0, 10), AutoSize = true };
            cmbMaterial = new ComboBox
            {
                Location = new Point(0, 32),
                Width = 260,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "MaterialName",
                ValueMember = "MaterialID"
            };

            var lblQty = new Label { Text = "Quantity", Location = new Point(290, 10), AutoSize = true };
            txtQuantity = new TextBox { Location = new Point(290, 32), Width = 120 };

            var btnIn = new Button
            {
                Text = "Stock In",
                Location = new Point(440, 30),
                Width = 110,
                Height = 30,
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnIn.Click += (s, e) => Record(StockMovementRepository.In);

            var btnOut = new Button
            {
                Text = "Stock Out",
                Location = new Point(560, 30),
                Width = 110,
                Height = 30,
                BackColor = Color.Firebrick,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOut.Click += (s, e) => Record(StockMovementRepository.Out);

            panel.Controls.Add(lblMaterial);
            panel.Controls.Add(cmbMaterial);
            panel.Controls.Add(lblQty);
            panel.Controls.Add(txtQuantity);
            panel.Controls.Add(btnIn);
            panel.Controls.Add(btnOut);

            var historyTitle = new Label
            {
                Text = "Recent movements",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 28
            };

            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };

            Controls.Add(grid);
            Controls.Add(historyTitle);
            Controls.Add(panel);
            Controls.Add(title);
        }

        private void LoadMaterials()
        {
            try
            {
                cmbMaterial.DataSource = materials.GetAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadHistory()
        {
            try
            {
                grid.DataSource = movements.GetRecent(50);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Record(string type)
        {
            try
            {
                if (!(cmbMaterial.SelectedItem is Material material))
                {
                    throw new ValidationException("Please select a material first.");
                }
                if (!int.TryParse(txtQuantity.Text, out int quantity))
                {
                    throw new ValidationException("Quantity must be a whole number.");
                }

                movements.RecordMovement(material.MaterialID, type, quantity, currentUser.UserName);

                txtQuantity.Clear();
                LoadMaterials();
                LoadHistory();
            }
            catch (ValidationException ex)
            {
                MessageBox.Show(ex.Message, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
