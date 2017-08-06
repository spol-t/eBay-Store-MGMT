using System;
using System.Collections;
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
using iTextSharp; 
using iTextSharp.text;
using iTextSharp.text.pdf; 

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
        private string[] itemsBarcodes = new string[256];
        private string[] packsBarcodes = new string[256];
        private ArrayList clientsEmails = new ArrayList();
        private EbayStore myStore = new EbayStore();
        private string storeType;
        private string[] storeSettings = new string[12];
        private double paypalConFee = 0.3;
        private double paypalFinalValFee = 0.029;
        private int itemsInSale = 0;
        private int packsInSale = 0;
        private bool updateSettingsFlag = false;


        public Welcome()
        {
            InitializeComponent();
        }

        private void Welcome_Load(object sender, EventArgs e)
        {
            
        }
        
        //Move to the setting s screen
        private void startBtn_Click(object sender, EventArgs e)
        {
            Main.Show();
            //check if there is a StoreSettings.txt file
            if (File.Exists(settingsFilePath))
            { 
                storeSettings = File.ReadAllLines(settingsFilePath);
                if (storeSettings[1] == "" || storeSettings[1] == null)
                {
                    addItemPackTb.Enabled = false;
                    addClientTb.Enabled = false;
                    shippingTb.Enabled = false;
                    salesTb.Enabled = false;
                    saleDetailsTb.Enabled = false;
                    reportsTb.Enabled = false;
                    settingsUpdateBtn.Enabled = false;
                }
                else
                {
                    FillSetingsTab();
                    settingsPanel.Enabled = false;
                    settingsUpdateBtn.Enabled = true;
                }
                
            }
            else
            {
                addItemPackTb.Enabled = false;
                addClientTb.Enabled = false;
                shippingTb.Enabled = false;
                salesTb.Enabled = false;
                saleDetailsTb.Enabled = false;
                reportsTb.Enabled = false;
                settingsUpdateBtn.Enabled = false;
            }
        }
       

        #region Input Validation

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

        //only allow letters and spaces in textbox
        private void OnlyLetters_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && 
                    (e.KeyChar != ' '))
                {
                    e.Handled = true;
                }
            }

            

        //only allow numbers in textbox
        private void OnlyIntNum_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
    
            //only allow numbers and '+' at the start textbox
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

            //only allow float numbers in textbox
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


        //select for comboBox - store type
        private void cmbStoreType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            storeType = (string)cmb.SelectedValue;
        }

        //apply settings
        private void applySettingsBtn_Click(object sender, EventArgs e)
            {
            
                int requiered = 0;

            //checking that all the required field are full before continue

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
                myStore = DB.GetEbayStore((string)cmbStoreType.SelectedItem);
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
                if (!string.IsNullOrEmpty(userAddress2Tbx.Text))
                    storeSettings[5] = userAddress2Tbx.Text;
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

                settingsPanel.Enabled = false;
                settingsUpdateBtn.Enabled = true;
                addItemPackTb.Enabled = true;
                addClientTb.Enabled = true;
                shippingTb.Enabled = true;
                salesTb.Enabled = true;
                saleDetailsTb.Enabled = true;
                reportsTb.Enabled = true;
                lblAddItemInfo.Visible = false;
            }

        }


        //update settings
        private void settingsUpdateBtn_Click(object sender, EventArgs e)
        {
            //disable all tabs except from settings tab
            addItemPackTb.Enabled = false;
            addClientTb.Enabled = false;
            shippingTb.Enabled = false;
            salesTb.Enabled = false;
            saleDetailsTb.Enabled = false;
            reportsTb.Enabled = false;
            settingsUpdateBtn.Enabled = false;
            settingsPanel.Enabled = true;

            //disable update button;
            settingsUpdateBtn.Enabled = false;
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
                item.Cost = Convert.ToDouble(itemCostTbx.Text);
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
                if (DB.IsItemPackExist(item.Barcode, "items", ""))
                {
                    lblAddItemInfo.Text = "item already exist, would you like to update it?";
                    lblAddItemInfo.Visible = true;
                    updateItemBtn.Visible = true;
                    addItemClearBtn.Visible = true;
                    addItemClearBtn.Visible = true;
                }
                else
                {
                    lblAddItemInfo.ForeColor = Color.Blue;
                    lblAddItemInfo.Text = "Item Add Successfully";
                    lblAddItemInfo.Visible = true;
                    DB.InsertItemForSale(item);
                    ClearItem(item);
                    ClearTextBoxes(addItemGb);
                }
            }            
        }

        //Update item
        private void updateItmeBtn_Click(object sender, EventArgs e)
        {
            DB.UpdateItem(item, "items");
            ClearTextBoxes(addItemGb);
            lblAddItemInfo.ForeColor = Color.Blue;
            lblAddItemInfo.Text = "Item Updated Successfully";
            updateItemBtn.Visible = false;
            addItemClearBtn.Visible = false;
        }

        private void addItemClearBtn_Click(object sender, EventArgs e)
        {
            ClearTextBoxes(addItemGb);
            updateItemBtn.Visible = false;
            addItemClearBtn.Visible = false;
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
                pack.Cost = Convert.ToDouble(packCostTbx.Text);
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
                if (DB.IsItemPackExist(pack.Barcode, "packs" , ""))
                {
                    lblAddPackInfo.Text = "pack already exist, would you like to update it?";
                    lblAddPackInfo.Visible = true;
                    updatePackBtn.Visible = true;
                    addPackClearBtn.Visible = true;
                }
                else
                {
                    lblAddPackInfo.ForeColor = Color.Blue;
                    lblAddPackInfo.Text = "Item Add Successfully";
                    lblAddPackInfo.Visible = true;
                    DB.InsertPack(pack);
                    ClearItem(pack);
                    ClearTextBoxes(addPackGb);    
                }
            }
        }

        //update pack
        private void updatePackBtn_Click(object sender, EventArgs e)
        {
            DB.UpdateItem(pack, "packs");
            ClearTextBoxes(addPackGb);
            lblAddPackInfo.ForeColor = Color.Blue;
            lblAddPackInfo.Text = "Item Updated Successfully";
            updatePackBtn.Visible = false;
            addPackClearBtn.Visible = false;
        }

        private void addPackClearBtn_Click(object sender, EventArgs e)
        {
            ClearTextBoxes(addPackGb);
            updatePackBtn.Visible = false;
            addPackClearBtn.Visible = false;
        }

        #endregion

        #endregion

        #region Add Client

        //update client's country combobox 
        private void cmbClientCountry_Enter(object sender, EventArgs e)
        {
            //remove all the items 
            cmbClientCountry.Items.Clear();
            //getting the updated lists of countrys withe available shipment
            shipToCountrys = DB.GetCountrys();
            foreach (string country in shipToCountrys)
            {
                if (!(country == "") && !(country == null))
                {
                    cmbClientCountry.Items.Add(country);
                }
            }
        }

        private void addClientBtn_Click(object sender, EventArgs e)
        {
            
            int requiered = 0;

            //checking that all the required field are full before continue

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
                //validating that the email entered is in the correct format
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
                    addClientInfoTbx.Text = "Client already exist, would like to update it?";
                    addClientInfoTbx.Visible = true;
                    updateClientBtn.Visible = true;
                    addClientClearBtn.Visible = true;

                }
                else
                {
                    addClientInfoTbx.Visible = false;

                    if (!string.IsNullOrEmpty(clientAddress2Tbx.Text))
                        client.Address2 = clientAddress2Tbx.Text;
                    else
                        client.Address2 = "";

                    ClearTextBoxes(addClientTb);
                    addClientInfoTbx.ForeColor = Color.Blue;
                    addClientInfoTbx.Text = "Client Add Successfully";
                    addClientInfoTbx.Visible = true;
                    DB.InsertClient(client);
                    
                }
            }
        }

        //update client
        private void updateClientBtn_Click(object sender, EventArgs e)
        {
            DB.UpdateClient(client);
            ClearTextBoxes(addClientTb);
            addClientInfoTbx.ForeColor = Color.Blue;
            addClientInfoTbx.Text = "Client Updated Successfully";
            updateClientBtn.Visible = false;
            addClientClearBtn.Visible = false;
        }

        private void addClientClearBtn_Click(object sender, EventArgs e)
        {
            ClearTextBoxes(addClientTb);
            updateClientBtn.Visible = false;
            addClientClearBtn.Visible = false;
        }

        #endregion

        #region Add Shipping Destination

        private void addShippingBtn_Click(object sender, EventArgs e)
        {

            int requiered = 0;

            //checking that all the required field are full before continue

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
                        "Would you like to update it's price and min/max Weight?";
                    shippingErrorTbx.ForeColor = Color.Red;
                    shippingErrorTbx.Visible = true;
                    updateShippingBtn.Visible = true;
                }
                else
                {
                    DB.InsertShipment(ship);
                    ClearTextBoxes(shippingTb);
                    shippingErrorTbx.Text = "Shipping add successfully";
                    shippingErrorTbx.ForeColor = Color.Blue;
                    shippingErrorTbx.Visible = true;
                    addShippingClearBtn.Visible = true;
                }
            }
        }

        //update existing shipping
        private void updateShippingBtn_Click(object sender, EventArgs e)
        {
            DB.UpdateShipping(ship);
            ClearTextBoxes(shippingTb);
            shippingErrorTbx.Text = "Shipping Updated Successfully";
            updateShippingBtn.Visible = false;
            addShippingClearBtn.Visible = false;
        }

        private void addShippingClearBtn_Click(object sender, EventArgs e)
        {
            ClearTextBoxes(shippingTb);
            updateShippingBtn.Visible = false;
            addShippingClearBtn.Visible = false;
        }
        #endregion

        #region Sales

        //retrieving sales data when entering the sale tab
        private void sales_Enter(object sender, EventArgs e)
        {
            //fill sale number text box when moving to sales tab
             saleNumberTbx.Text = Convert.ToString(DB.GetNextSaleNum());

            //Upload data to the DataGridView
            saleExistViewDgv.DataSource = DB.GetExistingSaleDetails(Convert.ToInt32(saleNumberTbx.Text));

            //check if the sale process for the above sale number was started before
            if (DB.IsSaleEmpty(Convert.ToInt32(saleNumberTbx.Text)) != "yes")
            {
                saleExistGb.Visible = true;
                saleFinalizeBtn.Enabled = false;
                saleExistTbx.Text = "Sale number: " + Convert.ToString(Convert.ToInt32(saleNumberTbx.Text)) +
                    "\r\nis in process. what would you like to do?";
            }
        }

        //update items barcode in sale screen
        private void cmbSaleItemBarcode_Enter(object sender, EventArgs e)
        {
            //remove all the items 
            cmbSaleItemBarcode.Items.Clear();
            //getting the updated lists of countrys withe available shipment
            itemsBarcodes = DB.GetItemsBarcodes("items");
            foreach (string barcode in itemsBarcodes)
            {
                if (!(barcode == "") && !(barcode == null))
                {
                    cmbSaleItemBarcode.Items.Add(barcode);
                }
            }
        }

        //update pack barcodes in sale screen
        private void cmbSalePackBarcode_Enter(object sender, EventArgs e)
        {
            //remove all the items 
            cmbSalePackBarcode.Items.Clear();
            //getting the updated lists of countrys withe available shipment
            packsBarcodes = DB.GetItemsBarcodes("packs");
            foreach (string barcode in packsBarcodes)
            {
                if (!(barcode == "") && !(barcode == null))
                {
                    cmbSalePackBarcode.Items.Add(barcode);
                }
            }
        }

        private void cmbSaleEmails_Enter(object sender, EventArgs e)
        {
            //remove all the items 
            cmbSaleEmails.Items.Clear();
            //getting the updated lists of countrys withe available shipment
            clientsEmails = DB.GetClientEmail();
            foreach (string email in clientsEmails)
            {
                if (!(email == "") && !(email == null))
                {
                    cmbSaleEmails.Items.Add(email);
                }
            }
        }

        //Adding an item to a sale
        private void saleAddItemBtn_Click(object sender, EventArgs e)
        {
            int requiered = 0;
            //check if an item was selected
            if (cmbSaleItemBarcode.SelectedIndex > -1)
            {
                requiered++;
                RemoveUnderline(lblSaleItemBarcode);
            }
            else
            {
                UnderlineReqierdField(lblSaleItemBarcode);
            }

            //check if quantity was entered
            if (saleItemQuantityTbx.Text != "" )
            {
                requiered++;
                RemoveUnderline(lblSaleItemQuantity);
            }
            else
            {
                UnderlineReqierdField(lblSaleItemQuantity);
            }

            //check if all the demands were filled and add th item
            if (requiered < 2)
            {
                lblSaleInfo.Text = "* Please fill all the required fields!";
                lblSaleInfo.ForeColor = Color.Red;
                lblSaleInfo.Visible = true;
            }
            else if (DB.IsThereEnoughStock(cmbSaleItemBarcode.Text, "items",
                Convert.ToInt16(saleItemQuantityTbx.Text))) 
            {
                DB.UpdateSaleDetails(Convert.ToInt32(saleNumberTbx.Text), 
                    (string)cmbSaleItemBarcode.SelectedItem, 
                    Convert.ToInt32(saleItemQuantityTbx.Text), "item");

                itemsInSale++;

                ////show message that item was add successfully
                lblSaleInfo.Text = "Item add successfully to sale: " + Convert.ToInt32(saleNumberTbx.Text);
                lblSaleInfo.ForeColor = Color.Blue;
                lblSaleInfo.Visible = true;
                ClearTextBoxes(saleAddItemGb);
                if (saleExistViewDgv.Visible == true)
                {
                    saleExistViewDgv.DataSource = DB.GetExistingSaleDetails(Convert.ToInt32(saleNumberTbx.Text));
                    saleExistViewDgv.Update();
                    saleExistViewDgv.Refresh();
                }
            }
            else 
            {
                lblSaleInfo.Text = "There are not enough items in stock!";
                lblSaleInfo.ForeColor = Color.Red;
                lblSaleInfo.Visible = true;
            }
            
        }

        //Adding an item to a sale
        private void saleAddPackBtn_Click(object sender, EventArgs e)
        {
            int requiered = 0;
            //check if a Pack was selected
            if (cmbSalePackBarcode.SelectedIndex > -1)
            {
                requiered++;
                RemoveUnderline(lblSalePackBarcode);
            }
            else
            {
                ;
                UnderlineReqierdField(lblSalePackBarcode);
            }

            //check if quantity was entered
            if (salePackQuantityTbx.Text != "")
            {
                requiered++;
                RemoveUnderline(lblSalePackQuantity);
            }
            else
            {
                UnderlineReqierdField(lblSalePackQuantity);
            }

            //check if all the demands were filled and add th pack
            if (requiered < 2)
            {
                lblSaleInfo.Text = "* Please fill all the required fields!";
                lblSaleInfo.ForeColor = Color.Red;
                lblSaleInfo.Visible = true;
            }
            else if (DB.IsThereEnoughStock(cmbSalePackBarcode.Text, "packs", 
                Convert.ToInt16(salePackQuantityTbx.Text)))
            {
                DB.UpdateSaleDetails(Convert.ToInt32(saleNumberTbx.Text),
                    (string)cmbSalePackBarcode.SelectedItem,
                    Convert.ToInt32(salePackQuantityTbx.Text), "pack");

                packsInSale++;

                ////show message that pack was add successfully
                lblSaleInfo.Text = "Pack add successfully to sale: " + Convert.ToInt32(saleNumberTbx.Text);
                lblSaleInfo.ForeColor = Color.Blue;
                lblSaleInfo.Visible = true;
                ClearTextBoxes(saleAddPackGb);
                if (saleExistViewDgv.Visible == true)
                {
                    saleExistViewDgv.DataSource = DB.GetExistingSaleDetails(Convert.ToInt32(saleNumberTbx.Text));
                    saleExistViewDgv.Update();
                    saleExistViewDgv.Refresh();
                }
            }
            else
            {
                lblSaleInfo.Text = "There are not enough packs in stock!";
                lblSaleInfo.ForeColor = Color.Red;
                lblSaleInfo.Visible = true;
            }
        }

        private void saleFinalizeBtn_Click(object sender, EventArgs e)
        {
            int requiered = 0;
            double payment = 0;

            
            //check if a payment was entered
            if (!string.IsNullOrEmpty(salePaymentTbx.Text))
            {
                requiered++;
                payment = Convert.ToDouble(salePaymentTbx.Text);
                RemoveUnderline(lblSalePaymentReceived);
            }
            else
            {
                UnderlineReqierdField(lblSalePaymentReceived);
                lblSaleInfo.Text = "* Please fill all the required fields!";
                lblSaleInfo.ForeColor = Color.Red;
                lblSaleInfo.Visible = true;
            }

            //check if a client was selected
            if (cmbSaleEmails.SelectedIndex > -1)
            {
                requiered++;
                RemoveUnderline(lblSaleClientEmail);
            }
            else
            {
                UnderlineReqierdField(lblSaleClientEmail);
                lblSaleInfo.Text = "* Please fill all the required fields!";
                lblSaleInfo.ForeColor = Color.Red;
                lblSaleInfo.Visible = true;
            }

            //check if there are items in the sale
            if (itemsInSale > 0)
            {
                requiered++;
            }
            else
            {
                lblSaleInfo.Text = "you must add items to the sale";
                lblSaleInfo.ForeColor = Color.Red;
                lblSaleInfo.Visible = true;
            }

            //check if there are packs in the sale
            if (packsInSale > 0)
            {
                requiered++;
            }
            else
            {
                lblSaleInfo.Text = "you must add packs to the sale";
                lblSaleInfo.ForeColor = Color.Red;
                lblSaleInfo.Visible = true;
            }

            if (requiered == 4)
            {
                Sale s = new Sale();
                s.Shipiing = DB.GetShippingCosts(s.TotalWeight, Convert.ToInt32(saleShippingRegYesRbt.Checked),
                    DB.GetClientCountry(s.ClientEmail));
                if (s.Shipiing == 0)
                {
                    lblSaleInfo.ForeColor = Color.Red;
                    lblSaleInfo.Text = "You don't have a shipping method that contain " +
                        "the total items weight, Please add a new shipping method";
                    lblSaleInfo.Visible = true;
                }
                else
                {
                    s.Number = Convert.ToInt32(saleNumberTbx.Text);
                    s.NumOfItems = DB.GetNumOfItemsInSale(s.Number, "item");
                    s.TotalItemsCost = DB.GetTotalItemsCost(s.Number, "item");
                    s.NumOfPacks = DB.GetNumOfItemsInSale(s.Number, "pack");
                    s.TotalPacksCost = DB.GetTotalItemsCost(s.Number, "pack");
                    s.TotalWeight = DB.GetTotalWeight(s.Number);
                    s.TotalEbayFees = DB.GetEbayFees(s.Number, storeSettings[1], payment, internationalSite.Checked);
                    s.TotalPayPalFees = (payment * paypalFinalValFee) + paypalConFee;
                    s.ClientEmail = (string)cmbSaleEmails.SelectedItem;

                    s.Income = payment;
                    s.TotalCost = s.TotalItemsCost + s.TotalPacksCost + s.TotalEbayFees +
                        s.TotalPayPalFees + s.Shipiing;
                    s.Profit = s.Income - s.TotalCost;

                    DB.FinalizeSale(s);

                    //clean all the cells
                    saleExistGb.Visible = false;
                    saleExistViewDgv.Visible = false;
                    ClearTextBoxes(salesTb);
                    lblSaleInfo.ForeColor = Color.Blue;
                    lblSaleInfo.Text = "Sale Completed!";
                    lblSaleInfo.Visible = true;

                    //get next sale number
                    saleNumberTbx.Text = Convert.ToString(DB.GetNextSaleNum());
                }
            }
        }

        private void saleExistFinalizeBtn_Click(object sender, EventArgs e)
        {
            int requiered = 0;
            double payment = 0;

            //check if a payment was entered
            if (!string.IsNullOrEmpty(salePaymentTbx.Text))
            {
                requiered++;
                payment = Convert.ToDouble(salePaymentTbx.Text);
                RemoveUnderline(lblSalePaymentReceived);
            }
            else
            {
                UnderlineReqierdField(lblSalePaymentReceived);
                lblSaleInfo.Text = "* Please fill all the required fields!";
                lblSaleInfo.ForeColor = Color.Red;
                lblSaleInfo.Visible = true;
            }

            //check if there are items and packs in the sale
            if (DB.CheckSaleItemsPacks(Convert.ToInt32(saleNumberTbx.Text)) == "OK")
            {
                requiered++;
            }
            else
            {
                lblSaleInfo.Text = DB.CheckSaleItemsPacks(Convert.ToInt32(saleNumberTbx.Text));
                lblSaleInfo.ForeColor = Color.Red;
                lblSaleInfo.Visible = true;
            }

            //check if a client was selected
            if (cmbSaleEmails.SelectedIndex > -1)
            {
                requiered++;
                RemoveUnderline(lblSaleClientEmail);
            }
            else
            {
                UnderlineReqierdField(lblSaleClientEmail);
                lblSaleInfo.Text = "* Please fill all the required fields!";
                lblSaleInfo.ForeColor = Color.Red;
                lblSaleInfo.Visible = true;
            }
            if (requiered == 3)
            {
                Sale s = new Sale();
                s.Number = Convert.ToInt32(saleNumberTbx.Text);
                s.TotalWeight = DB.GetTotalWeight(s.Number);
                s.ClientEmail = (string)cmbSaleEmails.SelectedItem;
                s.Shipiing = DB.GetShippingCosts(s.TotalWeight, Convert.ToInt32(saleShippingRegYesRbt.Checked),
                    DB.GetClientCountry(s.ClientEmail));
                if (s.Shipiing == 0)
                {
                    lblSaleInfo.ForeColor = Color.Blue;
                    lblSaleInfo.Text = "You don't have a shipping method that contain " +
                        "the total items weight, Please add a new shipping method";
                    lblSaleInfo.Visible = true;
                }
                else
                {
                    s.NumOfItems = DB.GetNumOfItemsInSale(s.Number, "item");
                    s.TotalItemsCost = DB.GetTotalItemsCost(s.Number, "item");
                    s.NumOfPacks = DB.GetNumOfItemsInSale(s.Number, "pack");
                    s.TotalPacksCost = DB.GetTotalItemsCost(s.Number, "pack"); 
                    s.TotalEbayFees = DB.GetEbayFees(s.Number, storeSettings[1], payment, internationalSite.Checked);
                    s.TotalPayPalFees = (payment * paypalFinalValFee) + paypalConFee;
                    s.Income = payment;
                    s.TotalCost = s.TotalItemsCost + s.TotalPacksCost + s.TotalEbayFees +
                        s.TotalPayPalFees + s.Shipiing;
                    s.Profit = s.Income - s.TotalCost;

                    DB.FinalizeSale(s);

                    //clean all the cells
                    saleExistGb.Visible = false;
                    saleExistViewDgv.Visible = false;
                    ClearTextBoxes(salesTb);
                    lblSaleInfo.ForeColor = Color.Blue;
                    lblSaleInfo.Text = "Sale Completed!";
                    lblSaleInfo.Visible = true;

                    //get next sale number
                    saleNumberTbx.Text = Convert.ToString(DB.GetNextSaleNum());
                }
                
            }
            
        }

        //Delete an existing sale and start a new one
        private void saleExistDeleteBtn_Click(object sender, EventArgs e)
        {
            DB.ClearSale(Convert.ToInt32(saleNumberTbx.Text));
            saleExistViewDgv.Visible = false;
            saleExistGb.Visible = false;
            saleFinalizeBtn.Enabled = true;
            lblSaleInfo.Text = "Sale Was Deleted";
            lblSaleInfo.ForeColor = Color.Blue;
            lblSaleInfo.Visible = true;
            ClearTextBoxes(salesTb);

            //get next sale number
            saleNumberTbx.Text = Convert.ToString(DB.GetNextSaleNum());
        }

        #endregion

        #region Sales Details

        //upload the data to the DataGridView
        private void saleDetails_Enter(object sender, EventArgs e)
        {
            saleDetailsDgv.DataSource = DB.GetSalesDetails();
        }


        //update sale number combobox in sale details
        private void cmbSaleDetailsSelectSale_Enter(object sender, EventArgs e)
        {
            //remove all the items 
            cmbSaleDetailsSelectSale.Items.Clear();
            //getting the updated lists of sales numbers
            string[] salesNums = DB.GetSalesNums();
            foreach (string num in salesNums)
            {
                if (!(num == "") && !(num == null))
                {
                    cmbSaleDetailsSelectSale.Items.Add(num);
                }
            }
        }


        private void saleCDSBtn_Click(object sender, EventArgs e)
        {
            if (cmbSaleDetailsSelectSale.SelectedIndex > -1)
            {
                string saleNum = Convert.ToString(cmbSaleDetailsSelectSale.SelectedItem);

                Document doc = new Document();

                string fileName = ".\\Documents\\CDS_" + saleNum + ".pdf";

                //creating a PDF file
                try
                {
                    FileStream pdfFile = new FileStream(fileName, FileMode.Create);
                    PdfWriter writer = PdfWriter.GetInstance(doc, pdfFile);
                }
                catch
                {
                    MessageBox.Show("Please close the open file first");
                    return;
                }
                

                //------------------fonts----------------
                //Header font
                iTextSharp.text.Font headerFont = new iTextSharp.text.Font(
                    iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.ITALIC);

                //normal font size 10pt
                iTextSharp.text.Font normal10Font = new iTextSharp.text.Font(
                    iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL);

                //normal font size 8pt
                iTextSharp.text.Font normal8Font = new iTextSharp.text.Font(
                    iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL);

                //bold font size 10pt
                iTextSharp.text.Font bold10Font = new iTextSharp.text.Font(
                    iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD);

                //Header
                Paragraph header = new Paragraph("Created by StoreMGMT \u00a9", headerFont);
                header.Alignment = Element.ALIGN_CENTER;
                


                //sender/seller details
                PdfPTable fromTable = new PdfPTable(1);
                fromTable.WidthPercentage = 100;
                PdfPCell fromCell = new PdfPCell(new Phrase("From:", bold10Font));
                fromCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                fromTable.AddCell(fromCell);

                //index starts at 2 to skip on store details
                for (int i = 2; i < storeSettings.Length; i++)
                {
                    fromCell = new PdfPCell(new Phrase(storeSettings[i], normal8Font));
                    fromCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    fromTable.AddCell(fromCell);
                }

                //client details
                PdfPTable toTable = new PdfPTable(1);
                toTable.WidthPercentage = 100;
                PdfPCell toCell = new PdfPCell(new Phrase("To:", bold10Font));
                toCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                toTable.AddCell(toCell);

                //retrieve the client details from the database
                string[] clientDetails = DB.GetClientBySaleNum(
                    Convert.ToInt32(cmbSaleDetailsSelectSale.SelectedItem));

                //insert the details into the table
                for (int i = 0; i < clientDetails.Length; i++)
                {
                    toCell = new PdfPCell(new Phrase(clientDetails[i], normal8Font));
                    toCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    toTable.AddCell(toCell);
                }

                //items in the pack
                PdfPTable itemsTable = new PdfPTable(4);
                itemsTable.WidthPercentage = 100;

                //Description of contents header
                PdfPCell descriptionOfContents = new PdfPCell(
                    new Phrase("Description Of Contents", bold10Font));
                descriptionOfContents.VerticalAlignment = Element.ALIGN_CENTER;
                descriptionOfContents.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER |
                    iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER;
                itemsTable.AddCell(descriptionOfContents);

                //Quantity header
                PdfPCell itemsQuantity = new PdfPCell(new Phrase("Quantity", bold10Font));
                itemsQuantity.HorizontalAlignment = Element.ALIGN_CENTER;
                itemsQuantity.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER |
                    iTextSharp.text.Rectangle.RIGHT_BORDER;
                itemsTable.AddCell(itemsQuantity);

                //weight header
                PdfPCell itemsWeight = new PdfPCell(new Phrase("Weight (g)", bold10Font));
                itemsWeight.HorizontalAlignment = Element.ALIGN_CENTER;
                itemsWeight.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER |
                    iTextSharp.text.Rectangle.RIGHT_BORDER;
                itemsTable.AddCell(itemsWeight);

                //value header
                PdfPCell itemsValue = new PdfPCell(new Phrase("Value ($)", bold10Font));
                itemsValue.HorizontalAlignment = Element.ALIGN_CENTER;
                itemsValue.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER |
                    iTextSharp.text.Rectangle.RIGHT_BORDER;
                itemsTable.AddCell(itemsValue);

                //items details table from the database
                DataTable items = DB.GetItemsForCDS(
                    Convert.ToInt32(cmbSaleDetailsSelectSale.SelectedItem));

                //the total weight of the items
                double weightSum = 0;

                //the total value of the items
                double valueSum = 0;

                //insert the details into the table
                for (int i = 0; i < items.Rows.Count; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        PdfPCell itemD = new PdfPCell(new Phrase(
                            Convert.ToString(items.Rows[i].ItemArray[j]), normal8Font));
                        itemD.HorizontalAlignment = Element.ALIGN_CENTER;
                        itemsTable.AddCell(itemD);

                        if (j == 2)
                            weightSum += Convert.ToDouble(items.Rows[i].ItemArray[j]);
                        if (j == 3)
                            valueSum += Convert.ToDouble(items.Rows[i].ItemArray[j]);
                    }

                }

                //items details table summery
                PdfPCell total = new PdfPCell(new Phrase("Total:", bold10Font));
                total.HorizontalAlignment = Element.ALIGN_CENTER;
                total.Colspan = 2;
                total.BorderWidth = 2f;
                itemsTable.AddCell(total);

                PdfPCell totalWeight = new PdfPCell(new Phrase(
                    Convert.ToString(weightSum), bold10Font));
                totalWeight.BorderWidth = 2f;
                totalWeight.HorizontalAlignment = Element.ALIGN_CENTER;
                itemsTable.AddCell(totalWeight);

                PdfPCell totalValue = new PdfPCell(new Phrase(
                    Convert.ToString(valueSum), bold10Font));
                totalValue.BorderWidth = 2f;
                totalValue.HorizontalAlignment = Element.ALIGN_MIDDLE;
                itemsTable.AddCell(totalValue);

                //goods type

                //checked pic
                iTextSharp.text.Image imgChecked = iTextSharp.text.Image.GetInstance("sCheck.png");
                imgChecked.ScaleAbsolute(10f, 10f);
                Phrase p = new Phrase(new Chunk(imgChecked, 0, 0));
                p.Add(new Phrase("GIft", normal10Font));

                PdfPTable goodsTypeTable = new PdfPTable(5);
                goodsTypeTable.WidthPercentage = 100;
                PdfPCell giftCell = new PdfPCell();
                giftCell.AddElement(p);
                giftCell.NoWrap = true;
                giftCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                goodsTypeTable.AddCell(giftCell);

                PdfPCell otherCell = new PdfPCell(new Phrase("Other", normal10Font));
                otherCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                goodsTypeTable.AddCell(otherCell);

                PdfPCell comSampleCell = new PdfPCell(new Phrase("Commercial Sample", normal10Font));
                comSampleCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                goodsTypeTable.AddCell(comSampleCell);

                PdfPCell docCell = new PdfPCell(new Phrase("Documents", normal10Font));
                docCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                goodsTypeTable.AddCell(docCell);

                PdfPCell retGoodsCell = new PdfPCell(new Phrase("Returned Goods", normal10Font));
                retGoodsCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                goodsTypeTable.AddCell(retGoodsCell);

                PdfPCell goodsTypeCell = new PdfPCell();
                goodsTypeCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                goodsTypeCell.Colspan = 4;
                goodsTypeCell.AddElement(goodsTypeTable);
                itemsTable.AddCell(goodsTypeCell);

                //Main table
                PdfPTable mainTable = new PdfPTable(2);
                mainTable.HorizontalAlignment = Element.ALIGN_CENTER;

                //customs declaration header
                PdfPCell declarCell = new PdfPCell(new Phrase("CUSTOMS DECLARATION", bold10Font));
                declarCell.Rowspan = 2;
                declarCell.BorderWidth = 2f;
                declarCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                declarCell.Border = iTextSharp.text.Rectangle.TOP_BORDER |
                    iTextSharp.text.Rectangle.LEFT_BORDER;
                mainTable.AddCell(declarCell);

                //sticker code
                PdfPCell codeCell = new PdfPCell(new Phrase("CN22", bold10Font));
                codeCell.BorderWidth = 2f;
                codeCell.Border = iTextSharp.text.Rectangle.TOP_BORDER |
                    iTextSharp.text.Rectangle.RIGHT_BORDER;
                mainTable.AddCell(codeCell);

                //May Be Opened Officially notice
                PdfPCell noticeCell = new PdfPCell(new Phrase("May Be Opened Officially", normal10Font));
                noticeCell.BorderWidth = 2f;
                noticeCell.Border = iTextSharp.text.Rectangle.RIGHT_BORDER; 
                mainTable.AddCell(noticeCell);

                //Sender details
                PdfPCell senderCell = new PdfPCell();
                senderCell.BorderWidth = 2f;
                senderCell.AddElement(fromTable);
                senderCell.Border = iTextSharp.text.Rectangle.LEFT_BORDER |
                    iTextSharp.text.Rectangle.TOP_BORDER |
                    iTextSharp.text.Rectangle.BOTTOM_BORDER;
                mainTable.AddCell(senderCell);

                //recipient details
                PdfPCell clientCell = new PdfPCell();
                clientCell.BorderWidth = 2f;
                clientCell.AddElement(toTable);
                clientCell.Border = iTextSharp.text.Rectangle.RIGHT_BORDER |
                    iTextSharp.text.Rectangle.TOP_BORDER |
                    iTextSharp.text.Rectangle.BOTTOM_BORDER;
                mainTable.AddCell(clientCell);

                //table of contents
                PdfPCell contentsCell = new PdfPCell();
                contentsCell.BorderWidth = 2f;
                contentsCell.Colspan = 2;
                contentsCell.AddElement(itemsTable);
                mainTable.AddCell(contentsCell);

                //deceleration
                PdfPCell declerationCell = new PdfPCell(new Phrase(
                    "I, the undersigned, whose name and address are given on this item, " +
                    "certify that the particulars given in this declaration are correct " +
                    "and that this item does not contain any dangerous article or articles " +
                    "prohibited by legislation or by postal or customs regulations", normal8Font));
                declerationCell.BorderWidth = 2f;
                declerationCell.Border = iTextSharp.text.Rectangle.TOP_BORDER |
                    iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER;
                declerationCell.Colspan = 2;
                mainTable.AddCell(declerationCell);

                //signature
                PdfPCell sigCell = new PdfPCell(new Phrase("Sender Signature & Date Signed:"));
                sigCell.BorderWidth = 2f;
                sigCell.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER |
                    iTextSharp.text.Rectangle.RIGHT_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER;
                sigCell.Colspan = 2;
                mainTable.AddCell(sigCell);

                //open the document for writing
                doc.Open();

                //doc header
                doc.Add(header);
                doc.Add(new Paragraph("\n\n", headerFont));

                //add main table to the document
                doc.Add(mainTable);

                //close the document
                doc.Close();

                System.Diagnostics.Process.Start(fileName);
            }

        }

        #endregion

        #region Reports

        //generate items  stock report
        private void ItemsPacksReports(object sender, EventArgs e)
        {
            Document doc = new Document();

            //Get the current date
            DateTime thisDay = DateTime.Today;

            //creating a PDF file
            string fileName = "";
            string tableHeader = "";

            DataTable items = new DataTable();
            switch (((Button)sender).Name)
            {
                case "reportsItemsInStockBtn":
                    items = DB.GetItemsPacksInStock("items");
                    fileName = "ItemsInStock_" + thisDay.ToString("ddMMyyyy") + ".pdf";
                    tableHeader = "Items In Stock " + thisDay.ToString("dd/MM/yyyy");
                    break;
                case "reportsPacksInStockBtn":
                    items = DB.GetItemsPacksInStock("packs");
                    fileName = "PacksInStock_" + thisDay.ToString("ddMMyyyy") + ".pdf";
                    tableHeader = "Packs In Stock " + thisDay.ToString("dd/MM/yyyy");
                    break;
                case "reportsItemsAboutToEndBtn":
                    items = DB.GetItemsPacksEnding("items");
                    fileName = "ItemsAboutToEnd_" + thisDay.ToString("ddMMyyyy") + ".pdf";
                    tableHeader = "Items About To End " + thisDay.ToString("dd/MM/yyyy");
                    break;
                case "reportsPacksAboutToEndBtn":
                    items = DB.GetItemsPacksEnding("packs");
                    fileName = "PacksAboutToEnd_" + thisDay.ToString("ddMMyyyy") + ".pdf";
                    tableHeader = "Packs About To End " + thisDay.ToString("dd/MM/yyyy");
                    break;
            }

            //call save file dialog
            fileName = SaveFileLocation(fileName);

            try
            {
                FileStream pdfFile = new FileStream(fileName, FileMode.Create);
                PdfWriter writer = PdfWriter.GetInstance(doc, pdfFile);
            }
            catch
            {
                MessageBox.Show("Please close the open file first");
                return;
            }

            



            iTextSharp.text.BaseColor tableTextColor = new iTextSharp.text.BaseColor(0, 0, 255);
            iTextSharp.text.BaseColor tableCellColor = new iTextSharp.text.BaseColor(255, 255, 180);

            //------------------fonts----------------

            //Header font
            iTextSharp.text.Font headerFont = new iTextSharp.text.Font(
                iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.ITALIC);

            //normal font size 10pt
            iTextSharp.text.Font normal10Font = new iTextSharp.text.Font(
                iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL,
                tableTextColor);

            //bold font size 10pt
            iTextSharp.text.Font bold10Font = new iTextSharp.text.Font(
                iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD,
                tableTextColor);

            //Header
            Paragraph header = new Paragraph("Created by StoreMGMT \u00a9", headerFont);
            header.Alignment = Element.ALIGN_CENTER;


            //Table Header
            Paragraph tableHeaderP = new Paragraph(tableHeader, headerFont);
            tableHeaderP.Alignment = Element.ALIGN_CENTER;


            //main table
            PdfPTable itemsTable = new PdfPTable(5);

            //barcode column header
            PdfPCell itemBarcode = new PdfPCell(new Phrase("Barcode", bold10Font));
            itemBarcode.BorderWidth = 2f;
            itemBarcode.Border = iTextSharp.text.Rectangle.TOP_BORDER |
                    iTextSharp.text.Rectangle.LEFT_BORDER;
            itemsTable.AddCell(itemBarcode);

            //description column header
            PdfPCell descriptionBarcode = new PdfPCell(new Phrase("Description", bold10Font));
            descriptionBarcode.BorderWidth = 2f;
            descriptionBarcode.Border = iTextSharp.text.Rectangle.TOP_BORDER;
            itemsTable.AddCell(descriptionBarcode);

            //weight column header
            PdfPCell weightBarcode = new PdfPCell(new Phrase("Weight", bold10Font));
            weightBarcode.BorderWidth = 2f;
            weightBarcode.Border = iTextSharp.text.Rectangle.TOP_BORDER;
            itemsTable.AddCell(weightBarcode);

            //cost column header
            PdfPCell costBarcode = new PdfPCell(new Phrase("Cost", bold10Font));
            costBarcode.BorderWidth = 2f;
            costBarcode.Border = iTextSharp.text.Rectangle.TOP_BORDER;
            itemsTable.AddCell(costBarcode);

            //quantity column header
            PdfPCell quantityBarcode = new PdfPCell(new Phrase("Quantity", bold10Font));
            quantityBarcode.BorderWidth = 2f;
            quantityBarcode.Border = iTextSharp.text.Rectangle.TOP_BORDER |
                iTextSharp.text.Rectangle.RIGHT_BORDER; ;
            itemsTable.AddCell(quantityBarcode);

            //the total weight of the items
            double weightSum = 0;

            //the total value of the items
            double valueSum = 0;

            //the total number of items in stock
            double itemsSum = 0;

            //insert the details into the table
            for (int i = 0; i < items.Rows.Count; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    PdfPCell itemD = new PdfPCell(new Phrase(
                        Convert.ToString(items.Rows[i].ItemArray[j]), normal10Font));

                    itemD.Border = iTextSharp.text.Rectangle.NO_BORDER;

                    if (i % 2 == 0)
                        itemD.BackgroundColor = tableCellColor;

                    if (j == 0)
                    {
                        itemD.Border = iTextSharp.text.Rectangle.LEFT_BORDER;
                        itemD.BorderWidth = 2f;
                    }

                    if (j == 4)
                    {
                        itemD.Border = iTextSharp.text.Rectangle.RIGHT_BORDER;
                        itemD.BorderWidth = 2f;
                    }

                    itemsTable.AddCell(itemD);

                    if (j == 2)
                        weightSum += (Convert.ToDouble(items.Rows[i].ItemArray[j]) * Convert.ToDouble(items.Rows[i].ItemArray[4]));
                    if (j == 3)
                        valueSum += Convert.ToDouble(items.Rows[i].ItemArray[j]) * Convert.ToDouble(items.Rows[i].ItemArray[4]);
                    if (j == 4)
                        itemsSum += Convert.ToInt16(items.Rows[i].ItemArray[j]);
                }

            }

            //summery
            PdfPCell totalCell = new PdfPCell(new Phrase("Total", bold10Font));
            totalCell.Colspan = 2;
            totalCell.BorderWidth = 2f;
            itemsTable.AddCell(totalCell);

            PdfPCell totalWeightCell = new PdfPCell(new Phrase(
                Convert.ToString(weightSum), bold10Font));
            totalWeightCell.BorderWidth = 2f;
            itemsTable.AddCell(totalWeightCell);

            PdfPCell totalValueCell = new PdfPCell(new Phrase(Convert.ToString(valueSum), bold10Font));
            totalValueCell.BorderWidth = 2f;
            itemsTable.AddCell(totalValueCell);

            PdfPCell totalItemsCell = new PdfPCell(new Phrase(Convert.ToString(itemsSum), bold10Font));
            totalItemsCell.BorderWidth = 2f;
            itemsTable.AddCell(totalItemsCell);

            //open the document for writing
            doc.Open();

            //doc header
            doc.Add(header);
            doc.Add(new Paragraph("\n\n", headerFont));

            //table header
            doc.Add(tableHeaderP);
            doc.Add(new Paragraph("\n", headerFont));

            //main table
            doc.Add(itemsTable);

            //close the document foe editing
            doc.Close();

            System.Diagnostics.Process.Start(fileName);

        }

        //generate best seller report
        private void reportsBestSellerBtn_Click(object sender, EventArgs e)
        {
            Document doc = new Document();

            //Get the current date
            DateTime thisDay = DateTime.Today;

            //creating a PDF file
            string fileName = ".\\Documents\\BestSellers" + thisDay.ToString("ddMMyyyy") + ".pdf";
            string tableHeader = "Best Sellers " + thisDay.ToString("dd/MM/yyyy");

            DataTable items = DB.GetBestSellers();

            try
            {
                FileStream pdfFile = new FileStream(fileName, FileMode.Create);
                PdfWriter writer = PdfWriter.GetInstance(doc, pdfFile);
            }
            catch
            {
                MessageBox.Show("Please close the open file first");
                return;
            }

            iTextSharp.text.BaseColor tableTextColor = new iTextSharp.text.BaseColor(0, 0, 255);
            iTextSharp.text.BaseColor tableCellColor = new iTextSharp.text.BaseColor(255, 255, 180);

            //------------------fonts----------------

            //Header font
            iTextSharp.text.Font headerFont = new iTextSharp.text.Font(
                iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.ITALIC);

            //normal font size 10pt
            iTextSharp.text.Font normal10Font = new iTextSharp.text.Font(
                iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL,
                tableTextColor);

            //bold font size 10pt
            iTextSharp.text.Font bold10Font = new iTextSharp.text.Font(
                iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD,
                tableTextColor);

            //Header
            Paragraph header = new Paragraph("Created by StoreMGMT \u00a9", headerFont);
            header.Alignment = Element.ALIGN_CENTER;


            //Table Header
            Paragraph tableHeaderP = new Paragraph(tableHeader, headerFont);
            tableHeaderP.Alignment = Element.ALIGN_CENTER;


            //main table
            PdfPTable itemsTable = new PdfPTable(3);

            //barcode column header
            PdfPCell itemBarcode = new PdfPCell(new Phrase("Barcode", bold10Font));
            itemBarcode.BorderWidth = 2f;
            itemBarcode.Border = iTextSharp.text.Rectangle.TOP_BORDER |
                    iTextSharp.text.Rectangle.LEFT_BORDER;
            itemsTable.AddCell(itemBarcode);

            //description column header
            PdfPCell descriptionBarcode = new PdfPCell(new Phrase("Description", bold10Font));
            descriptionBarcode.BorderWidth = 2f;
            descriptionBarcode.Border = iTextSharp.text.Rectangle.TOP_BORDER;
            itemsTable.AddCell(descriptionBarcode);

            //quantity column header
            PdfPCell quantityBarcode = new PdfPCell(new Phrase("Quantity", bold10Font));
            quantityBarcode.BorderWidth = 2f;
            quantityBarcode.Border = iTextSharp.text.Rectangle.TOP_BORDER |
                iTextSharp.text.Rectangle.RIGHT_BORDER; ;
            itemsTable.AddCell(quantityBarcode);

            //insert the details into the table
            for (int i = 0; i < items.Rows.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    PdfPCell itemD = new PdfPCell(new Phrase(
                        Convert.ToString(items.Rows[i].ItemArray[j]), normal10Font));

                    itemD.Border = iTextSharp.text.Rectangle.NO_BORDER;

                    if (i % 2 == 0)
                        itemD.BackgroundColor = tableCellColor;

                    if(i == items.Rows.Count - 1)
                    {
                        itemD.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER;
                        itemD.BorderWidth = 2f;
                    }

                    if (j == 0)
                    {
                        itemD.Border += iTextSharp.text.Rectangle.LEFT_BORDER;
                        itemD.BorderWidth = 2f;
                    }

                    if (j == 2)
                    {
                        itemD.Border += iTextSharp.text.Rectangle.RIGHT_BORDER;
                        itemD.BorderWidth = 2f;
                    }

                    itemsTable.AddCell(itemD);
                }

            }

            //open the document for writing
            doc.Open();

            //doc header
            doc.Add(header);
            doc.Add(new Paragraph("\n\n", headerFont));

            //table header
            doc.Add(tableHeaderP);
            doc.Add(new Paragraph("\n", headerFont));

            //main table
            doc.Add(itemsTable);

            //close the document foe editing
            doc.Close();

            System.Diagnostics.Process.Start(fileName);
            
        }

        //generate clients details report
        private void reportsClientsBtn_Click(object sender, EventArgs e)
        {
            Document doc = new Document(new RectangleReadOnly(842, 595), 88f, 88f, 10f, 10f);

            //Get the current date
            DateTime thisDay = DateTime.Today;

            //creating a PDF file
            string fileName = ".\\Documents\\ClientsDetails_" + thisDay.ToString("ddMMyyyy") + ".pdf";
            string tableHeader = "Clients Details " + thisDay.ToString("dd/MM/yyyy");

            DataTable clientsDT = DB.GetClients();
            
            try
            {
                FileStream pdfFile = new FileStream(fileName, FileMode.Create);
                PdfWriter writer = PdfWriter.GetInstance(doc, pdfFile);
            }
            catch
            {
                MessageBox.Show("Please close the open file first");
                return;
            }

            iTextSharp.text.BaseColor tableTextColor = new iTextSharp.text.BaseColor(0, 0, 255);
            iTextSharp.text.BaseColor tableCellColor = new iTextSharp.text.BaseColor(255, 255, 180);

            //------------------fonts----------------

            //Header font
            iTextSharp.text.Font headerFont = new iTextSharp.text.Font(
                iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.ITALIC);

            //normal font size 8pt
            iTextSharp.text.Font normal8Font = new iTextSharp.text.Font(
                iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL,
                tableTextColor);

            //bold font size 10pt
            iTextSharp.text.Font bold10Font = new iTextSharp.text.Font(
                iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD,
                tableTextColor);

            //Header
            Paragraph header = new Paragraph("Created by StoreMGMT \u00a9", headerFont);
            header.Alignment = Element.ALIGN_CENTER;


            //Table Header
            Paragraph tableHeaderP = new Paragraph(tableHeader, headerFont);
            tableHeaderP.Alignment = Element.ALIGN_CENTER;


            //main table
            PdfPTable clientsTable = new PdfPTable(10);
            clientsTable.WidthPercentage = 100;

            //Email column header
            PdfPCell clientEmailCell = new PdfPCell(new Phrase("Email", bold10Font));
            clientEmailCell.BorderWidth = 2f;
            clientEmailCell.Border = iTextSharp.text.Rectangle.TOP_BORDER |
                    iTextSharp.text.Rectangle.LEFT_BORDER;
            clientsTable.AddCell(clientEmailCell);

            //First Name column header
            PdfPCell clientFirstNameCell = new PdfPCell(new Phrase("First Name", bold10Font));
            clientFirstNameCell.BorderWidth = 2f;
            clientFirstNameCell.Border = iTextSharp.text.Rectangle.TOP_BORDER;
            clientsTable.AddCell(clientFirstNameCell);

            //Last Name column header
            PdfPCell clientLastNameCell = new PdfPCell(new Phrase("Last Name", bold10Font));
            clientLastNameCell.BorderWidth = 2f;
            clientLastNameCell.Border = iTextSharp.text.Rectangle.TOP_BORDER;
            clientsTable.AddCell(clientLastNameCell);

            //Address 1 column header
            PdfPCell clientAddress1Cell = new PdfPCell(new Phrase("Address 1", bold10Font));
            clientAddress1Cell.BorderWidth = 2f;
            clientAddress1Cell.Border = iTextSharp.text.Rectangle.TOP_BORDER;
            clientsTable.AddCell(clientAddress1Cell);

            //Address 2 column header
            PdfPCell clientAddress2Cell = new PdfPCell(new Phrase("Address 2", bold10Font));
            clientAddress2Cell.BorderWidth = 2f;
            clientAddress2Cell.Border = iTextSharp.text.Rectangle.TOP_BORDER;
            clientsTable.AddCell(clientAddress2Cell);

            //City column header
            PdfPCell clientCityCell = new PdfPCell(new Phrase("City", bold10Font));
            clientCityCell.BorderWidth = 2f;
            clientCityCell.Border = iTextSharp.text.Rectangle.TOP_BORDER;
            clientsTable.AddCell(clientCityCell);

            //State/Province column header
            PdfPCell clientSPCell = new PdfPCell(new Phrase("State/Province", bold10Font));
            clientSPCell.BorderWidth = 2f;
            clientSPCell.Border = iTextSharp.text.Rectangle.TOP_BORDER;
            clientsTable.AddCell(clientSPCell);

            //Zip/Postal Code column header
            PdfPCell clientZPCell = new PdfPCell(new Phrase("Zip/Postal Code", bold10Font));
            clientZPCell.BorderWidth = 2f;
            clientZPCell.Border = iTextSharp.text.Rectangle.TOP_BORDER;
            clientsTable.AddCell(clientZPCell);

            //Country column header
            PdfPCell clintCountryCell = new PdfPCell(new Phrase("Country", bold10Font));
            clintCountryCell.BorderWidth = 2f;
            clintCountryCell.Border = iTextSharp.text.Rectangle.TOP_BORDER;
            clientsTable.AddCell(clintCountryCell);



            //Phone number column header
            PdfPCell clientPNCell = new PdfPCell(new Phrase("Phone number", bold10Font));
            clientPNCell.BorderWidth = 2f;
            clientPNCell.Border = iTextSharp.text.Rectangle.TOP_BORDER |
                iTextSharp.text.Rectangle.RIGHT_BORDER; ;
            clientsTable.AddCell(clientPNCell);  

            //insert the details into the table
            for (int i = 0; i < clientsDT.Rows.Count; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    PdfPCell clientD = new PdfPCell(new Phrase(
                        Convert.ToString(clientsDT.Rows[i].ItemArray[j]), normal8Font));

                    clientD.Border = iTextSharp.text.Rectangle.NO_BORDER;

                    if (i % 2 == 0)
                        clientD.BackgroundColor = tableCellColor;

                    if (i == clientsDT.Rows.Count - 1)
                    {
                        clientD.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER;
                        clientD.BorderWidth = 2f;
                    }

                    if (j == 0)
                    {
                        clientD.Border += iTextSharp.text.Rectangle.LEFT_BORDER;
                        clientD.BorderWidth = 2f;
                    }

                    if (j == 9)
                    {
                        clientD.Border += iTextSharp.text.Rectangle.RIGHT_BORDER;
                        clientD.BorderWidth = 2f;
                    }

                    clientsTable.AddCell(clientD);
                }

            }

            
            //open the document for writing
            doc.Open();

            //doc header
            doc.Add(header);
            doc.Add(new Paragraph("\n\n", headerFont));

            //table header
            doc.Add(tableHeaderP);
            doc.Add(new Paragraph("\n", headerFont));

            //main table
            doc.Add(clientsTable);

            //close the document foe editing
            doc.Close();

            System.Diagnostics.Process.Start(fileName);
        }

        private void reportsShippingBtn_Click(object sender, EventArgs e)
        {
            Document doc = new Document();

            //Get the current date
            DateTime thisDay = DateTime.Today;

            //creating a PDF file
            string fileName = ".\\Documents\\shippingDetails_" + thisDay.ToString("ddMMyyyy") + ".pdf";
            string tableHeader = "Shipping Details " + thisDay.ToString("dd/MM/yyyy");

            DataTable shippingDT = DB.GetShipping();

            try
            {
                FileStream pdfFile = new FileStream(fileName, FileMode.Create);
                PdfWriter writer = PdfWriter.GetInstance(doc, pdfFile);
            }
            catch
            {
                MessageBox.Show("Please close the open file first");
                return;
            }

            iTextSharp.text.BaseColor tableTextColor = new iTextSharp.text.BaseColor(0, 0, 255);
            iTextSharp.text.BaseColor tableCellColor = new iTextSharp.text.BaseColor(255, 255, 180);

            //------------------fonts----------------

            //Header font
            iTextSharp.text.Font headerFont = new iTextSharp.text.Font(
                iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.ITALIC);

            //normal font size 10pt
            iTextSharp.text.Font normal8Font = new iTextSharp.text.Font(
                iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL,
                tableTextColor);

            //bold font size 10pt
            iTextSharp.text.Font bold10Font = new iTextSharp.text.Font(
                iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD,
                tableTextColor);

            //Header
            Paragraph header = new Paragraph("Created by StoreMGMT \u00a9", headerFont);
            header.Alignment = Element.ALIGN_CENTER;


            //Table Header
            Paragraph tableHeaderP = new Paragraph(tableHeader, headerFont);
            tableHeaderP.Alignment = Element.ALIGN_CENTER;


            //main table
            PdfPTable shippingTable = new PdfPTable(5);
            shippingTable.WidthPercentage = 100;

            //Country column header
            PdfPCell shippingCountryCell = new PdfPCell(new Phrase("Country", bold10Font));
            shippingCountryCell.BorderWidth = 2f;
            shippingCountryCell.Border = iTextSharp.text.Rectangle.TOP_BORDER |
                    iTextSharp.text.Rectangle.LEFT_BORDER;
            shippingTable.AddCell(shippingCountryCell);

            //Min Weight column header
            PdfPCell shippingMinWeightCell = new PdfPCell(new Phrase("Min Weight", bold10Font));
            shippingMinWeightCell.BorderWidth = 2f;
            shippingMinWeightCell.Border = iTextSharp.text.Rectangle.TOP_BORDER;
            shippingTable.AddCell(shippingMinWeightCell);

            //Max Weight column header
            PdfPCell shippingMaxWeightCell = new PdfPCell(new Phrase("Max Weight", bold10Font));
            shippingMaxWeightCell.BorderWidth = 2f;
            shippingMaxWeightCell.Border = iTextSharp.text.Rectangle.TOP_BORDER;
            shippingTable.AddCell(shippingMaxWeightCell);

            //Is Registered column header
            PdfPCell shippingRegCell = new PdfPCell(new Phrase("Is Registered", bold10Font));
            shippingRegCell.BorderWidth = 2f;
            shippingRegCell.Border = iTextSharp.text.Rectangle.TOP_BORDER;
            shippingTable.AddCell(shippingRegCell);

            
            //Phone number column header
            PdfPCell shippingPriceCell = new PdfPCell(new Phrase("Price", bold10Font));
            shippingPriceCell.BorderWidth = 2f;
            shippingPriceCell.Border = iTextSharp.text.Rectangle.TOP_BORDER |
                iTextSharp.text.Rectangle.RIGHT_BORDER; ;
            shippingTable.AddCell(shippingPriceCell);

            //insert the details into the table
            for (int i = 0; i < shippingDT.Rows.Count; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    PdfPCell shippingD = new PdfPCell(new Phrase(
                        Convert.ToString(shippingDT.Rows[i].ItemArray[j]), normal8Font));

                    shippingD.Border = iTextSharp.text.Rectangle.NO_BORDER;

                    if (i % 2 == 0)
                        shippingD.BackgroundColor = tableCellColor;

                    if (i == shippingDT.Rows.Count - 1)
                    {
                        shippingD.Border = iTextSharp.text.Rectangle.BOTTOM_BORDER;
                        shippingD.BorderWidth = 2f;
                    }

                    if (j == 0)
                    {
                        shippingD.Border += iTextSharp.text.Rectangle.LEFT_BORDER;
                        shippingD.BorderWidth = 2f;
                    }

                    if (j == 4)
                    {
                        shippingD.Border += iTextSharp.text.Rectangle.RIGHT_BORDER;
                        shippingD.BorderWidth = 2f;
                    }

                    shippingTable.AddCell(shippingD);
                }

            }


            //open the document for writing
            doc.Open();

            //doc header
            doc.Add(header);
            doc.Add(new Paragraph("\n\n", headerFont));

            //table header
            doc.Add(tableHeaderP);
            doc.Add(new Paragraph("\n", headerFont));

            //main table
            doc.Add(shippingTable);

            //close the document foe editing
            doc.Close();

            System.Diagnostics.Process.Start(fileName);
        }

        #endregion

        #region General Methods

        //add underline to required field not filled
        private void UnderlineReqierdField(object sender)
        {
            ((Label)sender).Font = new System.Drawing.Font(((Label)sender).Font.Name,
                    ((Label)sender).Font.SizeInPoints, FontStyle.Underline);
        }

        //remove underline
        private void RemoveUnderline(object sender)
        {
            ((Label)sender).Font = new System.Drawing.Font(((Label)sender).Font.Name,
                    ((Label)sender).Font.SizeInPoints, FontStyle.Regular);
        }

        //prevent any actions before configuring the store settings
        private void Main_SelectedIndexChanged(object sender, EventArgs e)
        {
            //show message box if store was not yet configured
            if ((!File.Exists(settingsFilePath) && Main.SelectedIndex != 0) ||
                updateSettingsFlag)
            {
                MessageBox.Show("Store Settings must be configured first!");

            }
        }

        //clear item data
        private void ClearItem (Item item)
        {
            item.Barcode = "";
            item.Cost = 0;
            item.Description = "";
            item.Cost = 0;
        }

        //clear conmbobox and textbox and checkbox
        private void ClearTextBoxes(Control control)
        {
            foreach (Control c in control.Controls)
            {
                //Check if the argument is a textbox, if so clear it
                if (c is TextBox)
                {
                    ((TextBox)c).Clear();
                }

                //Check if the argument is a combobox, if so returned it's index to -1
                if (c is ComboBox)
                {
                    ((ComboBox)c).SelectedIndex = -1;
                }

                //Check if the argument is a chackbox, if so marks as unchecked
                if (c is CheckBox)
                {
                    ((CheckBox)c).Checked = false;

                }
            }
        }

        //fill the settings tab
        private void FillSetingsTab()
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

        //choose where to save file
        private string SaveFileLocation (string fileName)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Pdf files (*.Pdf)|*.Pdf";
            sfd.FileName = fileName;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return sfd.FileName;
            }
            return "Error";
        }


        #endregion


    }
}
