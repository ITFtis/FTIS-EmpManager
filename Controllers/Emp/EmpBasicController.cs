using Dou.Misc;
using Dou.Models.DB;
using DouImp._core;
using DouImp.Models;
using FtisHelperV2.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DouImp.Controllers.Emp
{
    [Dou.Misc.Attr.MenuDef(Id = "EmpBasic", Name = "員工基本資料", MenuPath = "員工資料", Action = "Index", Index = 2, Func = Dou.Misc.Attr.FuncEnum.ALL, AllowAnonymous = false)]
    public class EmpBasicController : Dou.Controllers.AGenericModelController<F22cmmEmpData>
    {
        // GET: EmpBasic
        public ActionResult Index()
        {
            return View();
        }

        protected override void UpdateDBObject(IModelEntity<F22cmmEmpData> dbEntity, IEnumerable<F22cmmEmpData> objs)
        {
            base.UpdateDBObject(dbEntity, objs);
        }

        public override DataManagerOptions GetDataManagerOptions()
        {
            var options = base.GetDataManagerOptions();
            options.GetFiled("CkNo1").visible = false;
            options.GetFiled("CkNo1").visibleEdit = false;
            options.GetFiled("CkNo2").visible = false;
            options.GetFiled("CkNo2").visibleEdit = false;
            options.GetFiled("CkNo3").visible = false;
            options.GetFiled("CkNo3").visibleEdit = false;
            options.GetFiled("CkNo4").visible = false;
            options.GetFiled("CkNo4").visibleEdit = false;
            options.GetFiled("CkNo5").visible = false;
            options.GetFiled("CkNo5").visibleEdit = false;
            options.GetFiled("kpino1").visible = false;
            options.GetFiled("kpino1").visibleEdit = false;
            options.GetFiled("kpino2").visible = false;
            options.GetFiled("kpino2").visibleEdit = false;
            options.GetFiled("kpino3").visible = false;
            options.GetFiled("kpino3").visibleEdit = false;
            options.GetFiled("kpino4").visible = false;
            options.GetFiled("kpino4").visibleEdit = false;
            options.GetFiled("kpino5").visible = false;
            options.GetFiled("kpino5").visibleEdit = false;

            options.GetFiled("Mno").visibleEdit = false;
            options.GetFiled("ADRest").visibleEdit = false;
            options.GetFiled("TEnddate").visibleEdit = false;
            options.GetFiled("IsQuit").visibleEdit = false;
            options.GetFiled("Quit").visibleEdit = false;
            options.GetFiled("Name").editable = false;

            options.GetFiled("DCode").visibleEdit = false;
            options.GetFiled("TCode").visibleEdit = false;
            options.GetFiled("GCode").visibleEdit = false;

            options.GetFiled("AD").visibleEdit = false;
            options.GetFiled("AgentNo").visibleEdit = false;
            options.GetFiled("TCode_Display").visibleEdit = false;
            options.GetFiled("AD_Vacation").visibleEdit = false;

            options.GetFiled("QuitDate").visibleEdit = false;
            options.GetFiled("MpCode").visibleEdit = false;
            options.GetFiled("UseQuit").visibleEdit = false;
            options.GetFiled("QuitYN").visibleEdit = false;
            options.GetFiled("QuitNo").visibleEdit = false;

            options.GetFiled("UseTrainning").visibleEdit = false;
            options.GetFiled("eryn").visibleEdit = false;
            options.GetFiled("IsOT2V").visibleEdit = false;
            options.GetFiled("UpdateTime").visibleEdit = false;
            options.GetFiled("UpdateMan").visibleEdit = false;
            options.GetFiled("DCode_").visibleEdit = false;

            options.GetFiled("SeatNo").visible = false;
            options.GetFiled("SeatNo").visibleEdit = false;

            options.GetFiled("CkNo1_Dep").visibleEdit = false;
            options.GetFiled("CkNo2_Dep").visibleEdit = false;
            options.GetFiled("CkNo3_Dep").visibleEdit = false;
            options.GetFiled("CkNo4_Dep").visibleEdit = false;
            options.GetFiled("CkNo5_Dep").visibleEdit = false;

            options.GetFiled("AgentNo_Dep").visibleEdit = false;

            options.GetFiled("kpino1_Dep").visibleEdit = false;
            options.GetFiled("kpino2_Dep").visibleEdit = false;
            options.GetFiled("kpino3_Dep").visibleEdit = false;
            options.GetFiled("kpino4_Dep").visibleEdit = false;
            options.GetFiled("kpino5_Dep").visibleEdit = false;

            options.GetFiled("Da1s").visibleEdit = false;

            options.singleDataEdit = true;
            //options.singleDataEditCompletedReturnUrl = System.Web.HttpContext.Current.Request.UrlReferrer == null ?
            //    Dou.Context.CurrentUser<User>().DefaultPage : System.Web.HttpContext.Current.Request.UrlReferrer.ToString();
            //options.datas = new User[] { Dou.Context.CurrentUser<User>() };
            string userid = Dou.Context.CurrentUser<User>().Id;
            var dbe = GetModelEntity();
            options.datas = new F22cmmEmpData[] { dbe.FirstOrDefault(s => s.Fno == userid) };
            // (Dou.Misc.HelperUtilities.GetKeyValues<F22cmmEmpData>(((Dou.Models.DB.ModelEntity<F22cmmEmpData>)dbe)._context) };
            options.editformWindowStyle = "showEditformOnly";
            return options;
        }

        protected override IModelEntity<F22cmmEmpData> GetModelEntity()
        {
            return new Dou.Models.DB.ModelEntity<F22cmmEmpData>(FtisHelperV2.DB.Helper.CreateFtisModelContext());
        }

        //匯出基本資料表
        public ActionResult ExportBasicExcel(string fno)
        {
            Rpt_EmpBasic rep = new Rpt_EmpBasic();
            string url = rep.Export(fno, ".xlsx");

            if (url == "")
            {
                return Json(new { result = false, errorMessage = rep.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = true, url = url }, JsonRequestBehavior.AllowGet);
            }
        }

        //匯出基本資料表
        public ActionResult ExportBasicWord(string fno)
        {
            Rpt_EmpBasic rep = new Rpt_EmpBasic();
            string url = rep.Export(fno, ".docx");

            if (url == "")
            {
                return Json(new { result = false, errorMessage = rep.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = true, url = url }, JsonRequestBehavior.AllowGet);
            }
        }

        //匯出履歷表
        public ActionResult ExportCV(string fno)
        {
            Rpt_EmpCV rep = new Rpt_EmpCV();
            string url = rep.Export(fno, ".docx");

            if (url == "")
            {
                return Json(new { result = false, errorMessage = rep.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = true, url = url }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}