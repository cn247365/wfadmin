using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using SqlSugar;

namespace WebApp.Models.WfModel
{
  [SugarTable("YAVHSWBSElement")]
  public class YAVHSWBSNo
  {
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int ID { get; set; }
    public string WBSElement { get; set; }
    public string Description { get; set; }
    public string PorjectNo { get; set; }
    public string CreatedOn { get; set; }
    public string ChangedOn { get; set; }
    public string AddDate { get; set; }
  }
}