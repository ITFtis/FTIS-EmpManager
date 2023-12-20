using Dou.Models.DB;
using DouImp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FtisHelperV2.DB.Model;
using Dou.Misc;
using System.Threading.Tasks;
using Dou.Controllers;
using System.Data.Entity;
using System.Collections;
using System.Threading;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using FtisHelperV2.DB.Helpe;
using System.IO;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using Microsoft.Reporting;
using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using System.Dynamic;
using DouImp._core;
using ZXing;
using Microsoft.Ajax.Utilities;
using System.Drawing.Drawing2D;
using System.Security.Policy;
using DouImp._report;

namespace DouImp.Controllers
{   
    [Dou.Misc.Attr.MenuDef(Id = "EmpAll", Name = "員工資料總表", MenuPath = "人資專區", Action = "Index", Index = 1, Func = Dou.Misc.Attr.FuncEnum.ALL, AllowAnonymous = false)]
    public class EmpController : Dou.Controllers.APaginationModelController<F22cmmEmpData>
    {
        // GET: Disposals
        public ActionResult Index()
        {
            return View();
        }

        ////public override Task<ActionResult> GetData(params KeyValueParams[] paras)
        ////{
        ////    //Mvc MaxJsonLength序列化長度問題
        ////    var datas = base.GetData(paras);
        ////    (datas.Result as JsonResult).MaxJsonLength = Int32.MaxValue;

        ////    return datas;
        ////}

        protected override IQueryable<F22cmmEmpData> BeforeIQueryToPagedList(IQueryable<F22cmmEmpData> iquery, params KeyValueParams[] paras)
        {
            //編輯或權限功能
            var cusPowerRole = Dou.Context.CurrentUser<User>().CusPowerRole();
            var cusPowerKPIFnos = Dou.Context.CurrentUser<User>().CusPowerKPIFnos();


            if (cusPowerRole.Key == "admin" || cusPowerRole.Key == "hr")
            {
                //查詢所有人
            }
            else
            {
                string Fno = Dou.Context.CurrentUser<User>().Id;

                //查詢自己或屬下員工
                iquery = iquery.Where(a => Fno == a.Fno
                                || cusPowerKPIFnos.Any(b => b == a.Fno));
            }

            ////////Test Left Join
            ////Dou.Models.DB.IModelEntity<F22cmmEmpData> data = new Dou.Models.DB.ModelEntity<F22cmmEmpData>(FtisHelperV2.DB.Helper.CreateFtisModelContext());
            ////Dou.Models.DB.IModelEntity<F22cmmEmpDa1> da1s = new Dou.Models.DB.ModelEntity<F22cmmEmpDa1>(FtisHelperV2.DB.Helper.CreateFtisModelContext());
            //////( e1 ) . GroupJoin( e2 , x1 => k1 , x2 => k2 , ( x1 , g ) => v )
            ////var z = data.GetAll().GroupJoin(da1s.GetAll(), a => a.Fno, b => b.Fno, (a, b) => new { a, b });

            ////foreach (var item in z)
            ////{
            ////    string ss = "Abc";
            ////}

            ////return iquery;
            return base.BeforeIQueryToPagedList(iquery, paras);
        }

        public override DataManagerOptions GetDataManagerOptions()
        {
            var options = base.GetDataManagerOptions();

            //編輯或權限功能
            var cusPowerRole = Dou.Context.CurrentUser<User>().CusPowerRole();
            var cusPowerKPIFnos = Dou.Context.CurrentUser<User>().CusPowerKPIFnos();

            if (cusPowerRole.Key == "admin" || cusPowerRole.Key == "hr")
            {
                options.addable = true;
                options.editable = true;
                options.deleteable = true;
            }
            else
            {
                options.addable = false;
                options.editable = false;
                options.deleteable = false;
            }

            options.ctrlFieldAlign = "left";
            options.editformWindowStyle = "modal";
            options.editformWindowClasses = "modal-xl";
            options.editformSize.height = "fixed";
            options.editformSize.width = "auto";
            //options.useMutiDelete = true;
            options.GetFiled("DCode_").visibleEdit = false;
            options.GetFiled("UpdateTime").visibleEdit = false;
            options.GetFiled("UpdateMan").visibleEdit = false;
            options.GetFiled("SeatNo").visibleEdit = false;

            options.GetFiled("Da1s").visible = false;
            options.GetFiled("Da1s").visibleEdit = false;

            //職等沒再使用 
            options.GetFiled("EMail").colsize = 6;
            options.GetFiled("GCode").visible = false;
            options.GetFiled("GCode").visibleEdit = false;

            //正祥
            options.GetFiled("IsOT2V").defaultvalue = "";  //資料轉入(null)，預設'N',視為有異動

            //共用頁面
            options.editformWindowStyle = "showEditformOnly";

            return options;
        }

        /// <summary>
        /// 新增異動者與異動時間
        /// </summary>
        /// <param name="dbEntity"></param>
        /// <param name="objs"></param>
        protected override void AddDBObject(IModelEntity<F22cmmEmpData> dbEntity, IEnumerable<F22cmmEmpData> objs)
        {
            foreach (var obj in objs)
            {
                obj.UpdateMan = Dou.Context.CurrentUser<User>().Id;
                obj.UpdateTime = DateTime.Now;
            }
            
            dbEntity.Add(objs);
            FtisHelperV2.DB.Helper.ResetGetAllEmployee();
        }

