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
  [SugarTable("YAVOCSFiCACostCenter")]
  public partial class YAVOCSFiCACostCenter
  {
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int ID { get; set; }
    public string CostCenter { get; set; }
    public string CCMGlobalID { get; set; }
    public string CCMShortName { get; set; }
    public string CCMDisplayName { get; set; }
  
    [SugarColumn(IsIgnore = true)]
    public   TrackingState TrackingState{get;set;}



  }
}