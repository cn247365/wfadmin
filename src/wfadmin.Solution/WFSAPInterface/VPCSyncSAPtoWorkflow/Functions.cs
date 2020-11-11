using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPVoithProxy;
using System.Data;
using SAP.Middleware.Connector;

namespace VPCSyncSAPtoWorkflow
{
    class Functions
    {

        public static DataTable ReadTable(RfcDestination prd,string tableName, RFC_DB_OPTTable options)
        {
            DataTable table = new DataTable(tableName);         

            try
            {
                RFC_DB_FLDTable fields = new RFC_DB_FLDTable();
                RFC_DB_FLD fld = new RFC_DB_FLD();
                fld.Fieldname = "BUKRS ";
                fields.Add(fld);
                RFC_DB_FLD fld2 = new RFC_DB_FLD();
                fld2.Fieldname = "LIFNR ";
                fields.Add(fld2);
                RfcRepository SapRfcRepository = prd.Repository;
                IRfcFunction BapiRFCReadTable = SapRfcRepository.CreateFunction("RFC_READ_TABLE");
                string strDelimiter = null; string strNodata = null;
                BapiRFCReadTable.SetValue("Delimiter", strDelimiter);
                BapiRFCReadTable.SetValue("No_Data", strNodata);
                BapiRFCReadTable.SetValue("Query_Table", "LFB1");
                IRfcTable RFC_READ_TABLE = BapiRFCReadTable.GetTable("Fields");
                for (int i = 0; i < fields.Count; i++)
                {
                    RFC_READ_TABLE.Append();
                    RFC_READ_TABLE.SetValue("Fieldname", fields[i].Fieldname);
                    RFC_READ_TABLE.SetValue("Offset", fields[i].Offset);
                    RFC_READ_TABLE.SetValue("Length", fields[i].Length);
                    RFC_READ_TABLE.SetValue("Type", fields[i].Type);
                    RFC_READ_TABLE.SetValue("FieldText", fields[i].Fieldtext);
                }
                BapiRFCReadTable.Invoke(prd);
                IRfcTable dr = BapiRFCReadTable.GetTable("Fields");
                //处理获得数据
                for (int i = 0; i < dr.Count; i++)
                {
                    dr.CurrentIndex = i;
                    string colName = dr.GetString("Fieldname");
                    table.Columns.Add(new DataColumn(colName, typeof(string)));
                }
                IRfcTable data = BapiRFCReadTable.GetTable("Data");
                if (data.Count > 0)
                {
                    //Loop over data rows
                    for (int i = 0; i < data.Count; i++)
                    {
                        data.CurrentIndex = i;
                        DataRow row = table.NewRow();
                        table.Rows.Add(row);

                        //Loop over fields
                        for (int j = 0; j < dr.Count; j++)
                        {
                            dr.CurrentIndex = j;
                            string val = Functions.ParseTableRow(dr, data);
                            row[j] = val;
                        }
                    }
                }
                return table;
            }
            finally
            {
               
            }

        }

        public static string ParseTableRow(IRfcTable field, IRfcTable dataRow)
        {
            // Length of the field
            int len = int.Parse(field.GetString("Length"));
            // Position where the field's value starts in the data row
            int offset = int.Parse(field.GetString("Offset"));

            // The data row, containing all the field values concatenated
            // by SAP's rfc_read_table
            string row = dataRow.GetString("Wa");
            string retValue = "";

            try
            {
                if (offset < row.Length)
                {
                    // Read the field's value starting at the position specified 
                    // by offset.
                    if (offset + len > row.Length)
                        // Read until the end of the row string
                        retValue = dataRow.GetString("Wa").Substring(offset);
                    else
                        // Read only len characters, otherwise.
                        retValue = dataRow.GetString("Wa").Substring(offset, len);
                }
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                Console.WriteLine(e.ToString());
                Environment.Exit(0);
            }
            return retValue;
        }
        public static string ParseVendorCode(string code)
        {
            char[] temp = code.ToCharArray();
            int i = 0;
            for (; i < temp.Length; i++)
            {
                if (temp[i] != '0')
                    break;
            }
            if (i < temp.Length)
                return code.Substring(i, code.Length - i);
            return code;
        }
        public static string FormatVendorCode(object obj)
        {
            if (obj is string)
            {
                string ret = (string)obj;
                if (ret.Length == 6)
                    ret = "0000" + ret;
                if (ret.Length == 7)
                    ret = "000" + ret;
                return ret;
            }
            return null;
        }

