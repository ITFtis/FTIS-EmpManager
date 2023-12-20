﻿using Dou.Help;
using Dou.Misc;
using Dou.Models.DB;
using DouImp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DouImp.Controllers
{
    // GET: User
    [Dou.Misc.Attr.MenuDef(Name = "使用者管理", MenuPath = "系統管理", Action = "Index", Index = 90, Func = Dou.Misc.Attr.FuncEnum.ALL, AllowAnonymous = false)]
    [Dou.Misc.Attr.AutoLogger(Status = LoggerEntity.LoggerDataStatus.All, Content = Dou.Misc.Attr.AutoLoggerAttribute.LogContent.AssignContent,
        AssignContent = "KEY:{FKey}, 字串:{FText}")]

    public class UserController : Dou.Controllers.UserBaseControll<User, Role>
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        internal static System.Data.Entity.DbContext _dbContext = new DouModelContextExt();
        protected override Dou.Models.DB.IModelEntity<User> GetModelEntity()
        {
            return new Dou.Models.DB.ModelEntity<User>(_dbContext);
        }
        //public ActionResult ResetCache()
        //{
        //    User user = FindUser("admin");
        //    user.LastRenewPassword = DateTime.Now.AddHours(- Dou.Context.Config.RenewPasswordInterval- 1);
        //    UpdateDBObject(GetModelEntity(), new User[] { user });

        //    return Json(new { ok=true}, JsonRequestBehavior.AllowGet);
        //}

        protected override void AddDBObject(IModelEntity<User> dbEntity, IEnumerable<User> objs)
        {            
            base.AddDBObject(dbEntity, objs);
            Role.ResetGetAllDatas();
        }

        protected override void UpdateDBObject(IModelEntity<User> dbEntity, IEnumerable<User> objs)
        {            
            base.UpdateDBObject(dbEntity, objs);
            Role.ResetGetAllDatas();
        }

        protected override void DeleteDBObject(IModelEntity<User> dbEntity, IEnumerable<User> objs)
        {            
            base.DeleteDBObject(dbEntity, objs);
            Role.ResetGetAllDatas();
        }

        public override DataManagerOptions GetDataManagerOptions()
        {
            var opts = base.GetDataManagerOptions();

            opts.GetFiled("Id").title = "員工編號";

            return opts;
        }

        #region SSO Code
        /*********Sso code start ************/

        //const string SsoServer = "https://pj.ftis.org.tw/Sample/Sso/";
        //const string SsoServer = "https://pj4.ftis.org.tw/SsoTest/";
        //string SsoLogin = SsoServer; //AD
        //string SsoLogoff = SsoServer + "Auth/Logoff"; //AD
        //string SsoGetUser = SsoServer + "Auth/UserInfo";//AD
        const string SsoServer = "https://pj4.ftis.org.tw/Auth/";
        string SsoLogin = SsoServer + "Account/SignIn"; //AD
        string SsoLogoff = SsoServer + "Account/SignOut";//AD
        string SsoGetUser = SsoServer + "Account/UserInfo";//AD

        internal static FtisHelperV2.DB.Model.F22cmmEmpData CurrentFtisEmployee
        {
            get { return DouUnobtrusiveSession.Session["CurrentFtisUser"] as FtisHelperV2.DB.Model.F22cmmEmpData; }
        }

        [AllowAnonymous]
        //如需Sso請將以下method DouLoginNonSso註解掉改以 override DouLogin
        public override ActionResult DouLogin(User user, string returnUrl, bool redirectLogin = false)
        //public ActionResult DouLoginNonSso(User user, string returnUrl, bool redirectLogin = false)
        {
            if (DouUnobtrusiveSession.Session[SkipSsoKey] ==null)
            {
                //取sso token
                var _ssotoken = HttpContext.Request.QueryString["ssotoken"];
                //有token(以驗證)
                if (_ssotoken != null)
                {
                    _ssotoken = _ssotoken.ToLower();
                    //取驗證使用者資料
                    var ssou = GetUserInfoSSO(_ssotoken);
                    if ((bool)ssou.Success)
                    {
                        dynamic ssouser = ssou.User;
                        string ssouid = ssouser.Fno.Value + ""; //員編
                        string ssouname = ssouser.Name.Value + "";//姓名
                        string ssouemail = ssouser.EMail.Value + "";    //EMail
                        string ssoudc = ssouser.DCode.Value + "";       //部門代碼

                        ////ssouid = "F00878";

                        User u = FindUser(ssouid); //已驗證，故直接取系統使用者
                        if (u != null)
                        {
                            user = u;
                            //可考慮是須更新本身系統user
                            //if (ssoudc != u.Dep || ssouemail != u.EMail || !Dou.Context.Config.VerifyPassword(u.Password, pw))
                            //{
                            //    u.Dep = ssoudc;
                            //    u.EMail = ssouemail;
                            //    u.Password = Dou.Context.Config.PasswordEncode(pw);
                            //    this.UpdateDBObject(GetModelEntity(), new User[] { u });
                            //}
                        }
                        else //系統尚無此使用者
                        {

                            user = new User() { Id = ssouid, Name = ssouname, Enabled = false };
                            this.AddDBObject(GetModelEntity(), new User[] { user });
                        }
                        redirectLogin = false;
                    }
                    else //取sso使用者失敗
                    {
                        ViewBag.ErrorMessage = ssou.Desc;
                    }
                }
                else
                {
                    //導向sso驗證
                    return new RedirectResult(SsoLogin + "?redirectLogin=true&returnUrl=" + HttpUtility.UrlEncode(HttpContext.Request.Url + ""));
                }
            }
            ActionResult v = base.DouLogin(user, returnUrl, redirectLogin);

           
            if (ViewBag.ErrorMessage != null)
            {
                ViewBag.LoginUrl = Dou.Context.Config.LoginPage;
                ViewBag.LogoffUrl = new UrlHelper(System.Web.HttpContext.Current.Request.RequestContext).Action("DouLogoff", "User");
                return PartialView("DouLoginError", user);
            }
            else
            {
                if (v is RedirectResult || v is RedirectToRouteResult)
                {
                    //可將login的Ftis員工資訊儲存，可用DouUnobtrusiveSession.Session["CurrentFtisUser"]取得
                    //DouUnobtrusiveSession.Session.Add("CurrentFtisUser", FtisHelper.DB.Hepler.GetEmployeeIncludeDepartment(user.Id));
                }
                return v is RedirectResult || v is RedirectToRouteResult ? v : PartialView(user);
            }
        }
        [AllowAnonymous]
        //如需Sso請將以下method DouLogoffNonSso註解掉改以 override DouLogoff
        public override ActionResult DouLogoff()
        //public ActionResult DouLogoffNonSso()
        {
            if (DouUnobtrusiveSession.Session[SkipSsoKey] == null)
            {
                base.DouLogoff();
                var returnurl = Dou.Context.Config.LoginPage;
                if (!returnurl.ToLower().StartsWith("http"))
                {
                    var logoffUrl = new UrlHelper(HttpContext.Request.RequestContext).Action("DouLogoff", "User");
                    returnurl = HttpContext.Request.Url.AbsoluteUri.Replace(logoffUrl, returnurl);
                }
                return Redirect(SsoLogoff + "?returnUrl=" + HttpUtility.UrlEncode(returnurl));
            }
            else
            {
                DouUnobtrusiveSession.Session.Remove(SkipSsoKey);
                return base.DouLogoff();
            }
        }

        const string SkipSsoKey = "SkipSso";
        /// <summary>
        /// 略過Sso驗證
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult SkipSso()
        {
            DouUnobtrusiveSession.Session.Add(SkipSsoKey, true);
            return RedirectToAction("DouLogin");
        }
        dynamic GetUserInfoSSO(string token)
        {
            var ttask = DouHelper.HClient.Get<Newtonsoft.Json.Linq.JToken>(SsoGetUser + "?token=" + token);
            if (ttask.Result.Success)
            {
                return ttask.Result.Result;
            }
            else
            {
                throw new Exception(ttask.Result.Message);
            }
          
        }

        /*********Sso code end ************/
    }
    #endregion
}