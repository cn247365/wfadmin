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
  [SugarTable("YAVHSRoleList")]
  public partial class YAVHSRoleList
  {
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int ID { get; set; }
    public string Role { get; set; }
    public string DisplayName { get; set; }
    public string ShortName { get; set; }
    public string GlobalID { get; set; }
    public string Company { get; set; }





    [SugarColumn(IsIgnore = true)]
    public   TrackingState TrackingState{get;set;}



  }
}