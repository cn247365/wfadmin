using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using SAP.Middleware.Connector;
using System.Data.SqlClient;
using System.Configuration;
using SAPVoithProxy;
using System.Collections;
using System.IO;

namespace VPCVendorSAPtoWorkflow
{
    public class SyncSAPToWorkflow {

        private string SqlConnectionString;
        public SyncSAPToWorkflow(string connectionstring)
        {
            SqlConnectionString = connectionstring;
        }

        public IEnumerable<string> UpdateVendor(string vendorcode)
        {
            var result = new List<string>();

            IDestinationConfiguration ID = new MyBackendConfig();
            if (RfcDestinationManager.TryGetDestination("PRD_000") == null)
            {
                RfcDestinationManager.RegisterDestinationConfiguration(ID);
            }
              
            RfcDestination prd = RfcDestinationManager.GetDestination("PRD_000");
            List<string> CompanyList = new List<string>();
            CompanyList.Add("2130");
            CompanyList.Add("2131");
            CompanyList.Add("2170");

            foreach (string CompanyCode in CompanyList)
            {
                //string strsql = string.Format("select * From YAVPCVendor where CompanyCode={0}", CompanyCode);
                //DataSet dsVendor = DBHelper.Query(strsql, SqlConnectionString);
                //List<string> dsVendorList = new List<string>();

                //foreach (DataRow rows in dsVendor.Tables[0].Rows)
                //{
                //    dsVendorList.Add(rows["VendorCode"].ToString().Trim());
                //}

                List<string> fields = new List<string>();
                fields.Add("LIFNR");
                fields.Add("BUKRS");
                fields.Add("ERDAT");

                DataTable dt = Functions.ReadLFB1Table(prd, fields, CompanyCode);
                foreach (DataRow row in dt.Rows)
                {
                    string VendorCode = row["LIFNR"].ToString();
                    if (VendorCode.Contains(vendorcode)) // Check if current vendor in the table
                    {
                        string EditOn = row["ERDAT"].ToString();
                        string Company = row["BUKRS"].ToString();
                        VendorData objTemp = GetVendorData(prd, VendorCode, CompanyCode);
                        if (!existVendorCode(objTemp.VendorCode, objTemp.CompanyCode)){
                            string sql = string.Format(" insert into YAVPCVendor (VendorCode,Name_CN,Name_EN,BankName,BankAccount,PaymentTerms,ContactPerson,Email,Address,Tel,Tel2,Address_CN,CompanyCode) Values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')", objTemp.VendorCode, objTemp.Name_CN, objTemp.Name_EN, objTemp.Bank, objTemp.BankAccount, objTemp.PaymentTerms, objTemp.ContactPerson, objTemp.Email, objTemp.Address, objTemp.Tel, objTemp.Tel2, objTemp.AddressCN, objTemp.CompanyCode);
                            DBHelper.ExecuteSql(sql, SqlConnectionString);
                        }
                        else
                        {
                            string sql = string.Format("update YAVPCVendor set Name_CN ='{0}',Name_EN='{1}',BankName='{2}',BankAccount='{3}',PaymentTerms='{4}',ContactPerson='{5}',Email='{6}',Address='{7}',Tel='{8}',Tel2='{9}',Address_CN='{10}'where VendorCode='{11}'", objTemp.Name_CN, objTemp.Name_EN, objTemp.Bank, objTemp.BankAccount, objTemp.PaymentTerms, objTemp.ContactPerson, objTemp.Email, objTemp.Address, objTemp.Tel, objTemp.Tel2, objTemp.AddressCN, objTemp.VendorCode);
                            DBHelper.ExecuteSql(sql, SqlConnectionString);
                        }
                        result.Add(VendorCode);
                        //break;
                    }
                     
                }
            }

            return result;
        }

