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
  [SugarTable("YAVPCFiTANetWork")]
  public partial class YAVPCFiTANetWork
  {
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int ID { get; set; }
    public string Network { get; set; }
    public string Description { get; set; }
    public string Activity { get; set; }
    public string Description2 { get; set; }
    public string WBS { get; set; }
    public DateTime UpdateTime { get; set; }
    
    [SugarColumn(IsIgnore = true)]
    public   TrackingState TrackingState{get;set;}



  }
}