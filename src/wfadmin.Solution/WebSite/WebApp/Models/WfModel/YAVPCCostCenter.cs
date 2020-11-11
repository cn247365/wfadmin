using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web;
using Repository.Pattern.Ef6;
using SqlSugar;
using TrackableEntities;

namespace WebApp.Models.WfModel
{
  [SugarTable("YAVPCCostCenter")]
  public partial class YAVPCCostCenter
  {
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int ID { get; set; }
    public string Company { get; set; }
    public string CostCenter { get; set; }
    public string Department { get; set; }
    public string CCMGlobalID { get; set; }
    public string CCMDisplayName { get; set; }
    public string CCMShortName { get; set; }
    public string CCM2GlobalID { get; set; }
    public string CCM2DisplayName { get; set; }
    public string CCM2ShortName { get; set; }
    public string CAGlobalID { get; set; }
    public string CADisplayName { get; set; }
    public string CAShortName { get; set; }
    public string CA2GlobalID { get; set; }
    public string CA2DisplayName { get; set; }
    public string CA2ShortName { get; set; }
    public string BOMGlobalID { get; set; }
    public string BOMDisplayName { get; set; }
    public string BOMShortName { get; set; }
    public string IsProjectCC { get; set; }

    [SugarColumn(IsIgnore = true)]
    public TrackingState TrackingState { get; set; }



  }
}