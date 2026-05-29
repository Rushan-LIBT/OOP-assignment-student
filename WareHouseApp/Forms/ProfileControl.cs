using System;
using System.Drawing;
using System.Windows.Forms;
using WareHouseApp.Data;
using WareHouseApp.Exceptions;
using WareHouseApp.People;
using WareHouseApp.Properties;

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
            BackColor = UiTheme.AppBg;

            Controls.Add(new Panel { Dock = DockStyle.Fill, BackColor = UiTheme.AppBg });
            Controls.Add(BuildPasswordCard());
            Controls.Add(new Panel { Dock = DockStyle.Top, Height = 16, BackColor = UiTheme.AppBg });
            Controls.Add(BuildInfoCard());
        }

        private Panel BuildInfoCard()
        {
            var card = UiTheme.CardPanel(DockStyle.Top, 150);

            var avatar = new PictureBox
            {
                Image = new Bitmap(Resources.user, new Size(72, 72)),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(72, 72),
                Location = new Point(24, 36),
                BackColor = Color.Transparent
            };

            var name = new Label
            {
                Text = string.IsNullOrEmpty(currentUser.FullName) ? currentUser.UserName : currentUser.FullName,
                Font = new Font("Segoe UI Semibold", 15F),
                ForeColor = UiTheme.TextDark,
                Location = new Point(112, 42),
                AutoSize = true
            };

            var meta = new Label
            {
                Text = "Username: " + currentUser.UserName + "      Role: " + currentUser.Role,
                Font = new Font("Segoe UI", 10F),
                ForeColor = UiTheme.Muted,
                Location = new Point(114, 78),
                AutoSize = true
            };

            card.Controls.Add(avatar);
            card.Controls.Add(name);
            card.Controls.Add(meta);
            return card;
        }

        private Panel BuildPasswordCard()
        {
            var card = UiTheme.CardPanel(DockStyle.Top, 230);

            var title = new Label
            {
                Text = "Change Password",
                Font = new Font("Segoe UI Semibold", 12F),
                ForeColor = UiTheme.TextDark,
                Location = new Point(24, 18),
                AutoSize = true
            };

            txtCurrent = UiTheme.AddField(card, "CURRENT PASSWORD", 24, 58, 240, true);
            txtNew = UiTheme.AddField(card, "NEW PASSWORD", 288, 58, 240, true);
            txtConfirm = UiTheme.AddField(card, "CONFIRM NEW PASSWORD", 552, 58, 240, true);

            var btnSave = UiTheme.ActionButton("Update Password", UiTheme.Accent, 24, 150, 200, 42);
            btnSave.Click += BtnSave_Click;

            card.Controls.Add(title);
            card.Controls.Add(btnSave);
            return card;
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