        public static DataTable ReadSalesOrderTableCreator(RfcDestination prd, string tableName,string strSalesOrder)
        {
            DataTable table = new DataTable(tableName);
            RFC_DB_FLDTable fields = new RFC_DB_FLDTable();
            RFC_DB_FLD fld = new RFC_DB_FLD();
            fld.Fieldname = "ERNAM";
            fields.Add(fld);
            RFC_DB_FLD fld2 = new RFC_DB_FLD();
            fld2.Fieldname = "VBELN";
            fields.Add(fld2);
            RfcRepository SapRfcRepository = prd.Repository;
            IRfcFunction BapiRFCReadTable = SapRfcRepository.CreateFunction("RFC_READ_TABLE");
            string strDelimiter = null; string strNodata = null;
            BapiRFCReadTable.SetValue("Delimiter", strDelimiter);
            BapiRFCReadTable.SetValue("No_Data", strNodata);
            BapiRFCReadTable.SetValue("Query_Table", tableName);
         
            IRfcTable RFC_READ_TABLE = BapiRFCReadTable.GetTable("Fields");
            for (int i = 0; i < fields.Count; i++)
            {
                RFC_READ_TABLE.Append();
                RFC_READ_TABLE.SetValue("Fieldname", fields[i].Fieldname);
                RFC_READ_TABLE.SetValue("Offset", fields[i].Offset);
                RFC_READ_TABLE.SetValue("Length", fields[i].Length);
                RFC_READ_TABLE.SetValue("Type", fields[i].Type);
                RFC_READ_TABLE.SetValue("FieldText", fields[i].Fieldtext);
            }
            IRfcTable tableInop = BapiRFCReadTable.GetTable("OPTIONS");
            tableInop.Append();//添加一行
            tableInop.SetValue("Text",string.Format(" VBELN = '{0}'",strSalesOrder));

            BapiRFCReadTable.Invoke(prd);

            IRfcTable dr = BapiRFCReadTable.GetTable("Fields");
            //处理获得数据
            for (int i = 0; i < dr.Count; i++)
            {
                dr.CurrentIndex = i;
                string colName = dr.GetString("Fieldname");
                table.Columns.Add(new DataColumn(colName, typeof(string)));
            }

            IRfcTable data = BapiRFCReadTable.GetTable("Data");
            if (data.Count > 0)
            {
                //Loop over data rows
                for (int i = 0; i < data.Count; i++)
                {
                    data.CurrentIndex = i;
                    DataRow row = table.NewRow();
                    table.Rows.Add(row);

                    //Loop over fields
                    for (int j = 0; j < dr.Count; j++)
                    {
                        dr.CurrentIndex = j;
                        string val = Functions.ParseTableRow(dr, data);
                        row[j] = val;
                    }
                }
            }
           
            return table;
        }

        public static DataTable ReadSalesOrderTableGetPM(RfcDestination prd, string tableName, string strSalesOrder)
        {
            DataTable table = new DataTable(tableName);
            RFC_DB_FLDTable fields = new RFC_DB_FLDTable();
            RFC_DB_FLD fld = new RFC_DB_FLD();
            fld.Fieldname = "KUNNR";
            fields.Add(fld);
            RFC_DB_FLD fld2 = new RFC_DB_FLD();
            fld2.Fieldname = "VBELN";
            fields.Add(fld2);
            RFC_DB_FLD fld3 = new RFC_DB_FLD();
            fld3.Fieldname = "PARVW";
            fields.Add(fld3);
            RfcRepository SapRfcRepository = prd.Repository;
            IRfcFunction BapiRFCReadTable = SapRfcRepository.CreateFunction("RFC_READ_TABLE");
            string strDelimiter = null; string strNodata = null;
            BapiRFCReadTable.SetValue("Delimiter", strDelimiter);
            BapiRFCReadTable.SetValue("No_Data", strNodata);
            BapiRFCReadTable.SetValue("Query_Table", tableName);

            IRfcTable RFC_READ_TABLE = BapiRFCReadTable.GetTable("Fields");
            for (int i = 0; i < fields.Count; i++)
            {
                RFC_READ_TABLE.Append();
                RFC_READ_TABLE.SetValue("Fieldname", fields[i].Fieldname);
                RFC_READ_TABLE.SetValue("Offset", fields[i].Offset);
                RFC_READ_TABLE.SetValue("Length", fields[i].Length);
                RFC_READ_TABLE.SetValue("Type", fields[i].Type);
                RFC_READ_TABLE.SetValue("FieldText", fields[i].Fieldtext);
            }
            IRfcTable tableInop = BapiRFCReadTable.GetTable("OPTIONS");
            tableInop.Append();//添加一行
            tableInop.SetValue("Text", string.Format(" VBELN = '{0}'", strSalesOrder));
            

            BapiRFCReadTable.Invoke(prd);

            IRfcTable dr = BapiRFCReadTable.GetTable("Fields");
            //处理获得数据
            for (int i = 0; i < dr.Count; i++)
            {
                dr.CurrentIndex = i;
                string colName = dr.GetString("Fieldname");
                table.Columns.Add(new DataColumn(colName, typeof(string)));
            }

            IRfcTable data = BapiRFCReadTable.GetTable("Data");
            if (data.Count > 0)
            {
                //Loop over data rows
                for (int i = 0; i < data.Count; i++)
                {
                    data.CurrentIndex = i;
                    DataRow row = table.NewRow();
                    table.Rows.Add(row);

                    //Loop over fields
                    for (int j = 0; j < dr.Count; j++)
                    {
                        dr.CurrentIndex = j;
                        string val = Functions.ParseTableRow(dr, data);
                        row[j] = val;
                    }
                }
            }

            return table;
        }

