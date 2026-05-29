using System;
using System.Drawing;
using System.Windows.Forms;
using WareHouseApp.Data;
using WareHouseApp.Exceptions;

namespace WareHouseApp.Forms
{
    /// <summary>
    /// Registers a new user account in the database.
    /// </summary>
    public class SignUpForm : Form
    {
        private readonly UserRepository userRepository = new UserRepository();

        private TextBox txtUsername;
        private TextBox txtFullName;
        private TextBox txtPassword;
        private TextBox txtConfirm;
        private ComboBox cmbRole;

        public SignUpForm()
        {
            BuildUi();
        }

        private void BuildUi()
        {
            Text = "Create Account";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(380, 360);
            BackColor = Color.White;

            int x = 40;
            int width = 300;

            var title = new Label
            {
                Text = "Create a new account",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(x, 20),
                AutoSize = true
            };

            var lblUser = new Label { Text = "Username", Location = new Point(x, 60), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(x, 80), Width = width };

            var lblName = new Label { Text = "Full name", Location = new Point(x, 112), AutoSize = true };
            txtFullName = new TextBox { Location = new Point(x, 132), Width = width };

            var lblPass = new Label { Text = "Password", Location = new Point(x, 164), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(x, 184), Width = width, UseSystemPasswordChar = true };

            var lblConfirm = new Label { Text = "Confirm password", Location = new Point(x, 216), AutoSize = true };
            txtConfirm = new TextBox { Location = new Point(x, 236), Width = width, UseSystemPasswordChar = true };

            var lblRole = new Label { Text = "Role", Location = new Point(x, 268), AutoSize = true };
            cmbRole = new ComboBox
            {
                Location = new Point(x, 288),
                Width = width,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRole.Items.AddRange(new object[] { "Operator", "Admin" });
            cmbRole.SelectedIndex = 0;

            var btnRegister = new Button
            {
                Text = "Register",
                Location = new Point(x, 320),
                Width = width,
                Height = 32,
                BackColor = Color.RoyalBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRegister.Click += BtnRegister_Click;
            AcceptButton = btnRegister;

            Controls.Add(title);
            Controls.Add(lblUser);
            Controls.Add(txtUsername);
            Controls.Add(lblName);
            Controls.Add(txtFullName);
            Controls.Add(lblPass);
            Controls.Add(txtPassword);
            Controls.Add(lblConfirm);
            Controls.Add(txtConfirm);
            Controls.Add(lblRole);
            Controls.Add(cmbRole);
            Controls.Add(btnRegister);
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPassword.Text != txtConfirm.Text)
                {
                    throw new ValidationException("Passwords do not match.");
                }

                userRepository.Register(
                    txtUsername.Text.Trim(),
                    txtPassword.Text,
                    txtFullName.Text.Trim(),
                    cmbRole.SelectedItem.ToString());

                MessageBox.Show("Account created. You can now log in.",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (ValidationException ex)
            {
                MessageBox.Show(ex.Message, "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create the account.\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
