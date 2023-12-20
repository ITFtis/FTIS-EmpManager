using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DouImp
{
    public class _ReportClass
    {
    }

    public class ReportClass
    {
        public string _errorMessage = "";
        public System.Data.Entity.DbContext _dbContext = null;

        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
        }

        /// <summary>
        /// 報表檔案格式
        /// </summary>
        public Dictionary<string, string> RType = new Dictionary<string, string>()
        {
            {".docx","WORDOPENXML" },
            {".xlsx","EXCELOPENXML" },
        };
    }
}