        private bool existVendorCode(string vendorcode, string companycode) { 
            var sql=$"select * from YAVPCVendor where VendorCode = '{vendorcode}' and CompanyCode = '{companycode}'";
            var ds=DBHelper.Query(sql, this.SqlConnectionString);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else {
                return false;
            }
        
        }
        public IEnumerable<string> UpdateVendor()
        {
            var result = new List<string>();

            IDestinationConfiguration ID = new MyBackendConfig();

            RfcDestinationManager.RegisterDestinationConfiguration(ID);

            RfcDestination prd = RfcDestinationManager.GetDestination("PRD_000");

            List<string> CompanyList = new List<string>();
            CompanyList.Add("2130");
            CompanyList.Add("2131");
            CompanyList.Add("2170");

            foreach (string CompanyCode in CompanyList)
            {
                string strsql = string.Format("select * From YAVPCVendor where CompanyCode={0}", CompanyCode);
                DataSet dsVendor = DBHelper.Query(strsql, SqlConnectionString);
                List<string> dsVendorList = new List<string>();

                foreach (DataRow rows in dsVendor.Tables[0].Rows)
                {
                    dsVendorList.Add(rows["VendorCode"].ToString().Trim());
                }

                List<string> fields = new List<string>();
                fields.Add("LIFNR");
                fields.Add("BUKRS");
                fields.Add("ERDAT");

                DataTable dt = Functions.ReadLFB1Table(prd, fields, CompanyCode);
                foreach (DataRow row in dt.Rows)
                {
                    string VendorCode = row["LIFNR"].ToString();
                    if (!dsVendorList.Contains(VendorCode)) // Check if current vendor in the table
                    {
                        string EditOn = row["ERDAT"].ToString();
                        string Company = row["BUKRS"].ToString();
                        VendorData objTemp = GetVendorData(prd, VendorCode, CompanyCode);
                        string sql = string.Format(" insert into YAVPCVendor (VendorCode,Name_CN,Name_EN,BankName,BankAccount,PaymentTerms,ContactPerson,Email,Address,Tel,Tel2,Address_CN,CompanyCode) Values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')", objTemp.VendorCode, objTemp.Name_CN, objTemp.Name_EN, objTemp.Bank, objTemp.BankAccount, objTemp.PaymentTerms, objTemp.ContactPerson, objTemp.Email, objTemp.Address, objTemp.Tel, objTemp.Tel2, objTemp.AddressCN, objTemp.CompanyCode);
                        DBHelper.ExecuteSql(sql, SqlConnectionString);
                    }
                }
            }

            return result;
        }

        public void UpdateVendorByVendorCode_InVendorDataTable()
        {
            IDestinationConfiguration ID = new MyBackendConfig();
            RfcDestinationManager.RegisterDestinationConfiguration(ID);
            RfcDestination prd = RfcDestinationManager.GetDestination("PRD_000");

            string strsql = "Select * From YAVPCVendorData";
            DataSet dsVendor = DBHelper.Query(strsql, SqlConnectionString);
            List<string> dsVendorList = new List<string>();
            string objTemp = string.Empty;
            foreach (DataRow rows in dsVendor.Tables[0].Rows)
            {
                objTemp = rows["VendorCode"].ToString().Trim();
                if (objTemp.Length == 10)
                {
                    dsVendorList.Add(objTemp);
                }
                else
                {
                    dsVendorList.Add(objTemp.PadLeft(10 - objTemp.Length, '0'));
                }
            }

            foreach (string VendorCode in dsVendorList)
            {
                string strsql2 = string.Format("Select * From YAVPCVendor where VendorCode = '{0}'", VendorCode);
                DataSet dsCompanyCode = DBHelper.Query(strsql2, SqlConnectionString);
                if (dsCompanyCode.Tables[0].Rows.Count > 0)
                {
                    UpdateVendorbyCode(prd, VendorCode, dsCompanyCode.Tables[0].Rows[0]["CompanyCode"].ToString());
                }
                else
                {
                    insertException(VendorCode);
                }
            }
        }

        private void UpdateVendorbyCode(RfcDestination prd, string VendorCode, string CompanyCode)
        {
            VendorData objTemp = GetVendorData(prd, VendorCode, CompanyCode);
            string sql = string.Format("update YAVPCVendor set Name_CN ='{0}',Name_EN='{1}',BankName='{2}',BankAccount='{3}',PaymentTerms='{4}',ContactPerson='{5}',Email='{6}',Address='{7}',Tel='{8}',Tel2='{9}',Address_CN='{10}'where VendorCode='{11}'", objTemp.Name_CN, objTemp.Name_EN, objTemp.Bank, objTemp.BankAccount, objTemp.PaymentTerms, objTemp.ContactPerson, objTemp.Email, objTemp.Address, objTemp.Tel, objTemp.Tel2, objTemp.AddressCN, objTemp.VendorCode);

            DBHelper.ExecuteSql(sql, SqlConnectionString);
        }

        private void InsertVendorbyCode(RfcDestination prd, string VendorCode, string CompanyCode)
        {
            VendorData objTemp = GetVendorData(prd, VendorCode, CompanyCode);
            string sql = string.Format("insert into YAVPCVendor (VendorCode,Name_CN,Name_EN,BankName,BankAccount,PaymentTerms,ContactPerson,Email,Address,Tel,Tel2,Address_CN,CompanyCode) Values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')", objTemp.VendorCode, objTemp.Name_CN, objTemp.Name_EN, objTemp.Bank, objTemp.BankAccount, objTemp.PaymentTerms, objTemp.ContactPerson, objTemp.Email, objTemp.Address, objTemp.Tel, objTemp.Tel2, objTemp.AddressCN, objTemp.CompanyCode);

            DBHelper.ExecuteSql(sql, SqlConnectionString);
        }

