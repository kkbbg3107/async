using ClassLibraryAsync.Interface;
using ClassLibraryAsync.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibraryAsync.Services
{
    public class ThirdServices : IReport
    {

        public ThirdServices(int requestMax, int requestSecond, string name)
        {
            RequestMax = requestMax;
            RequestSecond = requestSecond;
            Name = name;
        }

        public int RequestNum = 0;
        public int RequestMax { get; set; }
        public int RequestSecond { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// 報表服務
        /// </summary>
        /// <param name="reportObj">假物件</param>
        /// <returns>服務請求總數</returns>
        public async Task<Result> GetAsync(ReportObj reportObj)
        {
            //var count = Interlocked.Increment(ref RequestNum); // 請求進來 計數+1                               

            //if (count > RequestMax) // 當前次數多於最大請求數
            //{
            //    throw new Exception("請求超過最大次數上限");
            //}

            //// 碰到await 返回呼叫端等 await中的方法執行完畢 從該處繼續執行
            //await Task.Delay(RequestSecond * 1000);

            return new Result
            {
                signature = Name,
            };
        }
    }
}
