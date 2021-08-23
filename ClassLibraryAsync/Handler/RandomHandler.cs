using ClassLibraryAsync.Interface;
using ClassLibraryAsync.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryAsync.Handler
{
    /// <summary>
    /// 建立服務的容器
    /// </summary>
    public class RandomHandler
    {
        /// <summary>
        /// 建立私有欄位=>建構事參數
        /// </summary>
        private IReport[] Report;

        /// <summary>
        /// 定義隨機變數
        /// </summary>
        private Random Random = new Random();

        /// <summary>
        /// 建立建構式
        /// </summary>
        /// <param name="report">各式服務</param>
        public RandomHandler(IReport[] report)
        {
            Report = report;
        }
    
        /// <summary>
        /// 以亂數隨機分派服務給request
        /// </summary>
        /// <param name="reportObj">資料包物件</param>
        /// <returns>response結果</returns>
        public async Task<Result> GetAsync(ReportObj reportObj)
        {
            var num = Random.Next(0, Report.Length);
            return await Report[num].GetAsync(reportObj);
        }
    }
}
