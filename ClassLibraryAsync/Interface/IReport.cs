using ClassLibraryAsync.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryAsync.Interface
{
    /// <summary>
    /// 抽換各式service
    /// </summary>
    public interface IReport
    {
        /// <summary>
        /// 執行服務的方法
        /// </summary>
        /// <param name="reportObj">帶入服務的物件</param>
        /// <returns>response資料</returns>
        Task<Result> GetAsync(ReportObj reportObj);
    }
}
