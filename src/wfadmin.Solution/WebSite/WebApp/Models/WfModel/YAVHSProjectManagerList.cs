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
  [SugarTable("YAVHSProjectManagerList")]
  public partial class YAVHSProjectManagerList
  {
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int ID { get; set; }
    public string DDisplayName { get; set; }
    public string DGlobalID { get; set; }
    public string DShortName { get; set; }
    public string BDisplayName { get; set; }
    public string BGlobalID { get; set; }
    public string BShortName { get; set; }
    public string ADisplayName { get; set; }
    public string AGlobalID { get; set; }
    public string AShortName { get; set; }




    [SugarColumn(IsIgnore = true)]
    public   TrackingState TrackingState{get;set;}



  }
}