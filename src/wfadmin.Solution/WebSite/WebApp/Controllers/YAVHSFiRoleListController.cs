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
  [Authorize]
  public class YAVHSFiRoleListController : Controller
  {
    private readonly NLog.ILogger logger;
    private readonly SqlSugar.ISqlSugarClient db;
    public YAVHSFiRoleListController(NLog.ILogger logger, SqlSugar.ISqlSugarClient db)
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
    // GET: YAVHSFiRoleList
    public ActionResult Index()
    {
      return View();
    }

    [HttpGet]
    //[OutputCache(Duration = 10, VaryByParam = "*")]
    public async Task<JsonResult> GetData(int page = 1, int rows = 10, string sort = "ID", string order = "desc", string filterRules = "")
    {
      var filters = PredicateBuilder.From<YAVHSFiRoleList>(filterRules);
      var count =await this.db.Queryable<YAVHSFiRoleList>().Where(filters)
         .CountAsync();
      var result=await this.db.Queryable<YAVHSFiRoleList>().Where(filters)
        .OrderBy($"{sort} {order}")
        .ToPageListAsync(page, rows);
      return Json(new { total = count, rows = result }, JsonRequestBehavior.AllowGet);
    }
    [HttpPost]
    public async Task<JsonResult> AcceptChanges(YAVHSFiRoleList[] rolelist)
    {
      try
      {
        //await this.db.Updateable<YAVHSFiRoleList>(rolelist).ExecuteCommandAsync();
        foreach (var item in rolelist)
        {
          if(item.TrackingState== TrackableEntities.TrackingState.Added)
          {
           await this.db.Insertable<YAVHSFiRoleList>(item).ExecuteCommandAsync();
          }else
            if ( item.TrackingState == TrackableEntities.TrackingState.Modified )
          {
            await this.db.Updateable<YAVHSFiRoleList>(item).ExecuteCommandAsync();
          } else if( item.TrackingState == TrackableEntities.TrackingState.Deleted )
          {
            await this.db.Deleteable<YAVHSFiRoleList>(item).ExecuteCommandAsync();
          } 
        }
        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
      }
      catch (Exception e)
      {
        return Json(new { success = false, err = e.GetMessage() }, JsonRequestBehavior.AllowGet);
      }
    }

    //删除选中的记录
    [HttpPost]
    public async Task<JsonResult> DeleteChecked(int[] id)
    {
      try
      {
        await this.db.Deleteable<YAVHSFiRoleList>().In(id).ExecuteCommandAsync();
 
        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
      }
      catch (Exception e)
      {
        return Json(new { success = false, err = e.GetMessage() }, JsonRequestBehavior.AllowGet);
      }
    }


  }


}