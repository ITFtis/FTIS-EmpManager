using Dou.Models.DB;
using FtisHelperV2.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DouImp.Controllers
{    
    [Dou.Misc.Attr.MenuDef(Id = "Pjm", Name = "專案對應", MenuPath = "人資專區", Action = "Index", Index = 8, Func = Dou.Misc.Attr.FuncEnum.ALL, AllowAnonymous = false)]
    public class PjmController : Dou.Controllers.APaginationModelController<F22cmmProjectDataMap>
    {
        // GET: Pjm
        public ActionResult Index()
        {
            return View();
        }

        protected override Dou.Models.DB.IModelEntity<F22cmmProjectDataMap> GetModelEntity()
        {
            System.Data.Entity.DbContext dbContext = FtisHelperV2.DB.FtisModelContext.Create();

            return new Dou.Models.DB.ModelEntity<F22cmmProjectDataMap>(dbContext);
        }

        protected override void AddDBObject(IModelEntity<F22cmmProjectDataMap> dbEntity, IEnumerable<F22cmmProjectDataMap> objs)
        {
            if (!ValidateSave(objs.First(), "Add"))
                return;

            base.AddDBObject(dbEntity, objs);
        }

        protected override void UpdateDBObject(IModelEntity<F22cmmProjectDataMap> dbEntity, IEnumerable<F22cmmProjectDataMap> objs)
        {
            if (!ValidateSave(objs.First(), "Update"))
                return;

            base.UpdateDBObject(dbEntity, objs);
        }

        private bool ValidateSave(F22cmmProjectDataMap f, string type)
        {
            bool result = false;

            string prjID = f.PrjID;
            string mapPrjID = f.MapPrjID;

            using (var db = new FtisHelperV2.DB.FtisModelContext())
            {
                IEnumerable<F22cmmProjectDataMap> tmp;

                if (type == "Update")
                {
                    tmp = db.F22cmmProjectDataMap.Where(a => a.PrjID != prjID);
                }
                else
                {
                    tmp = db.F22cmmProjectDataMap;
                }

                if (tmp.Where(a => a.PrjID == prjID).Count() > 0)
                {
                    string errorMessage = string.Format("此專案編號已設定對應財務資料，不可重複：{0}", prjID);
                    throw new Exception(errorMessage);
                }

                if (db.F22cmmProjectData.Where(a => a.PrjID == prjID).Count() == 0)
                {
                    string errorMessage = string.Format("專案編號尚未建立：{0}", prjID);
                    throw new Exception(errorMessage);
                }

                if (db.F22cmmProjectData.Where(a => a.PrjID == mapPrjID).Count() == 0)
                {
                    string errorMessage = string.Format("專案編號尚未建立：{0}", mapPrjID);
                    throw new Exception(errorMessage);
                }
            }

            result = true;

            return result;
        }
    }
}