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



namespace VHSSyncSAPToWorkflow
{
    public partial class SyncSAPToWorkflow 
    {
        protected string SqlConnectionString = "";
        public SyncSAPToWorkflow(string connectionString)
        {
            SqlConnectionString = connectionString;
        }



        public void nco()
        {

            IDestinationConfiguration ID = new MyBackendConfig();

            RfcDestinationManager.RegisterDestinationConfiguration(ID);

            RfcDestination prd = RfcDestinationManager.GetDestination("PRD_000");

            //UpdateWBSForVHS(prd);
            //UpdateQMOrderForVHS(prd);
            //UpdateSOForVHS(prd);
            //UpdateInternalOrderForVHS(prd);


        
        }

        private RfcDestination CreateRfcDestination() {
            IDestinationConfiguration ID = new MyBackendConfig();
            if (RfcDestinationManager.TryGetDestination("PRD_000") == null)
            {
                RfcDestinationManager.RegisterDestinationConfiguration(ID);
            }

           return RfcDestinationManager.GetDestination("PRD_000");
        }
        public IEnumerable<string> UpdateWBSForVHS(string wbsno)
        {
            var result = new List<string>();
            var inwbsarray = wbsno.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            RfcDestination prd = CreateRfcDestination();
            DataTable ds = Functions.ReadPRPSTable(prd);
            int i = 0;
            string CheckDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd").Replace("-", "");
            //string CheckDate = DateTime.Now.ToString("yyyy-MM-dd").Replace("-", "");
            foreach (DataRow row in ds.Rows)
            {
                i++;
                string WBS = row["POSID_EDIT"].ToString();
                //string WBS = row["POSID"].ToString();
                string ProjectNo = row["PSPHI"].ToString();
                string Description = row["POST1"].ToString().Replace("'", "''");
                string CreatedOn = row["ERDAT"].ToString();
                string ChangedOn = row["AEDAT"].ToString();

                if(inwbsarray.Any(x=>WBS.Contains(x)))
                {
                    result.Add(WBS);
                    string strsqlDelete = string.Format("delete YAVHSWBSElement where WBSElement = '{0}'", WBS);
                    DBHelper.ExecuteSql(strsqlDelete, SqlConnectionString);
                    string strsql = string.Format("insert into YAVHSWBSElement(WBSElement,PorjectNo,Description,CreatedOn,ChangedOn,AddDate) values('{0}','{1}','{2}','{3}','{4}','{5}')", WBS, ProjectNo, Description, CreatedOn, ChangedOn, DateTime.Now.ToString("yyyy-MM-dd").Replace("-", ""));
                    DBHelper.ExecuteSql(strsql, SqlConnectionString);
                }


                 
            }
            return result;

        }


        public IEnumerable<string> UpdateQMOrderForVHS(string order)
        {

            var result = new List<string>();
            var includeorders = order.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            RfcDestination prd = CreateRfcDestination();
            string tableName = "AUFK";
            //string orderType="QN01";
            int orderType = 06;
            int i = 0;
            // string CheckDate = DateTime.Now.ToString("yyyy-MM-dd").Replace("-", "");
            DataTable dsAUFK = Functions.ReadAUFKTable(prd, tableName, orderType);
            {
                foreach (DataRow rowAUFK in dsAUFK.Rows)
                {
                    i++;
                    string QMOrder = rowAUFK["AUFNR"].ToString();
                    //string status = rowAUFK["ASTNR"].ToString(); //
                    string CreatedOn = rowAUFK["ERDAT"].ToString();
                    string ChangedOn = rowAUFK["AEDAT"].ToString();

                    string Description = rowAUFK["KTEXT"].ToString().Replace("'", " ");

                    if (includeorders.Any(x => QMOrder.Contains(x)))
                    {
                        result.Add(QMOrder);
                        string strsqlDelete = string.Format("delete YAVHSQMOrder where OrderName = '{0}'", QMOrder);
                            DBHelper.ExecuteSql(strsqlDelete, SqlConnectionString);
                            string strsql = string.Format("insert into YAVHSQMOrder(OrderName,Description,ChangedOn,CreatedOn,AddDate) values('{0}','{1}','{2}','{3}','{4}')", QMOrder, Description, ChangedOn, CreatedOn, DateTime.Now.ToString("yyyy-MM-dd").Replace("-", ""));
                            DBHelper.ExecuteSql(strsql, SqlConnectionString);
                        }
                    
                }
            }
            return result;
        }


        public IEnumerable<string> UpdateSOForVHS(string so)
        {
            var result = new List<string>();
            var includeso = so.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var prd = CreateRfcDestination();
            ArrayList list = new ArrayList();
            list.Add("4130");
            // list.Add("6131");

            string CompanyCode = string.Empty;

            for (int j = 0; j < list.Count; j++)
            {
                CompanyCode = list[j].ToString();
                DataTable ds = Functions.ReadVBAKTable(prd, CompanyCode);
                int i = 0;
                foreach (DataRow row in ds.Rows)
                {
                    i++;
                    string strSalesOrderVPAK = row["VBELN"].ToString().Trim();
                    string strType = row["AUART"].ToString().Trim();
                    string strSOheaderCreateOn = row["ERDAT"].ToString().Trim();
                    string strSOheaderChangeOn = row["AEDAT"].ToString().Trim();
                    string CheckDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd").Replace("-", "");
                    if (strType == "ZJO")
                    {
                        if (includeso.Any(x => strSalesOrderVPAK.Contains(x)))
                        {
                            result.Add(strSalesOrderVPAK);
                            string strsqlDeleteSO = string.Format("Delete YAVHSSalesOrder where SD_DOC = '{0}'", strSalesOrderVPAK);
                            DBHelper.ExecuteSql(strsqlDeleteSO, SqlConnectionString);

                            //Insert the SO
                            DataTable dsVBPA = Functions.ReadVBPATable(prd, strSalesOrderVPAK);
                            foreach (DataRow rowVBPA in dsVBPA.Rows)
                            {
                                string strSalesOrder = rowVBPA["VBELN"].ToString(); //SalesOrder
                                string strItem = rowVBPA["POSNR"].ToString(); //Item
                                string strMaterial = rowVBPA["MATNR"].ToString().TrimEnd(); //Material
                                string strDescription = rowVBPA["ARKTX"].ToString().TrimEnd(); //Material Description
                                string strCreatedOn = rowVBPA["ERDAT"].ToString(); //Created On
                                string strChangedOn = rowVBPA["AEDAT"].ToString(); //Changed On
                                string strsql = string.Format(" insert into YAVHSSalesOrder(SD_DOC,ITM_NUMBER,Material,SHORT_TEXT,Created_On, Changed_On , DOC_TYPE ,UpdateDate) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", strSalesOrder, strItem, strMaterial, strDescription, strCreatedOn, strChangedOn, strType, DateTime.Now.ToString("yyyy-MM-dd"));
                                DBHelper.ExecuteSql(strsql, SqlConnectionString);
                            }

                        }

                    }
                }
            }
            return result;
        }


        public IEnumerable<string> UpdateInternalOrderForVHS(string orderno)
        {
            var result = new List<string>();
            var includeso = orderno.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var prd = CreateRfcDestination();

            string strsql = "Select CostCenter from YAVHSCostCenter";
            DataSet ds = DBHelper.Query(strsql, SqlConnectionString);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string cc = dr["CostCenter"].ToString();
                RfcRepository SapRfcRepository = prd.Repository;
                IRfcFunction BapiGetInternalorder = SapRfcRepository.CreateFunction("BAPI_INTERNALORDER_GETLIST");
                string strOrder = null; string strOrder_External_No = null; string strOrder_Type = null;
                string strOrder_External_No_To = null; string strOrder_To = null;
                BapiGetInternalorder.SetValue("CONTROLLING_AREA", "4000");
                BapiGetInternalorder.SetValue("RESP_COST_CENTER", dr["CostCenter"]);
                BapiGetInternalorder.SetValue("ORDER", strOrder);
                BapiGetInternalorder.SetValue("ORDER_EXTERNAL_NO", strOrder_External_No);
                BapiGetInternalorder.SetValue("ORDER_EXTERNAL_NO_TO", strOrder_External_No_To);
                BapiGetInternalorder.SetValue("ORDER_TO", strOrder_To);
                BapiGetInternalorder.SetValue("ORDER_TYPE", strOrder_Type);

                BapiGetInternalorder.Invoke(prd);


                IRfcTable IrfTable = BapiGetInternalorder.GetTable("ORDER_LIST");

                for (int i = 0; i < IrfTable.Count; i++)
                {

                    IrfTable.CurrentIndex = i;

                    string order = IrfTable.GetString("Order");
                    string ordername = IrfTable.GetString("Order_Name").Replace("'", "''");
                    if (includeso.Any(x => order.Contains(x)))
                    {
                        result.Add(order);
                        string strsqlDeleteSO = string.Format("Delete YAVHSInternalOrder where Number = '{0}'", order);
                        DBHelper.ExecuteSql(strsqlDeleteSO, SqlConnectionString);

                        string strinsertsql = string.Format("Insert into YAVHSInternalOrder(number,name,companyID,costcenterID,AddDate)values('{0}','{1}','{2}','{3}','{4}')", IrfTable.GetString("Order"), IrfTable.GetString("Order_Name").Replace("'", "''"), "4130", cc, DateTime.Now.ToString("yyyy-MM-dd").Replace("-", ""));
                        DBHelper.ExecuteSql(strinsertsql, SqlConnectionString);

                    }
                }

            }

            return result;
        }
     










