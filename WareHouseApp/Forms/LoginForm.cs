using System;
using System.Drawing;
using System.Windows.Forms;
using WareHouseApp.Data;
using WareHouseApp.Exceptions;
using WareHouseApp.Models;
using WareHouseApp.People;
using WareHouseApp.Properties;

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
            ClientSize = new Size(820, 480);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9F);

            int brandWidth = 330;
            Controls.Add(BuildFormPanel(brandWidth));
            Controls.Add(UiTheme.BrandPanel(brandWidth, ClientSize.Height, Resources.wholesale,
                "Manage your inventory, customers and team in one place."));
        }

        private Panel BuildFormPanel(int brandWidth)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(0)
            };

            int x = 60;
            int width = ClientSize.Width - brandWidth - 120;

            var heading = new Label
            {
                Text = "Welcome back",
                ForeColor = UiTheme.TextDark,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                Location = new Point(x, 70),
                AutoSize = true
            };

            var sub = new Label
            {
                Text = "Please sign in to your account",
                ForeColor = UiTheme.Muted,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(x, 112),
                AutoSize = true
            };

            txtUsername = UiTheme.AddField(panel, "USERNAME", x, 160, width, false);
            txtPassword = UiTheme.AddField(panel, "PASSWORD", x, 235, width, true);

            var btnLogin = UiTheme.PrimaryButton("Sign In", x, 315, width);
            btnLogin.Click += BtnLogin_Click;
            AcceptButton = btnLogin;

            var prompt = new Label
            {
                Text = "Don't have an account?",
                ForeColor = UiTheme.Muted,
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(x, 385),
                AutoSize = true
            };

            var lnkSignUp = new LinkLabel
            {
                Text = "Sign up",
                Font = new Font("Segoe UI Semibold", 9.5F),
                LinkColor = UiTheme.Accent,
                ActiveLinkColor = UiTheme.AccentHover,
                Location = new Point(x + prompt.PreferredWidth + 4, 385),
                AutoSize = true
            };
            lnkSignUp.LinkClicked += (s, e) => OpenSignUp();

            panel.Controls.Add(heading);
            panel.Controls.Add(sub);
            panel.Controls.Add(btnLogin);
            panel.Controls.Add(prompt);
            panel.Controls.Add(lnkSignUp);
            return panel;
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
