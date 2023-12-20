using FtisHelperV2.DB;
using FtisHelperV2.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DouImp.Models
{
    public class TestF22cmmEmpDa4 { }

    ////public class TestF22cmmEmpDa4 : F22cmmEmpDa4
    ////{
    ////    static object lockGetAllF22cmmEmpDa4 = new object();
    ////    public static IEnumerable<TestF22cmmEmpDa4> GetAllF22cmmEmpDa4(int cachetimer = 0)
    ////    {
    ////        if (cachetimer == 0) cachetimer = Constant.cacheTime;

    ////        string key = "HRM_F22.Models.F22Holiday.GetAllF22cmmEmpDa4";
    ////        var allHoliday = DouHelper.Misc.GetCache<IEnumerable<TestF22cmmEmpDa4>>(cachetimer, key);
    ////        lock (lockGetAllF22cmmEmpDa4)
    ////        {
    ////            if (allHoliday == null)
    ////            {
    ////                System.Data.Entity.DbContext dbContext = new FtisModelContext();
    ////                Dou.Models.DB.IModelEntity<TestF22cmmEmpDa4> db = new Dou.Models.DB.ModelEntity<TestF22cmmEmpDa4>(dbContext);
    ////                allHoliday = db.GetAll();

    ////                DouHelper.Misc.AddCache(allHoliday, key);
    ////            }
    ////        }

    ////        return allHoliday;
    ////    }
    ////    public static void ResetGetAllF22cmmEmpDa4()
    ////    {
    ////        string key = "HRM_F22.Models.F22Holiday.GetAllF22cmmEmpDa4";
    ////        DouHelper.Misc.ClearCache(key);
    ////    }
    ////}
}