        /// <summary>
        /// 更新異動者與異動時間
        /// </summary>
        /// <param name="dbEntity"></param>
        /// <param name="objs"></param>
        protected override void UpdateDBObject(IModelEntity<F22cmmEmpData> dbEntity, IEnumerable<F22cmmEmpData> objs)
        {
            foreach (var obj in objs)
            {
                obj.UpdateMan = Dou.Context.CurrentUser<User>().Id;
                obj.UpdateTime = DateTime.Now;
            }
            
            dbEntity.Update(objs);
            FtisHelperV2.DB.Helper.ResetGetAllEmployee();
        }

        protected override void DeleteDBObject(IModelEntity<F22cmmEmpData> dbEntity, IEnumerable<F22cmmEmpData> objs)
        {
            var obj = objs.FirstOrDefault();

            //DB有關聯
            ////if (obj.Da1s != null)
            ////    dbEntity.SetEntityState<F22cmmEmpDa1>(obj.Da1s, System.Data.Entity.EntityState.Deleted);
            ////if (obj.Da4s != null)
            ////    dbEntity.SetEntityState<F22cmmEmpDa4>(obj.Da4s, System.Data.Entity.EntityState.Deleted);

            //DB沒關聯
            var dbContext = FtisHelperV2.DB.Helper.CreateFtisModelContext();

            ////Employee.ResetGetAllF22cmmEmpDa1();
            ////Employee.ResetGetAllF22cmmEmpDa4();

            if (obj.Da1s != null)
            {
                Dou.Models.DB.IModelEntity<F22cmmEmpDa1> da1 = new Dou.Models.DB.ModelEntity<F22cmmEmpDa1>(dbContext);
                da1.Delete(obj.Da1s);
                Employee.ResetGetAllF22cmmEmpDa1();
            }
            if (obj.Da4s != null)
            {
                Dou.Models.DB.IModelEntity<F22cmmEmpDa4> da4 = new Dou.Models.DB.ModelEntity<F22cmmEmpDa4>(dbContext);
                da4.Delete(obj.Da4s);
                Employee.ResetGetAllF22cmmEmpDa4();
            }
            if (obj.Da5s != null)
            {
                Dou.Models.DB.IModelEntity<F22cmmEmpDa5> da5 = new Dou.Models.DB.ModelEntity<F22cmmEmpDa5>(dbContext);
                da5.Delete(obj.Da5s);
                Employee.ResetGetAllF22cmmEmpDa5();
            }
            if (obj.Da6s != null)
            {
                Dou.Models.DB.IModelEntity<F22cmmEmpDa6> da6 = new Dou.Models.DB.ModelEntity<F22cmmEmpDa6>(dbContext);
                da6.Delete(obj.Da6s);
                Employee.ResetGetAllF22cmmEmpDa6();
            }
            if (obj.Da7s != null)
            {
                Dou.Models.DB.IModelEntity<F22cmmEmpDa7> da7 = new Dou.Models.DB.ModelEntity<F22cmmEmpDa7>(dbContext);
                da7.Delete(obj.Da7s);
                Employee.ResetGetAllF22cmmEmpDa7();
            }
            if (obj.Da8s != null)
            {
                Dou.Models.DB.IModelEntity<F22cmmEmpDa8> da8 = new Dou.Models.DB.ModelEntity<F22cmmEmpDa8>(dbContext);
                da8.Delete(obj.Da8s);
                Employee.ResetGetAllF22cmmEmpDa8();
            }
            if (obj.Da9s != null)
            {
                Dou.Models.DB.IModelEntity<F22cmmEmpDa9> da9 = new Dou.Models.DB.ModelEntity<F22cmmEmpDa9>(dbContext);
                da9.Delete(obj.Da9s);
                Employee.ResetGetAllF22cmmEmpDa9();
            }
            
            base.DeleteDBObject(dbEntity, objs);
            FtisHelperV2.DB.Helper.ResetGetAllEmployee();
        }

        protected override IModelEntity<F22cmmEmpData> GetModelEntity()
        {
            return new Dou.Models.DB.ModelEntity<F22cmmEmpData>(FtisHelperV2.DB.Helper.CreateFtisModelContext());
            //return new Dou.Models.DB.ModelEntity<AssetDisposals>(new DouImp.Models.DouModelContextExt());
        }


        //匯出新人簡報
        public ActionResult ExportPPtNew(List<string> Fnos)
        {
            Rpt_EmpPPtNew rep = new Rpt_EmpPPtNew();
            string url = rep.Export(Fnos, ".docx");

            if (url == "")
            {
                return Json(new { result = false, errorMessage = rep.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = true, url = url }, JsonRequestBehavior.AllowGet);
            }

            ////return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        //匯出晉升員工
        public ActionResult ExportPPtPromote(List<string> Fnos)
        {
            Rpt_EmpPPtPromote rep = new Rpt_EmpPPtPromote();
            string url = rep.Export(Fnos, ".docx");

            if (url == "")
            {
                return Json(new { result = false, errorMessage = rep.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = true, url = url }, JsonRequestBehavior.AllowGet);
            }

            ////return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

    }
}