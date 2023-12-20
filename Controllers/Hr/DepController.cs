using Dou.Controllers;
using Dou.Misc;
using Dou.Models.DB;
using DouImp.Models;
using FtisHelperV2.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DouImp.Controllers
{    
    [Dou.Misc.Attr.MenuDef(Id = "Dep", Name = "部門", MenuPath = "人資專區", Action = "Index", Index = 3, Func = Dou.Misc.Attr.FuncEnum.ALL, AllowAnonymous = false)]

    public class DepController : Dou.Controllers.AGenericModelController<F22cmmDep>
    {
        // GET: Country
        public ActionResult Index()
        {
            return View();
        }
        protected override IEnumerable<F22cmmDep> GetDataDBObject(IModelEntity<F22cmmDep> dbEntity, params KeyValueParams[] paras)
        {
            var iquery = base.GetDataDBObject(dbEntity, paras);
            if (string.IsNullOrEmpty(paras.FirstOrDefault(s => s.key == "sort").value + ""))
                iquery = iquery.OrderBy(s => s.DCode);
            return iquery;
        }
        public override DataManagerOptions GetDataManagerOptions()
        {
            var options = base.GetDataManagerOptions();
            options.editformWindowStyle = "modal";
            //options.editformWindowClasses = "modal-xl";
            options.editformSize.height = "fixed";
            options.editformSize.width = "auto";
            return options;
        }
        protected override IModelEntity<F22cmmDep> GetModelEntity()
        {
            return new Dou.Models.DB.ModelEntity<F22cmmDep>(FtisHelperV2.DB.Helper.CreateFtisModelContext());
        }
    }
}