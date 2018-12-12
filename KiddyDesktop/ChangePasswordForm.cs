using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KiddyDesktop.Models;
using Newtonsoft.Json;

namespace KiddyDesktop
{
    public partial class ChangePasswordForm : Form
    {
        private string username;
        private static HttpClient  client = new HttpClient();
        private static string base_URL = "http://localhost:50815/api/Employees/";
        public ChangePasswordForm()
        {
            InitializeComponent();
        }
        public ChangePasswordForm(string username)
        {
            InitializeComponent();
            this.username = username;
        }

        private bool checkBlank()
        {
            bool result = false;
            if(txtOldPassword.Text == "")
            {
                result = true;
                MessageBox.Show("Old Password cannot be null");
            }
            if (txtNewPassword.Text == "")
            {
                result = true;
                MessageBox.Show("New Password cannot be null");
            }
            if (txtConfirmPassword.Text == "")
            {
                result = true;
                MessageBox.Show("Confirm Password cannot be null");
            }
            return result;
        }
        private async void ChangePassword(string pass, string newPass)
        {
            EmployeeDTO dto = new EmployeeDTO()
            {
                username = username,
                password = pass
            };
            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(base_URL + "CheckLogin", dto);
            string resString = responseMessage.Content.ReadAsStringAsync().Result;
            EmployeeDTO employee = JsonConvert.DeserializeObject<EmployeeDTO>(resString);
            if(employee.username != null)
            {
                employee.password = newPass;
                ChangePasswordToDB(employee);
            }
            else
            {
                MessageBox.Show("Wrong old password!");
            }
        }

        private async void ChangePasswordToDB(EmployeeDTO employee)
        {
            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(base_URL + "ChangePassword", employee);
            try
            {
                responseMessage.EnsureSuccessStatusCode();
                MessageBox.Show("Change password success!");
            }
            catch (Exception)
            {

                MessageBox.Show("Change password fail!");
            }
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (!checkBlank())
                {
                    if(txtConfirmPassword.Text == txtNewPassword.Text)
                    {
                        ChangePassword(txtOldPassword.Text, txtConfirmPassword.Text);
                        
                    }
                    else
                    {
                        MessageBox.Show("Confirm password is not correct!");
                    }
                }
              
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtOldPassword_Validating(object sender, CancelEventArgs e)
        {
            if(txtOldPassword.Text.Equals(""))
            {
                errorProvider1.SetError(txtOldPassword, "Cannot null!");
                txtOldPassword.Focus();
            }
            else
            {
                errorProvider1.SetError(txtOldPassword, "");
            }
        }

        private void txtNewPassword_Validating(object sender, CancelEventArgs e)
        {
            if (txtNewPassword.Text.Equals(""))
            {
                errorProvider1.SetError(txtNewPassword, "Cannot null!");
                txtNewPassword.Focus();
            }
            else
            {
                errorProvider1.SetError(txtNewPassword, "");
            }
        }

        private void txtConfirmPassword_TextChanged(object sender, EventArgs e)
        {
            if (txtConfirmPassword.Text.Equals(""))
            {
                errorProvider1.SetError(txtConfirmPassword, "Cannot null!");
                txtConfirmPassword.Focus();
            }
            else
            {
                errorProvider1.SetError(txtConfirmPassword, "");
            }
            if (!txtConfirmPassword.Text.Equals(txtNewPassword.Text))
            {
                errorProvider1.SetError(txtConfirmPassword, "Confirm password is not match!");
            }
            else
            {
                errorProvider1.SetError(txtConfirmPassword, "");
            }

        }

    }
}
