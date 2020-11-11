using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Repository.Pattern.Ef6;

namespace WebApp.Models
{
   
  public partial class YADSCCostCenter:Entity
  {
    //public int ID { get; set; }
    public string CostCenter { get; set; }
    public string CostCenterName { get; set; }
    public string CCMGlobalID { get; set; }
    public string CCMShortName { get; set; }
    public string CCMDisplayName { get; set; }
    public string BOMGlobalID { get; set; }
    public string BOMShortName { get; set; }
    public string BOMDisplayName { get; set; }
    public string OverallResopnsible { get; set; }
    public string BusLineFunHeadDisplayName { get; set; }
    public string BusLineFunHeadGlobalID { get; set; }


  }
}