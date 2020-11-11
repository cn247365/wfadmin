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
  [SugarTable("YAFRSSpecialManagerList")]
  public partial class YAFRSSpecialManagerList
  {
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int ID { get; set; }
    public string DisplayName { get; set; }
    public string GlobalID { get; set; }
    public string ShortName { get; set; }
    public string Company { get; set; }
    public string ManagerDisplayName { get; set; }
    public string ManagerGlobalID { get; set; }
    public string ManagerShortName { get; set; }
    public string AL1DisplayName { get; set; }
    public string AL1GlobalID { get; set; }
    public string AL1ShortName { get; set; }
    public string ControllerDisplayName { get; set; }
    public string ControllerGlobalID { get; set; }
    public string ControllerShortName { get; set; }
    public string CashierDisplayName { get; set; }
    public string CashierGlobalID { get; set; }
    public string CashierShortName { get; set; }
    public string AdminDisplayName { get; set; }
    public string AdminGlobalID { get; set; }
    public string AdminShortName { get; set; }



    [SugarColumn(IsIgnore = true)]
    public   TrackingState TrackingState{get;set;}



  }
}