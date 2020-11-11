using SAP.Middleware.Connector;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPCSyncSAPtoWorkflow
{
  public   class SyncSAPToWorkflow
    {
        private string SqlConnectionString;
        public SyncSAPToWorkflow(string connectionstring)
        {
            SqlConnectionString = connectionstring;
        }
        private RfcDestination CreateRfcDestination()
        {
            IDestinationConfiguration ID = new MyBackendConfig();
            if (RfcDestinationManager.TryGetDestination("PRD_000") == null)
            {
                RfcDestinationManager.RegisterDestinationConfiguration(ID);
            }

            return RfcDestinationManager.GetDestination("PRD_000");
        }
        public IEnumerable<string>  UpdateWBSForVPC(string wbsno)
        {
            var result = new List<string>();
            var inwbsarray = wbsno.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);


            RfcDestination prd = CreateRfcDestination();
            ArrayList list = new ArrayList();
            list.Add("2130");
            list.Add("2131");
            list.Add("2170");
            foreach (string companycode in list)
            {
                DataTable ds = Functions.ReadPRPSTable(prd, companycode);
                int a = ds.Rows.Count;
                int i = 0;
                string CheckDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd").Replace("-", "");
                //string CheckDate = DateTime.Now.ToString("yyyy-MM-dd").Replace("-", "");
                foreach (DataRow row in ds.Rows)
                {


                    i++;
                    string WBS = row["POSID_EDIT"].ToString();
                    //zzzhu,hualin modified for sync special wbs no.
                    if (inwbsarray.Any(x => WBS.Contains(x)))
                    {
                        //string WBS = row["POSID"].ToString();
                        string ProjectNo = row["PSPHI"].ToString();
                        string Description = row["POST1"].ToString().Replace("'", "''");
                        string CreatedOn = row["ERDAT"].ToString();
                        string ChangedOn = row["AEDAT"].ToString();
                        string strsqlDelete = string.Format("delete YAVPCWBSNo where WBSElement = '{0}'", WBS);
                        DBHelper.ExecuteSql(strsqlDelete, SqlConnectionString);
                        string strsql = string.Format("insert into YAVPCWBSNo(WBSElement,ProjectNo,Description,CreatedOn,ChangedOn,AddDate) values('{0}','{1}','{2}','{3}','{4}','{5}')", WBS, ProjectNo, Description, CreatedOn, ChangedOn, DateTime.Now.ToString("yyyy-MM-dd").Replace("-", ""));
                        DBHelper.ExecuteSql(strsql, SqlConnectionString);
                        result.Add(WBS);
                    }
                }
            }

            return result;
        }




        public IEnumerable<string> UpdateSOForVPC(string so)
        {
            var result = new List<string>();
            var inarray = so.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            RfcDestination prd = CreateRfcDestination();
            ArrayList list = new ArrayList();
            list.Add("2130");
            list.Add("2131");
            list.Add("2170");
            ArrayList SOList = new ArrayList();

            string CompanyCode = string.Empty;

            #region //Update SO
            for (int j = 0; j < list.Count; j++)
            {
                ArrayList a = new ArrayList();
                CompanyCode = list[j].ToString();
                DataTable ds = Functions.ReadVBAKTable(prd, CompanyCode);
                foreach (DataRow row in ds.Rows)
                {
     
                    string strSalesOrderVPAK = row["VBELN"].ToString().Trim();
                    string strType = row["AUART"].ToString().Trim();
                    string strSOheaderCreateOn = row["ERDAT"].ToString().Trim();
                    string strSOheaderChangeOn = row["AEDAT"].ToString().Trim();
                    string strCreator = row["ERNAM"].ToString().Trim();
                    string CheckDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd").Replace("-", "");
                    if (inarray.Any(x => strSalesOrderVPAK.Contains(x)))
                    {
                        result.Add(strSalesOrderVPAK);
                        //Delete the SO 
                        string strsqlDeleteSO = string.Format("Delete YAVPCSalesOrder where SD_DOC = '{0}'", strSalesOrderVPAK);
                        DBHelper.ExecuteSql(strsqlDeleteSO, SqlConnectionString);

                        //Insert the SO
                        DataTable dsVBPA = Functions.ReadVBAPTable(prd, strSalesOrderVPAK);
                        foreach (DataRow rowVBPA in dsVBPA.Rows)
                        {
                            string strSalesOrder = rowVBPA["VBELN"].ToString(); //SalesOrder
                            SOList.Add(strSalesOrder);
                            string strItem = rowVBPA["POSNR"].ToString(); //Item
                            string strMaterial = rowVBPA["MATNR"].ToString().TrimEnd(); //Material
                            string strDescription = rowVBPA["ARKTX"].ToString().TrimEnd().Replace("'", "''"); //Material Description
                            string strCreatedOn = rowVBPA["ERDAT"].ToString(); //Created On
                            string strChangedOn = rowVBPA["AEDAT"].ToString(); //Changed On

                            string strsql = string.Format(" insert into YAVPCSalesOrder(SD_DOC,ITM_NUMBER,Material,SHORT_TEXT,Created_On, Changed_On , DOC_TYPE ,AddDate, Creator) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", strSalesOrder, strItem, strMaterial, strDescription, strCreatedOn, strChangedOn, strType, DateTime.Now.ToString("yyyy-MM-dd"), strCreator);
                            DBHelper.ExecuteSql(strsql, SqlConnectionString);
                        }
                    }

                    
                }
            }
            #endregion

            //  update customer
            UpdateCustomer(prd, SOList);
            UpdatePMbySO(prd, SOList);
            return result;
        }
        private void UpdateCustomer(RfcDestination prd, ArrayList SOList)
        {
            ArrayList CustomerList = new ArrayList();
            ArrayList NameList = new ArrayList();
            RfcRepository SapRfcRepository = prd.Repository;
            IRfcFunction BapiCustomerGetList = SapRfcRepository.CreateFunction("BAPI_CUSTOMER_GETLIST");
            BapiCustomerGetList.SetValue("Cpdonly", "");
            BapiCustomerGetList.SetValue("Maxrows", "0");
            IRfcTable IRFCIdrange = BapiCustomerGetList.GetTable("Idrange");
            IRFCIdrange.Append();
            IRFCIdrange.SetValue("Sign", "I");
            IRFCIdrange.SetValue("Option", "BT");
            IRFCIdrange.SetValue("Low", "0006000000");
            IRFCIdrange.SetValue("High", "0006999999");
            BapiCustomerGetList.Invoke(prd);
            //处理获得数据

            IRfcTable Address = BapiCustomerGetList.GetTable("Addressdata");
            for (int i = 0; i < Address.Count; i++)
            {
                Address.CurrentIndex = i;
                string strtest = Address.GetString("Customer");
                CustomerList.Add(strtest);
                string strName = Address.GetString("NAME");
                NameList.Add(strName);
            }

            foreach (var SO in SOList)
            {
                DataTable ds = Functions.ReadVBPATable(prd, SO.ToString());
                string CustomerCode = ds.Rows[0]["KUNNR"].ToString();
                string CustomerName = NameList[CustomerList.IndexOf(CustomerCode)].ToString();
                string strsqls = string.Format("update YAVPCSalesOrder set Customer = '{0}', CustomerCode = '{1}' where SD_DOC = '{2}'", CustomerName, CustomerCode, SO.ToString());
                DBHelper.ExecuteSql(strsqls, SqlConnectionString);
            }
        }

        private void UpdatePMbySO(RfcDestination prd, ArrayList SOList)
        {
            foreach (var SO in SOList)
            {
                string functiontype = string.Empty;
                string PMGlobalID = string.Empty;
                string strsqls = string.Empty;
                string OMGlobalID = string.Empty;
                string CCPGlobalID = string.Empty;
                DataTable ds = Functions.ReadVBPA(prd, SO.ToString());
                if (ds.Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Rows)
                    {
                        functiontype = row["PARVW"].ToString();
                        if (functiontype == "Z7")   // Z7 is project manager
                        {
                            PMGlobalID = row["PERNR"].ToString();
                        }
                        if (functiontype == "ZD")   // order manager
                        {
                            OMGlobalID = row["PERNR"].ToString();
                        }
                        if (functiontype == "ZK")  // commercial contact person
                        {
                            CCPGlobalID = row["PERNR"].ToString();
                        }
                    }
                    strsqls = string.Format("update YAVPCSalesOrder set PMGlobalID = '{0}', OMGlobalID='{1}', CCPGlobalID = '{2}' where SD_DOC = '{3}'", PMGlobalID, OMGlobalID, CCPGlobalID, SO);
                    DBHelper.ExecuteSql(strsqls, SqlConnectionString);
                }
            }
        }
    }
}
