using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DouImp.Controllers.Path
{
    [Dou.Misc.Attr.MenuDef(Id = "EmpPath", Name = "員工資料", Index = 1, IsOnlyPath = true)]
    public class EmpPathController : Controller
    {
        // GET: EmpPath
        public ActionResult Index()
        {
            return View();
        }
    }
}