        public static DataTable ReadWBSTableGetZPSOrder(RfcDestination prd, string tableName, string strSalesOrder)
        {
            DataTable table = new DataTable(tableName);
            RFC_DB_FLDTable fields = new RFC_DB_FLDTable();
            RFC_DB_FLD fld = new RFC_DB_FLD();
            fld.Fieldname = "VBELN_MC";  //SO
            fields.Add(fld);
            RFC_DB_FLD fld2 = new RFC_DB_FLD();
            fld2.Fieldname = "VBELN";  //SO
            fields.Add(fld2);
            RfcRepository SapRfcRepository = prd.Repository;
            IRfcFunction BapiRFCReadTable = SapRfcRepository.CreateFunction("RFC_READ_TABLE");
            string strDelimiter = null; string strNodata = null;
            BapiRFCReadTable.SetValue("Delimiter", strDelimiter);
            BapiRFCReadTable.SetValue("No_Data", strNodata);
            BapiRFCReadTable.SetValue("Query_Table", tableName);

            IRfcTable RFC_READ_TABLE = BapiRFCReadTable.GetTable("Fields");
            for (int i = 0; i < fields.Count; i++)
            {
                RFC_READ_TABLE.Append();
                RFC_READ_TABLE.SetValue("Fieldname", fields[i].Fieldname);
                RFC_READ_TABLE.SetValue("Offset", fields[i].Offset);
                RFC_READ_TABLE.SetValue("Length", fields[i].Length);
                RFC_READ_TABLE.SetValue("Type", fields[i].Type);
                RFC_READ_TABLE.SetValue("FieldText", fields[i].Fieldtext);
            }
            IRfcTable tableInop = BapiRFCReadTable.GetTable("OPTIONS");
            tableInop.Append();//添加一行
            tableInop.SetValue("Text", string.Format(" VBELN = '{0}' ", strSalesOrder));
            //tableInop.Append();//添加一行
            //tableInop.SetValue("Text", " VBELN_MC is not null");


            BapiRFCReadTable.Invoke(prd);

            IRfcTable dr = BapiRFCReadTable.GetTable("Fields");
            //处理获得数据
            for (int i = 0; i < dr.Count; i++)
            {
                dr.CurrentIndex = i;
                string colName = dr.GetString("Fieldname");
                table.Columns.Add(new DataColumn(colName, typeof(string)));
            }

            IRfcTable data = BapiRFCReadTable.GetTable("Data");
            if (data.Count > 0)
            {
                //Loop over data rows
                for (int i = 0; i < data.Count; i++)
                {
                    data.CurrentIndex = i;
                    DataRow row = table.NewRow();
                    table.Rows.Add(row);

                    //Loop over fields
                    for (int j = 0; j < dr.Count; j++)
                    {
                        dr.CurrentIndex = j;
                        string val = Functions.ParseTableRow(dr, data);
                        row[j] = val;
                    }
                }
            }

            return table;
        }

        public static DataTable ReadTableSalesOrderCreatorByZMC(RfcDestination prd, string tableName, string strSalesOrder, string strCompanyCode)
        {
            DataTable table = new DataTable(tableName);
            RFC_DB_FLDTable fields = new RFC_DB_FLDTable();
            RFC_DB_FLD fld = new RFC_DB_FLD();
            fld.Fieldname = "ERNAM";
            fields.Add(fld);
            RFC_DB_FLD fld2 = new RFC_DB_FLD();
            fld2.Fieldname = "VBELN";
            fields.Add(fld2);
            RfcRepository SapRfcRepository = prd.Repository;
            IRfcFunction BapiRFCReadTable = SapRfcRepository.CreateFunction("RFC_READ_TABLE");
            string strDelimiter = null; string strNodata = null;
            BapiRFCReadTable.SetValue("Delimiter", strDelimiter);
            BapiRFCReadTable.SetValue("No_Data", strNodata);
            BapiRFCReadTable.SetValue("Query_Table", tableName);

            IRfcTable RFC_READ_TABLE = BapiRFCReadTable.GetTable("Fields");
            for (int i = 0; i < fields.Count; i++)
            {
                RFC_READ_TABLE.Append();
                RFC_READ_TABLE.SetValue("Fieldname", fields[i].Fieldname);
                RFC_READ_TABLE.SetValue("Offset", fields[i].Offset);
                RFC_READ_TABLE.SetValue("Length", fields[i].Length);
                RFC_READ_TABLE.SetValue("Type", fields[i].Type);
                RFC_READ_TABLE.SetValue("FieldText", fields[i].Fieldtext);
            }
            IRfcTable tableInop = BapiRFCReadTable.GetTable("OPTIONS");
            tableInop.Append();//添加一行
            tableInop.SetValue("Text", string.Format(" VBELN = '{0}' and VKORG = '{1}'", strSalesOrder, strCompanyCode));

            BapiRFCReadTable.Invoke(prd);

            IRfcTable dr = BapiRFCReadTable.GetTable("Fields");
            //处理获得数据
            for (int i = 0; i < dr.Count; i++)
            {
                dr.CurrentIndex = i;
                string colName = dr.GetString("Fieldname");
                table.Columns.Add(new DataColumn(colName, typeof(string)));
            }

            IRfcTable data = BapiRFCReadTable.GetTable("Data");
            if (data.Count > 0)
            {
                //Loop over data rows
                for (int i = 0; i < data.Count; i++)
                {
                    data.CurrentIndex = i;
                    DataRow row = table.NewRow();
                    table.Rows.Add(row);

                    //Loop over fields
                    for (int j = 0; j < dr.Count; j++)
                    {
                        dr.CurrentIndex = j;
                        string val = Functions.ParseTableRow(dr, data);
                        row[j] = val;
                    }
                }
            }

            return table;
        }

