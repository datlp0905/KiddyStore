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
    public partial class frmLogin : Form
    {
        private static HttpClient client = new HttpClient();
        private static readonly string BASE_URL = "http://localhost:50815/api/Employees/";
        public frmLogin()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private async void Login(EmployeeDTO dto)
        {
            EmployeeDTO result = null;
            HttpResponseMessage response = await client.PostAsJsonAsync(BASE_URL + "CheckLogin", dto);
            string strResponse = response.Content.ReadAsStringAsync().Result;
            result = JsonConvert.DeserializeObject<EmployeeDTO>(strResponse);
            if(result.username == null)
            {
                MessageBox.Show("Invalid username password!");
            }
            else
            {
                new frmMain(result, this);  
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            EmployeeDTO dto = new EmployeeDTO
            {
                username = txtUsername.Text,
                password = txtPassword.Text
                
            };
            Login(dto);

        }

        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                button1_Click(this, new EventArgs());
            }
        }
    }
}
