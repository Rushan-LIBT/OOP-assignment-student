using System;
using System.Drawing;
using System.Windows.Forms;
using WareHouseApp.Data;
using WareHouseApp.Exceptions;
using WareHouseApp.People;

namespace WareHouseApp.Forms
{
    /// <summary>Shows the signed-in user's details and lets them change their password.</summary>
    public class ProfileControl : UserControl
    {
        private readonly Person currentUser;
        private readonly UserRepository users = new UserRepository();

        private TextBox txtCurrent;
        private TextBox txtNew;
        private TextBox txtConfirm;

        public ProfileControl(Person user)
        {
            currentUser = user;
            BuildUi();
        }

        private void BuildUi()
        {
            BackColor = Color.WhiteSmoke;

            var title = new Label
            {
                Text = "My Profile",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 40
            };

            var details = new Label
            {
                Text = "Username: " + currentUser.UserName +
                       "\nFull name: " + (currentUser.FullName ?? "-") +
                       "\nRole: " + currentUser.Role,
                Font = new Font("Segoe UI", 10F),
                Dock = DockStyle.Top,
                Height = 80,
                Padding = new Padding(0, 10, 0, 0)
            };

            var section = new Label
            {
                Text = "Change password",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 30
            };

            var panel = new Panel { Dock = DockStyle.Top, Height = 200 };

            txtCurrent = AddField(panel, "Current password", 0);
            txtNew = AddField(panel, "New password", 1);
            txtConfirm = AddField(panel, "Confirm new password", 2);

            var btnSave = new Button
            {
                Text = "Update Password",
                Location = new Point(0, 160),
                Width = 200,
                Height = 34,
                BackColor = Color.RoyalBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.Click += BtnSave_Click;
            panel.Controls.Add(btnSave);

            Controls.Add(panel);
            Controls.Add(section);
            Controls.Add(details);
            Controls.Add(title);
        }

        private TextBox AddField(Panel parent, string label, int index)
        {
            int y = index * 50;
            var lbl = new Label { Text = label, Location = new Point(0, y), AutoSize = true };
            var box = new TextBox
            {
                Location = new Point(0, y + 20),
                Width = 260,
                UseSystemPasswordChar = true
            };
            parent.Controls.Add(lbl);
            parent.Controls.Add(box);
            return box;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtNew.Text != txtConfirm.Text)
                {
                    throw new ValidationException("New passwords do not match.");
                }

                users.ChangePassword(currentUser.EmpID, txtCurrent.Text, txtNew.Text);

                MessageBox.Show("Password updated successfully.",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtCurrent.Clear();
                txtNew.Clear();
                txtConfirm.Clear();
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
