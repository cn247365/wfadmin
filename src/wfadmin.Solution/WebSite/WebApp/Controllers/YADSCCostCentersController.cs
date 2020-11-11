using System;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Repository.Pattern.UnitOfWork;
using Repository.Pattern.Infrastructure;
using Z.EntityFramework.Plus;
using TrackableEntities;
using WebApp.Models;
using WebApp.Services;
using WebApp.Repositories;
namespace WebApp.Controllers
{
/// <summary>
/// File: YADSCCostCentersController.cs
/// Purpose:Home/YADSCCostCenter
/// Created Date: 2020-10-28 14:51:17
/// Author: neo.zhu
/// Tools: SmartCode MVC5 Scaffolder for Visual Studio 2017
/// TODO: Registers the type mappings with the Unity container(Mvc.UnityConfig.cs)
/// <![CDATA[
///    container.RegisterType<IRepositoryAsync<YADSCCostCenter>, Repository<YADSCCostCenter>>();
///    container.RegisterType<IYADSCCostCenterService, YADSCCostCenterService>();
/// ]]>
/// Copyright (c) 2012-2018 All Rights Reserved
/// </summary>
    [Authorize]
    [RoutePrefix("YADSCCostCenters")]
	public class YADSCCostCentersController : Controller
	{
		private readonly IYADSCCostCenterService  yADSCCostCenterService;
		private readonly IUnitOfWorkAsync unitOfWork;
        private readonly NLog.ILogger logger;
		public YADSCCostCentersController (
          IYADSCCostCenterService  yADSCCostCenterService, 
          IUnitOfWorkAsync unitOfWork,
          NLog.ILogger logger
          )
		{
			this.yADSCCostCenterService  = yADSCCostCenterService;
			this.unitOfWork = unitOfWork;
            this.logger = logger;
		}
        		//GET: YADSCCostCenters/Index
        //[OutputCache(Duration = 60, VaryByParam = "none")]
        [Route("Index", Name = "YADSCCostCenter", Order = 1)]
		public ActionResult Index() => this.View();

		//Get :YADSCCostCenters/GetData
		//For Index View datagrid datasource url
        
		[HttpGet]
        //[OutputCache(Duration = 10, VaryByParam = "*")]
		 public async Task<JsonResult> GetData(int page = 1, int rows = 10, string sort = "Id", string order = "asc", string filterRules = "")
		{
			var filters = JsonConvert.DeserializeObject<IEnumerable<filterRule>>(filterRules);
			var pagerows  = (await this.yADSCCostCenterService
						               .Query(new YADSCCostCenterQuery().Withfilter(filters))
							           .OrderBy(n=>n.OrderBy(sort,order))
							           .SelectPageAsync(page, rows, out var totalCount))
                                       .Select(  n => new { 

    Id = n.Id,
    CostCenter = n.CostCenter,
    CostCenterName = n.CostCenterName,
    CCMGlobalID = n.CCMGlobalID,
    CCMShortName = n.CCMShortName,
    CCMDisplayName = n.CCMDisplayName,
    BOMGlobalID = n.BOMGlobalID,
    BOMShortName = n.BOMShortName,
    BOMDisplayName = n.BOMDisplayName,
    OverallResopnsible = n.OverallResopnsible,
    BusLineFunHeadDisplayName = n.BusLineFunHeadDisplayName,
    BusLineFunHeadGlobalID = n.BusLineFunHeadGlobalID
}).ToList();
			var pagelist = new { total = totalCount, rows = pagerows };
			return Json(pagelist, JsonRequestBehavior.AllowGet);
		}
        //easyui datagrid post acceptChanges 
		[HttpPost]
		public async Task<JsonResult> AcceptChanges(YADSCCostCenter[] yadsccostcenters)
		{
            try{
               this.yADSCCostCenterService.ApplyChanges( yadsccostcenters);
               var result = await this.unitOfWork.SaveChangesAsync();
			   return Json(new {success=true,result}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, err = e.GetMessage() }, JsonRequestBehavior.AllowGet);
            }
        }
				
