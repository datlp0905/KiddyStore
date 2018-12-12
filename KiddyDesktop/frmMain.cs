using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KiddyDesktop.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace KiddyDesktop
{
    public partial class frmMain : Form
    {
        private EmployeeDTO emp;
        private frmLogin login;
        private IEnumerable<EmployeeDTO> listEmployees;
        private IEnumerable<ToyDTO> listToys;
        private IEnumerable<CustomerDTO> listCustomer;
        private IEnumerable<ToyDTO> listToyFeedback;
        private IEnumerable<FeedbackDTO> listFeedback;
        private IEnumerable<OrderDTO> listOrders;
        private IEnumerable<OrderDetailDTO> listOfOrderDetails;

        private string empImgString;
        private static HttpClient client = new HttpClient();
        private static readonly string BASE_URL = "http://localhost:50815/api/";

        public static void initClient()
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public frmMain()
        {
            InitializeComponent();
        }

        public frmMain(EmployeeDTO dto, frmLogin login)
        {
            InitializeComponent();
            emp = dto;
            this.login = login;
            login.Hide();
            this.Show();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (emp.role.Equals("Employee"))
            {
                TabControl.TabPages.Remove(tabEmployee);
                btnWelcomeEmployee.Enabled = false;
            }
        }

        #region Employee_functions

        private void tabEmployee_Enter(object sender, EventArgs e)
        {
            txtUsername.Enabled = false;
            loadEmployees();
            SetUpEmployeeData();
        }

        private void txtUsername_Validating(object sender, CancelEventArgs e)
        {
            if (txtUsername.Text.Equals(""))
            {
                usernameValidate.SetError(txtUsername, "Username can not be blank!");
                txtUsername.Focus();
            }
            else
            {
                this.usernameValidate.SetError(txtUsername, "");
            }
        }

        private void txtFirstName_Validating(object sender, CancelEventArgs e)
        {
            if (txtFirstName.Text.Equals(""))
            {
                firstnameValidate.SetError(txtFirstName, "Firstname can not be blank!");
                txtFirstName.Focus();

            }
            else
            {
                this.firstnameValidate.SetError(txtFirstName, "");
            }
        }

        private void txtLastName_Validating(object sender, CancelEventArgs e)
        {
            if (txtLastName.Text.Equals(""))
            {
                lastnameValidate.SetError(txtLastName, "Firstname can not be blank!");
                txtLastName.Focus();
            }
            else
            {
                this.lastnameValidate.SetError(txtLastName, "");
            }
        }

        private void SetEmployeeTabBlank()
        {
            txtUsername.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            PBEmployee.Image = null;
        }

        private bool CheckEmployeeTabBlank()
        {
            bool result = false;
            if (txtUsername.Text.Equals(""))
            {
                txtUsername.Focus();
                result = true;
            }
            if (txtFirstName.Text.Equals(""))
            {
                txtFirstName.Focus();
                result = true;
            }
            if (txtLastName.Text.Equals(""))
            {
                txtLastName.Focus();
                result = true;
            }
            if (PBEmployee.Image == null)
            {
                result = true;
                btnUploadImage.Focus();
            }
            return result;
        }

        private bool CheckEmployeeExistedUsername(string username)
        {
            bool result = false;
            try
            {
                EmployeeDTO emp = listEmployees.Single(em => em.username.Equals(username));
                result = true;
            }
            catch (Exception)
            {
            }
            return result;
        }

        private bool CheckValidDate(string date)
        {
            bool result = true;
            try
            {
                DateTime dateTime = DateTime.Parse(date);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        private async void loadEmployees()
        {
            HttpResponseMessage response = await client.GetAsync(BASE_URL + "Employees");
            if (response.IsSuccessStatusCode)
            {
                string strResponse = response.Content.ReadAsStringAsync().Result;
                listEmployees = JsonConvert.DeserializeObject<IEnumerable<EmployeeDTO>>(strResponse);
                LoadEmployeeData(listEmployees);
                ClearDataBindingForEmployee();
                AddDataBindingForEmployee();
            }
        }

        private void LoadEmployeeData(IEnumerable<EmployeeDTO> list)
        {
            gvEmployee.DataSource = null;
            gvEmployee.DataSource = list;
            gvEmployee.Columns["image"].Visible = false;
            gvEmployee.Columns["dob"].Visible = false;
            gvEmployee.Columns["isActived"].Visible = false;
            gvEmployee.Columns["gender"].Visible = false;
            gvEmployee.Columns["role"].Visible = false;
            gvEmployee.Columns["password"].Visible = false;
            gvEmployee.Columns["firstname"].Visible = false;
            gvEmployee.Columns[0].HeaderText = "Username";
            gvEmployee.Columns[1].HeaderText = "Name";
        }


        private async void AddEmployeeToDB(EmployeeDTO dto)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(BASE_URL + "Employees", dto);
            try
            {
                response.EnsureSuccessStatusCode();
                loadEmployees();
                MessageBox.Show("Add Employee Success!");
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
            }
        }

        private async void EditEmployeeToDB(EmployeeDTO dto)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(BASE_URL + "Employees/" + dto.username, dto);
            try
            {
                response.EnsureSuccessStatusCode();
                MessageBox.Show("Edit employee success");
                loadEmployees();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private async void DeleteEmployee(string username)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(BASE_URL + "Employees/Delete?id=" + username, "");
            try
            {
                response.EnsureSuccessStatusCode();
                loadEmployees();
                MessageBox.Show("Delete employee success!");
            }
            catch (Exception)
            {
                MessageBox.Show("Delete employee fail!");
            }
        }

        private void SetUpEmployeeData()
        {
            dtDOB.Format = DateTimePickerFormat.Custom;
            dtDOB.CustomFormat = "yyyy-MMM-dd";
            btnEmployeeSave.Enabled = false;
        }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        private void SaveEmployee()
        {
            if (!CheckEmployeeTabBlank())
            {
                if (!CheckEmployeeExistedUsername(txtUsername.Text))
                {
                    if (CheckValidDate(dtDOB.Text))
                    {
                        DateTime myDate = DateTime.Parse(dtDOB.Text);
                        DateTime currDate = DateTime.Now;
                        int compare = DateTime.Compare(myDate, currDate);
                        if(compare < 0)
                        {
                            string mydob = string.Format("{0}-{1}-{2}", myDate.Year, myDate.Month, myDate.Day);
                            string myGender = "female";
                            if (rdMale.Checked == true)
                            {
                                myGender = "male";
                            }
                            Image img = Image.FromFile(empImgString);
                            byte[] imgByte = imageToByteArray(img);
                            EmployeeDTO addEmp = new EmployeeDTO();
                            addEmp.username = txtUsername.Text;
                            addEmp.password = "1";
                            addEmp.dob = mydob;
                            addEmp.gender = myGender;
                            addEmp.role = "Employee";
                            addEmp.isActived = true;
                            addEmp.firstname = txtFirstName.Text;
                            addEmp.lastname = txtLastName.Text;
                            addEmp.image = imgByte;
                            AddEmployeeToDB(addEmp);
                        }
                        else
                        {
                            MessageBox.Show("DOB must be earlier than current day!");
                        }
                        
                    }
                    else
                    {
                        MessageBox.Show("Invalid date");
                    }
                }
                else
                {
                    MessageBox.Show("Username has already existed!");
                }
            }
            else
            {
                MessageBox.Show("Data cannot be null!");
            }
        }

        private void EditEmployee()
        {
            if (!CheckEmployeeTabBlank())
            {
                if (CheckEmployeeExistedUsername(txtUsername.Text))
                {
                    if (CheckValidDate(dtDOB.Text))
                    {
                        DateTime myDate = DateTime.Parse(dtDOB.Text);
                        DateTime currDate = DateTime.Now;
                        int compare = DateTime.Compare(myDate, currDate);
                        if (compare < 0)
                        {
                            string mydob = string.Format("{0}-{1}-{2}", myDate.Year, myDate.Month, myDate.Day);
                            string myGender = "female";
                            if (rdMale.Checked == true)
                            {
                                myGender = "male";
                            }
                            Image image = PBEmployee.Image;
                            byte[] imagebyte = imageToByteArray(image);
                            EmployeeDTO editEmp = new EmployeeDTO();
                            editEmp.username = txtUsername.Text;
                            editEmp.firstname = txtFirstName.Text;
                            editEmp.lastname = txtLastName.Text;
                            editEmp.dob = mydob;
                            editEmp.gender = myGender;
                            editEmp.image = imagebyte;
                            EditEmployeeToDB(editEmp);

                        }
                        else
                        {
                            MessageBox.Show("DOB must be earlier than current day!");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Username is not existed!");
                }

            }

        }

        private void DeleteEmployee()
        {
            string username = txtUsername.Text;

            if (CheckEmployeeExistedUsername(txtUsername.Text))
            {
                DeleteEmployee(txtUsername.Text);
                SetEmployeeTabBlank();
            }
            else
            {
                MessageBox.Show("Username is not existed!");
            }

        }

        private void ClearDataBindingForEmployee()
        {
            txtUsername.DataBindings.Clear();
            txtFirstName.DataBindings.Clear();
            txtLastName.DataBindings.Clear();
            dtDOB.DataBindings.Clear();
            PBEmployee.DataBindings.Clear();
            rdMale.DataBindings.Clear();
            rdFemail.DataBindings.Clear();
        }

        private void AddDataBindingForEmployee()
        {
            txtUsername.DataBindings.Add("Text", listEmployees, "username");
            txtFirstName.DataBindings.Add("Text", listEmployees, "firstname");
            txtLastName.DataBindings.Add("Text", listEmployees, "lastname");
            dtDOB.DataBindings.Add("Text", listEmployees, "dob");
            PBEmployee.DataBindings.Add("Image", listEmployees, "image", true);
            Binding maleBinding = new Binding("Checked", listEmployees, "gender");
            maleBinding.Format += (s, args) => args.Value = ((string)args.Value) == "male";
            maleBinding.Parse += (s, args) => args.Value = (bool)args.Value ? "male" : "female";
            rdMale.DataBindings.Add(maleBinding);
            Binding femaleBinding = new Binding("Checked", listEmployees, "gender");
            femaleBinding.Format += (s, args) => args.Value = ((string)args.Value) == "female";
            femaleBinding.Parse += (s, args) => args.Value = (bool)args.Value ? "male" : "female";
            rdFemail.DataBindings.Add(femaleBinding);

        }

        //Add Employee button
        private void button2_Click(object sender, EventArgs e)
        {
            SetEmployeeTabBlank();
            btnEmployeeSave.Enabled = true;
            ClearDataBindingForEmployee();
            empImgString = null;
            txtUsername.Enabled = true;
        }

        private void gvEmployee_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtUsername.Enabled = false;
            ClearDataBindingForEmployee();
            AddDataBindingForEmployee();
            CheckEmployeeTabBlank();
        }


        private void btnEmployeeSave_Click(object sender, EventArgs e)
        {
            SaveEmployee();
            
        }

        private void btnEmployeeEdit_Click(object sender, EventArgs e)
        {
            EditEmployee();
           
        }

        private void btnEmployeeDelete_Click(object sender, EventArgs e)
        {
            DeleteEmployee(txtUsername.Text);
        }
        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            IEnumerable<EmployeeDTO> searchListEmployees = listEmployees.Where(em => (em.firstname + " " + em.lastname).Contains(txtSearch.Text)).ToList();
            LoadEmployeeData(searchListEmployees);
        }

        //<----------------------------end------------------------------------------------>     
        #endregion

        #region Customer_functions
        //<----------------------------Customer function----------------------------------->
        private void tabCustomer_Enter(object sender, EventArgs e)
        {
            LoadCustomerData();
            LoadOrders();
            LoadOrderDetails();
        }
        private async void LoadCustomerData()
        {
            HttpResponseMessage response = await client.GetAsync(BASE_URL + "Customers");
            if (response.IsSuccessStatusCode)
            {
                string strResponse = response.Content.ReadAsStringAsync().Result;
                listCustomer = JsonConvert.DeserializeObject<IEnumerable<CustomerDTO>>(strResponse);

            }
            gvCustomer.DataSource = listCustomer.Select(cus => new
            {
                username = cus.username,
                name = cus.firstname + " " + cus.lastname

            }).ToList();

        }
        private void AddOrdersDataBinding(IEnumerable<OrderDTO> listOrdersByCusID)
        {
            txtPayment.DataBindings.Add("Text", listOrdersByCusID, "payment");
            txtAddress.DataBindings.Add("Text", listOrdersByCusID, "address");
        }
        private void ClearOrdersDataBinding()
        {
            txtPayment.DataBindings.Clear();
            txtAddress.DataBindings.Clear();
        }

        private void LoadOrdersByCustomerID(string cusID)
        {
            IEnumerable<OrderDTO> listOrdersByCusID = listOrders.Where(ord => ord.cusID == cusID).ToList();
            txtPayment.Text = "";
            txtAddress.Text = "";
            gvOrders.DataSource = listOrdersByCusID;
            gvOrders.Columns["date"].DefaultCellStyle.Format = "dd-MM-yyyy";
            gvOrders.Columns["payment"].Visible = false;
            gvOrders.Columns["address"].Visible = false;
            gvOrders.Columns["cusID"].Visible = false;
            gvOrders.Columns["emlID"].Visible = false;
            gvOrders.Columns["status"].Visible = false;
            ClearOrdersDataBinding();
            AddOrdersDataBinding(listOrdersByCusID);
        }

        private async void LoadOrders()
        {
            HttpResponseMessage responseMessage = await client.GetAsync(BASE_URL + "Orders");
            if (responseMessage.IsSuccessStatusCode)
            {
                string strResponse = responseMessage.Content.ReadAsStringAsync().Result;
                listOrders = JsonConvert.DeserializeObject<IEnumerable<OrderDTO>>(strResponse);
                SetUpConfirmOrder();
            }
        }
        private async void LoadOrderDetails()
        {
            HttpResponseMessage response = await client.GetAsync(BASE_URL + "OrderDetails");
            if (response.IsSuccessStatusCode)
            {
                string strResponse = response.Content.ReadAsStringAsync().Result;
                listOfOrderDetails = JsonConvert.DeserializeObject<IEnumerable<OrderDetailDTO>>(strResponse);
            }
        }
        private void ViewOrderDetail(int orderID)
        {
            IEnumerable<OrderDetailDTO> listDetails = listOfOrderDetails.Where(ordDetail => ordDetail.orderID == orderID).ToList();
            gvOrderDetail.DataSource = null;
            gvOrderDetail.DataSource = listDetails;
            gvOrderDetail.Columns["id"].Visible = false;
            gvOrderDetail.Columns["toyID"].Visible = false;
            gvOrderDetail.Columns["orderID"].Visible = false;
        }

        private async void BlockCustomer(string cusID)
        {
            CustomerDTO dto = new CustomerDTO { username = cusID };
            HttpResponseMessage responseMessage2 = await client.PutAsJsonAsync(BASE_URL + "Customers/Block", dto);
            try
            {
                responseMessage2.EnsureSuccessStatusCode();
                MessageBox.Show("Block Customer success!");
                listCustomer = listCustomer.Where(cus => cus.username != cusID).ToList();
                gvCustomer.DataSource = listCustomer.Select(cus => new
                {
                    username = cus.username,
                    name = cus.firstname + " " + cus.lastname

                }).ToList();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        
        private void btnBlock_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = gvCustomer.CurrentRow;
            if (row != null)
            {
                string cusID = row.Cells["username"].Value.ToString();
                BlockCustomer(cusID);
            } else
            {
                MessageBox.Show("Please choose a customer to block!");
            }
        }

        private void gvOrders_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int orderID = int.Parse(gvOrders.CurrentRow.Cells[0].Value.ToString());
            ViewOrderDetail(orderID);
        }

        private void gvCustomer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            gvOrderDetail.DataSource = null;
            gvOrders.DataSource = null;
            string cusID = gvCustomer.CurrentRow.Cells[0].Value.ToString();
            LoadOrdersByCustomerID(cusID);
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            gvCustomer.DataSource = listCustomer.Where(cus => (cus.firstname + " " + cus.lastname).Contains(txtCustomerSearch.Text)).Select(cus => new
            {
                username = cus.username,
                name = cus.firstname + " " + cus.lastname
            }).ToList();
        }
        #endregion

        #region Order&Feedback_function
        private void tabOrderFeedback_Enter(object sender, EventArgs e)
        {
            loadToyFeedback();
            loadFeedback();
            LoadOrders();
            LoadOrderDetails();
        }
      
        private void AddConfirmOrdersDataBinding(IEnumerable<OrderDTO> listConfirm)
        {
            txtConfirmPayment.DataBindings.Add("Text", listConfirm, "payment");
            txtConfirmAddress.DataBindings.Add("Text", listConfirm, "address");
        }
        private void ClearConfirmOrdersDataBinding()
        {
            txtConfirmPayment.DataBindings.Clear();
            txtConfirmAddress.DataBindings.Clear();
        }

        private void SetUpConfirmOrder()
        {
            IEnumerable<OrderDTO> list = listOrders.Where(ord => ord.status == 0).ToList();
            if (list != null)
            {
                gvConfirmOrder.DataSource = list;
                gvConfirmOrder.Columns["date"].DefaultCellStyle.Format = "dd-MM-yyyy";
                gvConfirmOrder.Columns["cusID"].Visible = false;
                gvConfirmOrder.Columns["emlID"].Visible = false;
                gvConfirmOrder.Columns["payment"].Visible = false;
                gvConfirmOrder.Columns["status"].Visible = false;
                gvConfirmOrder.Columns["address"].Visible = false;
                ClearConfirmOrdersDataBinding();
                AddConfirmOrdersDataBinding(list);

            }
        }
        private void ViewOrderDetail2(int orderIDRef)
        {
            
            gvOrderDetail2.DataSource = listOfOrderDetails.Select(ordDetail => new
            {
                Name = ordDetail.name,
                Quantity = ordDetail.quantity,
                orderID = ordDetail.orderID
            }).Where(ordDetail => ordDetail.orderID == orderIDRef).ToList();

        }
        private async void ConfirmOrder(int ordIDConfirm)
        {
            OrderDTO dto = listOrders.Single(ord => ord.id == ordIDConfirm);
            dto.status = 1;
            if (ordIDConfirm != -1)
            {
                HttpResponseMessage responseMessage = await client.PutAsJsonAsync(BASE_URL + "Orders/" + dto.id, dto);
                try
                {
                    responseMessage.EnsureSuccessStatusCode();
                    ClearConfirmOrdersDataBinding();
                    gvOrderDetail2.DataSource = null;
                    LoadOrders();
                    
                }
                catch (Exception)
                {
                    MessageBox.Show("Confirm fail!");
                }
            }

        }

        private async void RejectOrder(int ordIDConfirm)
        {
            OrderDTO dto = listOrders.Single(ord => ord.id == ordIDConfirm);
            dto.status = -1;
            if (ordIDConfirm != -1)
            {
                HttpResponseMessage responseMessage = await client.PutAsJsonAsync(BASE_URL + "Orders/" + dto.id, dto);
                try
                {
                    responseMessage.EnsureSuccessStatusCode();
                    gvOrderDetail2.DataSource = null;
                    LoadOrders();
                }
                catch (Exception)
                {
                    MessageBox.Show("Confirm fail!");
                }
            }
        }

        #endregion

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            ChangePasswordForm CPasswordform = new ChangePasswordForm(emp.username);
            CPasswordform.Show();
        }

        private void gvConfirmOrder_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int ordIDConfirm = int.Parse(gvConfirmOrder.Rows[e.RowIndex].Cells["id"].Value.ToString());
            ViewOrderDetail2(ordIDConfirm);
        }

        //btnConfirmOrder
        private void button9_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = gvConfirmOrder.CurrentRow;
            if (row != null)
            {
                int ordIDConfirm = int.Parse(gvConfirmOrder.Rows[row.Index].Cells["id"].Value.ToString());
                if (ordIDConfirm != -1)
                {
                    ConfirmOrder(ordIDConfirm);
                }
            }
            else
            {
                MessageBox.Show("Please choose an order to confirm!");
            }

        }

        private void btnRejectOrder_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = gvConfirmOrder.CurrentRow;
            if (row != null)
            {
                int ordIDConfirm = int.Parse(gvConfirmOrder.Rows[row.Index].Cells["id"].Value.ToString());
                if (ordIDConfirm != -1)
                {
                    RejectOrder(ordIDConfirm);
                }
            }
            else
            {
                MessageBox.Show("Please choose an order to reject!");
            }
        }

        private void btnUploadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileChooser = new OpenFileDialog();
            fileChooser.Title = "Please select a photo";
            fileChooser.Filter = "JPG|*.jpg|PNG|*.png|GIF|*gif";
            fileChooser.Multiselect = false;
            if (fileChooser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.PBEmployee.ImageLocation = fileChooser.FileName;
                empImgString = fileChooser.FileName;
                PBEmployee.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void btnUploadImage_Validating(object sender, CancelEventArgs e)
        {
            if (empImgString == null && PBEmployee.Image == null)
            {
                imageValidate.SetError(btnUploadImage, "Please choose employee image");
                btnUploadImage.Focus();
            }
            else
            {
                imageValidate.SetError(btnUploadImage, "");
            }
        }

        #region Toy_Functions

        private void tabProduct_Enter(object sender, EventArgs e)
        {
            loadToys();
        }

        private async void loadToys()
        {
            HttpResponseMessage response = await client.GetAsync(BASE_URL + "Toys");
            if (response.IsSuccessStatusCode)
            {
                string strResponse = response.Content.ReadAsStringAsync().Result;
                listToys = JsonConvert.DeserializeObject<IEnumerable<ToyDTO>>(strResponse);
                dgvProducts.DataSource = null;
                dgvProducts.DataSource = listToys;
                dgvProducts.Columns["image"].Visible = false;
                dgvProducts.Columns["createdBy"].Visible = false;
                dgvProducts.Columns["isActived"].Visible = false;
                dgvProducts.Columns["description"].Visible = false;
                dgvProducts.Columns["category"].Visible = false;
                clearDataBindingForProduct();
                addDataBindingForProduct();
            }
            else
            {
                MessageBox.Show("Load product failed! Please check your connection!");
            }
        }

        private async void addToy(ToyDTO dto)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(BASE_URL + "Toys", dto);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private async void updateToy(ToyDTO dto)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(BASE_URL + "Toys/" + dto.id, dto);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void deleteToy(int id)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(BASE_URL + "Toys/Delete?id=" + id, "");
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                MessageBox.Show("Delete fail!");
            }
        }

        private void btnClearPro_Click(object sender, EventArgs e)
        {
            clearDataBindingForProduct();

            txtProID.Text = "";
            txtProName.Text = "";
            txtProPrice.Text = "";
            txtProQuantity.Text = "";
            txtProDescription.Text = "";
            cbProCategory.SelectedIndex = 0;
            pbProImage.Image = null;
        }


        private void addDataBindingForProduct()
        {
            txtProID.DataBindings.Add("Text", listToys, "id");
            txtProName.DataBindings.Add("Text", listToys, "name");
            txtProPrice.DataBindings.Add("Text", listToys, "price");
            txtProQuantity.DataBindings.Add("Text", listToys, "quantity");
            txtProDescription.DataBindings.Add("Text", listToys, "description");
            cbProCategory.DataBindings.Add("Text", listToys, "category");
            pbProImage.DataBindings.Add("Image", listToys, "image", true);

        }

        private void clearDataBindingForProduct()
        {
            txtProID.DataBindings.Clear();
            txtProName.DataBindings.Clear();
            txtProPrice.DataBindings.Clear();
            txtProQuantity.DataBindings.Clear();
            txtProDescription.DataBindings.Clear();
            cbProCategory.DataBindings.Clear();
            pbProImage.DataBindings.Clear();
        }

        private void btnAddPro_Click(object sender, EventArgs e)
        {
            string proName, proCategory, proDescription;
            float proPrice = -1;
            int proQuantity = -1;
            byte[] image = null;
            bool check = true;
            proName = txtProName.Text.Trim();
            proCategory = (string)cbProCategory.SelectedItem;
            proDescription = txtProDescription.Text.Trim();
            if (proName.Length == 0 || proCategory.Length == 0 || proDescription.Length == 0)
            {
                check = false;
            }
            try
            {
                proPrice = float.Parse(txtProPrice.Text.Trim());
                if (proPrice <= 0)
                {
                    check = false;
                }
            }
            catch (Exception)
            {
                check = false;
            }
            try
            {
                proQuantity = int.Parse(txtProQuantity.Text.Trim());
                if (proQuantity <= 0)
                {
                    check = false;
                }
            }
            catch (Exception)
            {
                check = false;
            }
            if (pbProImage.Image == null)
            {
                check = false;
            }
            else
            {
                image = imageToByteArray(pbProImage.Image);
            }

            if (check)
            {
                ToyDTO dto = new ToyDTO
                {
                    name = proName,
                    price = proPrice,
                    quantity = proQuantity,
                    category = proCategory,
                    description = proDescription,
                    image = image
                };

                addToy(dto);
                MessageBox.Show("Add a toy success!");
                btnClearPro_Click(sender, e);
                loadToys();
            }
            else
            {
                MessageBox.Show("Your input is invalid! Please try again!");
            }

        }

        private void dgvProducts_MouseClick(object sender, MouseEventArgs e)
        {
            clearDataBindingForProduct();
            addDataBindingForProduct();
        }

        private void btnSavePro_Click(object sender, EventArgs e)
        {
            try
            {
                int id = int.Parse(txtProID.Text);
                string proName = txtProName.Text.Trim();
                float proPrice = float.Parse(txtProPrice.Text.Trim());
                int proQuantity = int.Parse(txtProQuantity.Text.Trim());
                string proCategory = (string)cbProCategory.SelectedItem;
                string proDescription = txtProDescription.Text.Trim();
                byte[] image = imageToByteArray(pbProImage.Image);
                ToyDTO dto = new ToyDTO
                {
                    id = id,
                    name = proName,
                    price = proPrice,
                    quantity = proQuantity,
                    category = proCategory,
                    description = proDescription,
                    image = image
                };
                updateToy(dto);
                MessageBox.Show("Update toy id" + id + " success!");
                btnClearPro_Click(sender, e);
                loadToys();
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDeletePro_Click(object sender, EventArgs e)
        {
            int id = int.Parse(txtProID.Text);
            DialogResult confirm = MessageBox.Show("Are you sure to delete toy id " + id + " ?",
                                                    "Confirmation", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                deleteToy(id);
                MessageBox.Show("Delete toy id " + id + " success!");
                loadToys();
            }
        }

        private void txtProName_Enter(object sender, EventArgs e)
        {
            txtProName.DataBindings.Clear();
        }

        private void txtProPrice_Enter(object sender, EventArgs e)
        {
            txtProPrice.DataBindings.Clear();
        }

        private void txtProQuantity_Enter(object sender, EventArgs e)
        {
            txtProQuantity.DataBindings.Clear();
        }

        private void txtProDescription_Enter(object sender, EventArgs e)
        {
            txtProDescription.DataBindings.Clear();
        }

        private void txtSearchPro_TextChanged(object sender, EventArgs e)
        {
            string searchPro = txtSearchPro.Text.Trim();
            IEnumerable<ToyDTO> listSearch = listToys.Where(toy => toy.name.Contains(searchPro)).ToList();
            dgvProducts.DataSource = listSearch;
        }

        private void btnUploadProImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileChooser = new OpenFileDialog();
            fileChooser.Title = "Please select a photo";
            fileChooser.Filter = "JPG|*.jpg|PNG|*.png|GIF|*gif";
            fileChooser.Multiselect = false;
            if (fileChooser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.pbProImage.ImageLocation = fileChooser.FileName;
                pbProImage.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void txtProName_Validating(object sender, CancelEventArgs e)
        {
            if (txtProName.Text == "")
            {
                errProduct.SetError(txtProName, "Name is required!");
            }
            else
            {
                errProduct.SetError(txtProName, "");
            }
        }

        private void txtProPrice_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                float price = float.Parse(txtProPrice.Text);
                errProduct.SetError(txtProPrice, "");
            }
            catch (Exception)
            {
                errProduct.SetError(txtProPrice, "Price must be a number!");
            }
        }

        private void txtProQuantity_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                int price = int.Parse(txtProQuantity.Text);
                errProduct.SetError(txtProQuantity, "");
            }
            catch (Exception)
            {
                errProduct.SetError(txtProQuantity, "Quantity must be a number!");
            }
        }

        private void txtProDescription_Validating(object sender, CancelEventArgs e)
        {
            if (txtProDescription.Text == "")
            {
                errProduct.SetError(txtProDescription, "Description is required!");
            }
            else
            {
                errProduct.SetError(txtProDescription, "");
            }
        }

        private void btnUploadProImage_Validating(object sender, CancelEventArgs e)
        {
            if (pbProImage.Image == null)
            {
                errProduct.SetError(btnUploadProImage, "Image is required!");
            }
            else
            {
                errProduct.SetError(btnUploadProImage, "");
            }
        }

        #endregion

        private void button8_Click(object sender, EventArgs e)
        {
            this.Close();
            login.Show();
        }

        #region Feedback_Functions


        private async void loadToyFeedback()
        {
            HttpResponseMessage response = await client.GetAsync(BASE_URL + "Toys\\Feedbacks");
            string stringResponse = response.Content.ReadAsStringAsync().Result;
            listToyFeedback = JsonConvert.DeserializeObject<IEnumerable<ToyDTO>>(stringResponse);
            dgvProFeedback.DataSource = null;
            dgvProFeedback.DataSource = listToyFeedback;

            if (listToyFeedback != null)
            {
                dgvProFeedback.Columns["image"].Visible = false;
                dgvProFeedback.Columns["createdBy"].Visible = false;
                dgvProFeedback.Columns["isActived"].Visible = false;
                dgvProFeedback.Columns["description"].Visible = false;
                dgvProFeedback.Columns["category"].Visible = false;
                dgvProFeedback.Columns["price"].Visible = false;
                dgvProFeedback.Columns["quantity"].Visible = false;
            }
        }

        private async void loadFeedback()
        {
            HttpResponseMessage response = await client.GetAsync(BASE_URL + "Feedbacks");
            string stringResponse = response.Content.ReadAsStringAsync().Result;
            listFeedback = JsonConvert.DeserializeObject<IEnumerable<FeedbackDTO>>(stringResponse);
            dgvFeedback.DataSource = null;
        }

        private async void updateFeedback(FeedbackDTO dto)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(BASE_URL + "Feedbacks/" + dto.id, dto);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dgvProFeedback_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int currentRow = e.RowIndex;
            int toyID = (int)dgvProFeedback.Rows[currentRow].Cells["id"].Value;
            List<FeedbackDTO> feedbacks = new List<FeedbackDTO>();
            foreach (FeedbackDTO feedback in listFeedback)
            {
                if (feedback.toyID == toyID)
                {
                    feedbacks.Add(feedback);
                }
            }
            dgvFeedback.DataSource = null;
            dgvFeedback.DataSource = feedbacks;

            dgvFeedback.Columns["toyID"].Visible = false;
            dgvFeedback.Columns["content"].Visible = false;
            dgvFeedback.Columns["status"].Visible = false;
            dgvFeedback.Columns["id"].Width = 20;

            clearDatabindingForFeedback();
            addDataBindingForFeedback(feedbacks);
        }

        private void clearDatabindingForFeedback()
        {
            txtFeedback.DataBindings.Clear();
        }

        private void addDataBindingForFeedback(List<FeedbackDTO> feedbacks)
        {
            txtFeedback.DataBindings.Add("Text", feedbacks, "content");
        }

        private void btnConfirmFeedback_Click(object sender, EventArgs e)
        {
            if (dgvFeedback.CurrentRow == null)
            {
                if (listToyFeedback.Count() == 0)
                {
                    MessageBox.Show("There are no feedback to confirm!");
                }
                else
                {
                    MessageBox.Show("Please choose a feedback to confirm!");
                }
            }
            else
            {
                int currentRowIndex = dgvFeedback.CurrentRow.Index;
                int feedbackID = (int)dgvFeedback.Rows[currentRowIndex].Cells["id"].Value;
                FeedbackDTO dto = new FeedbackDTO { id = feedbackID, status = 1 };
                updateFeedback(dto);
                updateFeedbackGridview(feedbackID, sender);
            }
        }

        private void btnDeleteFeedback_Click(object sender, EventArgs e)
        {
            if (dgvFeedback.CurrentRow == null)
            {
                if (listToyFeedback.Count() == 0)
                {
                    MessageBox.Show("There are no feedback to confirm!");
                }
                else
                {
                    MessageBox.Show("Please choose a feedback to confirm!");
                }
            }
            else
            {
                int currentRowIndex = dgvFeedback.CurrentRow.Index;
                int feedbackID = (int)dgvFeedback.Rows[currentRowIndex].Cells["id"].Value;
                FeedbackDTO dto = new FeedbackDTO { id = feedbackID, status = -1 };
                updateFeedback(dto);
                updateFeedbackGridview(feedbackID, sender);
            }
        }

        private void updateFeedbackGridview(int feedbackID, Object sender)
        {
            listFeedback = listFeedback.Where(feedback => feedback.id != feedbackID).ToList();
            dgvProFeedback_CellClick(sender, new DataGridViewCellEventArgs(0, dgvProFeedback.CurrentRow.Index));
            if (dgvFeedback.Rows.Count == 0)
            {
                int toyID = (int)dgvProFeedback.Rows[dgvProFeedback.CurrentRow.Index].Cells["id"].Value;
                listToyFeedback = listToyFeedback.Where(toy => toy.id != toyID).ToList();
                dgvProFeedback.DataSource = null;
                dgvProFeedback.DataSource = listToyFeedback;

                dgvProFeedback.Columns["image"].Visible = false;
                dgvProFeedback.Columns["createdBy"].Visible = false;
                dgvProFeedback.Columns["isActived"].Visible = false;
                dgvProFeedback.Columns["description"].Visible = false;
                dgvProFeedback.Columns["category"].Visible = false;
                dgvProFeedback.Columns["price"].Visible = false;
                dgvProFeedback.Columns["quantity"].Visible = false;
            }
        }



        #endregion

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnLogout_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(btnLogout, "Logout");
        }

        private void btnMinimize_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(btnMinimize, "Minimize");
        }

        private void btnExit_MouseHover(object sender, EventArgs e)
        {
            toolTip.SetToolTip(btnExit, "Exit program");
        }

        private void btnWelcomeProduct_Click(object sender, EventArgs e)
        {
            TabControl.SelectedIndex = 1;
        }

        private void btnWelcomeEmployee_Click(object sender, EventArgs e)
        {
            TabControl.SelectedIndex = 4;
        }

        private void btnWelcomeOrder_Click(object sender, EventArgs e)
        {
            TabControl.SelectedIndex = 2;
        }

        private void btnWelcomeCustomer_Click(object sender, EventArgs e)
        {
            TabControl.SelectedIndex = 3;
        }


    }
}

