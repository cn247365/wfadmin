using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using SqlSugar;

namespace WebApp.Models.WfModel
{
  [SugarTable("YAVHSQMOrder")]
  public class YAVHSQMOrder
  {
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int ID { get; set; }
    public string OrderName { get; set; }
    public string Description { get; set; }
    public string ChangedOn { get; set; }
    public string AddDate { get; set; }
    public string CreatedOn { get; set; }



  }
}