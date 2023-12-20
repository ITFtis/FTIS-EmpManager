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
    [Dou.Misc.Attr.MenuDef(Id = "Country", Name = "縣市", MenuPath = "人資專區", Action = "Index", Index = 5, Func = Dou.Misc.Attr.FuncEnum.ALL, AllowAnonymous = false)]

    public class CountryController : Dou.Controllers.AGenericModelController<F22cmmCounty>
    {
        // GET: Country
        public ActionResult Index()
        {
            return View();
        }
        protected override IEnumerable<F22cmmCounty> GetDataDBObject(IModelEntity<F22cmmCounty> dbEntity, params KeyValueParams[] paras)
        {
            var iquery = base.GetDataDBObject(dbEntity, paras);
            if (string.IsNullOrEmpty(paras.FirstOrDefault(s => s.key == "sort").value + ""))
                iquery = iquery.OrderBy(s => s.CID);
            return iquery;
        }
        public override DataManagerOptions GetDataManagerOptions()
        {
            var options = base.GetDataManagerOptions();
            options.editformWindowStyle = "modal";
            return options;
        }
        protected override IModelEntity<F22cmmCounty> GetModelEntity()
        {
            return new Dou.Models.DB.ModelEntity<F22cmmCounty>(FtisHelperV2.DB.Helper.CreateFtisModelContext());
        }
    }
}