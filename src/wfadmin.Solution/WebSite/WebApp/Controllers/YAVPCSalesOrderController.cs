﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VPCSyncSAPtoWorkflow;
using WebApp.Models.WfModel;

namespace WebApp.Controllers
{
  public class YAVPCSalesOrderController : Controller
  {
    private readonly NLog.ILogger logger;
    private readonly SqlSugar.ISqlSugarClient db;
    public YAVPCSalesOrderController(NLog.ILogger logger, SqlSugar.ISqlSugarClient db)
    {
      this.logger = logger;
      this.db = db;
      this.db.Aop.OnLogExecuting = (sql, pars) =>
      {
        var includearray = new string[] { "INSERT", "UPDATE", "DELETE" };
        Console.WriteLine(sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
        if (includearray.Any(x => sql.Contains(x)))
        {
          this.logger.Info(sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));

        }
      };
    }
    // GET: YAVPCSalesOrder
    public ActionResult Index()
    {

      return View();
    }
    [HttpGet]
    //[OutputCache(Duration = 10, VaryByParam = "*")]
    public async Task<JsonResult> GetData(int page = 1, int rows = 10, string sort = "ID", string order = "desc", string filterRules = "")
    {
      var filters = PredicateBuilder.From<YAVPCSalesOrder>(filterRules);
      var count =await this.db.Queryable<YAVPCSalesOrder>().Where(filters)
         .CountAsync();
      var result=await this.db.Queryable<YAVPCSalesOrder>().Where(filters)
        .OrderBy($"{sort} {order}")
        .ToPageListAsync(page, rows);
      return Json(new { total = count, rows = result }, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    public async Task<JsonResult> Sync(string so) {
      try
      {
        var result = await Task.Run(() =>
        {
          var connectionstring = this.db.CurrentConnectionConfig.ConnectionString;
          var sync = new SyncSAPToWorkflow(connectionstring);
          return sync.UpdateSOForVPC(so);
        });
        this.logger.Info($"sync vpc So completed,{string.Join(",", result)}");
        return Json(new { success = true, result }, JsonRequestBehavior.AllowGet);
      }catch(Exception e)
      {
        return Json(new { success = false, err=e.Message }, JsonRequestBehavior.AllowGet);
      }
    }
  }


}