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
  [SugarTable("YAFRSFunctionalManger")]
  public partial class YAFRSFunctionalManager
  {
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int ID { get; set; }
    public string Applicant { get; set; }
    public string AppGlobalID { get; set; }
    public string AppShortName { get; set; }
    public string FunctionalManager { get; set; }
    public string FManagerGlobalID { get; set; }
    public string FmanagerShortName { get; set; }





    [SugarColumn(IsIgnore = true)]
    public   TrackingState TrackingState{get;set;}



  }
}