using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using SqlSugar;

namespace WebApp.Models.WfModel
{
  [SugarTable("YAVPCSalesOrder")]
  public class YAVPCSalesOrder
  {
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int ID { get; set; }
    public string SD_DOC { get; set; }
    public string ITM_NUMBER { get; set; }
    public string Material { get; set; }
    public string SHORT_TEXT { get; set; }
    public string DOC_TYPE { get; set; }
    public string DOC_DATE { get; set; }
    public string PMShortName { get; set; }
    public string PMDisplayName { get; set; }
    public string LeaderTitle { get; set; }
    public string Created_On { get; set; }
    public string Changed_On { get; set; }
    public string AddDate { get; set; }
    public string UpdateDate { get; set; }
    public string Customer { get; set; }
    public string CustomerCode { get; set; }
    public string PMGlobalID { get; set; }
    public string PMType { get; set; }
    public string Creator { get; set; }
    public string OMGlobalID { get; set; }
    public string CCPGlobalID { get; set; }
    public string CreatorGlobalID { get; set; }

  }
}