        private VendorData GetVendorData(RfcDestination prd, string VendorCode, string CompanyCode)
        {
            VendorData objTemp = new VendorData();
            objTemp.CompanyCode = CompanyCode;
            objTemp.VendorCode = VendorCode;

            #region //Get Payment Terms and Contact Person
            string Purchase_Organization = "2130"; //For Company 2130/2131/2170  Purchase Organization is 2130           

            List<string> fields = new List<string>();
            fields.Add("LIFNR");//PO Number
            fields.Add("EKORG");//Purchase Organization
            fields.Add("ZTERM");//Payment terms
            fields.Add("VERKF"); //Contact Person

            DataTable dt = Functions.ReadLFM1Table(prd, fields, Purchase_Organization, VendorCode);
            if (dt.Rows.Count > 0)
            {
                objTemp.PaymentTerms = dt.Rows[0]["ZTERM"].ToString().TrimEnd().Replace("'", "''");
                objTemp.ContactPerson = dt.Rows[0]["VERKF"].ToString().TrimEnd().Replace("'", "''");
            }
            #endregion

            #region //Get NameCN NameEN BankAccount BankName
            //Get NameCN NameEN BanckAccount
            RfcRepository SapRfcRepository = prd.Repository;
            IRfcFunction BapiVendorBank = SapRfcRepository.CreateFunction("ZBAPI_VENDOR_BANK");
            BapiVendorBank.SetValue("Companycode", CompanyCode);
            BapiVendorBank.SetValue("Vendor", Functions.FormatVendorCode(VendorCode));
            BapiVendorBank.Invoke(prd);
            IRfcTable VendorBank = BapiVendorBank.GetTable("Vendorbank");
            VendorBank.CurrentIndex = 0;
            objTemp.Name_CN = VendorBank.GetString("Name1").Replace("'", "''"); //Chinese Name
            objTemp.Name_EN = VendorBank.GetString("Name2").Replace("'", "''"); //English Name
            objTemp.BankAccount = VendorBank.GetString("Bankn").TrimEnd().Replace("'", "''"); //Bank Account
            string AccountKey = VendorBank.GetString("Bankl"); //Bank Account key


            //Get Bank Name by AccountKey 
            List<string> fields2 = new List<string>();
            fields2.Add("BANKL"); //Bank AccoutnKey
            fields2.Add("BANKA");
            DataTable dtBank = Functions.ReadBNKATable(prd, fields2, AccountKey);
            if (dtBank.Rows.Count > 0)
            {
                objTemp.Bank = dtBank.Rows[0]["BANKA"].ToString().TrimEnd().Replace("'", "''"); //Bank Name
            }
            #endregion

            #region //Get Street
            string VendorName = string.Empty;
            RfcRepository SapRfcRepository2 = prd.Repository;
            IRfcFunction BapiVendorGetDetail = SapRfcRepository2.CreateFunction("BAPI_VENDOR_GETDETAIL");
            BapiVendorGetDetail.SetValue("Companycode", CompanyCode);
            BapiVendorGetDetail.SetValue("Vendorno", VendorCode);
            BapiVendorGetDetail.Invoke(prd);
            IRfcStructure Generaldetail = BapiVendorGetDetail.GetStructure("Generaldetail");
            objTemp.Address = Generaldetail.GetString("Street").TrimEnd().Replace("'", "''");
            objTemp.Tel = Generaldetail.GetString("Telephone");
            objTemp.Tel2 = Generaldetail.GetString("Telephone2");
            #endregion

            #region //Get Email / Address /Address CN
            //Get Vendor Address
            List<string> fields3 = new List<string>();
            fields3.Add("LIFNR");
            fields3.Add("ADRNR");
            DataTable dtLFA1 = Functions.ReadLFA1Table(prd, fields3, VendorCode);
            if (dtLFA1.Rows.Count > 0)
            {
                string VendorAddress = dtLFA1.Rows[0]["ADRNR"].ToString();
                List<string> fields4 = new List<string>();
                fields4.Add("SMTP_ADDR");
                fields4.Add("ADDRNUMBER");
                DataTable dtADR6 = Functions.ReadADR6Table(prd, fields4, VendorAddress);
                if (dtADR6.Rows.Count > 0)
                {
                    objTemp.Email = dtADR6.Rows[0]["SMTP_ADDR"].ToString().TrimEnd().Replace("'", "''");
                }

                #region // Get Chinese Address
                List<string> fields5 = new List<string>();
                fields5.Add("ADDRNUMBER");
                fields5.Add("STREET");
                fields5.Add("NATION");
                DataTable dtADRC = Functions.ReadADRCTable(prd, fields5, VendorAddress);

                if (dtADRC.Rows.Count > 0)
                {
                    foreach (DataRow row in dtADRC.Rows)
                    {
                        if (row["NATION"].ToString().Contains("C"))
                        {
                            objTemp.AddressCN = row["STREET"].ToString().TrimEnd().Replace("'", "''");
                        }
                        else
                        {
                            objTemp.Address = row["STREET"].ToString().TrimEnd().Replace("'", "''");
                        }
                    }
                }

                #endregion
            }
            #endregion


            return objTemp;

        }

        private void insertException(string VendorCode)
        {
            string strsql = string.Format("update YAVPCVendorData set Status= 'invalid vendorcode' where VendorCode='{0}'", VendorCode);
            DBHelper.ExecuteSql(strsql, SqlConnectionString);
        }

         
     

     

    }
}