        public void UpdateVendor2(RfcDestination prd)
        {
            DataTable ds = Functions.ReadSalesOrderTableCreator(prd, "VBAK", "0001001096");
            string c=string.Empty;
            if (ds.Rows.Count > 0)
            {
                foreach(DataRow row in ds.Rows)
                {
                     c = row["ERNAM"].ToString();
                }
                string d =c;
            }

            string VendorName=string.Empty;
            string CompanyCode="2130";
            string VendorCode="0003086814";
            RfcRepository SapRfcRepository = prd.Repository;
            IRfcFunction BapiVendorGetDetail = SapRfcRepository.CreateFunction("BAPI_VENDOR_GETDETAIL");
            BapiVendorGetDetail.SetValue("Companycode", CompanyCode);
            BapiVendorGetDetail.SetValue("Vendorno", VendorCode);
            BapiVendorGetDetail.Invoke(prd);
            IRfcStructure Generaldetail = BapiVendorGetDetail.GetStructure("Generaldetail");
            if (string.IsNullOrEmpty(Generaldetail.GetString("Name_2")))
                VendorName = Generaldetail.GetString("Name") + " " + Generaldetail.GetString("Name_2");
            else
                VendorName = Generaldetail.GetString("Name");
 
            string a = VendorName;
            string b = a;
           // return VendorName;

        }


        #region//Update Exchange Rate  OK
        //Update Exchagne Rate
        public void UpdateExchangeRate(RfcDestination prd)
        {

            // LogFile.WriteLog("Update Exchange Rate Start...");
            try
            {
                string strsql = null;
                string[] strFromCurrency = new string[] { "EUR", "USD", "GBP", "JPY", "SGD", "SEK", "HKD", "CHF", "CZK", "TWD", "INR", "MYR", "KRW", "VND", "THB", "ZAR", "TRY", "PHP", "NZD", "BRL", "AUD","NOK" };
                for (int i = 0; i < strFromCurrency.Length; i++)
                {
                    strsql = string.Format("Update ZAFiCurrency Set ExchangeRate={0} Where Name='{1}'", GetExchangeRate(prd, "M", strFromCurrency[i], "CNY"), strFromCurrency[i]);
                    DBHelper.ExecuteSql(strsql, SqlConnectionString);
                }
            }
            catch (Exception e)
            {
                //LogFile.WriteLog(e.InnerException.ToString());
            }

        }
        //Get Exchange Rate
        public string GetExchangeRate(RfcDestination prd, string strRateType, string strFromCurrency, string strToCurrency)
        {
            //使用RfcDestination对象的repository属性创建一个IRfcFunction对象为fm提供调用   
            RfcRepository SapRfcRepository = prd.Repository;

            IRfcFunction BapiGetExchangeRate = SapRfcRepository.CreateFunction("BAPI_EXCHANGERATE_GETDETAIL");
            BapiGetExchangeRate.SetValue("RATE_TYPE", "M");
            BapiGetExchangeRate.SetValue("FROM_CURR", strFromCurrency);
            BapiGetExchangeRate.SetValue("TO_CURRNCY", strToCurrency);
            BapiGetExchangeRate.SetValue("DATE", Convert.ToDateTime(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day));
            IRfcStructure detail = BapiGetExchangeRate.GetStructure("EXCH_RATE");
            BapiGetExchangeRate.Invoke(prd);
            prd = null;
            return detail.GetString("Exch_Rate");

        }
        #endregion

        #region//Update Cost Center  OK
        public void UpdateCostCenter(RfcDestination prd)
        {

            //BAPI0015_10 Language2 = new BAPI0015_10();
            //string a = Language2.Langu;
            //string b = Language2.Langu_Iso;


             LogFile.WriteLog("Update Cost Center Start...");
            try
            {
                RfcRepository SapRfcRepository = prd.Repository;
                IRfcFunction BapiGetCostCenter = SapRfcRepository.CreateFunction("BAPI_COSTCENTER_GETLIST");
                //BapiGetCostCenter.SetValue("Companycode", "2130");
                //BapiGetCostCenter.SetValue("Companycode_To", "2170");
                BapiGetCostCenter.SetValue("Companycode", "6130");
                BapiGetCostCenter.SetValue("Companycode_To", "6131");
                BapiGetCostCenter.SetValue("Controllingarea", "2000");
                BapiGetCostCenter.SetValue("Controllingarea_To", string.Empty);
                BapiGetCostCenter.SetValue("Costcenter", string.Empty);
                BapiGetCostCenter.SetValue("Costcenter_To", string.Empty);
                BapiGetCostCenter.SetValue("Costcentergroup", string.Empty);
                BapiGetCostCenter.SetValue("Date", string.Empty);
                BapiGetCostCenter.SetValue("Date_To", string.Empty);
                BapiGetCostCenter.SetValue("Person_In_Charge", string.Empty);
                BapiGetCostCenter.SetValue("Person_In_Charge_To", string.Empty);

                BapiGetCostCenter.Invoke(prd);
                IRfcTable IrfTable = BapiGetCostCenter.GetTable("Costcenter_List");
                IrfTable.CurrentIndex = 0;
                string aa = IrfTable.GetString("Costcenter");
                int sapCostCenterCount = IrfTable.Count;//Get the SAP Cost Center Count
                string strcostcentersql = "select * from ZACostCenter";// Get the Cost Center Count in local database
                DataSet dsCostCenter = DBHelper.Query(strcostcentersql, SqlConnectionString);
                int count = 0;
                int costCenterCount = dsCostCenter.Tables[0].Rows.Count;
                if (sapCostCenterCount != costCenterCount)
                {
                    for (int i = 0; i < IrfTable.Count; i++)
                    {
                        IrfTable.CurrentIndex = i;
                        string strsql = string.Format("Select count(*) as c From ZACostCenter Where CostCtr='{0}'", IrfTable.GetString("Costcenter").Substring(4));
                        DataSet ds = DBHelper.Query(strsql, SqlConnectionString);
                        string temp = ds.Tables[0].Rows[0][0].ToString();
                        string a = IrfTable.GetString("Costcenter");
                        if (Int32.Parse(temp) == 0 && IrfTable.GetString("Costcenter").Substring(4).Length == 6)
                        {
                            // BAPI0015_10 Language = new BAPI0015_10();  Ding Yi Add 2017-09-08
                            IRfcFunction BapiGetCostCenterDetail = SapRfcRepository.CreateFunction("BAPI_COSTCENTER_GETDETAIL1");
                            BapiGetCostCenterDetail.SetValue("Controllingarea", "2000");
                            BapiGetCostCenterDetail.SetValue("Costcenter", IrfTable.GetString("Costcenter"));
                            //IRfcStructure IRfcLanguage = BapiGetCostCenterDetail.GetStructure("Language");
                            //IRfcLanguage.SetValue("Langu", Language.Langu);
                            //IRfcLanguage.SetValue("Langu_Iso", Language.Langu_Iso);
                            BapiGetCostCenterDetail.SetValue("Master_Data_Inactive", "");
                            IRfcStructure CostCenterdetail = BapiGetCostCenterDetail.GetStructure("COSTCENTERDETAIL");
                            BapiGetCostCenterDetail.Invoke(prd);
                            string strName = CostCenterdetail.GetString("Name");
                            string strPerson_In_Charge = CostCenterdetail.GetString("Person_In_Charge");
                            string strComp_Code = CostCenterdetail.GetString("Comp_Code");
                            string strValid_From = CostCenterdetail.GetString("Valid_From");
                            string strValid_To = CostCenterdetail.GetString("Valid_To");
                            string strComp_Code2 = CostCenterdetail.GetString("Comp_Code");
                            string strinsertcostcentersql = string.Format("Insert into ZACostCenter(CostCtr,name,PersonResp,CoCd,ValidFromDate,ValidToDate,CompanyID) select '{0}','{1}','{2}','{3}','{4}','{5}',Id From ZACompany Where CoCd='{6}'", IrfTable.GetString("Costcenter").Substring(4), strName, strPerson_In_Charge, strComp_Code, strValid_From, strValid_To, strComp_Code2);
                            DBHelper.ExecuteSql(strinsertcostcentersql, SqlConnectionString);
                            count++;
                        }

                    }
                }
                for (int j = 0; j < IrfTable.Count; j++)
                {
                    //BAPI0015_10 Language = new BAPI0015_10();   Ding Yi Add 2017-09-08
                    IrfTable.CurrentIndex = j;
                    string shortName = "";
                    IRfcFunction BapiGetCostCenterDetail = SapRfcRepository.CreateFunction("BAPI_COSTCENTER_GETDETAIL1");
                    BapiGetCostCenterDetail.SetValue("Controllingarea", "2000");
                    BapiGetCostCenterDetail.SetValue("Costcenter", IrfTable.GetString("Costcenter"));
                   // IRfcStructure IRfcLanguage = BapiGetCostCenterDetail.GetStructure("Language");
                    //IRfcLanguage.SetValue("Langu", Language.Langu);
                   // IRfcLanguage.SetValue("Langu_Iso", Language.Langu_Iso);
                    BapiGetCostCenterDetail.SetValue("Master_Data_Inactive", "");
                    IRfcStructure CostCenterdetail = BapiGetCostCenterDetail.GetStructure("COSTCENTERDETAIL");
                    BapiGetCostCenterDetail.Invoke(prd);
                    string strValid_From = CostCenterdetail.GetString("Valid_From");
                    string strValid_To = CostCenterdetail.GetString("Valid_To");
                    if (!string.IsNullOrEmpty(CostCenterdetail.GetString("Person_In_Charge")) && CostCenterdetail.GetString("Person_In_Charge").IndexOf(",") != -1)
                    {
                        string[] names = CostCenterdetail.GetString("Person_In_Charge").Split(new char[] { ',' });
                        string sql1 = string.Format("Select Shortname From ZAUserinfo where lastname='{0}' and firstname='{1}'", names[0].Trim(), names[1].Trim());
                        DataSet dsUpdateCostCenter = DBHelper.Query(sql1, SqlConnectionString);
                        if (dsUpdateCostCenter.Tables[0].Rows.Count != 0)
                        {
                            shortName = dsUpdateCostCenter.Tables[0].Rows[0][0].ToString();
                        }
                    }
                    string sql = string.Format("Update ZACostCenter Set PersonResp='{0}',PersonShortName='{1}',ValidFromDate='{3}',ValidToDate='{4}' Where CostCtr='{2}'", CostCenterdetail.GetString("Person_In_Charge"), shortName, IrfTable.GetString("Costcenter").Substring(4), strValid_From, strValid_To);
                    DBHelper.ExecuteSql(sql, SqlConnectionString);
                }
                LogFile.WriteLog("Total Insert: " + count);
                LogFile.WriteLog("Update Cost Center End...");

            }
            catch (Exception ex)
            {
                LogFile.WriteLog(ex.Message + "\n" + ex.StackTrace);
            }
        }

