using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SqlSugar;

namespace WebApp.Models.WfModel
{
  [SugarTable("YAVPCVendor")]
  public class YAVPCVendor
  {
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int ID { get; set; }

    public string VendorCode { get; set; }
    public string Name_CN { get; set; }
    public string Name_EN { get; set; }
    public string BankName { get; set; }
    public string BankAccount { get; set; }
    public string PaymentTerms { get; set; }
    public string ContactPerson { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Tel { get; set; }
    public string Tel2 { get; set; }
    public string Address_CN { get; set; }
    public string CompanyCode { get; set; }

  }
}