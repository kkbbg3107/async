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
    public class FakeServices : IReport
    {
        /// <summary>
        /// 當前請求次數 
        /// </summary>
        private int RequestNum = 0;

        public FakeServices(int requestMax, int requestSecond, string name)
        {
            RequestMax = requestMax;
            RequestSecond = requestSecond;
            Name = name;
        }

        /// <summary>
        /// 請求最大總數限制
        /// </summary>
        private int RequestMax;

        /// <summary>
        /// 請求等待秒數
        /// </summary>
        private int RequestSecond;

        /// <summary>
        /// 給予一個signature
        /// </summary>
        private string Name;

        /// <summary>
        /// 報表服務
        /// </summary>
        /// <param name="reportObj">假物件</param>
        /// <returns>服務請求總數</returns>
        public async Task<Result> GetAsync(ReportObj reportObj)
        {
            var count = Interlocked.Increment(ref RequestNum); // 請求進來 計數+1                               

            if (count > RequestMax) // 當前次數多於最大請求數
            {
                throw new Exception("請求超過最大次數上限");
            }

            var result = new Result()
            {
                signature = Name,
            };

            // 碰到await 返回呼叫端等 await中的方法執行完畢 從該處繼續執行
            await Task.Delay(RequestSecond * 1000);

            return result;
        }
    }
}