        #endregion

        #region//UpdateInternalOrder  OK
        public void UpdateInternalOrder(RfcDestination prd)
        {
            LogFile.WriteLog("Update InternalOrder Start...");
            try
            {
               // int count = 0;
                string strsql = "Select CostCenter from YAVHSCostCenter";
                DataSet ds = DBHelper.Query(strsql, SqlConnectionString);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {                    
                    string cc = dr["CostCenter"].ToString();
                    RfcRepository SapRfcRepository = prd.Repository;
                    IRfcFunction BapiGetInternalorder = SapRfcRepository.CreateFunction("BAPI_INTERNALORDER_GETLIST");
                    string strOrder = null; string strOrder_External_No = null; string strOrder_Type = null;
                    string strOrder_External_No_To = null; string strOrder_To = null;
                    BapiGetInternalorder.SetValue("CONTROLLING_AREA", "4000");
                    BapiGetInternalorder.SetValue("RESP_COST_CENTER", dr["CostCenter"]);
                    BapiGetInternalorder.SetValue("ORDER", strOrder);
                    BapiGetInternalorder.SetValue("ORDER_EXTERNAL_NO", strOrder_External_No);
                    BapiGetInternalorder.SetValue("ORDER_EXTERNAL_NO_TO", strOrder_External_No_To);
                    BapiGetInternalorder.SetValue("ORDER_TO", strOrder_To);
                    BapiGetInternalorder.SetValue("ORDER_TYPE", strOrder_Type);

                    BapiGetInternalorder.Invoke(prd);

                
                    IRfcTable IrfTable = BapiGetInternalorder.GetTable("ORDER_LIST");

                    for (int i = 0; i < IrfTable.Count; i++)
                    {

                        IrfTable.CurrentIndex = i;
                        
                        string order = IrfTable.GetString("Order");
                        string ordername = IrfTable.GetString("Order_Name").Replace("'", "''");

                        string strIsExitInDB = string.Format("select COUNT (*) as Count from YAVHSInternalOrder where Number='{0}'", order);
                        DataSet dsCount= DBHelper.Query(strIsExitInDB, SqlConnectionString);
                        string strCount= dsCount.Tables[0].Rows[0]["Count"].ToString();
                        if ( strCount =="0")
                        {
                            string strinsertsql = string.Format("Insert into YAVHSInternalOrder(number,name,companyID,costcenterID)values('{0}','{1}','{2}','{3}')", IrfTable.GetString("Order"), IrfTable.GetString("Order_Name").Replace("'", "''"), "4130", cc);
                            DBHelper.ExecuteSql(strinsertsql, SqlConnectionString);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                LogFile.WriteLog(e.InnerException.ToString());
            }

        }
        #endregion

        #region//UpdateWBS
        public void UpdateWBS(RfcDestination prd)
        {
           // WriteLog("WBS Start");
            LogFile.WriteLog("Update WBS Start...");
            RfcRepository SapRfcRepository = prd.Repository;
            IRfcFunction BapiGetWBS = SapRfcRepository.CreateFunction("BAPI_PROJECTDEF_GETLIST");
            BapiGetWBS.SetValue("Max_Rows", 99999);

            BAPIPREXPTable Project_Definition_List = new BAPIPREXPTable();
            BAPI_2002_PD_RANGETable Project_Definition_Range = new BAPI_2002_PD_RANGETable();
            BAPI_2002_PD_RANGE range = new BAPI_2002_PD_RANGE();
            BAPI_2002_PD_RANGE range1 = new BAPI_2002_PD_RANGE();
            range.Sign = "I";
            //  添加以1和C开头的WBS 号码
            range.High = "";
            range.Low = "1*";
            range.Option0 = "CP";
            Project_Definition_Range.Add(range);

            range1.Sign = "I";
            // 添加以1和C开头的WBS 号码
            range1.High = "";
            range1.Low = "C*";
            range1.Option0 = "CP";
            Project_Definition_Range.Add(range1);

           // string a = Project_Definition_Range[0].Sign;

            

            IRfcTable Description_Range = BapiGetWBS.GetTable("Project_Definition_Range");
            for (int j = 0; j < Project_Definition_Range.Count; j++)
            {
                Description_Range.Append();
                Description_Range.SetValue("Sign", Project_Definition_Range[j].Sign);
                Description_Range.SetValue("High", Project_Definition_Range[j].High);
                Description_Range.SetValue("Low", Project_Definition_Range[j].Low);
                Description_Range.SetValue("Option", Project_Definition_Range[j].Option0);

            }

            LogFile.WriteLog("Log1 329line...");

            IRfcTable Description_List = BapiGetWBS.GetTable("Project_Definition_List");
            BapiGetWBS.Invoke(prd);
            int sapProjectCount = Description_List.Count;
            string strsql = "Select count(*) as c From ZAFiProjectDEF";
            DataSet ds = DBHelper.Query(strsql, SqlConnectionString);
            int ProjectCount1 = 0;
            int WBSCount = 0;
            int ProjectCount = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
            if (sapProjectCount != ProjectCount)
            {
                //for (int i = 3000; i < Description_List.Count; i++)
                for (int i = 0; i < Description_List.Count; i++)
                {
                    LogFile.WriteLog("Log1 344 line...times"+i);

                    Description_List.CurrentIndex = i;
                    ////如果以WBS查询的companycode 不在2130,2131,2170 之内的,在list中删除
                    //Project_Definition_List.Remove(bapiPREXP);
                    string start_wbs = Convert.ToString(Description_List.GetString("Project_Definition"));
                    if (start_wbs == "1-006879-378")
                    {
                    if (!start_wbs.StartsWith("C") && !start_wbs.StartsWith("1"))
                        continue;
                    string sql = string.Format("Select count(*) as c From ZAFiProjectDEF Where Project='{0}'", start_wbs);
                    DataSet ds2 = DBHelper.Query(sql, SqlConnectionString);
                    string temp = ds2.Tables[0].Rows[0][0].ToString();
                    if (Int32.Parse(temp) == 1)
                    {
                        //Ding Yi add
                        BAPI_PROJECT_DEFINITION_EX E_Project_Definition = new BAPI_PROJECT_DEFINITION_EX();
                        SAPVoithProxy.BAPIRETURN1 Return1 = new SAPVoithProxy.BAPIRETURN1();
                        BAPI_NETWORK_ACTIVITY_EXPTable E_Activity_Table = new BAPI_NETWORK_ACTIVITY_EXPTable();
                        BAPI_METH_MESSAGETable E_Message_Table = new BAPI_METH_MESSAGETable();
                        // BAPI_WBS_ELEMENT_EXPTable E_Wbs_Element_Table = new BAPI_WBS_ELEMENT_EXPTable();
                        BAPI_WBS_HIERARCHIETable E_Wbs_Hierarchie_Table = new BAPI_WBS_HIERARCHIETable();
                        BAPI_WBS_MILESTONE_EXPTable E_Wbs_Milestone_Table = new BAPI_WBS_MILESTONE_EXPTable();
                        BAPI_WBS_ELEMENTSTable I_Wbs_Element_Table = new BAPI_WBS_ELEMENTSTable();
                        BAPI_PROJECT_DEFINITION_EX E_Project_Definition_Detail = new BAPI_PROJECT_DEFINITION_EX();

                        IRfcFunction BapiGetProjectInfo = SapRfcRepository.CreateFunction("BAPI_PROJECT_GETINFO");
                        BapiGetProjectInfo.SetValue("Project_Definition", start_wbs);
                        string strWith_Activities = null;
                        string strWith_Milestones = null;
                        string strWith_Subtree = null;
                        BapiGetProjectInfo.SetValue("With_Activities", strWith_Activities);
                        BapiGetProjectInfo.SetValue("With_Milestones", strWith_Milestones);
                        BapiGetProjectInfo.SetValue("With_Subtree", strWith_Subtree);
                        IRfcTable wbs = BapiGetProjectInfo.GetTable("E_Wbs_Element_Table");
                        BapiGetProjectInfo.Invoke(prd);
                        IRfcStructure E_Project_Definition2 = BapiGetProjectInfo.GetStructure("E_PROJECT_DEFINITION");

                        DataSet ds1 = new DataSet();
                        int CompanyID = -1;
                        //循环判断project的公司代码是否为2130,2131,2170
                        string strsqlcompanycode = string.Format("Select ID From ZACompany Where CoCd='{0}'", E_Project_Definition2.GetString("Comp_Code"));
                        ds1 = DBHelper.Query(strsqlcompanycode, SqlConnectionString);
                        if (ds1.Tables[0].Rows.Count > 0)
                            CompanyID = (int)ds1.Tables[0].Rows[0]["ID"];
                        //如果公司代码不在范围之内,再判断wbs号是否在范围之内
                        if (CompanyID == -1)
                        {
                            //Modify By Zhu,Jun
                            TAB512Table data = new TAB512Table();
                            RFC_DB_OPTTable options = new RFC_DB_OPTTable();
                            string strSalesOrder = Description_List.GetString("Project_Definition").Substring(0, Description_List.GetString("Project_Definition").LastIndexOf("-"));
                            string strSalesOrderFinal = strSalesOrder.Replace("-", "").PadLeft(10, '0'); string strDisplayName = string.Empty;
                            DataTable dsVBELNMC = Functions.ReadWBSTableGetZPSOrder(prd, "Z20PST_KONF_UP00", strSalesOrderFinal);
                            if (dsVBELNMC.Rows.Count > 0)
                            {

                                for (int iwbs = 0; iwbs < wbs.Count; iwbs++)
                                {
                                    wbs.CurrentIndex = iwbs;
                                    string strsqlwbs = string.Format("Select ID From ZACompany Where CoCd='{0}'", wbs.GetString("Comp_Code"));
                                    DataSet dswbs = DBHelper.Query(strsqlwbs, SqlConnectionString);
                                    if (dswbs.Tables[0].Rows.Count > 0)
                                    {
                                        CompanyID = (int)dswbs.Tables[0].Rows[0]["ID"];
                                        break;
                                    }
                                }
                                LogFile.WriteLog("Log1 410 line...");
                            }
                            else
                            {
                                for (int iwbs = 0; iwbs < wbs.Count; iwbs++)
                                {
                                    wbs.CurrentIndex = iwbs;
                                    string strsqlwbs = string.Format("Select ID From ZACompany Where CoCd='{0}'", wbs.GetString("Comp_Code"));
                                    DataSet dswbs = DBHelper.Query(strsqlwbs, SqlConnectionString);
                                    if (dswbs.Tables[0].Rows.Count > 0)
                                    {
                                        CompanyID = (int)dswbs.Tables[0].Rows[0]["ID"];
                                        break;
                                    }
                                }
                                LogFile.WriteLog("Log1 421 line...");
                            }
                        }
                       // 最终决定是否要添加该项目
                        if (CompanyID != -1)
                        {
                            //Modify By Zhu,Jun
                            TAB512Table data = new TAB512Table();
                            RFC_DB_OPTTable options = new RFC_DB_OPTTable();
                            string strSalesOrder = Description_List.GetString("Project_Definition").Substring(0, Description_List.GetString("Project_Definition").LastIndexOf("-"));
                            string strSalesOrderFinal = strSalesOrder.Replace("-", "").PadLeft(10, '0'); string strDisplayName = string.Empty;
                            DataTable dsVBELNMC = Functions.ReadWBSTableGetZPSOrder(prd, "Z20PST_KONF_UP00", strSalesOrderFinal);
                            if (dsVBELNMC.Rows.Count > 0)
                            {
                                int iShortName = 0;
                                foreach (DataRow row in dsVBELNMC.Rows)
                                {
                                    DataTable dsSalesOrder = Functions.ReadTableSalesOrderCreatorByZMC(prd, "VBAK", row["VBELN"].ToString(), E_Project_Definition2.GetString("Comp_Code"));
                                    if (dsSalesOrder.Rows.Count > 0)
                                    {
                                        foreach (DataRow row2 in dsSalesOrder.Rows)
                                        {
                                            if (!string.IsNullOrEmpty(row2["ERNAM"].ToString()))
                                            {
                                                //string strsql2 = string.Format("select * from zauserinfo where shortname='{0}'", row["ERNAM"].ToString().Trim());
                                                //DataSet ds3 = DBHelper.Query(strsql, SqlConnectionString);
                                                //if (ds3.Tables[0].Rows.Count > 0)
                                                //{
                                                //    foreach (DataRow row3 in ds2.Tables[0].Rows)
                                                //    {
                                                //        strDisplayName = row2["LastName"].ToString() + ", " + row2["FirstName"].ToString();
                                                //    }
                                                //}

                                                iShortName = iShortName + 1;
                                            }
                                        }
                                    }

                                }
                                LogFile.WriteLog("Log1 460 line...");
                                if (iShortName > 0)
                                {
                                    string strinsertsql = string.Format("Insert into ZAFiProjectDEF(Project,Description,companyID)values('{0}','{1}',{2})", Description_List.GetString("Project_Definition"), Description_List.GetString("Description").Replace("'", "''"), CompanyID);
                                    DBHelper.ExecuteSql(strinsertsql, SqlConnectionString);
                                    ProjectCount1++;
                                }
                            }


                        }
                        else
                            continue;

                        LogFile.WriteLog("Log1 475line...");

                        //添加在范围之内的WBS号
                        for (int iwbs = 0; iwbs < wbs.Count; iwbs++)
                        {
                            wbs.CurrentIndex = iwbs;
                            int subComID = -1;
                            string strsqlwbs2 = string.Format("Select ID From ZACompany Where CoCd='{0}'", wbs.GetString("Comp_Code"));
                            DataSet dswbs = DBHelper.Query(strsqlwbs2, SqlConnectionString);
                            if (dswbs.Tables[0].Rows.Count > 0)
                            {
                                subComID = (int)dswbs.Tables[0].Rows[0]["ID"];
                            }
                            if (subComID != -1)
                            {
                                string strsqlprojectdef = string.Format("Select Top 1 ID From ZAFiProjectDEF Order By ID Desc");
                                DataSet dsprojectdef = DBHelper.Query(strsqlprojectdef, SqlConnectionString);
                                int ProjectId = (int)dsprojectdef.Tables[0].Rows[0]["ID"];
                                //Modify By Zhu, Jun
                                string strSalesOrder = wbs.GetString("Wbs_Element").Substring(0, wbs.GetString("Wbs_Element").LastIndexOf("-"));
                                string strSalesOrderFinal = strSalesOrder.Replace("-", "").PadLeft(10, '0'); string strDisplayName = string.Empty;
                                DataTable dsVBELNMC2 = Functions.ReadWBSTableGetZPSOrder(prd, "Z20PST_KONF_UP00", strSalesOrderFinal);
                                DataView dv = new DataView(dsVBELNMC2);                           //虚拟视图吧，我这么认为
                                DataTable dsVBELNMC = dv.ToTable(true, "VBELN");
                                if (dsVBELNMC.Rows.Count > 0)
                                {
                                    foreach (DataRow row in dsVBELNMC.Rows)
                                    {
                                        string strcompanycode = wbs.GetString("Comp_Code");
                                        //DataTable dsSalesOrder = Functions.ReadTableSalesOrderCreatorByZMC(prd, "VBAK", row["VBELN"].ToString(), wbs.GetString("Comp_Code"));
                                        //if (dsSalesOrder.Rows.Count > 0)
                                        //{
                                            //foreach (DataRow row2 in dsSalesOrder.Rows)
                                            //{
                                                //if (!string.IsNullOrEmpty(row2["ERNAM"].ToString()))
                                               // {
                                                   // string strsql2 = string.Format("select * from zauserinfo where shortname='{0}'", row2["ERNAM"].ToString().Trim());
                                                   // DataSet ds3 = DBHelper.Query(strsql2, SqlConnectionString);
                                                   // if (ds3.Tables[0].Rows.Count > 0)
                                                    //{
                                                        //foreach (DataRow row3 in ds3.Tables[0].Rows)
                                                        //{
                                                            //strDisplayName = row3["LastName"].ToString() + ", " + row3["FirstName"].ToString();
                                                            string strinsertsql2 = string.Format("Insert into ZAFiWBSNo(WBSElement,Description,ProjectID,PMDisplayName,PMShortName,LeaderTitle)values('{0}','{1}',{2},'{3}','{4}','PM')", wbs.GetString("Wbs_Element"), wbs.GetString("Description").Replace("'", "''"), ProjectId, strDisplayName, "");
                                                            DBHelper.ExecuteSql(strinsertsql2, SqlConnectionString);
                                                        //}
                                                    //}
                                                    WBSCount++;
                                                //}
                                            //}
                                        //}
                                    }
                                }
                            }

                            }
                        LogFile.WriteLog("Log1 530 line...");

                        }
                    }
                }
            }

            //Update wbs
            string sql1 = string.Format("Select * From ZAFiProjectDEF");
            DataSet dsfiprojectdef = DBHelper.Query(sql1, SqlConnectionString);
            foreach (DataRow dr in dsfiprojectdef.Tables[0].Rows)
            {
                if (dr["Project"].ToString() == "1-006879-378")
                {

       
                BAPI_PROJECT_DEFINITION_EX E_Project_Definition = new BAPI_PROJECT_DEFINITION_EX();
                SAPVoithProxy.BAPIRETURN1 Return1 = new SAPVoithProxy.BAPIRETURN1();
                BAPI_NETWORK_ACTIVITY_EXPTable E_Activity_Table = new BAPI_NETWORK_ACTIVITY_EXPTable();
                BAPI_METH_MESSAGETable E_Message_Table = new BAPI_METH_MESSAGETable();
                BAPI_WBS_ELEMENT_EXPTable E_Wbs_Element_Table = new BAPI_WBS_ELEMENT_EXPTable();
                BAPI_WBS_HIERARCHIETable E_Wbs_Hierarchie_Table = new BAPI_WBS_HIERARCHIETable();
                BAPI_WBS_MILESTONE_EXPTable E_Wbs_Milestone_Table = new BAPI_WBS_MILESTONE_EXPTable();
                BAPI_WBS_ELEMENTSTable I_Wbs_Element_Table = new BAPI_WBS_ELEMENTSTable();
                IRfcFunction BapiGetProjectInfo = SapRfcRepository.CreateFunction("BAPI_PROJECT_GETINFO");
                BapiGetProjectInfo.SetValue("Project_Definition", (string)dr["Project"]);
                string strWith_Activities = null; string strWith_Milestones = null; string strWith_Subtree = null;
                BapiGetProjectInfo.SetValue("With_Activities", strWith_Activities);
                BapiGetProjectInfo.SetValue("With_Milestones", strWith_Milestones);
                BapiGetProjectInfo.SetValue("With_Subtree", strWith_Subtree);
                BapiGetProjectInfo.Invoke(prd);
                IRfcTable wbs = BapiGetProjectInfo.GetTable("E_Wbs_Element_Table");
                string strsql1 = string.Format("Select count(*) as c From ZAFiWBSNo WHere projectID=(Select ID From ZAFiProjectDEF WHere project='{0}')", dr["Project"]);
                DataSet ds3 = DBHelper.Query(strsql1, SqlConnectionString);
                int WBSNoCount = Int32.Parse(ds3.Tables[0].Rows[0][0].ToString());
                if (WBSNoCount != E_Wbs_Milestone_Table.Count)
                {
                    int ret = 0;
                    for (int iwbs = 0; iwbs < wbs.Count; iwbs++)
                    {
                        wbs.CurrentIndex = iwbs;
                        int subComID2 = -1;
                        string strsql2 = string.Format("Select ID From ZACompany Where CoCd='{0}'", wbs.GetString("Comp_Code"));
                        DataSet dsSub = DBHelper.Query(strsql2, SqlConnectionString);
                        if (dsSub.Tables[0].Rows.Count > 0)
                            subComID2 = (int)dsSub.Tables[0].Rows[0]["ID"];
                        if (subComID2 != -1)
                        {
                            //Modify By Zhu, Jun
                            string strSalesOrder = wbs.GetString("Wbs_Element").Substring(0, wbs.GetString("Wbs_Element").LastIndexOf("-"));
                            string strSalesOrderFinal = strSalesOrder.Replace("-", "").PadLeft(10, '0'); string strDisplayName = string.Empty;
                            DataTable dsVBELNMC2 = Functions.ReadWBSTableGetZPSOrder(prd, "Z20PST_KONF_UP00", strSalesOrderFinal);
                            DataView dv = new DataView(dsVBELNMC2);                           //虚拟视图吧，我这么认为
                            DataTable dsVBELNMC = dv.ToTable(true, "VBELN_MC");
                            if (dsVBELNMC.Rows.Count > 0)
                            {
                                foreach (DataRow row in dsVBELNMC.Rows)
                                {
                                    DataTable dsSalesOrder = Functions.ReadTableSalesOrderCreatorByZMC(prd, "VBAK", row["VBELN_MC"].ToString(), wbs.GetString("Comp_Code"));
                                    if (dsSalesOrder.Rows.Count > 0)
                                    {
                                        foreach (DataRow row2 in dsSalesOrder.Rows)
                                        {
                                            if (!string.IsNullOrEmpty(row2["ERNAM"].ToString()))
                                            {
                                                string strsql3 = string.Format("select * from zauserinfo where shortname='{0}'", row2["ERNAM"].ToString().Trim());
                                                DataSet ds4 = DBHelper.Query(strsql3, SqlConnectionString);
                                                if (ds4.Tables[0].Rows.Count > 0)
                                                {
                                                    foreach (DataRow row4 in ds4.Tables[0].Rows)
                                                    {
                                                        strDisplayName = row4["LastName"].ToString() + ", " + row4["FirstName"].ToString();
                                                        string strsqlupdate = string.Format("Select * From ZAFiWBSNo where wbsElement='{0}' and projectid=(Select ID from ZAFiProjectDEF where project='{1}')", wbs.GetString("Wbs_Element"), dr["Project"]);
                                                        DataSet dsupdate = DBHelper.Query(strsqlupdate, SqlConnectionString);
                                                        if (dsupdate.Tables[0].Rows.Count > 0)
                                                        {
                                                            string strsqlupdatepm = string.Format("update ZAFiWBSNo set PMDisplayName='{0}',PMShortName='{1}' where wbselement='{2}'", strDisplayName, row2["ERNAM"].ToString().Trim(), wbs.GetString("Wbs_Element"));
                                                            ret = DBHelper.ExecuteSql(strsqlupdatepm, SqlConnectionString);
                                                        }
                                                        else
                                                        {
                                                            string strsql5 = string.Format("insert into ZAFiWBSNo(wbselement,description,projectid,PMDisplayName,PMShortName,LeaderTitle) select '{0}','{2}',ID,'{3}','{4}','PM' From ZAFiProjectDEF where project='{1}'", wbs.GetString("Wbs_Element"), dr["Project"], wbs.GetString("Description").Replace("'", "''"), strDisplayName, row2["ERNAM"].ToString().Trim());
                                                            ret = DBHelper.ExecuteSql(strsql5, SqlConnectionString);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (ret > 0)
                                WBSCount++;
                        }
                        else
                        {
                            string strsql3 = string.Format("if exists(Select * From ZAFiWBSNo where wbsElement='{0}' and projectid=(Select ID from ZAFiProjectDEF where project='{1}')) delete from ZAFiWBSNo where wbsElement='{0}' and projectid=(Select ID from ZAFiProjectDEF where project='{1}')", wbs.GetString("Wbs_Element"), dr["Project"].ToString().Replace("'", "''"));
                            ret = DBHelper.ExecuteSql(strsql3, SqlConnectionString);
                        }
                    }
                }
                 }
            }

            LogFile.WriteLog("Log1 632 line...");


            //Update wbs status
            string strsql4 = string.Format("Select * From ZAFiWBSNo");
            DataSet ds0 = DBHelper.Query(strsql4, SqlConnectionString);
            foreach (DataRow dr in ds0.Tables[0].Rows)
            {

                int WBSNoId = (int)dr["ID"];
                //if (WBSNoId == 82527)
                //{
                string wbsNo = (string)dr["WBSElement"];
                BAPI_STATUS_RESULTTable E_Result = new BAPI_STATUS_RESULTTable();
                BAPI_WBS_SYSTEM_STATUSTable E_System_Status = new BAPI_WBS_SYSTEM_STATUSTable();
                BAPI_WBS_USER_STATUSTable E_User_Status = new BAPI_WBS_USER_STATUSTable();
                BAPI_WBS_ELEMENTSTable I_Wbs_Elements = new BAPI_WBS_ELEMENTSTable();
                BAPI_WBS_ELEMENTS e = new BAPI_WBS_ELEMENTS();
                e.Wbs_Element = wbsNo;
                I_Wbs_Elements.Add(e);
                IRfcFunction BapiBus2054GetStatus = SapRfcRepository.CreateFunction("BAPI_BUS2054_GET_STATUS");
                IRfcTable irfcI_Wbs_Elements = BapiBus2054GetStatus.GetTable("I_Wbs_Elements");
                for (int i = 0; i < I_Wbs_Elements.Count; i++)
                {
                    irfcI_Wbs_Elements.Append();
                    irfcI_Wbs_Elements.SetValue("WBS_ELEMENT", I_Wbs_Elements[i].Wbs_Element);
                }
                BapiBus2054GetStatus.Invoke(prd);
                IRfcTable status = BapiBus2054GetStatus.GetTable("E_System_Status");
                for (int istatus = 0; istatus < status.Count; istatus++)
                {
                    status.CurrentIndex = istatus;
                    string strsql1 = string.Format("Select Count(*) as c From ZAFiWBSStatus Where status='{0}' And wbsnoid={1}", status.GetString("System_Status"), WBSNoId);
                    DataSet ds1 = DBHelper.Query(strsql1, SqlConnectionString);
                    int c = (int)ds1.Tables[0].Rows[0]["c"];
                    if (c == 0)
                    {
                        string strsql2 = string.Format("Insert into ZAFiWBSStatus(status,wbsnoid)values('{0}','{1}')", status.GetString("System_Status"), WBSNoId);
                        DBHelper.ExecuteSql(strsql2, SqlConnectionString);
                    }
                }
                //}
            }


            LogFile.WriteLog("Wbs End...");
         
            int baohh = 0;
           
            //Console.WriteLine("Project Total Insert: " + ProjectCount1);
            //Console.WriteLine("WBS Total Insert: " + WBSCount);
            //Console.WriteLine("Update WBS End...");

        }
        #endregion

        #region//UpdateVendor  ok
        public void UpdateVendor(RfcDestination prd)
        {
            //LogFile.WriteLog("Update Vendor Start...");
            try
            {
                string sql = null;
                int count = 0;
                //bhh  3月 修改添加feilds 参数
                TAB512Table data = new TAB512Table();
                RFC_DB_OPTTable options = new RFC_DB_OPTTable();
                RFC_DB_FLDTable fields = new RFC_DB_FLDTable();
                RFC_DB_FLD fld = new RFC_DB_FLD();
                fld.Fieldname = "BUKRS ";
                fields.Add(fld);
                RFC_DB_FLD fld2 = new RFC_DB_FLD();
                fld2.Fieldname = "LIFNR ";
                fields.Add(fld2);
                DataTable dt = Functions.ReadTable(prd, "LFB1", options);
                DataSet ds = null;
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string CompanyCode = dr["BUKRS"].ToString();
                        string VendorCode = dr["LIFNR"].ToString();
                        string VendorCountry = null;
                        string VendorName = null;
                        string VendorCity = null;
                        string VendorStreet = null;
                        string VendorTelephone = null;
                        if (VendorCode == "0003086814")
                        {
                        if (CompanyCode == "2130" || CompanyCode == "2131" || CompanyCode == "2170")
                        {
                            sql = string.Format("Select Count(*) As C From ZAVendorInfo Where vendorcode='{0}' And CompanyCode='{1}'", Functions.ParseVendorCode(VendorCode), CompanyCode);
                            ds = DBHelper.Query(sql, SqlConnectionString);
                            string VendorCount = ds.Tables[0].Rows[0][0].ToString();
                            if (Int32.Parse(VendorCount) == 1)
                            {

                                RfcRepository SapRfcRepository = prd.Repository;
                                IRfcFunction BapiVendorGetDetail = SapRfcRepository.CreateFunction("BAPI_VENDOR_GETDETAIL");
                                BapiVendorGetDetail.SetValue("Companycode", CompanyCode);
                                BapiVendorGetDetail.SetValue("Vendorno", VendorCode);
                                BapiVendorGetDetail.Invoke(prd);
                                IRfcStructure Generaldetail = BapiVendorGetDetail.GetStructure("Generaldetail");
                                if (string.IsNullOrEmpty(Generaldetail.GetString("Name_2")))
                                    VendorName = Generaldetail.GetString("Name") + " " + Generaldetail.GetString("Name_2");
                                else
                                    VendorName = Generaldetail.GetString("Name");
                                VendorCountry = Generaldetail.GetString("Country");
                                VendorCity = Generaldetail.GetString("City");
                                VendorStreet = Generaldetail.GetString("Street");
                                VendorTelephone = Generaldetail.GetString("Telephone");

                                sql = string.Format("insert into ZAVendorInfo(VendorCode,CompanyCode,CompanyID,VendorName,Country,City,Street,Telephone)Select '{0}','{1}',ID,'{2}','{3}','{4}','{5}','{6}' From ZACompany Where Cocd='{1}'", Functions.ParseVendorCode(VendorCode), CompanyCode, VendorName.Replace('\'', ' '), VendorCountry, VendorCity.Replace('\'', ' '), VendorStreet.Replace('\'', ' '), VendorTelephone);
                                int ret = DBHelper.ExecuteSql(sql, SqlConnectionString);
                                if (ret > 0)
                                    count++;
                            }
                        }
                        }
                    }
                }

                //Update Vendor zh-cn,bank info
               // LogFile.WriteLog("Update Vendor bank info start...");
                sql = string.Format("select * From ZAvendorinfo");
                ds = DBHelper.Query(sql, SqlConnectionString);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string a = dr["VendorCode"].ToString();
                    
                    if (dr["VendorCode"].ToString() == "3313083")
                    {
                        RfcRepository SapRfcRepository = prd.Repository;
                        IRfcFunction BapiVendorBank = SapRfcRepository.CreateFunction("ZBAPI_VENDOR_BANK");
                        BapiVendorBank.SetValue("Companycode", (string)dr["CompanyCode"]);
                        BapiVendorBank.SetValue("Vendor", Functions.FormatVendorCode(dr["VendorCode"]));
                        BapiVendorBank.Invoke(prd);
                        IRfcTable VendorBank = BapiVendorBank.GetTable("Vendorbank");
                        for (int i = 0; i < VendorBank.Count; i++)
                        {
                            VendorBank.CurrentIndex = i;
                            sql = string.Format("Update zavendorinfo set [vendorname-zh-cn]='{0}',bankname='{1}',[bankname-zh-cn]='{2}',account='{3}',[subbankname]='{4}' where vendorcode='{5}' ", VendorBank.GetString("Name1"), VendorBank.GetString("Name4"), VendorBank.GetString("Name3"), VendorBank.GetString("Bankn"), VendorBank.GetString("Brnch"), dr["VendorCode"]);
                            DBHelper.ExecuteSql(sql, SqlConnectionString);
                        }
                    
                }
                }
                //LogFile.WriteLog("Update Vendor bank info end...");
                // LogFile.WriteLog("Total Insert:" + count);
                // LogFile.WriteLog("Update Vendor End...");
            }
            catch (Exception e)
            {

            }
        }
        #endregion

        #region//Update Purchase Order  OK
        //Update Exchagne Rate
        public void UpdatePurchaseOrder(RfcDestination prd)
        {
            //LogFile.WriteLog("Update Purchase Order Start...");
            string sql = null;
            int count = 0;
            try
            {
                //使用RfcDestination对象的repository属性创建一个IRfcFunction对象为fm提供调用   
                RfcRepository SapRfcRepository = prd.Repository;
                IRfcFunction BapiPOGetItems = SapRfcRepository.CreateFunction("BAPI_PO_GETITEMS");
                BapiPOGetItems.SetValue("Acctasscat", string.Empty);
                BapiPOGetItems.SetValue("Created_By", string.Empty);
                BapiPOGetItems.SetValue("Deleted_Items", string.Empty);
                BapiPOGetItems.SetValue("Doc_Date", string.Empty);
                BapiPOGetItems.SetValue("Doc_Type", string.Empty);
                BapiPOGetItems.SetValue("Item_Cat", string.Empty);
                BapiPOGetItems.SetValue("Items_Open_For_Receipt", string.Empty);
                BapiPOGetItems.SetValue("Mat_Grp", string.Empty);
                BapiPOGetItems.SetValue("Material", string.Empty);
                BapiPOGetItems.SetValue("Plant", string.Empty);
                BapiPOGetItems.SetValue("Preq_Name", string.Empty);
                BapiPOGetItems.SetValue("Pur_Group", string.Empty);
                BapiPOGetItems.SetValue("Pur_Mat", string.Empty);
                BapiPOGetItems.SetValue("Purch_Org", "2130");
                BapiPOGetItems.SetValue("Purchaseorder", string.Empty);
                BapiPOGetItems.SetValue("Short_Text", string.Empty);
                BapiPOGetItems.SetValue("Suppl_Plant", string.Empty);
                BapiPOGetItems.SetValue("Trackingno", string.Empty);
                BapiPOGetItems.SetValue("Vendor", string.Empty);
                BapiPOGetItems.SetValue("With_Po_Headers", "X");
                //BapiPOGetItems.SetValue("", string.Empty);
                BapiPOGetItems.Invoke(prd);
                IRfcTable RFCPOHeaders = BapiPOGetItems.GetTable("Po_Headers");
                if (RFCPOHeaders.Count > 0)
                {
                    for (int i = 0; i < RFCPOHeaders.Count; i++)
                    {
                        RFCPOHeaders.CurrentIndex = i;
                        string vendorcode = null;
                        vendorcode = RFCPOHeaders.GetString("Vendor") == null ? "" : RFCPOHeaders.GetString("Vendor").Substring(3);
                        if (vendorcode == "3079462")
                        {
                         string s = RFCPOHeaders.GetString("Po_Number");
                        sql = string.Format("if not exists(Select * From ZAFiPurchaseOrder where number='{0}') Insert into ZAFiPurchaseOrder(Number,CompanyCode,CompanyID,VendorCode,VendorID)Select '{0}','{1}',ZACompany.ID,'{2}',ZAVendorInfo.ITEM From ZACompany,ZAVendorInfo WHere ZACompany.Cocd='{1}' And ZAVendorInfo.VendorCode='{2}' And ZAVendorInfo.CompanyCode='{1}'", RFCPOHeaders.GetString("Po_Number"), RFCPOHeaders.GetString("Co_Code"), vendorcode);
                        int ret = DBHelper.ExecuteSql(sql, SqlConnectionString);
                        if (ret > 0)
                            count++;
                         }
                    }
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                prd = null;
            }
            //LogFile.WriteLog("Total Insert:" + count);
            //LogFile.WriteLog("Update Purchase Order End...");
        }
        #endregion

        #region//Update Sales Order  OK
        public void UpdateSalesOrder(RfcDestination prd)
        {
            //LogFile.WriteLog("Update Sales Order Start...");
            try
            {
                string strDisplayName = string.Empty;
                //使用RfcDestination对象的repository属性创建一个IRfcFunction对象为fm提供调用 
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

                ArrayList list = new ArrayList();
                list.Add("2130");
                list.Add("2131");
                list.Add("2170");
                IRfcTable Address = BapiCustomerGetList.GetTable("Addressdata");
                for (int i = 0; i < Address.Count; i++)
                {
                    Address.CurrentIndex = i;

                    string strtest = Address.GetString("Customer");
                    // if (strtest == "0006026547") 0006024077 0006019940 0006007580
                    if (strtest == "0006014690")
                    {
                        for (int j = 0; j < list.Count; j++)
                        {
                            string salesOrg = (string)list[j];
                            IRfcFunction BapiSalesORDERGetList = SapRfcRepository.CreateFunction("BAPI_SALESORDER_GETLIST");
                            BapiSalesORDERGetList.SetValue("Customer_Number", Address.GetString("Customer"));
                            BapiSalesORDERGetList.SetValue("Document_Date", string.Empty);
                            BapiSalesORDERGetList.SetValue("Document_Date_To", string.Empty);
                            BapiSalesORDERGetList.SetValue("Material", string.Empty);
                            BapiSalesORDERGetList.SetValue("Purchase_Order", string.Empty);
                            BapiSalesORDERGetList.SetValue("Purchase_Order_Number", string.Empty);
                            BapiSalesORDERGetList.SetValue("Sales_Organization", salesOrg);
                            BapiSalesORDERGetList.SetValue("Transaction_Group", "0");
                            BapiSalesORDERGetList.Invoke(prd);

                            // IRfcStructure RETURNStr = BapiSalesORDERGetList.GetStructure("Return");

                            // MessageBox.Show(RETURNStr);  

                            IRfcTable order = BapiSalesORDERGetList.GetTable("Sales_Orders");
                            DataTable dsPM22= Functions.ReadSalesOrderTableGetPM(prd, "VBPA", order.GetString("Sd_Doc"));
                            DataTable ds22 = Functions.ReadSalesOrderTableCreator(prd, "VBAK", order.GetString("Sd_Doc"));
                            string ds22Test = dsPM22.Rows.Count.ToString();
                            string dsPM22TEST = ds22.Rows.Count.ToString();

                            for (int k = 0; k < order.Count; k++)
                            {
                                order.CurrentIndex = k;
                                StringBuilder sql = new StringBuilder();
                                DataTable ds = Functions.ReadSalesOrderTableCreator(prd, "VBAK", order.GetString("Sd_Doc"));
                                if (ds.Rows.Count > 0)
                                {
                                    foreach (DataRow row in ds.Rows)
                                    {
                                        if (!string.IsNullOrEmpty(row["ERNAM"].ToString().Trim()))         
                                        {
                                            string strsql = string.Format("select * from zauserinfo where shortname='{0}'", row["ERNAM"].ToString().Trim());
                                            DataSet ds2 = DBHelper.Query(strsql, SqlConnectionString);

                                            if (ds2.Tables[0].Rows.Count > 0)
                                            {
                                                foreach (DataRow row2 in ds2.Tables[0].Rows)
                                                {
                                                    strDisplayName = row2["LastName"].ToString() + ", " + row2["FirstName"].ToString();
                                                    DataSet dsSalesOrder = DBHelper.Query(string.Format("Select * from ZAFiSalesOrder where SD_DOC='{0}' and ITM_NUMBER='{1}'", row["VBELN"].ToString().Trim(), order.GetString("Itm_Number")), SqlConnectionString);
                                                    if (dsSalesOrder.Tables[0].Rows.Count > 0)
                                                    {
                                                        //sql.AppendFormat("if exists(Select * from ZAFiSalesOrder where SD_DOC='{0}')", row["VBELN"].ToString().Trim());
                                                        sql.AppendFormat(" update ZAFiSalesOrder set PMDisplayName='{0}',PMShortName='{1}',LeaderTitle='PM' where SD_DOC='{2}'", strDisplayName, row["ERNAM"].ToString().Trim(), row["VBELN"].ToString().Trim());
                                                        DBHelper.ExecuteSql(sql.ToString(), SqlConnectionString);
                                                    }
                                                    else
                                                    {
                                                        //sql.AppendFormat("if not exists(Select * from ZAFiSalesOrder where SD_DOC='{0}' and ITM_NUMBER='{1}')", order.GetString("Sd_Doc"), order.GetString("Itm_Number"));
                                                        sql.AppendFormat(" insert into ZAFiSalesOrder(SD_DOC,ITM_NUMBER,Material,SHORT_TEXT,DOC_TYPE,DOC_DATE,PMDisplayName,PMShortName,LeaderTitle) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','PM')", order.GetString("Sd_Doc"), order.GetString("Itm_Number"), order.GetString("Material"), order.GetString("Short_Text").Replace("'", "''"), order.GetString("Doc_Type"), Convert.ToDateTime(order.GetString("Doc_Date")).ToString("yyyyMMdd"), strDisplayName, row["ERNAM"].ToString().Trim());
                                                        DBHelper.ExecuteSql(sql.ToString(), SqlConnectionString);
                                                    }
                                                }

                                            }
                                        }

                                    }
                                }
                                else
                                {
                                    if (order.GetString("Sd_Doc").ToString().StartsWith("1"))
                                    {
                                        DataTable dsPM = Functions.ReadSalesOrderTableGetPM(prd, "VBPA", order.GetString("Sd_Doc"));
                                        if (dsPM.Rows.Count > 0)
                                        {
                                            foreach (DataRow row in ds.Rows)
                                            {
                                                if (!string.IsNullOrEmpty(row["KUNNR"].ToString().Trim()) && row["PARVW"].ToString() == "Z7")
                                                {
                                                    string strsql = string.Format("select * from zauserinfo where shortname='{0}'", row["KUNNR"].ToString().Trim());
                                                    DataSet ds2 = DBHelper.Query(strsql, SqlConnectionString);
                                                    if (ds2.Tables[0].Rows.Count > 0)
                                                    {
                                                        foreach (DataRow row2 in ds2.Tables[0].Rows)
                                                        {
                                                            strDisplayName = row2["LastName"].ToString() + ", " + row2["FirstName"].ToString();
                                                            DataSet dsSalesOrder = DBHelper.Query(string.Format("Select * from ZAFiSalesOrder where SD_DOC='{0}' and ITM_NUMBER='{1}'", row["VBELN"].ToString().Trim(), order.GetString("Itm_Number")), SqlConnectionString);
                                                            if (dsSalesOrder.Tables[0].Rows.Count > 0)
                                                            {
                                                                //sql.AppendFormat("if exists(Select * from ZAFiSalesOrder where SD_DOC='{0}')", row["VBELN"].ToString().Trim());
                                                                sql.AppendFormat(" update ZAFiSalesOrder set PMDisplayName='{0}',PMShortName='{1}',LeaderTtitle='PM' where SD_DOC='{2}'", strDisplayName, row["KUNNR"].ToString().Trim(), row["VBELN"].ToString().Trim());
                                                                DBHelper.ExecuteSql(sql.ToString(), SqlConnectionString);
                                                            }
                                                            else
                                                            {
                                                                //sql.AppendFormat("if not exists(Select * from ZAFiSalesOrder where SD_DOC='{0}' and ITM_NUMBER='{1}')", order.GetString("Sd_Doc"), order.GetString("Itm_Number"));
                                                                sql.AppendFormat(" insert into ZAFiSalesOrder(SD_DOC,ITM_NUMBER,Material,SHORT_TEXT,DOC_TYPE,DOC_DATE,PMDisplayName,PMShortName,LeaderTitle) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','PM')", order.GetString("Sd_Doc"), order.GetString("Itm_Number"), order.GetString("Material"), order.GetString("Short_Text").Replace("'", "''"), order.GetString("Doc_Type"), Convert.ToDateTime(order.GetString("Doc_Date")).ToString("yyyyMMdd"), strDisplayName, row["KUNNR"].ToString().Trim());
                                                                DBHelper.ExecuteSql(sql.ToString(), SqlConnectionString);
                                                            }
                                                        }

                                                    } 
                                                }
                                            }

                                        }
                                    }
                                }
                                //sql.AppendFormat("if not exists(Select * from ZAFiSalesOrder where SD_DOC='{0}' and ITM_NUMBER='{1}')", order.GetString("Sd_Doc"), order.GetString("Itm_Number"));
                                //sql.AppendFormat(" insert into ZAFiSalesOrder(SD_DOC,ITM_NUMBER,Material,SHORT_TEXT,DOC_TYPE,DOC_DATE) values('{0}','{1}','{2}','{3}','{4}','{5}')", order.GetString("Sd_Doc"), order.GetString("Itm_Number"), order.GetString("Material"), order.GetString("Short_Text").Replace("'", "''"), order.GetString("Doc_Type"), Convert.ToDateTime(order.GetString("Doc_Date")).ToString("yyyyMMdd"));
                                //DBHelper.ExecuteSql(sql.ToString(), SqlConnectionString);
                            }

                        }
                   }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                prd = null;
            }
            //LogFile.WriteLog("Update Sales Order End...");
             

        }
        #endregion

        public void Test(RfcDestination prd)
        {
            TAB512Table data = new TAB512Table();
            RFC_DB_OPTTable options = new RFC_DB_OPTTable();
            string strWBSNo = "1-003701-310";
            string strSalesOrder = strWBSNo.Substring(0, strWBSNo.LastIndexOf("-"));
            string s = strSalesOrder.Replace("-", "").PadLeft(10, '0');
            // string s2 = "000" + s;
            DataTable ds = Functions.ReadWBSTableGetZPSOrder(prd, "Z20PST_KONF_UP00", s);
            string s3 = "1";

        }

        public void TestForInternalOrder(RfcDestination prd)
        {
            RfcRepository SapRfcRepository = prd.Repository;
            IRfcFunction BapiGetInternalorderDetail = SapRfcRepository.CreateFunction("BAPI_INTERNALORDER_GETDETAIL");
            BapiGetInternalorderDetail.SetValue("Language", string.Empty);
            BapiGetInternalorderDetail.SetValue("Orderid", "CC213000985");
            IRfcStructure detail = BapiGetInternalorderDetail.GetStructure("Master_Data");
            BapiGetInternalorderDetail.Invoke(prd);

            string a = detail.GetString("Comp_Code");

            string b = a;
        }

        public void SOTest(RfcDestination prd)
        {
            string salesOrg = "2130";
            string customer = "0006009905";
            StringBuilder sql = new StringBuilder();
            RfcRepository SapRfcRepository = prd.Repository;

            IRfcFunction BapiSalesORDERGetList = SapRfcRepository.CreateFunction("BAPI_SALESORDER_GETLIST");
            BapiSalesORDERGetList.SetValue("Customer_Number", customer);
            BapiSalesORDERGetList.SetValue("Document_Date", string.Empty);
            BapiSalesORDERGetList.SetValue("Document_Date_To", string.Empty);
            BapiSalesORDERGetList.SetValue("Material", string.Empty);
            BapiSalesORDERGetList.SetValue("Purchase_Order", string.Empty);
            BapiSalesORDERGetList.SetValue("Purchase_Order_Number", string.Empty);
            BapiSalesORDERGetList.SetValue("Sales_Organization", salesOrg);
            BapiSalesORDERGetList.SetValue("Transaction_Group", "0");
            BapiSalesORDERGetList.Invoke(prd);

            IRfcTable order = BapiSalesORDERGetList.GetTable("Sales_Orders");
            for (int i = 0; i < order.Count; i++)
            {
              //  DataTable ds = Functions.ReadSalesOrderTableCreator(prd, "VBAK", order.GetString("Sd_Doc"));
                order.CurrentIndex = i;
                if (order.GetString("Sd_Doc").StartsWith("0000720345")) // 4000193470
                {
                    string a = order.GetString("Sd_Doc");
                   
                   
                    sql.AppendFormat(" insert into ZAFiSalesOrder(SD_DOC,ITM_NUMBER,Material,SHORT_TEXT,DOC_TYPE,DOC_DATE,PMDisplayName,PMShortName,LeaderTitle) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','PM')", order.GetString("Sd_Doc"), order.GetString("Itm_Number"), order.GetString("Material"), order.GetString("Short_Text").Replace("'", "''"), order.GetString("Doc_Type"), Convert.ToDateTime(order.GetString("Doc_Date")).ToString("yyyyMMdd"),"DingYI" , "PM");
                    DBHelper.ExecuteSql(sql.ToString(), SqlConnectionString);
                }
            }
        }


       


        //public void UpdateWBSForVHS(RfcDestination prd)
        //{
        //    string CheckDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd").Replace("-", "");
        //    DataTable ds = Functions.ReadPRPSTable(prd);
        //    //int i = ds.Rows.Count;
        //    foreach (DataRow row in ds.Rows)
        //    {
        //        //string WBS = row["PSPNR"].ToString();
        //        string WBS = row["POSID"].ToString();
        //        string ProjectNo = row["PSPHI"].ToString();
        //        string Description = row["POST1"].ToString();
                
        //        string CreatedOn = row["ERDAT"].ToString();
        //        string ChangedOn = row["AEDAT"].ToString();
        //        if (CreatedOn == CheckDate)
        //        {
        //            string strsql = string.Format(" insert into YAVHSWBSElement(WBSElement,PorjectNo,Description,CreatedOn,ChangedOn,AddDate) values('{0}','{1}','{2}','{3}','{4}','{5}')", WBS, ProjectNo, Description, CreatedOn, ChangedOn, DateTime.Now.ToString("yyyy-MM-dd").Replace("-", ""));
        //            DBHelper.ExecuteSql(strsql, SqlConnectionString);
        //        }
        //        else
        //        {
        //            if (ChangedOn == CheckDate)
        //            {
        //                string strsqlDelete = string.Format(" delete YAVHSWBSElement where WBSElement = '{0}'" ,WBS);
        //                DBHelper.ExecuteSql(strsqlDelete, SqlConnectionString);

        //                string strsql = string.Format(" insert into YAVHSWBSElement(WBSElement,PorjectNo,Description,CreatedOn,ChangedOn,AddDate) values('{0}','{1}','{2}','{3}','{4}','{5}')", WBS, ProjectNo, Description, CreatedOn, ChangedOn, DateTime.Now.ToString("yyyy-MM-dd").Replace("-", ""));
        //                DBHelper.ExecuteSql(strsql, SqlConnectionString);
        //            }
        //        }
               
        //    }

        //}




       


        

        public void UpdateSOForDSC2(RfcDestination prd)
        {
            // string DateofToday2 = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd").Replace("-", "");

            ArrayList list = new ArrayList();
            list.Add("6130");
            list.Add("6131");

            string CompanyCode = string.Empty;

            for (int j = 0; j < list.Count; j++)
            {
                CompanyCode = list[j].ToString();
                DataTable ds = Functions.ReadVBAKTable(prd, CompanyCode);
                foreach (DataRow row in ds.Rows)
                {
                    string strSalesOrderVPAK = row["VBELN"].ToString().Trim();
                    string strType = row["AUART"].ToString().Trim();
                    // string strSOheaderCreateOn = row["ERDAT"].ToString().Trim();
                    // string strSOheaderChangeOn = row["AEDAT"].ToString().Trim();

                    if (strType == "ZSE" || strType == "ZPR" || strType == "ZSKL" || strType == "YCO1")
                    {
                        DataTable dsVBPA = Functions.ReadVBPATable(prd, strSalesOrderVPAK);
                        foreach (DataRow rowVBPA in dsVBPA.Rows)
                        {
                            string strSalesOrder = rowVBPA["VBELN"].ToString(); //SalesOrder
                            string strItem = rowVBPA["POSNR"].ToString(); //Item
                            string strMaterial = rowVBPA["MATNR"].ToString().TrimEnd(); //Material
                            string strDescription = rowVBPA["ARKTX"].ToString().TrimEnd(); //Material Description
                            string strCreatedOn = rowVBPA["ERDAT"].ToString(); //Created On
                            string strChangedOn = rowVBPA["AEDAT"].ToString(); //Changed On
                            string CheckDate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd").Replace("-", "");

                            if (strCreatedOn == CheckDate)
                            {

                                string strsql = string.Format(" insert into YADSCSalesOrder(SD_DOC,ITM_NUMBER,Material,SHORT_TEXT,Created_On, Changed_On , DOC_TYPE ,AddDate) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}',{7})", strSalesOrder, strItem, strMaterial, strDescription, strCreatedOn, strChangedOn, strType, DateTime.Now.ToString("yyyy-MM-dd"));
                                DBHelper.ExecuteSql(strsql, SqlConnectionString);
                            }
                            else
                            {
                                if (strChangedOn == CheckDate)
                                {
                                    string strsqlDeleteSO = string.Format("update YADSCSalesOrder set SD_DOC = '{0}', ITM_NUMBER = '{1}', Material='{2}', SHORT_TEXT = '{3}', Created_On = '{4}', Changed_On = '{5}',DOC_TYPE = '{6}', UpdateDate = '{7}' where SD_DOC = '{7}' and ITM_NUMBER = '{8}'", strSalesOrder, strItem, strMaterial, strDescription, strCreatedOn, strChangedOn, strType, DateTime.Now.ToString("yyyy-MM-dd"), strSalesOrder, strItem);
                                    DBHelper.ExecuteSql(strsqlDeleteSO, SqlConnectionString);
                                    // string strsqlUpdateSO = string.Format("insert into YADSCSalesOrder(SD_DOC,ITM_NUMBER,Material,SHORT_TEXT,Created_On, Changed_On , DOC_TYPE,UpdateDate) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", , DateTime.Now.ToString("yyyy-MM-dd"));
                                    //  DBHelper.ExecuteSql(strsqlUpdateSO, SqlConnectionString);
                                }
                            }
                        }
                    }

                    //if (strSOheaderChangeOn == "DateofToday")
                    //{
                    //    if (strType == "ZSE" || strType == "ZPR" || strType == "ZSKL" || strType == "YCO1")
                    //    {
                    //        //Delete the SO 
                    //        string strsqlDeleteSO = string.Format("Delete YADSCSalesOrder where SD_DOC = '{0}'", strSalesOrderVPAK);
                    //        DBHelper.ExecuteSql(strsqlDeleteSO, SqlConnectionString);

                    //        //Insert the SO
                    //        DataTable dsVBPA = Functions.ReadVBPATable(prd, strSalesOrderVPAK);
                    //        foreach (DataRow rowVBPA in dsVBPA.Rows)
                    //        {
                    //            string strSalesOrder = rowVBPA["VBELN"].ToString(); //SalesOrder
                    //            string strItem = rowVBPA["POSNR"].ToString(); //Item
                    //            string strMaterial = rowVBPA["MATNR"].ToString().TrimEnd(); //Material
                    //            string strDescription = rowVBPA["ARKTX"].ToString().TrimEnd(); //Material Description
                    //            string strCreatedOn = rowVBPA["ERDAT"].ToString(); //Created On
                    //            string strChangedOn = rowVBPA["AEDAT"].ToString(); //Changed On

                    //            string strsql = string.Format(" insert into YADSCSalesOrder(SD_DOC,ITM_NUMBER,Material,SHORT_TEXT,Created_On, Changed_On , DOC_TYPE) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", strSalesOrder, strItem, strMaterial, strDescription, strCreatedOn, strChangedOn, strType);
                    //            // DBHelper.ExecuteSql(strsql, SqlConnectionString);

                    //        }
                    //    }
                    //}

                }

            }
        }


       ////Write log by zhengbang 2017-05-04
       //  public static void WriteLog(string msg)
       //  {
       //      string path = @"C:\Users\lzhengb\Desktop\logTest\log.txt";
       //      // This text is added only once to the file.
       //      if (!File.Exists(path))
       //      {
       //          // Create a file to write to.
       //         // using (StreamWriter writer = File.CreateText(path))
       //         // {
       //              //sw.WriteLine("Hello");
       //              //sw.WriteLine("And");
       //              //sw.WriteLine("Welcome");
       //              StreamWriter writer = null;
       //              try
       //              {
       //                  //writer = File.AppendText(@"a.log");
       //                  writer = File.AppendText(path);
                            
       //                  writer.WriteLine("{0} {1}", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"), msg);
       //                  writer.Flush();
       //              }
       //              catch (Exception ex)
       //              {
       //                  throw ex;
       //              }
       //              finally
       //              {
       //                  if (writer != null)
       //                  {
       //                      writer.Close();
       //                  }
       //              }
       //          //}
       //      }

            

    
       //  }

    }
}
