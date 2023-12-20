using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dou.Controllers;
using Dou.Misc;
using Dou.Models.DB;
using FtisHelperV2.DB.Model;

namespace DouImp.Controllers
{    
    [Dou.Misc.Attr.MenuDef(Id = "MP", Name = "人力分布", MenuPath = "人資專區", Action = "Index", Index = 6, Func = Dou.Misc.Attr.FuncEnum.ALL, AllowAnonymous = false)]

    public class MPController : Dou.Controllers.AGenericModelController<F22cmmMP>
    {
        // GET: Country
        public ActionResult Index()
        {
            return View();
        }
        protected override IEnumerable<F22cmmMP> GetDataDBObject(IModelEntity<F22cmmMP> dbEntity, params KeyValueParams[] paras)
        {
            var iquery = base.GetDataDBObject(dbEntity, paras);
            if (string.IsNullOrEmpty(paras.FirstOrDefault(s => s.key == "sort").value + ""))
                iquery = iquery.OrderBy(s => s.MPCode);
            return iquery;
        }
        public override DataManagerOptions GetDataManagerOptions()
        {
            var options = base.GetDataManagerOptions();
            options.editformWindowStyle = "modal";
            return options;
        }
        protected override IModelEntity<F22cmmMP> GetModelEntity()
        {
            return new Dou.Models.DB.ModelEntity<F22cmmMP>(FtisHelperV2.DB.Helper.CreateFtisModelContext());
        }
    }
}