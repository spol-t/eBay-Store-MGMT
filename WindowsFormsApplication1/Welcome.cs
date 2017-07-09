using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StoreMGMT
{
    public partial class Welcome : Form
    {
        private string settingsFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\SotreSettings.txt";
        private string storeName;
        private DBMySQL DB = new DBMySQL();
        private Person user = new Person();
        private Person client = new Person();
        private Item item = new Item();
        private Item pack = new Item();
        private Shipment ship = new Shipment();
        private string[] shipToCountrys = new string[256];
        private EbayStore myStore = new EbayStore();
        private string storeType;
        private string[] storeSettings = new string[12];


        public Welcome()
        {
            InitializeComponent();
        }

        private void Welcome_Load(object sender, EventArgs e)
        {
            
        }

        //select for comboBox - store type
        private void cmbStoreType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            storeType = (string)cmb.SelectedValue;
        }


        
        //Move to the setting s screen
        private void startBtn_Click(object sender, EventArgs e)
        {
            Main.Show();
            //check if there is a StoreSettings.txt file
            if (File.Exists(settingsFilePath))
            { 
                storeSettings = File.ReadAllLines(settingsFilePath);
                FillSetingsTab();
                settingsTb.Enabled = false;
            }
            else
            {
                addItemPackTb.Enabled = false;
                addClientTb.Enabled = false;
            }
        }





        #region Input Validation

        //only allow letters and spaces in textbox
        private void OnlyLetters_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && 
                    (e.KeyChar != ' '))
                {
                    e.Handled = true;
                }
            }

            

        //only alow numbers in textbox
        private void OnlyIntNum_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
    
            //only alow numbers and '+' at the start textbox
            private void OnlyPhoneNum_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                    (e.KeyChar != '+'))
                {
                    e.Handled = true;
                }
    
                // only allow one '+' at the start of the phone number
                if ((e.KeyChar == '+') && (((sender as TextBox).Text.IndexOf('+') > -1) ||
                    ((sender as TextBox).Text.Length > 0)))
                {
                    e.Handled = true;
                }
            }

            //only alow float numbers in textbox
            private void OnlyFloatNum_KeyPress(object sender, KeyPressEventArgs e)  
            {   
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&   
                (e.KeyChar != '.'))
                {
                    e.Handled = true;
                }
    
                // only allow one decimal point
                if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
                {
                    e.Handled = true;
                }
            }
        #endregion
    
        #region Settings
            private void applySettingsBtn_Click(object sender, EventArgs e)
            {
            
                int requiered = 0;

            //checking that all the requiered field are full before continue

            //Store Name
            if (!string.IsNullOrEmpty(storeNameTbx.Text))
            {
                requiered++; 
                storeName = storeNameTbx.Text;
                storeSettings[0] = storeName;
                RemoveUnderline(lblStoreName);
            }
            else
            {
                UnderlineReqierdField(lblStoreName);
            }

            //Store Type
            if (cmbStoreType.SelectedIndex > -1)
            {
                requiered++;
                myStore.Type = (string)cmbStoreType.SelectedItem;
                storeSettings[1] = myStore.Type;
                RemoveUnderline(lblStoreType);
            }
            else
            {
                UnderlineReqierdField(lblStoreType);
            }

            //User First Name
            if (!string.IsNullOrEmpty(userFirstNameTbx.Text))
            {
                requiered++;
                user.FirstName = userFirstNameTbx.Text;
                storeSettings[2] = user.FirstName;
                RemoveUnderline(lblUserFirstName);
            }
            else
            {
                UnderlineReqierdField(lblUserFirstName);
            }

            //User Last Name
            if (!string.IsNullOrEmpty(userLastNameTbx.Text))
            {
                requiered++;
                user.LastName = userLastNameTbx.Text;
                storeSettings[3] = user.LastName;
                RemoveUnderline(lblUserLastName);
            }
            else
            {
                UnderlineReqierdField(lblUserLastName);
            }

            //User address1
            if (!string.IsNullOrEmpty(userAddress1Tbx.Text))
            {
                requiered++;
                user.Address1 = userAddress1Tbx.Text;
                storeSettings[4] = user.Address1;
                RemoveUnderline(lblUserAddress1);
            }
            else
            {
                UnderlineReqierdField(lblUserAddress1);
            }

            //User City
            if (!string.IsNullOrEmpty(userCityTbx.Text))
            {
                requiered++;
                user.City = userCityTbx.Text;
                storeSettings[6] = user.City;
                RemoveUnderline(lblUserCity);
            }
            else
            {
                UnderlineReqierdField(lblUserCity);
            }

            //User State/Province
            if (!string.IsNullOrEmpty(userStateTbx.Text))
            {
                requiered++;
                user.State_province = userStateTbx.Text;
                storeSettings[7] = user.State_province;
                RemoveUnderline(lblUserState);
            }
            else
            {
                UnderlineReqierdField(lblUserState);
            }

            //User Zip/Postal Code
            if (!string.IsNullOrEmpty(userZipTbx.Text))
            {
                requiered++;
                user.Zip_postal = userZipTbx.Text;
                storeSettings[8] = user.Zip_postal;
                RemoveUnderline(lblUserZip);
            }
            else
            {
                UnderlineReqierdField(lblUserZip);
            }

            //User Country
            if (!string.IsNullOrEmpty(userCountryTbx.Text))
            {
                requiered++;
                user.Country = userCountryTbx.Text;
                storeSettings[9] = user.Country;
                RemoveUnderline(lblUserCountry);
            }
            else
            {
                UnderlineReqierdField(lblUserCountry);
            }

            //User Phone
            if (!string.IsNullOrEmpty(userPhoneTbx.Text))
            {
                requiered++;
                user.Phone = userPhoneTbx.Text;
                storeSettings[10] = user.Phone;
                RemoveUnderline(lblUserPhone);
            }
            else
            {
                UnderlineReqierdField(lblUserPhone);
            }

            //User Email
            if (!string.IsNullOrEmpty(userEmailTbx.Text))
            {
                if (IsValidEmail(userEmailTbx.Text))
                {
                    lblValidEmail.Visible = false;
                    requiered++;
                    user.Email = userEmailTbx.Text;
                    storeSettings[11] = user.Email;
                    RemoveUnderline(lblUserEmail);
                }

                else
                {
                    
                    lblValidEmail.Visible = true;
                    UnderlineReqierdField(lblUserEmail);
                }
                
            }
            else
            {
                UnderlineReqierdField(lblUserEmail);
            }

            //Check that all the required fields are full and submit to database
            if (requiered < 11)
            {
                
                errorTbx.Visible = true;
            }
            else
            {
                errorTbx.Visible = false;
                
                //Address2 is optional
                if (!string.IsNullOrEmpty(user.Address2))
                    storeSettings[5] = user.Address2;
                else
                    storeSettings[5] = "";

                using (var tw = new StreamWriter(settingsFilePath))
                {
                    for (var i = 0; i < storeSettings.Length; i++)
                    {
                        tw.WriteLine(storeSettings[i]);
                    }
                    tw.Close();
                }

                settingsTb.Enabled = false;
                addItemPackTb.Enabled = true;
                addClientTb.Enabled = true;
                lblAddItemInfo.Visible = false;
            }

        }
        #endregion

        #region Add Item or Pack

        #region Add Item
        //add a item to the data base
        private void addItemBtn_Click(object sender, EventArgs e)
        {
           
            int requierd = 0;
            string itemErr = "Error";

            //Item Barcode
            if (!string.IsNullOrEmpty(itemBarcodeTbx.Text))
            {
                requierd++;
                item.Barcode = itemBarcodeTbx.Text;
            }
            else
            {
                itemErr += " Barcode is missing!";
            }

            //Item Weight
            if (!string.IsNullOrEmpty(itemWeightTbx.Text))
            {
                requierd++;
                item.Weight = Convert.ToDouble(itemWeightTbx.Text);
            }
            else
            {
                itemErr += " Weight is missing!";
            }

            //Item Cost
            if (!string.IsNullOrEmpty(itemCostTbx.Text))
            {
                requierd++;
                item.CostILS = Convert.ToDouble(itemCostTbx.Text);
            }
            else
            {
                itemErr += " Cost is missing!";
            }

            //Item Description
            if (!string.IsNullOrEmpty(itemDescriptionTbx.Text))
            {
                requierd++;
                item.Description = itemDescriptionTbx.Text;
            }
            else
            {
                itemErr += " Description is missing!";
            }

            //Item quantity
            if (!string.IsNullOrEmpty(itemQuantityTbx.Text))
            {
                requierd++;
                item.Quantity = Convert.ToInt32(itemQuantityTbx.Text);
            }
            else
            {
                itemErr += " Quantity is missing!";
            }

            if (requierd < 5)
            {
                lblAddItemInfo.Text = itemErr;
                lblAddItemInfo.Visible = true;
            }
            else
            {
                if (DB.IsItemPackExist(item, "items"))
                {
                    lblAddItemInfo.Text = "item already exist, would you like to update it?";
                    lblAddItemInfo.Visible = true;
                    updateItemBtn.Visible = true;
                }
                else
                {
                    lblAddItemInfo.ForeColor = Color.Blue;
                    lblAddItemInfo.Text = "Item Add Secssesfully";
                    lblAddItemInfo.Visible = true;
                    DB.InsertItemForSale(item);
                    ClearItem(item);
                }
            }            
        }

        //Update item
        private void updateItmeBtn_Click(object sender, EventArgs e)
        {
            DB.UpdateItem(item, "items");
            ClearTextBoxes(addItemGb);
            lblAddPackInfo.Text = "Item Updated Secssesfully";
            updateItemBtn.Visible = false;
        }
        #endregion

        #region Add Pack
        private void addPackBtn_Click(object sender, EventArgs e)
        {
            
            int requierd = 0;
            string packErr = "Error";

            //Pack Barcode
            if (!string.IsNullOrEmpty(packBarcodeTbx.Text))
            {
                requierd++;
                pack.Barcode = packBarcodeTbx.Text;
            }
            else
            {
                packErr += " Barcode is missing!";
            }

            //Pack Weight
            if (!string.IsNullOrEmpty(packWeightTbx.Text))
            {
                requierd++;
                pack.Weight = Convert.ToDouble(packWeightTbx.Text);
            }
            else
            {
                packErr += " Weight is missing!";
            }

            //Pack Cost
            if (!string.IsNullOrEmpty(packCostTbx.Text))
            {
                requierd++;
                pack.CostILS = Convert.ToDouble(packCostTbx.Text);
            }
            else
            {
                packErr += " Cost is missing!";
            }

            //Pack Description
            if (!string.IsNullOrEmpty(packDescriptionTbx.Text))
            {
                requierd++;
                pack.Description = packDescriptionTbx.Text;
            }
            else
            {
                packErr += " Description is missing!";
            }

            //Pack quantity
            if (!string.IsNullOrEmpty(packQuantityTbx.Text))
            {
                requierd++;
                pack.Quantity = Convert.ToInt32(packQuantityTbx.Text);
            }
            else
            {
                packErr += " Quantity is missing!";
            }

            if (requierd < 4)
            {
                lblAddPackInfo.Text = packErr;
                lblAddPackInfo.Visible = true;
            }
            else
            {
                if (DB.IsItemPackExist(pack, "packs"))
                {
                    lblAddPackInfo.Text = "pack already exist, would you like to update it?";
                    lblAddPackInfo.Visible = true;
                    updatePackBtn.Visible = true;
                }
                else
                {
                    lblAddPackInfo.ForeColor = Color.Blue;
                    lblAddPackInfo.Text = "Item Add Secssesfully";
                    lblAddPackInfo.Visible = true;
                    DB.InsertPack(pack);
                    ClearItem(pack);
                }
            }
        }

        //update pack
        private void updatePackBtn_Click(object sender, EventArgs e)
        {
            DB.UpdateItem(pack, "packs");
            ClearTextBoxes(addPackegGb);
            lblAddPackInfo.Text = "Item Updated Secssesfully";
            updatePackBtn.Visible = false;
        }
        #endregion

        #endregion

        #region Add Client

        private void addClientBtn_Click(object sender, EventArgs e)
        {
            
            int requiered = 0;

            //checking that all the requiered field are full before continue

            //Client First Name
            if (!string.IsNullOrEmpty(clientFirstNameTbx.Text))
            {
                requiered++;
                client.FirstName = clientFirstNameTbx.Text;
                RemoveUnderline(lblClientFirstName);
            }
            else
            {
                UnderlineReqierdField(lblClientFirstName);
            }

            //Client Last Name
            if (!string.IsNullOrEmpty(clientLastNameTbx.Text))
            {
                requiered++;
                client.LastName = clientLastNameTbx.Text;
                RemoveUnderline(lblClientLastName);
            }
            else
            {
                UnderlineReqierdField(lblClientLastName);
            }

            //Client address1
            if (!string.IsNullOrEmpty(clientAddress1Tbx.Text))
            {
                requiered++;
                client.Address1 = clientAddress1Tbx.Text;
                RemoveUnderline(lblClientAddress1);
            }
            else
            {
                UnderlineReqierdField(lblClientAddress1);
            }

            //Client City
            if (!string.IsNullOrEmpty(clientCityTbx.Text))
            {
                requiered++;
                client.City = clientCityTbx.Text;
                RemoveUnderline(lblClientCity);
            }
            else
            {
                UnderlineReqierdField(lblClientCity);
            }

            //Client State/Province
            if (!string.IsNullOrEmpty(clientStateTbx.Text))
            {
                requiered++;
                client.State_province = clientStateTbx.Text;
                RemoveUnderline(lblClientState);
            }
            else
            {
                UnderlineReqierdField(lblClientState);
            }

            //Client Zip/Postal Code
            if (!string.IsNullOrEmpty(clientZipTbx.Text))
            {
                requiered++;
                client.Zip_postal = clientZipTbx.Text;
                RemoveUnderline(lblClientZip);
            }
            else
            {
                UnderlineReqierdField(lblClientZip);
            }

            //Client Country
            if (cmbClientCountry.SelectedIndex > -1)
            {
                requiered++;
                client.Country = (string)cmbClientCountry.SelectedItem;
                RemoveUnderline(lblClientCountry);
            }
            else
            {
                UnderlineReqierdField(lblClientCountry);
            }

            //Client Phone
            if (!string.IsNullOrEmpty(clientPhoneTbx.Text))
            {
                requiered++;
                client.Phone = clientPhoneTbx.Text;
                RemoveUnderline(lblUserPhone);
            }
            else
            {
                UnderlineReqierdField(lblUserPhone);
            }

            //Client Email
            if (!string.IsNullOrEmpty(clientEmailTbx.Text))
            {
                //valdating thet the email entered is in the correct format
                if (IsValidEmail(clientEmailTbx.Text))
                {
                    lblClientValidEmail.Visible = false;
                    requiered++;
                    client.Email = clientEmailTbx.Text;
                    RemoveUnderline(lblUserEmail);
                }

                else
                {

                    lblClientValidEmail.Visible = true;
                    UnderlineReqierdField(lblUserEmail);
                }

            }
            else
            {
                UnderlineReqierdField(lblUserEmail);
            }

            //Check that all the required fields are full and submit to database
            if (requiered < 9)
            {
                addClientInfoTbx.Visible = true;
            }
            else
            {
                if (DB.IsClientExist(client))
                {
                    addClientInfoTbx.Text = "Client already exist, would like to updete it?";
                    addClientInfoTbx.Visible = true;
                    updateClientBtn.Visible = true;

                }
                else
                {
                    addClientInfoTbx.Visible = false;

                    if (!string.IsNullOrEmpty(user.Address2))
                        client.Address2 = clientAddress2Tbx.Text;
                    else
                        client.Address2 = "";

                    addClientInfoTbx.ForeColor = Color.Blue;
                    addClientInfoTbx.Text = "Client Add Secssesfully";
                    addClientInfoTbx.Visible = true;
                    DB.InsertClient(client);
                    ClearTextBoxes(addClientTb);
                }
            }
        }

        //update client
        private void updateClientBtn_Click(object sender, EventArgs e)
        {
            DB.UpdateClient(client);
            ClearTextBoxes(addClientTb);
            addClientInfoTbx.Text = "Client Updated Secssesfully";
            updateClientBtn.Visible = false;
        }
        #endregion

        #region Add Shippind Destination

        private void addShippingBtn_Click(object sender, EventArgs e)
        {

            int requiered = 0;

            //checking that all the requiered field are full before continue

            //Shipping country
            if (!string.IsNullOrEmpty(shippingCountryTbx.Text))
            {
                requiered++;
                ship.Country = shippingCountryTbx.Text;
                RemoveUnderline(lblShippingCountry);
            }
            else
            {
                UnderlineReqierdField(lblShippingCountry);
            }

            //Shipping Min Weight
            if (!string.IsNullOrEmpty(shippingMinWeightTbx.Text))
            {
                requiered++;
                ship.MinWeight = Convert.ToInt32(shippingMinWeightTbx.Text);
                RemoveUnderline(lblShippingMinWeight);
            }
            else
            {
                UnderlineReqierdField(lblShippingMinWeight);
            }

            //Shipping Max Weight
            if (!string.IsNullOrEmpty(shippingMaxWeightTbx.Text))
            {
                requiered++;
                ship.MaxWeight = Convert.ToInt32(shippingMaxWeightTbx.Text);
                RemoveUnderline(lblShippingMaxWeight);
            }
            else
            {
                UnderlineReqierdField(lblShippingMaxWeight);
            }

            //Shipping Price
            if (!string.IsNullOrEmpty(shippingPriceTbx.Text))
            {
                requiered++;
                ship.Price = Convert.ToDouble(shippingPriceTbx.Text);
                RemoveUnderline(lblShippingPrice);
            }
            else
            {
                UnderlineReqierdField(lblShippingPrice);
            }

            //Shipping is Registered
            ship.Registered = Convert.ToInt32(shippingRegYesRbt.Checked);

            if (requiered < 4)
            {
                shippingErrorTbx.Visible = true;
            }
            else
            {
                if (DB.IsShipmentExist(ship))
                {
                    shippingErrorTbx.Text = "The Requested configuration already exist," +
                        "Whould you like to update it's price and min/max Weight?";
                    shippingErrorTbx.Visible = true;
                    updateShippingBtn.Visible = true;
                }
            }
        }

        //update existing shipping
        private void updateShippingBtn_Click(object sender, EventArgs e)
        {
            DB.UpdateShipping(ship);
            ClearTextBoxes(shipping);
            shippingErrorTbx.Text = "Shipping Updated Secssesfully";
            updateShippingBtn.Visible = false;
        }
        #endregion

        #region Sales

        
        #endregion

        #region General Methodes
        //add underline to requierd field not filled
        public void UnderlineReqierdField(object sender)
        {
            ((Label)sender).Font = new Font(((Label)sender).Font.Name,
                    ((Label)sender).Font.SizeInPoints, FontStyle.Underline);
        }

        //remove underline
        public void RemoveUnderline(object sender)
        {
            ((Label)sender).Font = new Font(((Label)sender).Font.Name,
                    ((Label)sender).Font.SizeInPoints, FontStyle.Regular);
        }
        
        
        private void Main_SelectedIndexChanged(object sender, EventArgs e)
        {
            //show message box if store was not yet configured
            if (!File.Exists(settingsFilePath) && Main.SelectedIndex != 0)
            {
                MessageBox.Show("Store Settings must be configured first!");

            }

            //fill sale number text box when moving to sales tab
            if(Main.SelectedIndex == 4 )
            {
                saleNumberTbx.Text = Convert.ToString(DB.GetNextSaleNum());
            }
        }

        //clear item data
        private void ClearItem (Item item)
        {
            item.Barcode = "";
            item.CostILS = 0;
            item.Description = "";
            item.CostILS = 0;
        }

        //verify a valid email address
        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        //clean conmbobox and textbox
        public void ClearTextBoxes(Control control)
        {
            foreach (Control c in control.Controls)
            {
                if (c is TextBox)
                {
                    ((TextBox)c).Clear();
                }
                
                if(c is ComboBox)
                {
                    ((ComboBox)c).SelectedIndex = -1;
                }

            }
        }

        //fill the setings tab
        public void FillSetingsTab()
        {
            storeNameTbx.Text = storeSettings[0];
            cmbStoreType.Text = storeSettings[1];
            userFirstNameTbx.Text = storeSettings[2];
            userLastNameTbx.Text = storeSettings[3];
            userAddress1Tbx.Text = storeSettings[4];
            userAddress2Tbx.Text = storeSettings[5];
            userCityTbx.Text = storeSettings[6];
            userStateTbx.Text = storeSettings[7];
            userZipTbx.Text = storeSettings[8];
            userCountryTbx.Text = storeSettings[9];
            userPhoneTbx.Text = storeSettings[10];
            userEmailTbx.Text = storeSettings[11];
            
        }
        #region Update Combobox

        //update client's country combobox 
        private void cmbClientCountry_Enter(object sender, EventArgs e)
        {
            //remove all the itms 
            cmbClientCountry.Items.Clear();
            //geting the updated lists of countrys withe avilable shipment
            shipToCountrys = DB.GetCountrys();
            foreach (string country in shipToCountrys)
            {
                if (!(country == "") && !(country == null))
                {
                    cmbClientCountry.Items.Add(country);
                }
            }
        }

        #endregion

        #endregion


    }
}
