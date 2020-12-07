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
  [SugarTable("YAVTCNFiCostCenter")]
  public partial class YAVTCNFiCostCenter
  {
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int ID { get; set; }
    public string SiteID { get; set; }
    public string CostCenter { get; set; }
    public string Description { get; set; }
    public int DivisionID { get; set; }
    public string PersonResp { get; set; }
    public string PersonShortName { get; set; }
    public string PersonGlobalID { get; set; }
    public int CCMLevel { get;set;}
    public string SCCM1 { get; set; }
    public string SCCM1ShortName { get; set; }
    public string SCCM1GlobalID { get; set; }
    public string SCCM2 { get; set; }
    public string SCCM2ShortName { get; set; }
    public string SCCM2GlobalID { get; set; }
    [SugarColumn(IsIgnore = true)]
    public   TrackingState TrackingState{get;set;}



  }
}