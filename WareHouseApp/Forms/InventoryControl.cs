using System;
using System.Drawing;
using System.Windows.Forms;
using WareHouseApp.Data;
using WareHouseApp.Exceptions;
using WareHouseApp.Models;

namespace WareHouseApp.Forms
{
    /// <summary>Inventory management: list, search and CRUD for materials.</summary>
    public class InventoryControl : UserControl
    {
        private readonly MaterialRepository repository = new MaterialRepository();
        private int selectedId;

        private DataGridView grid;
        private TextBox txtName;
        private TextBox txtCategory;
        private TextBox txtQuantity;
        private TextBox txtPrice;
        private TextBox txtSearch;

        public InventoryControl()
        {
            BuildUi();
            LoadData();
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
            var card = UiTheme.CardPanel(DockStyle.Top, 210);

            var title = new Label
            {
                Text = "Add / Edit Material",
                Font = new Font("Segoe UI Semibold", 12F),
                ForeColor = UiTheme.TextDark,
                Location = new Point(24, 18),
                AutoSize = true
            };
            card.Controls.Add(title);

            int y = 58;
            txtName = UiTheme.AddField(card, "NAME", 24, y, 190, false);
            txtCategory = UiTheme.AddField(card, "CATEGORY", 238, y, 190, false);
            txtQuantity = UiTheme.AddField(card, "QUANTITY", 452, y, 150, false);
            txtPrice = UiTheme.AddField(card, "UNIT PRICE", 626, y, 150, false);

            var btnAdd = UiTheme.ActionButton("Add", UiTheme.Success, 24, 150);
            btnAdd.Click += (s, e) => Save(false);
            var btnUpdate = UiTheme.ActionButton("Update", UiTheme.Accent, 144, 150);
            btnUpdate.Click += (s, e) => Save(true);
            var btnDelete = UiTheme.ActionButton("Delete", UiTheme.Danger, 264, 150);
            btnDelete.Click += (s, e) => DeleteSelected();
            var btnClear = UiTheme.ActionButton("Clear", UiTheme.Neutral, 384, 150);
            btnClear.Click += (s, e) => ClearForm();

            card.Controls.Add(btnAdd);
            card.Controls.Add(btnUpdate);
            card.Controls.Add(btnDelete);
            card.Controls.Add(btnClear);
            return card;
        }

        private Panel BuildTableCard()
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.Card, Padding = new Padding(16) };

            var searchRow = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = UiTheme.Card };
            var lblSearch = new Label
            {
                Text = "Search",
                ForeColor = UiTheme.Muted,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(2, 16),
                AutoSize = true
            };
            txtSearch = UiTheme.BoxedInput(60, 8, 300, out Panel searchBox);
            txtSearch.TextChanged += (s, e) => LoadData();
            searchRow.Controls.Add(searchBox);
            searchRow.Controls.Add(lblSearch);

            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            UiTheme.StyleGrid(grid);
            grid.SelectionChanged += Grid_SelectionChanged;

            card.Controls.Add(grid);
            card.Controls.Add(searchRow);
            return card;
        }

        private void LoadData()
        {
            try
            {
                grid.DataSource = repository.Search(txtSearch.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null || !(grid.CurrentRow.DataBoundItem is Material material))
            {
                return;
            }

            selectedId = material.MaterialID;
            txtName.Text = material.MaterialName;
            txtCategory.Text = material.Category;
            txtQuantity.Text = material.Quantity.ToString();
            txtPrice.Text = material.UnitPrice.ToString();
        }

        private void Save(bool isUpdate)
        {
            try
            {
                var material = new Material
                {
                    MaterialID = selectedId,
                    MaterialName = txtName.Text.Trim(),
                    Category = txtCategory.Text.Trim(),
                    Quantity = ParseInt(txtQuantity.Text, "Quantity"),
                    UnitPrice = ParseDecimal(txtPrice.Text, "Unit price")
                };

                if (isUpdate)
                {
                    if (selectedId == 0)
                    {
                        throw new ValidationException("Select a row to update first.");
                    }
                    repository.Update(material);
                }
                else
                {
                    repository.Add(material);
                }

                ClearForm();
                LoadData();
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

        private void DeleteSelected()
        {
            if (selectedId == 0)
            {
                MessageBox.Show("Select a row to delete first.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("Delete this material?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                repository.Delete(selectedId);
                ClearForm();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            selectedId = 0;
            txtName.Clear();
            txtCategory.Clear();
            txtQuantity.Clear();
            txtPrice.Clear();
        }

        private static int ParseInt(string value, string field)
        {
            if (!int.TryParse(value, out int result))
            {
                throw new ValidationException(field + " must be a whole number.");
            }
            return result;
        }

        private static decimal ParseDecimal(string value, string field)
        {
            if (!decimal.TryParse(value, out decimal result))
            {
                throw new ValidationException(field + " must be a number.");
            }
            return result;
        }
    }
}
