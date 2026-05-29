using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WareHouseApp.Data;
using WareHouseApp.Exceptions;
using WareHouseApp.Models;

namespace WareHouseApp.Forms
{
    /// <summary>Employee management: list, search and CRUD for staff (Admin only).</summary>
    public class EmployeeControl : UserControl
    {
        private const string AllRoles = "All roles";

        private readonly EmployeeRepository repository = new EmployeeRepository();
        private int selectedId;
        private bool refreshingFilter;

        private DataGridView grid;
        private TextBox txtName;
        private TextBox txtRole;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtSalary;
        private TextBox txtSearch;
        private ComboBox cmbRoleFilter;

        public EmployeeControl()
        {
            BuildUi();
            RefreshRoleFilter();
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
                Text = "Add / Edit Employee",
                Font = new Font("Segoe UI Semibold", 12F),
                ForeColor = UiTheme.TextDark,
                Location = new Point(24, 18),
                AutoSize = true
            };
            card.Controls.Add(title);

            int y = 58;
            txtName = UiTheme.AddField(card, "NAME", 24, y, 180, false);
            txtRole = UiTheme.AddField(card, "ROLE", 228, y, 150, false);
            txtEmail = UiTheme.AddField(card, "EMAIL", 402, y, 200, false);
            txtPhone = UiTheme.AddField(card, "PHONE", 626, y, 150, false);
            txtSalary = UiTheme.AddField(card, "SALARY", 24, 130, 180, false);

            var btnAdd = UiTheme.ActionButton("Add", UiTheme.Success, 402, 150);
            btnAdd.Click += (s, e) => Save(false);
            var btnUpdate = UiTheme.ActionButton("Update", UiTheme.Accent, 518, 150);
            btnUpdate.Click += (s, e) => Save(true);
            var btnDelete = UiTheme.ActionButton("Delete", UiTheme.Danger, 634, 150);
            btnDelete.Click += (s, e) => DeleteSelected();
            var btnClear = UiTheme.ActionButton("Clear", UiTheme.Neutral, 750, 150);
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

            var searchRow = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = UiTheme.Card };
            var lblSearch = new Label
            {
                Text = "Search",
                ForeColor = UiTheme.Muted,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(2, 20),
                AutoSize = true
            };
            txtSearch = UiTheme.BoxedInput(60, 12, 260, out Panel searchBox);
            txtSearch.TextChanged += (s, e) => LoadData();

            var lblFilter = new Label
            {
                Text = "Role",
                ForeColor = UiTheme.Muted,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(348, 20),
                AutoSize = true
            };
            cmbRoleFilter = UiTheme.FilterCombo(420, 13, 200);
            cmbRoleFilter.SelectedIndexChanged += (s, e) =>
            {
                if (!refreshingFilter)
                {
                    LoadData();
                }
            };

            searchRow.Controls.Add(searchBox);
            searchRow.Controls.Add(lblSearch);
            searchRow.Controls.Add(cmbRoleFilter);
            searchRow.Controls.Add(lblFilter);

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
                var list = repository.Search(txtSearch.Text);

                string role = cmbRoleFilter.SelectedItem as string;
                if (!string.IsNullOrEmpty(role) && role != AllRoles)
                {
                    list = list.Where(emp => (emp.Role ?? string.Empty) == role).ToList();
                }

                grid.DataSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshRoleFilter()
        {
            try
            {
                refreshingFilter = true;
                string current = cmbRoleFilter.SelectedItem as string;

                cmbRoleFilter.Items.Clear();
                cmbRoleFilter.Items.Add(AllRoles);
                foreach (string role in repository.GetAll()
                    .Select(emp => emp.Role)
                    .Where(r => !string.IsNullOrWhiteSpace(r))
                    .Distinct()
                    .OrderBy(r => r))
                {
                    cmbRoleFilter.Items.Add(role);
                }

                cmbRoleFilter.SelectedItem = current != null && cmbRoleFilter.Items.Contains(current)
                    ? current
                    : AllRoles;
            }
            finally
            {
                refreshingFilter = false;
            }
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null || !(grid.CurrentRow.DataBoundItem is Employee employee))
            {
                return;
            }

            selectedId = employee.EmployeeID;
            txtName.Text = employee.Name;
            txtRole.Text = employee.Role;
            txtEmail.Text = employee.Email;
            txtPhone.Text = employee.Phone;
            txtSalary.Text = employee.Salary.ToString();
        }

        private void Save(bool isUpdate)
        {
            try
            {
                var employee = new Employee
                {
                    EmployeeID = selectedId,
                    Name = txtName.Text.Trim(),
                    Role = txtRole.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Salary = ParseDecimal(txtSalary.Text, "Salary")
                };

                if (isUpdate)
                {
                    if (selectedId == 0)
                    {
                        throw new ValidationException("Select a row to update first.");
                    }
                    repository.Update(employee);
                }
                else
                {
                    repository.Add(employee);
                }

                ClearForm();
                RefreshRoleFilter();
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

            if (MessageBox.Show("Delete this employee?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            try
            {
                repository.Delete(selectedId);
                ClearForm();
                RefreshRoleFilter();
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
            txtRole.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtSalary.Clear();
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
