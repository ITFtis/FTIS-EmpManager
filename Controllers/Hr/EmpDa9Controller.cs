using Dou.Misc;
using Dou.Models.DB;
using FtisHelperV2.DB;
using FtisHelperV2.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DouImp.Controllers.Hr
{
    public class EmpDa9Controller : Dou.Controllers.AGenericModelController<F22cmmEmpDa9>
    {
        // GET: EmpDa9 著作
        public ActionResult Index()
        {
            return View();
        }
        protected override IModelEntity<F22cmmEmpDa9> GetModelEntity()
        {
            return new Dou.Models.DB.ModelEntity<F22cmmEmpDa9>(new FtisModelContext());
        }

        protected override void AddDBObject(IModelEntity<F22cmmEmpDa9> dbEntity, IEnumerable<F22cmmEmpDa9> objs)
        {
            var f = objs.First();

            Dou.Models.DB.IModelEntity<FtisHelperV2.DB.Model.F22cmmEmpDa9> models = new Dou.Models.DB.ModelEntity<FtisHelperV2.DB.Model.F22cmmEmpDa9>(new FtisHelperV2.DB.FtisModelContext());
            var snos = models.GetAll(a => a.Fno == f.Fno).Select(a => a.sno).ToList();
            int max = 100;
            byte sno = 1;
            for (; sno < max; sno++)
                if (!snos.Contains(sno)) break;

            f.mno = f.Fno;
            f.sno = sno;
            f.UpdateTime = DateTime.Now;
            f.UpdateMan = Dou.Context.CurrentUserBase.Name;

            base.AddDBObject(dbEntity, objs);
            FtisHelperV2.DB.Helpe.Employee.ResetGetAllF22cmmEmpDa9();
        }

        protected override void UpdateDBObject(IModelEntity<F22cmmEmpDa9> dbEntity, IEnumerable<F22cmmEmpDa9> objs)
        {
            var f = objs.First();
            f.mno = f.Fno;
            f.UpdateTime = DateTime.Now;
            f.UpdateMan = Dou.Context.CurrentUserBase.Name;

            base.UpdateDBObject(dbEntity, objs);
            FtisHelperV2.DB.Helpe.Employee.ResetGetAllF22cmmEmpDa9();
        }

        protected override void DeleteDBObject(IModelEntity<F22cmmEmpDa9> dbEntity, IEnumerable<F22cmmEmpDa9> objs)
        {
            base.DeleteDBObject(dbEntity, objs);
            FtisHelperV2.DB.Helpe.Employee.ResetGetAllF22cmmEmpDa9();
        }

        public override DataManagerOptions GetDataManagerOptions()
        {
            var opts = base.GetDataManagerOptions();

            opts.GetFiled("mno").visibleEdit = false;
            opts.GetFiled("sno").visibleEdit = false;
            opts.GetFiled("UpdateTime").visibleEdit = false;
            opts.GetFiled("UpdateMan").visibleEdit = false;

            //全部欄位排序
            foreach (var field in opts.fields)
                field.sortable = true;

            //正祥
            opts.GetFiled("Fno").visibleEdit = false;
            opts.GetFiled("Fno").visible = false;
            opts.GetFiled("mno").visible = false;
            opts.GetFiled("sno").visible = false;
            opts.GetFiled("UpdateTime").visible = false;
            opts.GetFiled("UpdateMan").visible = false;

            opts.GetFiled("da901").colsize = 12;
            opts.GetFiled("da902").colsize = 12;
            
            opts.GetFiled("da902").datatype = "textarea";
            opts.GetFiled("da902").textareaheight = 9;

            return opts;
        }
    }
}