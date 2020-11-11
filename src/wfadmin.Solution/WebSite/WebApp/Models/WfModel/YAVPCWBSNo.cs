using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using SqlSugar;

namespace WebApp.Models.WfModel
{
  [SugarTable("YAVPCWBSNo")]
  public class YAVPCWBSNo
  {
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int ID { get; set; }
    public string WBSElement { get; set; }
    public string Description { get; set; }
    public int ProjectNo { get; set; }
    public int CompanyID { get; set; }
    public string PMDisplayName { get; set; }
    public string PMShortName { get; set; }
    public string LeaderTitle { get; set; }
    public string CreatedOn { get; set; }
    public string ChangedOn { get; set; }
    public string AddDate { get; set; }
  }
}