        public static DataTable ReadVBAKTable(RfcDestination prd , string CompanyCode)
        {       
            try
            {
                DataTable table = new DataTable("VBAK");
                RFC_DB_FLDTable fields = new RFC_DB_FLDTable();
                RFC_DB_FLD fld = new RFC_DB_FLD();
                fld.Fieldname = "ERNAM";
                fields.Add(fld);
                RFC_DB_FLD fld2 = new RFC_DB_FLD();
                fld2.Fieldname = "VBELN";
                fields.Add(fld2);  
                RFC_DB_FLD fld3 = new RFC_DB_FLD();
                fld3.Fieldname = "AUART";
                fields.Add(fld3);
                RFC_DB_FLD fld4 = new RFC_DB_FLD();
                fld4.Fieldname = "ERDAT";
                fields.Add(fld4);
                RFC_DB_FLD fld5 = new RFC_DB_FLD();
                fld5.Fieldname = "AEDAT";
                fields.Add(fld5);   
                RfcRepository SapRfcRepository = prd.Repository;
                IRfcFunction BapiRFCReadTable = SapRfcRepository.CreateFunction("RFC_READ_TABLE");
                string strDelimiter = null; string strNodata = null;
                BapiRFCReadTable.SetValue("Delimiter", strDelimiter);
                BapiRFCReadTable.SetValue("No_Data", strNodata);
                BapiRFCReadTable.SetValue("Query_Table", "VBAK");

                IRfcTable RFC_READ_TABLE = BapiRFCReadTable.GetTable("Fields");
                for (int i = 0; i < fields.Count; i++)
                {
                    RFC_READ_TABLE.Append();
                    RFC_READ_TABLE.SetValue("Fieldname", fields[i].Fieldname);
                    RFC_READ_TABLE.SetValue("Offset", fields[i].Offset);
                    RFC_READ_TABLE.SetValue("Length", fields[i].Length);
                    RFC_READ_TABLE.SetValue("Type", fields[i].Type);
                    RFC_READ_TABLE.SetValue("FieldText", fields[i].Fieldtext);
                }
                IRfcTable tableInop = BapiRFCReadTable.GetTable("OPTIONS");
                tableInop.Append();//添加一行
                tableInop.SetValue("Text", string.Format("VKORG = '{0}'", CompanyCode));

                BapiRFCReadTable.Invoke(prd);

                IRfcTable dr = BapiRFCReadTable.GetTable("Fields");
                //处理获得数据
                for (int i = 0; i < dr.Count; i++)
                {
                    dr.CurrentIndex = i;
                    string colName = dr.GetString("Fieldname");
                    table.Columns.Add(new DataColumn(colName, typeof(string)));
                }

                IRfcTable data = BapiRFCReadTable.GetTable("Data");
                if (data.Count > 0)
                {
                    //Loop over data rows
                    for (int i = 0; i < data.Count; i++)
                    {
                        data.CurrentIndex = i;
                        DataRow row = table.NewRow();
                        table.Rows.Add(row);

                        //Loop over fields
                        for (int j = 0; j < dr.Count; j++)
                        {
                            dr.CurrentIndex = j;
                            string val = Functions.ParseTableRow(dr, data);
                            row[j] = val;
                        }
                    }
                }

                return table;
                //RfcRepository SapRfcRepository = prd.Repository;
                //IRfcFunction RFCReadTable = SapRfcRepository.CreateFunction("RFC_READ_TABLE");
                //RFCReadTable.SetValue("Delimiter", string.Empty);
                //RFCReadTable.SetValue("Query_Table", "VBAK");
                //IRfcTable options = RFCReadTable.GetTable("OPTIONS");
                //options.Append();
                //options.CurrentRow.SetValue("TEXT", "VKORG='2130'");

                //IRfcTable fields = RFCReadTable.GetTable("Fields");
                //fields.Append();
                //fields.SetValue("Fieldname", "VBELN");
                //RFCReadTable.Invoke(prd);

                //IRfcTable data = BapiRFCReadTable.GetTable("DATA");
                //DataTable dTable = ToDataTable(data);
                //return dTable;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }


        public static DataTable ReadVBAPTable(RfcDestination prd, string strSalesOrder)
        {
            try
            {
                DataTable table = new DataTable("VBAP");
                RFC_DB_FLDTable fields = new RFC_DB_FLDTable();
                RFC_DB_FLD fld = new RFC_DB_FLD();
                fld.Fieldname = "POSNR";
                fields.Add(fld);

                RFC_DB_FLD fld2 = new RFC_DB_FLD();
                fld2.Fieldname = "VBELN";
                fields.Add(fld2);

                RFC_DB_FLD fld3 = new RFC_DB_FLD();
                fld3.Fieldname = "MATNR";
                fields.Add(fld3);

                RFC_DB_FLD fld4 = new RFC_DB_FLD();
                fld4.Fieldname = "ARKTX";
                fields.Add(fld4);

                RFC_DB_FLD fld5 = new RFC_DB_FLD();
                fld5.Fieldname = "ERDAT";
                fields.Add(fld5);

                RFC_DB_FLD fld6 = new RFC_DB_FLD();
                fld6.Fieldname = "AEDAT";
                fields.Add(fld6);

                //RFC_DB_FLD fld7 = new RFC_DB_FLD();
                //fld6.Fieldname = "KUNNR";
                //fields.Add(fld7);

                RfcRepository SapRfcRepository = prd.Repository;
                IRfcFunction BapiRFCReadTable = SapRfcRepository.CreateFunction("RFC_READ_TABLE");
                string strDelimiter = null; string strNodata = null;
                BapiRFCReadTable.SetValue("Delimiter", strDelimiter);
                BapiRFCReadTable.SetValue("No_Data", strNodata);
                BapiRFCReadTable.SetValue("Query_Table", "VBAP");

                IRfcTable RFC_READ_TABLE = BapiRFCReadTable.GetTable("Fields");
                for (int i = 0; i < fields.Count; i++)
                {
                    RFC_READ_TABLE.Append();
                    RFC_READ_TABLE.SetValue("Fieldname", fields[i].Fieldname);
                    RFC_READ_TABLE.SetValue("Offset", fields[i].Offset);
                    RFC_READ_TABLE.SetValue("Length", fields[i].Length);
                    RFC_READ_TABLE.SetValue("Type", fields[i].Type);
                    RFC_READ_TABLE.SetValue("FieldText", fields[i].Fieldtext);
                }
                IRfcTable tableInop = BapiRFCReadTable.GetTable("OPTIONS");
                tableInop.Append();//添加一行
                tableInop.SetValue("Text", string.Format(" VBELN = '{0}'", strSalesOrder));


                BapiRFCReadTable.Invoke(prd);

                IRfcTable dr = BapiRFCReadTable.GetTable("Fields");
                //处理获得数据
                for (int i = 0; i < dr.Count; i++)
                {
                    dr.CurrentIndex = i;
                    string colName = dr.GetString("Fieldname");
                    table.Columns.Add(new DataColumn(colName, typeof(string)));
                }

                IRfcTable data = BapiRFCReadTable.GetTable("Data");
                if (data.Count > 0)
                {
                    //Loop over data rows
                    for (int i = 0; i < data.Count; i++)
                    {
                        data.CurrentIndex = i;
                        DataRow row = table.NewRow();
                        table.Rows.Add(row);

                        //Loop over fields
                        for (int j = 0; j < dr.Count; j++)
                        {
                            dr.CurrentIndex = j;
                            string val = Functions.ParseTableRow(dr, data);
                            row[j] = val;
                        }
                    }
                }

                return table;
               
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable ReadVBPATable(RfcDestination prd, string strSalesOrder)
        {
            try
            {
                DataTable table = new DataTable("VBPA");
                RFC_DB_FLDTable fields = new RFC_DB_FLDTable();
                RFC_DB_FLD fld = new RFC_DB_FLD();
                fld.Fieldname = "POSNR";
                fields.Add(fld);

                RFC_DB_FLD fld2 = new RFC_DB_FLD();
                fld2.Fieldname = "VBELN";
                fields.Add(fld2);

                RFC_DB_FLD fld3 = new RFC_DB_FLD();
                fld3.Fieldname = "KUNNR";
                fields.Add(fld3);

                //RFC_DB_FLD fld4 = new RFC_DB_FLD();
                //fld4.Fieldname = "ARKTX";
                //fields.Add(fld4);

                //RFC_DB_FLD fld5 = new RFC_DB_FLD();
                //fld5.Fieldname = "ERDAT";
                //fields.Add(fld5);

                //RFC_DB_FLD fld6 = new RFC_DB_FLD();
                //fld6.Fieldname = "AEDAT";
                //fields.Add(fld6);

                //RFC_DB_FLD fld7 = new RFC_DB_FLD();
                //fld6.Fieldname = "KUNNR";
                //fields.Add(fld7);

                RfcRepository SapRfcRepository = prd.Repository;
                IRfcFunction BapiRFCReadTable = SapRfcRepository.CreateFunction("RFC_READ_TABLE");
                string strDelimiter = null; string strNodata = null;
                BapiRFCReadTable.SetValue("Delimiter", strDelimiter);
                BapiRFCReadTable.SetValue("No_Data", strNodata);
                BapiRFCReadTable.SetValue("Query_Table", "VBPA");

                IRfcTable RFC_READ_TABLE = BapiRFCReadTable.GetTable("Fields");
                for (int i = 0; i < fields.Count; i++)
                {
                    RFC_READ_TABLE.Append();
                    RFC_READ_TABLE.SetValue("Fieldname", fields[i].Fieldname);
                    RFC_READ_TABLE.SetValue("Offset", fields[i].Offset);
                    RFC_READ_TABLE.SetValue("Length", fields[i].Length);
                    RFC_READ_TABLE.SetValue("Type", fields[i].Type);
                    RFC_READ_TABLE.SetValue("FieldText", fields[i].Fieldtext);
                }
                IRfcTable tableInop = BapiRFCReadTable.GetTable("OPTIONS");
                tableInop.Append();//添加一行
                tableInop.SetValue("Text", string.Format(" VBELN = '{0}'", strSalesOrder));


                BapiRFCReadTable.Invoke(prd);

                IRfcTable dr = BapiRFCReadTable.GetTable("Fields");
                //处理获得数据
                for (int i = 0; i < dr.Count; i++)
                {
                    dr.CurrentIndex = i;
                    string colName = dr.GetString("Fieldname");
                    table.Columns.Add(new DataColumn(colName, typeof(string)));
                }

                IRfcTable data = BapiRFCReadTable.GetTable("Data");
                if (data.Count > 0)
                {
                    //Loop over data rows
                    for (int i = 0; i < data.Count; i++)
                    {
                        data.CurrentIndex = i;
                        DataRow row = table.NewRow();
                        table.Rows.Add(row);

                        //Loop over fields
                        for (int j = 0; j < dr.Count; j++)
                        {
                            dr.CurrentIndex = j;
                            string val = Functions.ParseTableRow(dr, data);
                            row[j] = val;
                        }
                    }
                }

                return table;

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable ReadVBUKTable(RfcDestination prd, string strSalesOrder)
        {
            try
            {
                DataTable table = new DataTable("VBUK");
                RFC_DB_FLDTable fields = new RFC_DB_FLDTable();
                RFC_DB_FLD fld = new RFC_DB_FLD();
                fld.Fieldname = "BESTK";
                fields.Add(fld);

                RFC_DB_FLD fld2 = new RFC_DB_FLD();
                fld2.Fieldname = "VBELN";
                fields.Add(fld2);

                RFC_DB_FLD fld3 = new RFC_DB_FLD();
                fld3.Fieldname = "RFSTK";
                fields.Add(fld3);


                RfcRepository SapRfcRepository = prd.Repository;
                IRfcFunction BapiRFCReadTable = SapRfcRepository.CreateFunction("RFC_READ_TABLE");
                string strDelimiter = null; string strNodata = null;
                BapiRFCReadTable.SetValue("Delimiter", strDelimiter);
                BapiRFCReadTable.SetValue("No_Data", strNodata);
                BapiRFCReadTable.SetValue("Query_Table", "VBUK");

                IRfcTable RFC_READ_TABLE = BapiRFCReadTable.GetTable("Fields");
                for (int i = 0; i < fields.Count; i++)
                {
                    RFC_READ_TABLE.Append();
                    RFC_READ_TABLE.SetValue("Fieldname", fields[i].Fieldname);
                    RFC_READ_TABLE.SetValue("Offset", fields[i].Offset);
                    RFC_READ_TABLE.SetValue("Length", fields[i].Length);
                    RFC_READ_TABLE.SetValue("Type", fields[i].Type);
                    RFC_READ_TABLE.SetValue("FieldText", fields[i].Fieldtext);
                }
                IRfcTable tableInop = BapiRFCReadTable.GetTable("OPTIONS");
                tableInop.Append();//添加一行
                tableInop.SetValue("Text", string.Format(" VBELN = '{0}'", strSalesOrder));


                BapiRFCReadTable.Invoke(prd);

                IRfcTable dr = BapiRFCReadTable.GetTable("Fields");
                //处理获得数据
                for (int i = 0; i < dr.Count; i++)
                {
                    dr.CurrentIndex = i;
                    string colName = dr.GetString("Fieldname");
                    table.Columns.Add(new DataColumn(colName, typeof(string)));
                }

                IRfcTable data = BapiRFCReadTable.GetTable("Data");
                if (data.Count > 0)
                {
                    //Loop over data rows
                    for (int i = 0; i < data.Count; i++)
                    {
                        data.CurrentIndex = i;
                        DataRow row = table.NewRow();
                        table.Rows.Add(row);

                        //Loop over fields
                        for (int j = 0; j < dr.Count; j++)
                        {
                            dr.CurrentIndex = j;
                            string val = Functions.ParseTableRow(dr, data);
                            row[j] = val;
                        }
                    }
                }

                return table;

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }


        public static DataTable ReadPRPSTable(RfcDestination prd, string companycode)
        {
           // string companycode = "2130";
            string ControllingArea = "2000";
            try
            {
                DataTable table = new DataTable("PRPS");
                RFC_DB_FLDTable fields = new RFC_DB_FLDTable();
                RFC_DB_FLD fld = new RFC_DB_FLD();
                fld.Fieldname = "PSPNR";
                fields.Add(fld);

                RFC_DB_FLD fld2 = new RFC_DB_FLD();
                fld2.Fieldname = "POSID_EDIT";
                fields.Add(fld2);

                RFC_DB_FLD fld3 = new RFC_DB_FLD();
                fld3.Fieldname = "POST1";
                fields.Add(fld3);

                RFC_DB_FLD fld4 = new RFC_DB_FLD();
                fld4.Fieldname = "PSPHI";
                fields.Add(fld4);
                RFC_DB_FLD fld5 = new RFC_DB_FLD();
                fld5.Fieldname = "ERDAT";
                fields.Add(fld5);
                RFC_DB_FLD fld6 = new RFC_DB_FLD();
                fld6.Fieldname = "AEDAT";
                fields.Add(fld6);
                RfcRepository SapRfcRepository = prd.Repository;
                IRfcFunction BapiRFCReadTable = SapRfcRepository.CreateFunction("RFC_READ_TABLE");
                string strDelimiter = null; string strNodata = null;
                BapiRFCReadTable.SetValue("Delimiter", strDelimiter);
                BapiRFCReadTable.SetValue("No_Data", strNodata);
                BapiRFCReadTable.SetValue("Query_Table", "PRPS");

                IRfcTable RFC_READ_TABLE = BapiRFCReadTable.GetTable("Fields");
                for (int i = 0; i < fields.Count; i++)
                {
                    RFC_READ_TABLE.Append();
                    RFC_READ_TABLE.SetValue("Fieldname", fields[i].Fieldname);
                    RFC_READ_TABLE.SetValue("Offset", fields[i].Offset);
                    RFC_READ_TABLE.SetValue("Length", fields[i].Length);
                    RFC_READ_TABLE.SetValue("Type", fields[i].Type);
                    RFC_READ_TABLE.SetValue("FieldText", fields[i].Fieldtext);
                }
                IRfcTable tableInop = BapiRFCReadTable.GetTable("OPTIONS");
                tableInop.Append();//添加一行
                tableInop.SetValue("Text", string.Format(" PBUKR = '{0}' and PKOKR = '{1}'", companycode,ControllingArea));


                BapiRFCReadTable.Invoke(prd);

                IRfcTable dr = BapiRFCReadTable.GetTable("Fields");
                //处理获得数据
                for (int i = 0; i < dr.Count; i++)
                {
                    dr.CurrentIndex = i;
                    string colName = dr.GetString("Fieldname");
                    table.Columns.Add(new DataColumn(colName, typeof(string)));
                }

                IRfcTable data = BapiRFCReadTable.GetTable("Data");
                if (data.Count > 0)
                {
                    //Loop over data rows
                    for (int i = 0; i < data.Count; i++)
                    {
                        data.CurrentIndex = i;
                        DataRow row = table.NewRow();
                        table.Rows.Add(row);

                        //Loop over fields
                        for (int j = 0; j < dr.Count; j++)
                        {
                            dr.CurrentIndex = j;
                            string val = Functions.ParseTableRow(dr, data);
                            row[j] = val;
                        }
                    }
                }

                return table;

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable ReadAUFKTable(RfcDestination prd, string companycode)
        {
           // string companycode = "6130";
            string ControllingArea = "2000";
            try
            {
                DataTable table = new DataTable("AUFK");
                RFC_DB_FLDTable fields = new RFC_DB_FLDTable();
                RFC_DB_FLD fld2 = new RFC_DB_FLD();
                fld2.Fieldname = "AUFNR"; //Order Number
                fields.Add(fld2);

                RFC_DB_FLD fld3 = new RFC_DB_FLD();
                fld3.Fieldname = "KTEXT"; //Description
                fields.Add(fld3);

                RFC_DB_FLD fld4 = new RFC_DB_FLD();
                fld4.Fieldname = "KOSTV"; //Responsible cost center
                fields.Add(fld4);
                RFC_DB_FLD fld5 = new RFC_DB_FLD();
                fld5.Fieldname = "ERDAT";
                fields.Add(fld5);
                RFC_DB_FLD fld6 = new RFC_DB_FLD();
                fld6.Fieldname = "AEDAT";
                fields.Add(fld6);
                RfcRepository SapRfcRepository = prd.Repository;
                IRfcFunction BapiRFCReadTable = SapRfcRepository.CreateFunction("RFC_READ_TABLE");
                string strDelimiter = null; string strNodata = null;
                BapiRFCReadTable.SetValue("Delimiter", strDelimiter);
                BapiRFCReadTable.SetValue("No_Data", strNodata);
                BapiRFCReadTable.SetValue("Query_Table", "AUFK");

                IRfcTable RFC_READ_TABLE = BapiRFCReadTable.GetTable("Fields");
                for (int i = 0; i < fields.Count; i++)
                {
                    RFC_READ_TABLE.Append();
                    RFC_READ_TABLE.SetValue("Fieldname", fields[i].Fieldname);
                    RFC_READ_TABLE.SetValue("Offset", fields[i].Offset);
                    RFC_READ_TABLE.SetValue("Length", fields[i].Length);
                    RFC_READ_TABLE.SetValue("Type", fields[i].Type);
                    RFC_READ_TABLE.SetValue("FieldText", fields[i].Fieldtext);
                }
                IRfcTable tableInop = BapiRFCReadTable.GetTable("OPTIONS");
                tableInop.Append();//添加一行
                tableInop.SetValue("Text", string.Format(" BUKRS = '{0}' and KOKRS = '{1}'", companycode, ControllingArea));


                BapiRFCReadTable.Invoke(prd);

                IRfcTable dr = BapiRFCReadTable.GetTable("Fields");
                //处理获得数据
                for (int i = 0; i < dr.Count; i++)
                {
                    dr.CurrentIndex = i;
                    string colName = dr.GetString("Fieldname");
                    table.Columns.Add(new DataColumn(colName, typeof(string)));
                }

                IRfcTable data = BapiRFCReadTable.GetTable("Data");
                if (data.Count > 0)
                {
                    //Loop over data rows
                    for (int i = 0; i < data.Count; i++)
                    {
                        data.CurrentIndex = i;
                        DataRow row = table.NewRow();
                        table.Rows.Add(row);

                        //Loop over fields
                        for (int j = 0; j < dr.Count; j++)
                        {
                            dr.CurrentIndex = j;
                            string val = Functions.ParseTableRow(dr, data);
                            row[j] = val;
                        }
                    }
                }

                return table;

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }


        public static DataTable ReadKNA1Table(RfcDestination prd, string CustomerCode)
        {
            try
            {
                DataTable table = new DataTable("KNA1");
                RFC_DB_FLDTable fields = new RFC_DB_FLDTable();
                RFC_DB_FLD fld = new RFC_DB_FLD();
                fld.Fieldname = "NAME1";
                fields.Add(fld);
                RFC_DB_FLD fld2 = new RFC_DB_FLD();
                fld2.Fieldname = "NAME2";
                fields.Add(fld2);
                RFC_DB_FLD fld3 = new RFC_DB_FLD();
                fld3.Fieldname = "KUNNR";
                fields.Add(fld3);
                RfcRepository SapRfcRepository = prd.Repository;
                IRfcFunction BapiRFCReadTable = SapRfcRepository.CreateFunction("RFC_READ_TABLE");
                string strDelimiter = null; string strNodata = null;
                BapiRFCReadTable.SetValue("Delimiter", strDelimiter);
                BapiRFCReadTable.SetValue("No_Data", strNodata);
                BapiRFCReadTable.SetValue("Query_Table", "KNA1");

                IRfcTable RFC_READ_TABLE = BapiRFCReadTable.GetTable("Fields");
                for (int i = 0; i < fields.Count; i++)
                {
                    RFC_READ_TABLE.Append();
                    RFC_READ_TABLE.SetValue("Fieldname", fields[i].Fieldname);
                    RFC_READ_TABLE.SetValue("Offset", fields[i].Offset);
                    RFC_READ_TABLE.SetValue("Length", fields[i].Length);
                    RFC_READ_TABLE.SetValue("Type", fields[i].Type);
                    RFC_READ_TABLE.SetValue("FieldText", fields[i].Fieldtext);
                }
                IRfcTable tableInop = BapiRFCReadTable.GetTable("OPTIONS");
                tableInop.Append();//添加一行
                tableInop.SetValue("Text", string.Format("KUNNR = '{0}'", CustomerCode));
           
                BapiRFCReadTable.Invoke(prd);

                IRfcTable dr = BapiRFCReadTable.GetTable("Fields");
                //处理获得数据
                for (int i = 0; i < dr.Count; i++)
                {
                    dr.CurrentIndex = i;
                    string colName = dr.GetString("Fieldname");
                    table.Columns.Add(new DataColumn(colName, typeof(string)));
                }

                IRfcTable data = BapiRFCReadTable.GetTable("Data");
                if (data.Count > 0)
                {
                    //Loop over data rows
                    for (int i = 0; i < data.Count; i++)
                    {
                        data.CurrentIndex = i;
                        DataRow row = table.NewRow();
                        table.Rows.Add(row);

                        //Loop over fields
                        for (int j = 0; j < dr.Count; j++)
                        {
                            dr.CurrentIndex = j;
                            string val = Functions.ParseTableRow(dr, data);
                            row[j] = val;
                        }
                    }
                }

                return table;
                //RfcRepository SapRfcRepository = prd.Repository;
                //IRfcFunction RFCReadTable = SapRfcRepository.CreateFunction("RFC_READ_TABLE");
                //RFCReadTable.SetValue("Delimiter", string.Empty);
                //RFCReadTable.SetValue("Query_Table", "VBAK");
                //IRfcTable options = RFCReadTable.GetTable("OPTIONS");
                //options.Append();
                //options.CurrentRow.SetValue("TEXT", "VKORG='2130'");

                //IRfcTable fields = RFCReadTable.GetTable("Fields");
                //fields.Append();
                //fields.SetValue("Fieldname", "VBELN");
                //RFCReadTable.Invoke(prd);

                //IRfcTable data = BapiRFCReadTable.GetTable("DATA");
                //DataTable dTable = ToDataTable(data);
                //return dTable;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }


        public static DataTable ReadVBPA(RfcDestination prd, string SO)
        {
            // string companycode = "2130";
            try
            {
                DataTable table = new DataTable("VBPA");
                RFC_DB_FLDTable fields = new RFC_DB_FLDTable();
                RFC_DB_FLD fld = new RFC_DB_FLD();
                fld.Fieldname = "PERNR";
                fields.Add(fld);
                RFC_DB_FLD fld1 = new RFC_DB_FLD();
                fld1.Fieldname = "PARVW";
                fields.Add(fld1);

                RfcRepository SapRfcRepository = prd.Repository;
                IRfcFunction BapiRFCReadTable = SapRfcRepository.CreateFunction("RFC_READ_TABLE");
                string strDelimiter = null; string strNodata = null;
                BapiRFCReadTable.SetValue("Delimiter", strDelimiter);
                BapiRFCReadTable.SetValue("No_Data", strNodata);
                BapiRFCReadTable.SetValue("Query_Table", "VBPA");

                IRfcTable RFC_READ_TABLE = BapiRFCReadTable.GetTable("Fields");
                for (int i = 0; i < fields.Count; i++)
                {
                    RFC_READ_TABLE.Append();
                    RFC_READ_TABLE.SetValue("Fieldname", fields[i].Fieldname);
                    RFC_READ_TABLE.SetValue("Offset", fields[i].Offset);
                    RFC_READ_TABLE.SetValue("Length", fields[i].Length);
                    RFC_READ_TABLE.SetValue("Type", fields[i].Type);
                    RFC_READ_TABLE.SetValue("FieldText", fields[i].Fieldtext);
                }
                IRfcTable tableInop = BapiRFCReadTable.GetTable("OPTIONS");
                tableInop.Append();//添加一行
                tableInop.SetValue("Text", string.Format(" VBELN = '{0}'", SO));

                BapiRFCReadTable.Invoke(prd);

                IRfcTable dr = BapiRFCReadTable.GetTable("Fields");
                //处理获得数据
                for (int i = 0; i < dr.Count; i++)
                {
                    dr.CurrentIndex = i;
                    string colName = dr.GetString("Fieldname");
                    table.Columns.Add(new DataColumn(colName, typeof(string)));
                }

                IRfcTable data = BapiRFCReadTable.GetTable("Data");
                if (data.Count > 0)
                {
                    //Loop over data rows
                    for (int i = 0; i < data.Count; i++)
                    {
                        data.CurrentIndex = i;
                        DataRow row = table.NewRow();
                        table.Rows.Add(row);

                        //Loop over fields
                        for (int j = 0; j < dr.Count; j++)
                        {
                            dr.CurrentIndex = j;
                            string val = Functions.ParseTableRow(dr, data);
                            row[j] = val;
                        }
                    }
                }

                return table;

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}
