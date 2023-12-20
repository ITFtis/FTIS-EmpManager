using DouImp._core;
using FtisHelperV2.DB.Model;
using Microsoft.Reporting.WebForms;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace DouImp._report
{
    public class Rpt_EmpPPtPromote : ReportClass
    {
        /// <summary>
        /// 匯出晉升員工
        /// </summary>
        /// <param name="fnos"></param>
        /// <param name="ext">副檔名(包含.docx)</param>
        /// <returns></returns>
        public string Export(List<string> fnos, string ext)
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
                reportViewer.LocalReport.ReportPath = System.Web.HttpContext.Current.Server.MapPath("~/Report/EmpPPtPromote/Master.rdlc");

                //主表
                Dou.Models.DB.IModelEntity<F22cmmEmpData> modelData = new Dou.Models.DB.ModelEntity<F22cmmEmpData>(_dbContext);                
                var dtData = modelData.GetAll().Where(a => fnos.Contains(a.Fno)).ToList();

                if (dtData.Count == 0)
                    return "晉升員工-無資料匯出";

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

                string folder = FileHelper.GetFileFolder(Code.TempUploadFile.員工晉升);


                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string fileName = "員工晉升_" + DateFormat.ToDate1(DateTime.Now) + "_" + Guid.NewGuid().ToString().Substring(0, 5) + ext;  //"ext=.docx"
                path = folder + fileName;

                FileStream fs = new FileStream(path,
                   FileMode.Create);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }
            catch (Exception ex)
            {
                _errorMessage = "匯出員工晉升失敗" + "\n" + ex.InnerException + ex.Message + "\n" + ex.StackTrace;
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

                //學歷
                DataTable dtDa4s = GetDa4s(Fno);
                e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Sub2SourceDa4s", dtDa4s));

                //經歷(會外)
                DataTable dtDa5s = GetDa5s(Fno);
                e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Sub3SourceDa5s", dtDa5s));

                //經歷(會內)
                DataTable dtDa5s_in = GetDa5s_in(Fno);
                e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Sub3SourceDa5s_in", dtDa5s_in));
            }
        }

        //主表
        private DataTable GetEmpData(string Fno)
        {
            Dou.Models.DB.IModelEntity<F22cmmEmpData> modelData = new Dou.Models.DB.ModelEntity<F22cmmEmpData>(_dbContext);
            var data = modelData.GetAll().Where(a => a.Fno == Fno).First();

            DataTable dt = new DataTable();
            //dt.Columns.Add(new DataColumn("xxxx"));
            dt.Columns.Add(new DataColumn("姓名中"));
            dt.Columns.Add(new DataColumn("部門"));
            dt.Columns.Add(new DataColumn("職稱"));
            dt.Columns.Add(new DataColumn("到職日期"));            

            DataRow dr = dt.NewRow();
            //dr["xxxx"] = "oooooo";
            dr["姓名中"] = data.Name;
            dr["部門"] = FtisHelperV2.DB.Helpe.Department.GetDepartment(data.DCode) == null ? "" : FtisHelperV2.DB.Helpe.Department.GetDepartment(data.DCode).DName;
            List<string> titles = new List<string>();
            //titles.Add("財團法人台灣產業服務基金會");
            //部門(綠色技術發展中心)
            //titles.Add(FtisHelperV2.DB.Helpe.Department.GetDepartment(data.DCode) == null ? "" : FtisHelperV2.DB.Helpe.Department.GetDepartment(data.DCode).DName);
            //職稱(高級工程師)
            titles.Add(FtisHelperV2.DB.Helper.GetEmployeeTitle(Fno) == null ? "" : FtisHelperV2.DB.Helper.GetEmployeeTitle(Fno).Title);
            dr["職稱"] = string.Join(" ", titles);
            dr["到職日期"] = DateFormat.ToTwDate3_2(data.AD);                                   

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
            dt.Columns.Add(new DataColumn("科系"));
            dt.Columns.Add(new DataColumn("入學年月"));
            dt.Columns.Add(new DataColumn("畢業年月"));
            dt.Columns.Add(new DataColumn("學位"));

            foreach (var v in da4s)
            {
                DataRow dr = dt.NewRow();
                //dr4["xxxx"] = "oooooo";
                dr["學校"] = v.da401;
                dr["科系"] = v.da403;
                dr["入學年月"] = DateFormat.ToTwDate3_2(v.da404);
                dr["畢業年月"] = DateFormat.ToTwDate3_2(v.da405);
                dr["學位"] = v.da406;
                dt.Rows.Add(dr);
            }

            //沒資料顯示
            if (dt.Rows.Count == 0)
            {
                DataRow dr = dt.NewRow();
                foreach (DataColumn col in dt.Columns)
                {
                    dr[col] = "";
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        //經歷(會外)
        private DataTable GetDa5s(string Fno)
        {
            List<string> outs = new List<string>() {
                "%台灣產業服務基金會%", "%台基寰宇%"
            };

            Dou.Models.DB.IModelEntity<F22cmmEmpDa5> modelDa5s = new Dou.Models.DB.ModelEntity<F22cmmEmpDa5>(_dbContext);
            var da5s = modelDa5s.GetAll().Where(a => a.Fno == Fno)
                            .Where(a => !outs.Any(b => DbFunctions.Like(a.da501, b)))
                            .OrderByDescending(a => a.da504);  //會外

            var zz = da5s.ToList();

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("服務單位"));
            dt.Columns.Add(new DataColumn("職務"));
            dt.Columns.Add(new DataColumn("起始年月"));
            dt.Columns.Add(new DataColumn("結束年月"));
            dt.Columns.Add(new DataColumn("年資"));

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
                dr["起始年月"] = DateFormat.ToTwDate3_2(v.da504);
                dr["結束年月"] = DateFormat.ToTwDate3_2(v.da505);
                dr["年資"] = strJob;
                dt.Rows.Add(dr);
            }

            //沒資料顯示
            if (dt.Rows.Count == 0)
            {
                DataRow dr = dt.NewRow();
                foreach (DataColumn col in dt.Columns)
                {
                    dr[col] = "";
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        //經歷(會內)
        private DataTable GetDa5s_in(string Fno)
        {
            List<string> outs = new List<string>() {
                "%台灣產業服務基金會%", "%台基寰宇%"
            };

            Dou.Models.DB.IModelEntity<F22cmmEmpDa5> modelDa5s = new Dou.Models.DB.ModelEntity<F22cmmEmpDa5>(_dbContext);
            var da5s = modelDa5s.GetAll().Where(a => a.Fno == Fno)
                            .Where(a => outs.Any(b => DbFunctions.Like(a.da501, b)))
                            .OrderByDescending(a => a.da504);  //會內

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("服務單位"));
            dt.Columns.Add(new DataColumn("職務"));
            dt.Columns.Add(new DataColumn("起始年月"));
            dt.Columns.Add(new DataColumn("結束年月"));
            dt.Columns.Add(new DataColumn("年資"));

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

                Dou.Models.DB.IModelEntity<F22cmmEmpData> modelData = new Dou.Models.DB.ModelEntity<F22cmmEmpData>(_dbContext);
                var data = modelData.GetAll().Where(a => a.Fno == Fno).First();

                List<string> deps = new List<string>();
                deps.Add(FtisHelperV2.DB.Helpe.Department.GetDepartment(data.DCode) == null ? "" : FtisHelperV2.DB.Helpe.Department.GetDepartment(data.DCode).DName);
                string dep = string.Join(" ", deps);

                DataRow dr = dt.NewRow();
                dr["服務單位"] = dep; //v.da501;
                dr["職務"] = v.da502.Replace(dep, "");
                dr["起始年月"] = DateFormat.ToTwDate3_2(v.da504);
                dr["結束年月"] = DateFormat.ToTwDate3_2(v.da505);
                dr["年資"] = strJob;
                dt.Rows.Add(dr);
            }

            //沒資料顯示
            if (dt.Rows.Count == 0)
            {
                DataRow dr = dt.NewRow();
                foreach (DataColumn col in dt.Columns)
                {
                    dr[col] = "";
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}