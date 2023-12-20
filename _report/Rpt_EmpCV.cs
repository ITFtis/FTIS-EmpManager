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
    //履歷表
    public class Rpt_EmpCV : ReportClass
    {
        /// <summary>
        /// 匯出履歷表
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
                reportViewer.LocalReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath("~/Report/EmpCV/Master.rdlc");

                //參數設定(Fno)
                ReportParameter p1 = new ReportParameter("Fno", fno);
                reportViewer.LocalReport.SetParameters(new ReportParameter[] { p1 });

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

                string folder = FileHelper.GetFileFolder(Code.TempUploadFile.履歷表);


                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string empName = dtData.Rows[0]["姓名中"].ToString();
                string fileName = "履歷表_" + empName + "_" + DateFormat.ToDate1(DateTime.Now) + "_" + Guid.NewGuid().ToString().Substring(0, 5) + ext;  //"ext=.docx"
                path = folder + fileName;

                FileStream fs = new FileStream(path,
                   FileMode.Create);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }
            catch (Exception ex)
            {
                _errorMessage = "匯出履歷表失敗" + "\n" + ex.InnerException + ex.Message + "\n" + ex.StackTrace;
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
            else if (e.ReportPath == "Sub3Da5s")
            {
                //經歷
                DataTable dtDa5s = GetDa5s(Fno);
                e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Sub3SourceDa5s", dtDa5s));
            }
            else if (e.ReportPath == "Sub7Da9s")
            {
                //專業資格
                DataTable dtDa9s = GetDa9s(Fno);
                e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Sub7SourceDa9s", dtDa9s));
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
            dt.Columns.Add(new DataColumn("專長"));
            dt.Columns.Add(new DataColumn("學歷F"));
            dt.Columns.Add(new DataColumn("資格F"));
            dt.Columns.Add(new DataColumn("最後更新日期"));

            DataRow dr = dt.NewRow();
            //dr["xxxx"] = "oooooo";
            dr["姓名中"] = data.Name;
            dr["姓名英"] = data.En_Name;
            List<string> titles = new List<string>();
            titles.Add("財團法人台灣產業服務基金會");
            titles.Add(FtisHelperV2.DB.Helpe.Department.GetDepartment(data.DCode) == null ? "" : FtisHelperV2.DB.Helpe.Department.GetDepartment(data.DCode).DName);
            titles.Add(FtisHelperV2.DB.Helper.GetEmployeeTitle(Fno) == null ? "" : FtisHelperV2.DB.Helper.GetEmployeeTitle(Fno).Title);
            dr["職稱"] = string.Join(" ", titles);
            dr["到職日期"] = DateFormat.ToDate4(data.AD);
            dr["性別"] = data.Sex;
            dr["Email"] = data.EMail;
            dr["最後更新日期"] = data.UpdateTime != null ? DateFormat.ToTwDate4((DateTime)data.UpdateTime) : "";

            if (z_da1s.Count() > 0)
            {
                var da1s = z_da1s.First();
                dr["出生日期"] = DateFormat.ToTwDate2((DateTime)da1s.da03);
                dr["出生地"] = da1s.da04;
                dr["身分證字號"] = da1s.da05;
                dr["婚姻"] = da1s.da06a;
                dr["身高"] = da1s.da06;
                dr["體重"] = da1s.da07;
                dr["血型"] = da1s.da08;
                dr["戶籍地址"] = da1s.da10;
                dr["戶籍電話"] = da1s.da11;
                dr["通訊地址"] = da1s.da13;
                dr["住家電話"] = da1s.da14;
                dr["行動電話"] = da1s.da15;
                dr["緊急聯絡人1姓名"] = da1s.da17;
                dr["緊急聯絡人1關係"] = da1s.da18;
                dr["緊急聯絡人1電話"] = da1s.da19;
                dr["緊急聯絡人2姓名"] = da1s.da20;
                dr["緊急聯絡人2關係"] = da1s.da21;
                dr["緊急聯絡人2電話"] = da1s.da22;
                dr["專長"] = da1s.da24;
            }

            Dou.Models.DB.IModelEntity<F22cmmEmpDa4> modelDa4s = new Dou.Models.DB.ModelEntity<F22cmmEmpDa4>(_dbContext);
            var z_da4s = modelDa4s.GetAll().Where(a => a.Fno == Fno).ToList();

            if (z_da4s.Count > 0)
            {
                var da4s = z_da4s.OrderByDescending(a => a.da404).First();
                List<string> strs = new List<string>() { da4s.da401, da4s.da403, da4s.da406 };
                dr["學歷F"] = string.Join(" ", strs);
            }

            Dou.Models.DB.IModelEntity<F22cmmEmpDa8> modelDa8s = new Dou.Models.DB.ModelEntity<F22cmmEmpDa8>(_dbContext);
            var z_da8s = modelDa8s.GetAll().Where(a => a.Fno == Fno).ToList();

            if (z_da8s.Count > 0)
            {
                dr["資格F"] = string.Join("\n", z_da8s.Select(a => a.da801));
            }

            dt.Rows.Add(dr);

            return dt;
        }

        //經歷
        private DataTable GetDa5s(string Fno)
        {
            Dou.Models.DB.IModelEntity<F22cmmEmpDa5> modelDa5s = new Dou.Models.DB.ModelEntity<F22cmmEmpDa5>(_dbContext);
            var da5s = modelDa5s.GetAll().Where(a => a.Fno == Fno)
                        .OrderByDescending(a => a.da504).ToList();

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("服務單位"));
            dt.Columns.Add(new DataColumn("職務"));
            dt.Columns.Add(new DataColumn("起始年月"));
            dt.Columns.Add(new DataColumn("結束年月"));
            dt.Columns.Add(new DataColumn("年資"));
            dt.Columns.Add(new DataColumn("其他備註"));
            dt.Columns.Add(new DataColumn("工作內容"));

            foreach (var v in da5s.OrderByDescending(a => a.da504))
            {
                List<string> names = new List<string>() { v.da501, v.da502 };
                string strWorkDate = DateFormat.ToTwDate3(v.da504) + "~" + DateFormat.ToTwDate3(v.da505);

                //(1)只有第一筆需要顯示names(服務單位, 職稱)
                DataRow dr = dt.NewRow();
                dr["起始年月"] = strWorkDate;
                dr["工作內容"] = string.Join(" ", names);

                dt.Rows.Add(dr);

                //(2)固定行高，須將工作內容拆成row(\r\n)
                List<string> filters = new List<string>() { "待補" };

                string[] strs = v.da507.Split(new[] { "\r\n" }, StringSplitOptions.None);
                foreach (string str in strs)
                {
                    //排除文字(ex.待補)
                    if (filters.Contains(str))
                        continue;

                    dr = dt.NewRow();
                    dr["起始年月"] = strWorkDate;
                    dr["工作內容"] = str;

                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }

        //著作
        private DataTable GetDa9s(string Fno)
        {
            Dou.Models.DB.IModelEntity<F22cmmEmpDa9> modelDa9s = new Dou.Models.DB.ModelEntity<F22cmmEmpDa9>(_dbContext);
            var da9s = modelDa9s.GetAll().Where(a => a.Fno == Fno);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("名稱"));
            dt.Columns.Add(new DataColumn("簡介"));

            foreach (var v in da9s)
            {
                DataRow dr = dt.NewRow();
                dr["名稱"] = v.da901;
                dr["簡介"] = v.da902;
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}