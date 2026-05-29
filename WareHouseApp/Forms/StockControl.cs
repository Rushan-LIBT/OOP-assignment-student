using System;
using System.Drawing;
using System.Linq;
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
        private const string AllTypes = "All movements";

        private readonly Person currentUser;
        private readonly MaterialRepository materials = new MaterialRepository();
        private readonly StockMovementRepository movements = new StockMovementRepository();

        private ComboBox cmbMaterial;
        private TextBox txtQuantity;
        private DataGridView grid;
        private ComboBox cmbTypeFilter;

        public StockControl(Person user)
        {
            currentUser = user;
            BuildUi();
            LoadMaterials();
            LoadHistory();
        }

        private void BuildUi()
        {
            BackColor = UiTheme.AppBg;

            Controls.Add(BuildTableCard());
            Controls.Add(new Panel { Dock = DockStyle.Top, Height = 16, BackColor = UiTheme.AppBg });
            Controls.Add(BuildFormCard());
        }

        private Panel BuildFormCard()
        {
            var card = UiTheme.CardPanel(DockStyle.Top, 160);

            var title = new Label
            {
                Text = "Record Stock Movement",
                Font = new Font("Segoe UI Semibold", 12F),
                ForeColor = UiTheme.TextDark,
                Location = new Point(24, 18),
                AutoSize = true
            };

            var lblMaterial = new Label
            {
                Text = "MATERIAL",
                ForeColor = UiTheme.Muted,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(24, 58),
                AutoSize = true
            };
            cmbMaterial = new ComboBox
            {
                Location = new Point(24, 80),
                Width = 280,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F),
                DisplayMember = "MaterialName",
                ValueMember = "MaterialID"
            };

            txtQuantity = UiTheme.AddField(card, "QUANTITY", 330, 58, 150, false);

            var btnIn = UiTheme.ActionButton("Stock In", UiTheme.Success, 510, 78, 120, 40);
            btnIn.Click += (s, e) => Record(StockMovementRepository.In);
            var btnOut = UiTheme.ActionButton("Stock Out", UiTheme.Danger, 642, 78, 120, 40);
            btnOut.Click += (s, e) => Record(StockMovementRepository.Out);

            card.Controls.Add(title);
            card.Controls.Add(lblMaterial);
            card.Controls.Add(cmbMaterial);
            card.Controls.Add(btnIn);
            card.Controls.Add(btnOut);
            return card;
        }

        private Panel BuildTableCard()
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Card, Padding = new Padding(16) };

            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            UiTheme.StyleGrid(grid);

            var header = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = UiTheme.Card };
            var headerLabel = new Label
            {
                Text = "Recent Movements",
                Font = new Font("Segoe UI Semibold", 11F),
                ForeColor = UiTheme.TextDark,
                Location = new Point(2, 10),
                AutoSize = true
            };
            var lblFilter = new Label
            {
                Text = "Type",
                ForeColor = UiTheme.Muted,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(348, 12),
                AutoSize = true
            };
            cmbTypeFilter = UiTheme.FilterCombo(420, 6, 200);
            cmbTypeFilter.Items.AddRange(new object[] { AllTypes, "Stock In", "Stock Out" });
            cmbTypeFilter.SelectedIndex = 0;
            cmbTypeFilter.SelectedIndexChanged += (s, e) => LoadHistory();

            header.Controls.Add(headerLabel);
            header.Controls.Add(cmbTypeFilter);
            header.Controls.Add(lblFilter);

            card.Controls.Add(grid);
            card.Controls.Add(header);
            return card;
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
                var list = movements.GetRecent(50);

                string filter = cmbTypeFilter.SelectedItem as string;
                if (filter == "Stock In")
                {
                    list = list.Where(m => m.MovementType == StockMovementRepository.In).ToList();
                }
                else if (filter == "Stock Out")
                {
                    list = list.Where(m => m.MovementType == StockMovementRepository.Out).ToList();
                }

                grid.DataSource = list;
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
