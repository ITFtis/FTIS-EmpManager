﻿using DouImp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DouImp.Controllers
{
    [Dou.Misc.Attr.MenuDef(Name = "角色管理", MenuPath = "系統管理", Action = "Index", Index = 91, Func = Dou.Misc.Attr.FuncEnum.ALL, AllowAnonymous = false)]
    public class RoleController : Dou.Controllers.RoleBaseController<Role>
    {
        // GET: Role
        public ActionResult Index()
        {
            return View();
        }
        
        protected override Dou.Models.DB.IModelEntity<Role> GetModelEntity()
        {
            System.Data.Entity.DbContext dbContext = new DouModelContextExt();

            return new Dou.Models.DB.ModelEntity<Role>(dbContext);
        }
    }

}