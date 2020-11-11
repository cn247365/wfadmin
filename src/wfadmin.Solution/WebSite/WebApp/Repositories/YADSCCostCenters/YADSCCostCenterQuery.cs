using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity.SqlServer;
using Repository.Pattern.Repositories;
using Repository.Pattern.Ef6;
using System.Web.WebPages;
using WebApp.Models;

namespace WebApp.Repositories
{
/// <summary>
/// File: YADSCCostCenterQuery.cs
/// Purpose: easyui datagrid filter query 
/// Created Date: 2020-10-28 14:43:16
/// Author: neo.zhu
/// Tools: SmartCode MVC5 Scaffolder for Visual Studio 2017
/// Copyright (c) 2012-2018 All Rights Reserved
/// </summary>
   public class YADSCCostCenterQuery:QueryObject<YADSCCostCenter>
   {
		public YADSCCostCenterQuery Withfilter(IEnumerable<filterRule> filters)
        {
           if (filters != null)
           {
               foreach (var rule in filters)
               {
						if (rule.field == "Id" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
						{
							var val = Convert.ToInt32(rule.value);
							switch (rule.op) {
                            case "equal":
                                And(x => x.Id == val);
                                break;
                            case "notequal":
                                And(x => x.Id != val);
                                break;
                            case "less":
                                And(x => x.Id < val);
                                break;
                            case "lessorequal":
                                And(x => x.Id <= val);
                                break;
                            case "greater":
                                And(x => x.Id > val);
                                break;
                            case "greaterorequal" :
                                And(x => x.Id >= val);
                                break;
                            default:
                                And(x => x.Id == val);
                                break;
                        }
						}
						if (rule.field == "CostCenter"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.CostCenter.Contains(rule.value));
						}
						if (rule.field == "CostCenterName"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.CostCenterName.Contains(rule.value));
						}
						if (rule.field == "CCMGlobalID"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.CCMGlobalID.Contains(rule.value));
						}
						if (rule.field == "CCMShortName"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.CCMShortName.Contains(rule.value));
						}
						if (rule.field == "CCMDisplayName"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.CCMDisplayName.Contains(rule.value));
						}
						if (rule.field == "BOMGlobalID"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.BOMGlobalID.Contains(rule.value));
						}
						if (rule.field == "BOMShortName"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.BOMShortName.Contains(rule.value));
						}
						if (rule.field == "BOMDisplayName"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.BOMDisplayName.Contains(rule.value));
						}
						if (rule.field == "OverallResopnsible"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.OverallResopnsible.Contains(rule.value));
						}
						if (rule.field == "BusLineFunHeadDisplayName"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.BusLineFunHeadDisplayName.Contains(rule.value));
						}
						if (rule.field == "BusLineFunHeadGlobalID"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.BusLineFunHeadGlobalID.Contains(rule.value));
						}
						if (rule.field == "CreatedDate" && !string.IsNullOrEmpty(rule.value) )
						{	
							if (rule.op == "between")
                            {
                                var datearray = rule.value.Split(new char[] { '-' });
                                var start = Convert.ToDateTime(datearray[0]);
                                var end = Convert.ToDateTime(datearray[1]);
 
							    And(x => SqlFunctions.DateDiff("d", start, x.CreatedDate) >= 0);
                                And(x => SqlFunctions.DateDiff("d", end, x.CreatedDate) <= 0);
						    }
						}
						if (rule.field == "CreatedBy"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.CreatedBy.Contains(rule.value));
						}
						if (rule.field == "LastModifiedDate" && !string.IsNullOrEmpty(rule.value) )
						{	
							if (rule.op == "between")
                            {
                                var datearray = rule.value.Split(new char[] { '-' });
                                var start = Convert.ToDateTime(datearray[0]);
                                var end = Convert.ToDateTime(datearray[1]);
 
							    And(x => SqlFunctions.DateDiff("d", start, x.LastModifiedDate) >= 0);
                                And(x => SqlFunctions.DateDiff("d", end, x.LastModifiedDate) <= 0);
						    }
						}
						if (rule.field == "LastModifiedBy"  && !string.IsNullOrEmpty(rule.value))
						{
							And(x => x.LastModifiedBy.Contains(rule.value));
						}
						if (rule.field == "TenantId" && !string.IsNullOrEmpty(rule.value) && rule.value.IsInt())
						{
							var val = Convert.ToInt32(rule.value);
							switch (rule.op) {
                            case "equal":
                                And(x => x.TenantId == val);
                                break;
                            case "notequal":
                                And(x => x.TenantId != val);
                                break;
                            case "less":
                                And(x => x.TenantId < val);
                                break;
                            case "lessorequal":
                                And(x => x.TenantId <= val);
                                break;
                            case "greater":
                                And(x => x.TenantId > val);
                                break;
                            case "greaterorequal" :
                                And(x => x.TenantId >= val);
                                break;
                            default:
                                And(x => x.TenantId == val);
                                break;
                        }
						}
     
               }
           }
            return this;
        }
    }
}
