using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using SqlSugar;
using TrackableEntities;

namespace WebApp.Models.WfModel
{
  [SugarTable("YAVPCVendorApprover")]
  public class YAVPCVendorApprover
  {
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int ID { get; set; }
    public string Role { get; set; }
    public string GlobalID { get; set; }
    public string ShortName { get; set; }
    public string DisplayName { get; set; }

    [SugarColumn(IsIgnore = true)]
    public TrackingState TrackingState { get; set; }

  }
}