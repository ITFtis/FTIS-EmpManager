using DouImp._core;
using FtisHelperV2.DB.Model;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace DouImp
{
    //基本資料表
    public class Rpt_EmpBasic : ReportClass
    {
        /// <summary>
        /// 匯出基本資料表
        /// </summary>
        /// <param name="fno"></param>
        /// <param name="ext">副檔名(包含.docx)</param>
        /// <returns></returns>
        public string Export(string fno, string ext)
        {
            string resultUrl = "";
            string path = "";

            if (!RType.ContainsKey(ext))
            {
                _errorMessage = "報表格式尚未設定此附檔名：" + ext;
                return "";
            }

            //產出檔案
            try
            {
                _dbContext = FtisHelperV2.DB.Helper.CreateFtisModelContext();

                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;

                // 設定報表 iFrame Full Width
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);

                // Load Report File From Local Path
                reportViewer.LocalReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath("~/Report/EmpBasic/Master.rdlc");

                //參數設定(Fno)
                ReportParameter p1 = new ReportParameter("Fno", fno);

                // 欲連結外部圖片必須設定該屬性
                reportViewer.LocalReport.EnableExternalImages = true;
                // 參數設定(logoPath)              
                ReportParameter logo = new ReportParameter("logoPath", System.Web.HttpContext.Current.Server.MapPath("~/Images/logo.png"));
                reportViewer.LocalReport.SetParameters(new ReportParameter[] { p1, logo });

                //主表                
                DataTable dtData = GetEmpData(fno);
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("MasterEmpData", dtData));

                // 子報表事件
                reportViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(LocalReport_Content_SubreportProcessing);

                Microsoft.Reporting.WebForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                byte[] bytes = reportViewer.LocalReport.Render(
                   RType[ext], null, out mimeType, out encoding,
                    out extension,
                   out streamids, out warnings);

                string folder = FileHelper.GetFileFolder(Code.TempUploadFile.個人員工基本資料表);

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string empName = dtData.Rows[0]["姓名中"].ToString();
                string fileName = "員工基本資料_" + empName + "_" + DateFormat.ToDate1(DateTime.Now) + "_" + Guid.NewGuid().ToString().Substring(0, 5) + ext;  //"ext=.docx"
                path = folder + fileName;

                FileStream fs = new FileStream(path,
                   FileMode.Create);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }
            catch (Exception ex)
            {
                _errorMessage = "匯出基本資料表失敗" + ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
                return "";
            }

            //回傳檔案網址
            if (path != "")
            {
                resultUrl = DouImp.Cm.PhysicalToUrl(path);
            }

            return resultUrl;
        }

        //繫結子報表
        private void LocalReport_Content_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            //交易編號
            string Fno = "";

            if (e.Parameters["Fno"] != null && e.Parameters["Fno"].Values.Count != 0)
            {
                Fno = e.Parameters["Fno"].Values[0];
            }
            else
            {
                //Fno無值(參數傳遞失敗)
                return;
            }

            if (e.ReportPath == "Sub1Data")
            {
                //主表
                DataTable dt = GetEmpData(Fno);
                e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Sub1SourceData", dt));
            }
            else if (e.ReportPath == "Sub2Da4s")
            {
                //學歷
                DataTable dtDa4s = GetDa4s(Fno);
                e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Sub2SourceDa4s", dtDa4s));
            }
            else if (e.ReportPath == "Sub3Da5s")
            {
                //經歷
                DataTable dtDa5s = GetDa5s(Fno);
                e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Sub3SourceDa5s", dtDa5s));
            }
            else if (e.ReportPath == "Sub4Da6s")
            {
                //家庭狀況
                DataTable dtDa6s = GetDa6s(Fno);
                e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Sub4SourceDa6s", dtDa6s));
            }
            else if (e.ReportPath == "Sub5Da7s")
            {
                //外語檢定
                DataTable dtDa7s = GetDa7s(Fno);
                e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Sub5SourceDa7s", dtDa7s));
            }
            else if (e.ReportPath == "Sub6Da8s")
            {
                //專業資格
                DataTable dtDa8s = GetDa8s(Fno);
                e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Sub6SourceDa8s", dtDa8s));
            }
        }

        //主表
        private DataTable GetEmpData(string Fno)
        {
            Dou.Models.DB.IModelEntity<F22cmmEmpData> modelData = new Dou.Models.DB.ModelEntity<F22cmmEmpData>(_dbContext);
            var data = modelData.GetAll().Where(a => a.Fno == Fno).First();

            Dou.Models.DB.IModelEntity<F22cmmEmpDa1> modelDa1s = new Dou.Models.DB.ModelEntity<F22cmmEmpDa1>(_dbContext);
            var z_da1s = modelDa1s.GetAll().Where(a => a.Fno == Fno).ToList();

            DataTable dt = new DataTable();
            //dt.Columns.Add(new DataColumn("xxxx"));
            dt.Columns.Add(new DataColumn("姓名中"));
            dt.Columns.Add(new DataColumn("姓名英"));
            dt.Columns.Add(new DataColumn("部門"));
            dt.Columns.Add(new DataColumn("職稱"));
            dt.Columns.Add(new DataColumn("到職日期"));
            dt.Columns.Add(new DataColumn("出生日期"));
            dt.Columns.Add(new DataColumn("性別"));
            dt.Columns.Add(new DataColumn("出生地"));
            dt.Columns.Add(new DataColumn("身分證字號"));
            dt.Columns.Add(new DataColumn("婚姻"));
            dt.Columns.Add(new DataColumn("身高"));
            dt.Columns.Add(new DataColumn("體重"));
            dt.Columns.Add(new DataColumn("血型"));
            dt.Columns.Add(new DataColumn("戶籍地址"));
            dt.Columns.Add(new DataColumn("戶籍電話"));
            dt.Columns.Add(new DataColumn("通訊地址"));
            dt.Columns.Add(new DataColumn("住家電話"));
            dt.Columns.Add(new DataColumn("行動電話"));
            dt.Columns.Add(new DataColumn("Email"));
            dt.Columns.Add(new DataColumn("緊急聯絡人1姓名"));
            dt.Columns.Add(new DataColumn("緊急聯絡人1關係"));
            dt.Columns.Add(new DataColumn("緊急聯絡人1電話"));
            dt.Columns.Add(new DataColumn("緊急聯絡人2姓名"));
            dt.Columns.Add(new DataColumn("緊急聯絡人2關係"));
            dt.Columns.Add(new DataColumn("緊急聯絡人2電話"));
            dt.Columns.Add(new DataColumn("大頭照"));
            dt.Columns.Add(new DataColumn("大頭照mime"));

            DataRow dr = dt.NewRow();
            //dr["xxxx"] = "oooooo";
            dr["姓名中"] = data.Name;
            dr["姓名英"] = data.En_Name;
            dr["部門"] = FtisHelperV2.DB.Helpe.Department.GetDepartment(data.DCode) == null ? "" : FtisHelperV2.DB.Helpe.Department.GetDepartment(data.DCode).DName;
            dr["職稱"] = FtisHelperV2.DB.Helper.GetEmployeeTitle(Fno) == null ? "" : FtisHelperV2.DB.Helper.GetEmployeeTitle(Fno).Title;
            dr["到職日期"] = DateFormat.ToDate4(data.AD);
            dr["性別"] = data.Sex;
            dr["Email"] = data.EMail;

            //RDLC：image base64 header remove(ex.data:image/png;base64,iVBORw0KGgoAAAA)             
            if (!string.IsNullOrEmpty(data.ProfilePhoto))
            {
                String[] substrings = data.ProfilePhoto.Split(',');
                string header = substrings[0];
                string imgData = substrings[1];

                string[] infos = header.Split(';')[0].Split(':');
                string mime = infos.Count() > 0 ? infos[1] : infos[0];

                dr["大頭照"] = imgData;
                dr["大頭照mime"] = mime; //"image/png";
            }

            if (z_da1s.Count() > 0)
            {
                var da1s = z_da1s.First();
                dr["出生日期"] = DateFormat.ToDate4((DateTime)da1s.da03);
                dr["出生地"] = da1s.da04;
                dr["身分證字號"] = da1s.da05;
                dr["婚姻"] = da1s.da06a;
                dr["身高"] = da1s.da06;
                dr["體重"] = da1s.da07;
                dr["血型"] = da1s.da08;
                var da09 = FtisHelperV2.DB.Helpe.Employee.GetTownByZipCode(da1s.da09);
                string town10 = da09 == null ? "" : da09.CountyName + da09.TownName;
                dr["戶籍地址"] = town10 + da1s.da10;
                dr["戶籍電話"] = da1s.da11;
                var da12 = FtisHelperV2.DB.Helpe.Employee.GetTownByZipCode(da1s.da12);
                string town13 = da12 == null ? "" : da12.CountyName + da12.TownName;
                dr["通訊地址"] = town13 + da1s.da13;
                dr["住家電話"] = da1s.da14;
                dr["行動電話"] = da1s.da15;
                dr["緊急聯絡人1姓名"] = da1s.da17;
                dr["緊急聯絡人1關係"] = da1s.da18;
                dr["緊急聯絡人1電話"] = da1s.da19;
                dr["緊急聯絡人2姓名"] = da1s.da20;
                dr["緊急聯絡人2關係"] = da1s.da21;
                dr["緊急聯絡人2電話"] = da1s.da22;
            }

            dt.Rows.Add(dr);

            return dt;
        }

        //學歷
        private DataTable GetDa4s(string Fno)
        {
            Dou.Models.DB.IModelEntity<F22cmmEmpDa4> modelDa4s = new Dou.Models.DB.ModelEntity<F22cmmEmpDa4>(_dbContext);
            var da4s = modelDa4s.GetAll().Where(a => a.Fno == Fno)
                        .OrderByDescending(a => a.da404);

            DataTable dt = new DataTable();
            //dt4.Columns.Add(new DataColumn("xxxx"));
            dt.Columns.Add(new DataColumn("學校"));
            dt.Columns.Add(new DataColumn("學院"));
            dt.Columns.Add(new DataColumn("科系"));
            dt.Columns.Add(new DataColumn("入學年月"));
            dt.Columns.Add(new DataColumn("畢業年月"));
            dt.Columns.Add(new DataColumn("學位"));
            dt.Columns.Add(new DataColumn("指導教授"));

            foreach (var v in da4s)
            {
                DataRow dr = dt.NewRow();
                //dr4["xxxx"] = "oooooo";
                dr["學校"] = v.da401;
                dr["學院"] = v.da402;
                dr["科系"] = v.da403;
                dr["入學年月"] = v.da404;
                dr["畢業年月"] = v.da405;
                dr["學位"] = v.da406;
                dr["指導教授"] = v.da407;
                dt.Rows.Add(dr);
            }

            return dt;
        }

        //經歷
        private DataTable GetDa5s(string Fno)
        {
            Dou.Models.DB.IModelEntity<F22cmmEmpDa5> modelDa5s = new Dou.Models.DB.ModelEntity<F22cmmEmpDa5>(_dbContext);
            var da5s = modelDa5s.GetAll().Where(a => a.Fno == Fno)
                        .OrderByDescending(a => a.da504);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("服務單位"));
            dt.Columns.Add(new DataColumn("職務"));
            dt.Columns.Add(new DataColumn("起始年月"));
            dt.Columns.Add(new DataColumn("結束年月"));
            dt.Columns.Add(new DataColumn("年資"));
            dt.Columns.Add(new DataColumn("其他備註"));

            foreach (var v in da5s)
            {
                string strJob = "";
                DateTime sJob = DateFormat.ToDate10(v.da504);
                DateTime eJob = DateFormat.ToDate10(v.da505);

                //迄今
                if (eJob == DateTime.MinValue)
                {
                    eJob = DateFormat.ToDate10(DateFormat.ToDate9(DateTime.Now));
                }

                if (sJob != DateTime.MinValue && eJob != DateTime.MinValue)
                {
                    strJob = Transform.Seniority(sJob, eJob);
                }

                DataRow dr = dt.NewRow();
                dr["服務單位"] = v.da501;
                dr["職務"] = v.da502;
                dr["起始年月"] = v.da504;
                dr["結束年月"] = v.da505;
                dr["年資"] = strJob;
                dr["其他備註"] = v.da506;
                dt.Rows.Add(dr);
            }

            return dt;
        }

        //家庭狀況
        private DataTable GetDa6s(string Fno)
        {
            Dou.Models.DB.IModelEntity<F22cmmEmpDa6> modelDa6s = new Dou.Models.DB.ModelEntity<F22cmmEmpDa6>(_dbContext);
            var da6s = modelDa6s.GetAll().Where(a => a.Fno == Fno);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("稱謂"));
            dt.Columns.Add(new DataColumn("姓名"));
            dt.Columns.Add(new DataColumn("生日"));
            dt.Columns.Add(new DataColumn("職務"));
            dt.Columns.Add(new DataColumn("任職公司或在學學校"));

            foreach (var v in da6s)
            {
                DataRow dr = dt.NewRow();
                dr["稱謂"] = v.da601;
                dr["姓名"] = v.da602;
                dr["生日"] = v.da603;
                dr["職務"] = v.da604;
                dr["任職公司或在學學校"] = v.da605;
                dt.Rows.Add(dr);
            }

            return dt;
        }

        //外語檢定
        private DataTable GetDa7s(string Fno)
        {
            Dou.Models.DB.IModelEntity<F22cmmEmpDa7> modelDa7s = new Dou.Models.DB.ModelEntity<F22cmmEmpDa7>(_dbContext);
            var da7s = modelDa7s.GetAll().Where(a => a.Fno == Fno);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("名稱"));
            dt.Columns.Add(new DataColumn("取得年月"));
            dt.Columns.Add(new DataColumn("測驗成績"));

            foreach (var v in da7s)
            {
                DataRow dr = dt.NewRow();
                dr["名稱"] = v.da701;
                dr["取得年月"] = v.da702;
                dr["測驗成績"] = v.da703;
                dt.Rows.Add(dr);
            }

            return dt;
        }

        //專業資格
        private DataTable GetDa8s(string Fno)
        {
            Dou.Models.DB.IModelEntity<F22cmmEmpDa8> modelDa8s = new Dou.Models.DB.ModelEntity<F22cmmEmpDa8>(_dbContext);
            var da8s = modelDa8s.GetAll().Where(a => a.Fno == Fno);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("名稱"));
            dt.Columns.Add(new DataColumn("取得年月"));
            dt.Columns.Add(new DataColumn("證照字號"));

            foreach (var v in da8s)
            {
                DataRow dr = dt.NewRow();
                dr["名稱"] = v.da801;
                dr["取得年月"] = v.da802;
                dr["證照字號"] = v.da803;
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}