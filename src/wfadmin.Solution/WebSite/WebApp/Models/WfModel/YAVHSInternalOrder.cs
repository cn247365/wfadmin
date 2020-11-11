using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using SqlSugar;

namespace WebApp.Models.WfModel
{
  [SugarTable("YAVHSInternalOrder")]
  public class YAVHSInternalOrder
  {
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int ID { get; set; }
    public string Number { get; set; }
    public string Name { get; set; }
    public string CompanyID { get; set; }
    public string CostCenterID { get; set; }
    public string IsShow { get; set; }
    public string AddDate { get; set; }




  }
}