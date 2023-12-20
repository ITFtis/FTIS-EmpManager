using Dou.Controllers;
using Dou.Misc;
using Dou.Models.DB;
using FtisHelperV2.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DouImp.Controllers
{    
    [Dou.Misc.Attr.MenuDef(Id = "Pj", Name = "專案", MenuPath = "人資專區", Action = "Index", Index = 7, Func = Dou.Misc.Attr.FuncEnum.ALL, AllowAnonymous = false)]
    public class PjController : Dou.Controllers.APaginationModelController<F22cmmProjectData>
    {
        // GET: F22cmmProjectData
        public ActionResult Index()
        {
            return View();
        }

        internal static System.Data.Entity.DbContext _dbContext = FtisHelperV2.DB.FtisModelContext.Create();

        protected override Dou.Models.DB.IModelEntity<F22cmmProjectData> GetModelEntity()
        {
            return new Dou.Models.DB.ModelEntity<F22cmmProjectData>(_dbContext);
        }

        protected override IQueryable<F22cmmProjectData> BeforeIQueryToPagedList(IQueryable<F22cmmProjectData> iquery, params KeyValueParams[] paras)
        {
            var result = base.BeforeIQueryToPagedList(iquery, paras);

            var strFilterIsClosed = Dou.Misc.HelperUtilities.GetFilterParaValue(paras, "FilterIsClosed");
            if (strFilterIsClosed != null)
            {
                string IsClosed = strFilterIsClosed;
                result = result.Where(a => a.IsClosed == IsClosed);
            }

            return result.OrderByDescending(a => a.PrjYear);//.Take(3);
        }

        protected override void AddDBObject(IModelEntity<F22cmmProjectData> dbEntity, IEnumerable<F22cmmProjectData> objs)
        {
            var f = objs.First();

            int prjYear = 0;
            if (int.TryParse(f.PrjID.Substring(0, 3), out prjYear))
            {
                f.PrjYear = prjYear;
            }

            base.AddDBObject(dbEntity, objs);
        }

        public override DataManagerOptions GetDataManagerOptions()
        {
            DataManagerOptions opts = base.GetDataManagerOptions();
            //20230628, add by markhong
            opts.ctrlFieldAlign = "left";
            opts.editformWindowStyle = "modal";
            opts.editformWindowClasses = "modal-xl";
            opts.editformSize.height = "fixed";
            opts.editformSize.width = "auto";

            //全部欄位排序
            foreach (var field in opts.fields)
                field.sortable = true;

            opts.GetFiled("CkNo1").filter = false;
            opts.GetFiled("PjNo").filter = false;
            opts.GetFiled("IsClosed").filter = false;
            opts.GetFiled("PjNameM").editable = false;
            opts.GetFiled("PrjYear").visibleEdit = false;
            //opts.ctrlFieldAlign = "left";

            return opts;
        }

        public virtual ActionResult importMap(IEnumerable<F22cmmProjectDataMap> objs)
        {
            var f = objs.First();

            //設定對應財務專案編號
            string mapPrjID = "";
            string pjNameM = "";

            try
            {
                System.Data.Entity.DbContext dbContext = FtisHelperV2.DB.FtisModelContext.Create();


                Dou.Models.DB.IModelEntity<F22cmmProjectData> projects = new Dou.Models.DB.ModelEntity<F22cmmProjectData>(dbContext);
                //保留數字，比對現有的專案編號
                //string strPjNoM = String.Concat(f.PjNoM.Where(char.IsNumber));
                string strPjNoM = String.Concat(f.PjNoM.Where(char.IsLetterOrDigit));
                var project = projects.Get(a => a.PrjID == strPjNoM);
                if (project != null)
                {
                    mapPrjID = project.PrjID;
                    pjNameM = project.PrjName;
                }

                Dou.Models.DB.IModelEntity<F22cmmProjectDataMap> models = new Dou.Models.DB.ModelEntity<F22cmmProjectDataMap>(dbContext);
                var model = models.Get(a => a.PrjID == f.PrjID);

                if (model == null)
                {
                    F22cmmProjectDataMap projectDataMap = new F22cmmProjectDataMap();

                    projectDataMap.PrjID = f.PrjID;
                    projectDataMap.PjNoM = f.PjNoM;
                    projectDataMap.MapPrjID = mapPrjID;

                    models.Add(projectDataMap);
                }
                else
                {
                    //沒對應，不調整專案對應資料表
                    if (mapPrjID != "")
                    {
                        model.PjNoM = f.PjNoM;
                        model.MapPrjID = mapPrjID;

                        models.Update(model);
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = "執行失敗：" + ex.Message + " " + ex.InnerException;
                return Json(new { result = false, errorMessage = errorMessage });
            }

            if (mapPrjID == "")
            {
                string errorMessage = "此財務專案編號查無對應資料：" + f.PjNoM;
                return Json(new { result = false, errorMessage = errorMessage });
            }
            else
            {
                return Json(new { result = true, pjNameM = pjNameM });
            }
        }
    }
}