		//GET: YADSCCostCenters/Details/:id
		public ActionResult Details(int id)
		{
			
			var yADSCCostCenter = this.yADSCCostCenterService.Find(id);
			if (yADSCCostCenter == null)
			{
				return HttpNotFound();
			}
			return View(yADSCCostCenter);
		}
        //GET: YADSCCostCenters/GetItem/:id
        [HttpGet]
        public async Task<JsonResult> GetItem(int id) {
            var  yADSCCostCenter = await this.yADSCCostCenterService.FindAsync(id);
            return Json(yADSCCostCenter,JsonRequestBehavior.AllowGet);
        }
		//GET: YADSCCostCenters/Create
        		public ActionResult Create()
				{
			var yADSCCostCenter = new YADSCCostCenter();
			//set default value
			return View(yADSCCostCenter);
		}
		//POST: YADSCCostCenters/Create
		//To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(YADSCCostCenter yADSCCostCenter)
		{
            if (ModelState.IsValid)
			{
                try{ 
				this.yADSCCostCenterService.Insert(yADSCCostCenter);
				var result = await this.unitOfWork.SaveChangesAsync();
                return Json(new { success = true,result }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    return Json(new { success = false, err = e.GetMessage() }, JsonRequestBehavior.AllowGet);
                }
			    //DisplaySuccessMessage("Has update a yADSCCostCenter record");
			}
			else {
			   var modelStateErrors =string.Join(",", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
			   return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
			   //DisplayErrorMessage(modelStateErrors);
			}
			//return View(yADSCCostCenter);
		}

        //新增对象初始化
        [HttpGet]
        public async Task<JsonResult> NewItem() {
            var yADSCCostCenter = await Task.Run(() => {
                return new YADSCCostCenter();
                });
            return Json(yADSCCostCenter, JsonRequestBehavior.AllowGet);
        }

         
		//GET: YADSCCostCenters/Edit/:id
		public ActionResult Edit(int id)
		{
			var yADSCCostCenter = this.yADSCCostCenterService.Find(id);
			if (yADSCCostCenter == null)
			{
				return HttpNotFound();
			}
			return View(yADSCCostCenter);
		}
		//POST: YADSCCostCenters/Edit/:id
		//To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit(YADSCCostCenter yADSCCostCenter)
		{
			if (ModelState.IsValid)
			{
				yADSCCostCenter.TrackingState = TrackingState.Modified;
				                try{
				this.yADSCCostCenterService.Update(yADSCCostCenter);
				                
				var result = await this.unitOfWork.SaveChangesAsync();
                return Json(new { success = true,result = result }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {
                    return Json(new { success = false, err = e.GetMessage() }, JsonRequestBehavior.AllowGet);
                }
				
				//DisplaySuccessMessage("Has update a YADSCCostCenter record");
				//return RedirectToAction("Index");
			}
			else {
			var modelStateErrors =string.Join(",", this.ModelState.Keys.SelectMany(key => this.ModelState[key].Errors.Select(n=>n.ErrorMessage)));
			return Json(new { success = false, err = modelStateErrors }, JsonRequestBehavior.AllowGet);
			//DisplayErrorMessage(modelStateErrors);
			}
						//return View(yADSCCostCenter);
		}
        //删除当前记录
		//GET: YADSCCostCenters/Delete/:id
        [HttpGet]
		public async Task<ActionResult> Delete(int id)
		{
          try{
               await this.yADSCCostCenterService.Queryable().Where(x => x.Id == id).DeleteAsync();
               return Json(new { success = true }, JsonRequestBehavior.AllowGet);
           }
           catch (Exception e)
           {
                return Json(new { success = false, err = e.GetMessage() }, JsonRequestBehavior.AllowGet);
           }
		}
		 
       
 

        //删除选中的记录
        [HttpPost]
        public async Task<JsonResult> DeleteChecked(int[] id) {
           try{
               await this.yADSCCostCenterService.Delete(id);
               await this.unitOfWork.SaveChangesAsync();
               return Json(new { success = true }, JsonRequestBehavior.AllowGet);
           }
           catch (Exception e)
           {
                    return Json(new { success = false, err = e.GetMessage() }, JsonRequestBehavior.AllowGet);
           }
        }
		//导出Excel
		[HttpPost]
		public async Task<ActionResult> ExportExcel( string filterRules = "",string sort = "Id", string order = "asc")
		{
			var fileName = "yadsccostcenters_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
			var stream = await this.yADSCCostCenterService.ExportExcelAsync(filterRules,sort, order );
			return File(stream, "application/vnd.ms-excel", fileName);
		}
        //导入数据
    [HttpPost]
    public async Task<JsonResult> ImportData()
    {
      var watch = new Stopwatch();
      watch.Start();
      var uploadfile = this.Request.Files[0];
      var uploadfilename = uploadfile.FileName;
      var model = this.Request.Form["model"] ?? "model";
      var autosave = Convert.ToBoolean(this.Request.Form["autosave"] ?? "false");
      try
      {

        var ext = Path.GetExtension(uploadfilename);
        var newfileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{uploadfile.FileName.Replace(ext, "")}{ext}";//重组成新的文件名
        var stream = new MemoryStream();
        await uploadfile.InputStream.CopyToAsync(stream);
        stream.Seek(0, SeekOrigin.Begin);
        uploadfile.InputStream.Seek(0, SeekOrigin.Begin);
        var data = await NPOIHelper.GetDataTableFromExcelAsync(stream, ext);
        await this.yADSCCostCenterService.ImportDataTableAsync(data, Auth.GetFullName());
        await this.unitOfWork.SaveChangesAsync();
        if (autosave)
        {
          var folder = this.Server.MapPath($"/UploadFiles/{model}");
          if (!Directory.Exists(folder))
          {
            Directory.CreateDirectory(folder);
          }
          var savepath = Path.Combine(folder, newfileName);
          uploadfile.SaveAs(savepath);
        }
        watch.Stop();
        //获取当前实例测量得出的总运行时间（以毫秒为单位）
        var elapsedTime = watch.ElapsedMilliseconds.ToString();
        return Json(new { success = true, filename = newfileName, elapsedTime = elapsedTime }, JsonRequestBehavior.AllowGet);
      }
      catch (Exception e)
      {
        var message = e.GetMessage();
        this.logger.Error(e, $"导入失败,文件名:{uploadfilename}");
        return this.Json(new { success = false, filename = uploadfilename, message = message }, JsonRequestBehavior.AllowGet);
      }
    }
		 
	}
}
