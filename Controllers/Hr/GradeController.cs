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
    [Dou.Misc.Attr.MenuDef(Id = "Grade", Name = "職級", MenuPath = "人資專區", Action = "Index", Index = 2, Func = Dou.Misc.Attr.FuncEnum.ALL, AllowAnonymous = false)]
    public class GradeController : Dou.Controllers.AGenericModelController<F22cmmGrade>
    {
        // GET: Country
        public ActionResult Index()
        {
            return View();
        }
        protected override IEnumerable<F22cmmGrade> GetDataDBObject(IModelEntity<F22cmmGrade> dbEntity, params KeyValueParams[] paras)
        {
            var iquery = base.GetDataDBObject(dbEntity, paras);
            if (string.IsNullOrEmpty(paras.FirstOrDefault(s => s.key == "sort").value + ""))
                iquery = iquery.OrderBy(s => s.GCode);
            return iquery;
        }
        public override DataManagerOptions GetDataManagerOptions()
        {
            var options = base.GetDataManagerOptions();
            options.editformWindowStyle = "modal";
            return options;
        }
        protected override IModelEntity<F22cmmGrade> GetModelEntity()
        {
            return new Dou.Models.DB.ModelEntity<F22cmmGrade>(FtisHelperV2.DB.Helper.CreateFtisModelContext());
        }
    }
}