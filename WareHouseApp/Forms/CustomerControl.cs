using System;
using System.Drawing;
using System.Windows.Forms;
using WareHouseApp.Data;
using WareHouseApp.Exceptions;
using WareHouseApp.Models;

namespace WareHouseApp.Forms
{
    /// <summary>Customer management: list, search and CRUD for customers.</summary>
    public class CustomerControl : UserControl
    {
        private readonly CustomerRepository repository = new CustomerRepository();
        private int selectedId;

        private DataGridView grid;
        private TextBox txtName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtAddress;
        private TextBox txtSearch;

        public CustomerControl()
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
                Text = "Add / Edit Customer",
                Font = new Font("Segoe UI Semibold", 12F),
                ForeColor = UiTheme.TextDark,
                Location = new Point(24, 18),
                AutoSize = true
            };
            card.Controls.Add(title);

            int y = 58;
            txtName = UiTheme.AddField(card, "NAME", 24, y, 190, false);
            txtEmail = UiTheme.AddField(card, "EMAIL", 238, y, 220, false);
            txtPhone = UiTheme.AddField(card, "PHONE", 482, y, 160, false);
            txtAddress = UiTheme.AddField(card, "ADDRESS", 24, 130, 434, false);

            var btnAdd = UiTheme.ActionButton("Add", UiTheme.Success, 482, 150);
            btnAdd.Click += (s, e) => Save(false);
            var btnUpdate = UiTheme.ActionButton("Update", UiTheme.Accent, 598, 150);
            btnUpdate.Click += (s, e) => Save(true);
            var btnDelete = UiTheme.ActionButton("Delete", UiTheme.Danger, 714, 150);
            btnDelete.Click += (s, e) => DeleteSelected();
            var btnClear = UiTheme.ActionButton("Clear", UiTheme.Neutral, 830, 150);
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
            if (grid.CurrentRow == null || !(grid.CurrentRow.DataBoundItem is Customer customer))
            {
                return;
            }

            selectedId = customer.CustomerID;
            txtName.Text = customer.Name;
            txtEmail.Text = customer.Email;
            txtPhone.Text = customer.Phone;
            txtAddress.Text = customer.Address;
        }

        private void Save(bool isUpdate)
        {
            try
            {
                var customer = new Customer
                {
                    CustomerID = selectedId,
                    Name = txtName.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Address = txtAddress.Text.Trim()
                };

                if (isUpdate)
                {
                    if (selectedId == 0)
                    {
                        throw new ValidationException("Select a row to update first.");
                    }
                    repository.Update(customer);
                }
                else
                {
                    repository.Add(customer);
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

            if (MessageBox.Show("Delete this customer?", "Confirm",
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
            txtEmail.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
        }
    }
}
