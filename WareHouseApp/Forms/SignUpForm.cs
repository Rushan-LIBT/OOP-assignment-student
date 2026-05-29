using System;
using System.Drawing;
using System.Windows.Forms;
using WareHouseApp.Data;
using WareHouseApp.Exceptions;
using WareHouseApp.Properties;

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
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(820, 480);
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9F);

            int brandWidth = 330;
            Controls.Add(BuildFormPanel(brandWidth));
            Controls.Add(UiTheme.BrandPanel(brandWidth, ClientSize.Height, Resources.reliability,
                "Join the team and start managing the warehouse today."));
        }

        private Panel BuildFormPanel(int brandWidth)
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            int x1 = 45;
            int colWidth = 185;
            int gap = 25;
            int x2 = x1 + colWidth + gap;
            int fullWidth = (x2 + colWidth) - x1;

            var heading = new Label
            {
                Text = "Create account",
                ForeColor = UiTheme.TextDark,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                Location = new Point(x1, 45),
                AutoSize = true
            };

            var sub = new Label
            {
                Text = "Fill in the details below to get started",
                ForeColor = UiTheme.Muted,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(x1, 87),
                AutoSize = true
            };

            txtUsername = UiTheme.AddField(panel, "USERNAME", x1, 130, colWidth, false);
            txtFullName = UiTheme.AddField(panel, "FULL NAME", x2, 130, colWidth, false);
            txtPassword = UiTheme.AddField(panel, "PASSWORD", x1, 205, colWidth, true);
            txtConfirm = UiTheme.AddField(panel, "CONFIRM PASSWORD", x2, 205, colWidth, true);

            var roleLabel = new Label
            {
                Text = "ROLE",
                ForeColor = UiTheme.Muted,
                Font = new Font("Segoe UI", 9F),
                Location = new Point(x1, 280),
                AutoSize = true
            };
            cmbRole = new ComboBox
            {
                Location = new Point(x1, 302),
                Width = colWidth,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F)
            };
            cmbRole.Items.AddRange(new object[] { "Operator", "Admin" });
            cmbRole.SelectedIndex = 0;

            var btnRegister = UiTheme.PrimaryButton("Create Account", x1, 350, fullWidth);
            btnRegister.Click += BtnRegister_Click;
            AcceptButton = btnRegister;

            var prompt = new Label
            {
                Text = "Already have an account?",
                ForeColor = UiTheme.Muted,
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(x1, 415),
                AutoSize = true
            };

            var lnkBack = new LinkLabel
            {
                Text = "Back to login",
                Font = new Font("Segoe UI Semibold", 9.5F),
                LinkColor = UiTheme.Accent,
                ActiveLinkColor = UiTheme.AccentHover,
                Location = new Point(x1 + prompt.PreferredWidth + 4, 415),
                AutoSize = true
            };
            lnkBack.LinkClicked += (s, e) => Close();

            panel.Controls.Add(heading);
            panel.Controls.Add(sub);
            panel.Controls.Add(roleLabel);
            panel.Controls.Add(cmbRole);
            panel.Controls.Add(btnRegister);
            panel.Controls.Add(prompt);
            panel.Controls.Add(lnkBack);
            return panel;
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
