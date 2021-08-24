using ClassLibraryAsync.Interface;
using ClassLibraryAsync.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibraryAsync.Handler
{
    /// <summary>
    /// 控制每個server的最大請求數量
    /// 確認每台SERVER的狀態(忙碌 OR 閒置)
    /// 忙碌就非同步等待
    /// </summary>
    public class MaxRequestHandler
    {
        /// 建立私有欄位
        private List<AllServerObj> allServiceData;

        /// <summary>
        /// 放入閒置中的主機的集合
        /// </summary>
        private List<AllServerObj> stayServiceData = new List<AllServerObj>();

        // 建立一個任務的集合 => 裝每個忙碌中要準備釋放資源的主機
        List<Task> tasks = new List<Task>();

        /// <summary>
        /// 建立建構式
        /// </summary>
        /// <param name="reports">各式服務</param>
        public MaxRequestHandler(List<AllServerObj> allServerObjs)
        {
            allServiceData = allServerObjs;
        }

        /// <summary>
        /// 定義隨機變數
        /// </summary>
        private Random Random = new Random();

        /// <summary>
        /// 假如每個service 的請求數量上限為3 主程式寄發30個請求 
        /// 第10個請求時 3台server都忙碌中 請求需要非同步等待
        /// </summary>
        /// <param name="reportObj">帶入服務的物件</param>
        /// <returns>response物件</returns>
        public async Task<Result> GetAsync(ReportObj reportObj)
        {            
            // 隨機取主機
            var num = Random.Next(0, allServiceData.Count);

            // 請求進來 計數+1    
            var count = Interlocked.Increment(ref allServiceData[num].RequestNum); 

            // 如果這台server的當前請求數 <= 他的最大請求數  (閒置中)
            if (count <= allServiceData[num].maxRequests)
            {
                stayServiceData.Add(allServiceData[num]); // 把閒置的主機加到另一個集合              
            }
            else // 主機請求超過上限 => 進行釋放資源的任務
            {
                tasks.Add(Task.Run(() =>
               {
                   Interlocked.Decrement(ref allServiceData[num].RequestNum);
               }));
            }
            
            // 如果閒置主機集合不為零 => 做接受請求的服務
            if (stayServiceData != null && stayServiceData.Count > 0)
            {
                return await stayServiceData[stayServiceData.Count - 1].reports.GetAsync(reportObj);
            }
            else if (stayServiceData.Count == 0) // 三台主機都忙碌中
            {            
                Task busyTasks = await Task.WhenAny(tasks); // 哪台忙碌主機先完成釋放資源就 接著做服務
                tasks.Remove(busyTasks);

                return await stayServiceData[stayServiceData.Count - 1].reports.GetAsync(reportObj);
            }

            return await stayServiceData[stayServiceData.Count - 1].reports.GetAsync(reportObj);
        }   
    }

    public class AllServerObj
    {

        /// <summary>
        /// 當前請求次數
        /// </summary>
        public int RequestNum = 0;

        /// <summary>
        /// 最大請求次數
        /// </summary>
        public int maxRequests;

        /// <summary>
        /// 服務方法
        /// </summary>
        public IReport reports;
    }
}

