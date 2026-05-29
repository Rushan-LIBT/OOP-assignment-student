using System;
using System.Drawing;
using System.Windows.Forms;
using WareHouseApp.Data;
using WareHouseApp.Exceptions;
using WareHouseApp.Models;
using WareHouseApp.People;

namespace WareHouseApp.Forms
{
    /// <summary>
    /// Entry screen. Authenticates against the database and opens the dashboard.
    /// </summary>
    public class LoginForm : Form
    {
        private readonly UserRepository userRepository = new UserRepository();

        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private LinkLabel lnkSignUp;

        public LoginForm()
        {
            BuildUi();
        }

        private void BuildUi()
        {
            Text = "Warehouse Management - Login";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            ClientSize = new Size(380, 320);
            BackColor = Color.White;

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.RoyalBlue
            };
            var title = new Label
            {
                Text = "Warehouse Management System",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            header.Controls.Add(title);

            var lblUser = new Label { Text = "Username", Location = new Point(40, 100), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(40, 122), Width = 300 };

            var lblPass = new Label { Text = "Password", Location = new Point(40, 160), AutoSize = true };
            txtPassword = new TextBox { Location = new Point(40, 182), Width = 300, UseSystemPasswordChar = true };

            btnLogin = new Button
            {
                Text = "Login",
                Location = new Point(40, 225),
                Width = 300,
                Height = 38,
                BackColor = Color.RoyalBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogin.Click += BtnLogin_Click;

            lnkSignUp = new LinkLabel
            {
                Text = "Don't have an account? Sign up",
                Location = new Point(40, 275),
                AutoSize = true
            };
            lnkSignUp.Click += (s, e) => OpenSignUp();

            AcceptButton = btnLogin;

            Controls.Add(lblUser);
            Controls.Add(txtUsername);
            Controls.Add(lblPass);
            Controls.Add(txtPassword);
            Controls.Add(btnLogin);
            Controls.Add(lnkSignUp);
            Controls.Add(header);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                User user = userRepository.Authenticate(txtUsername.Text.Trim(), txtPassword.Text);
                Person person = PersonFactory.FromUser(user);

                var dashboard = new DashboardForm(person);
                Hide();
                dashboard.FormClosed += (s, args) => Close();
                dashboard.Show();
            }
            catch (AuthenticationException ex)
            {
                MessageBox.Show(ex.Message, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Clear();
                txtPassword.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not reach the database.\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenSignUp()
        {
            Hide();
            using (var signUp = new SignUpForm())
            {
                signUp.ShowDialog();
            }
            Show();
            txtUsername.Focus();
        }
    }
}
