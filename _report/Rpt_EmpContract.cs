using DouImp._core;
using FtisHelperV2.DB.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace DouImp._report
{
    public class Rpt_EmpContract : ReportClass
    {
        /// <summary>
        /// 匯出聯絡資料
        /// </summary>
        /// <param name="fnos"></param>
        /// <returns></returns>
        public string Export(List<string> fnos)
        {
            string url = "";

            ////List<string> titles = new List<string>() { "匯出專家清單，查詢條件:" };
            List<string> titles = new List<string>();

            try
            {
                _dbContext = FtisHelperV2.DB.Helper.CreateFtisModelContext();

                string fileTitle = "員工聯絡資料";
                string folder = FileHelper.GetFileFolder(Code.TempUploadFile.員工聯絡資料);

                //主表
                Dou.Models.DB.IModelEntity<F22cmmEmpData> modelData = new Dou.Models.DB.ModelEntity<F22cmmEmpData>(_dbContext);
                var datas = modelData.GetAll().Where(a => fnos.Contains(a.Fno)).ToList();

                //產出Dynamic資料 (給Excel)
                List<dynamic> list = new List<dynamic>();

                int serial = 1;
                foreach (var data in datas)
                {
                    dynamic f = new ExpandoObject();
                    f.序號 = serial;
                    serial++;
                    f.員編 = data.Fno;
                    f.姓名 = data.Name;
                    f.行動電話 = data.Da1s.da15;
                    f.戶籍電話 = data.Da1s.da11a + " " + data.Da1s.da11;
                    f.戶籍地址 = data.Da1s.da09 + " " + data.Da1s.da10;
                    f.通訊地址 = data.Da1s.da12 + " " + data.Da1s.da13;
                    f.住家電話 = data.Da1s.da14a + " " + data.Da1s.da14;                    

                    f.SheetName = fileTitle;//sheep.名稱;
                    list.Add(f);
                }

                //查無符合資料表數
                if (list.Count == 0)
                {
                    _errorMessage = "查無符合資料表數";
                }

                //產出excel
                string fileName = DouImp.ExcelSpecHelper.GenerateExcelByLinqF1(fileTitle, titles, list, folder, 2);
                string path = folder + fileName;                

                url = DouImp.Cm.PhysicalToUrl(path);
            }
            catch (Exception ex)
            {
                _errorMessage = "匯出員工聯絡資料失敗" + "\n" + ex.InnerException + ex.Message + "\n" + ex.StackTrace;
                return "";
            }

            return url;
        }
    }
}