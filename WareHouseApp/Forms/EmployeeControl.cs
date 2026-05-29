using System;
using System.Drawing;
using System.Windows.Forms;
using WareHouseApp.Data;
using WareHouseApp.Exceptions;
using WareHouseApp.Models;

namespace WareHouseApp.Forms
{
    /// <summary>Employee management: list, search and CRUD for staff (Admin only).</summary>
    public class EmployeeControl : UserControl
    {
        private readonly EmployeeRepository repository = new EmployeeRepository();
        private int selectedId;

        private DataGridView grid;
        private TextBox txtName;
        private TextBox txtRole;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtSalary;
        private TextBox txtSearch;

        public EmployeeControl()
        {
            BuildUi();
            LoadData();
        }

        private void BuildUi()
        {
            BackColor = Color.WhiteSmoke;

            var title = new Label
            {
                Text = "Employee Management",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 40
            };

            var form = new Panel { Dock = DockStyle.Top, Height = 120 };
            txtName = AddField(form, "Name", 0);
            txtRole = AddField(form, "Role", 1);
            txtEmail = AddField(form, "Email", 2);
            txtPhone = AddField(form, "Phone", 3);
            txtSalary = AddField(form, "Salary", 4);

            var actions = new Panel { Dock = DockStyle.Top, Height = 50 };
            var btnAdd = MakeButton("Add", 0, Color.SeaGreen);
            btnAdd.Click += (s, e) => Save(false);
            var btnUpdate = MakeButton("Update", 1, Color.RoyalBlue);
            btnUpdate.Click += (s, e) => Save(true);
            var btnDelete = MakeButton("Delete", 2, Color.Firebrick);
            btnDelete.Click += (s, e) => DeleteSelected();
            var btnClear = MakeButton("Clear", 3, Color.Gray);
            btnClear.Click += (s, e) => ClearForm();
            actions.Controls.Add(btnAdd);
            actions.Controls.Add(btnUpdate);
            actions.Controls.Add(btnDelete);
            actions.Controls.Add(btnClear);

            var searchPanel = new Panel { Dock = DockStyle.Top, Height = 40 };
            var lblSearch = new Label { Text = "Search:", Location = new Point(0, 10), AutoSize = true };
            txtSearch = new TextBox { Location = new Point(60, 7), Width = 250 };
            txtSearch.TextChanged += (s, e) => LoadData();
            searchPanel.Controls.Add(txtSearch);
            searchPanel.Controls.Add(lblSearch);

            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            grid.SelectionChanged += Grid_SelectionChanged;

            Controls.Add(grid);
            Controls.Add(searchPanel);
            Controls.Add(actions);
            Controls.Add(form);
            Controls.Add(title);
        }

        private TextBox AddField(Panel parent, string label, int index)
        {
            int x = index * 160;
            var lbl = new Label { Text = label, Location = new Point(x, 10), AutoSize = true };
            var box = new TextBox { Location = new Point(x, 32), Width = 140 };
            parent.Controls.Add(lbl);
            parent.Controls.Add(box);
            return box;
        }

        private Button MakeButton(string text, int index, Color color)
        {
            return new Button
            {
                Text = text,
                Location = new Point(index * 110, 8),
                Width = 100,
                Height = 34,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
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
