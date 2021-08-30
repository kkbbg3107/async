using ClassLibraryAsync.Interface;
using ClassLibraryAsync.Model;
using System;
using System.Collections.Concurrent;
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
    public class MaxRequestHandler : IReport
    {
        // 最大請求次數
        private int MaxRequestCount;

        /// 建立私有服務欄位
        private List<IReport> allServiceData;

        /// <summary>
        /// 建立semaphoreslim鎖
        /// </summary>
        private SemaphoreSlim semaphores;

        ///// <summary>
        ///// 執行緒安全字典 key:server的索引 value:server的請求次數
        ///// </summary>
        //private ConcurrentDictionary<int, int> dict_Thread = new ConcurrentDictionary<int, int>();

        /// <summary>
        /// 存正在忙線的主機
        /// </summary>
        private ConcurrentQueue<int> queue = new ConcurrentQueue<int>(); 

        /// <summary>
        /// 初始請求數
        /// </summary>
        private int requests = 0;

        /// <summary>
        /// 建立建構式
        /// </summary>
        /// <param name="reports">各式服務</param>
        public MaxRequestHandler(List<IReport> allServerObjs, int maxRequetCount)
        {
            allServiceData = allServerObjs;
            MaxRequestCount = maxRequetCount;
            semaphores = new SemaphoreSlim(maxRequetCount);
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
            var result = new Result();

            await semaphores.WaitAsync();

            // 拿到閒置主機
            var idleIndex = GetIdleServiceIndex();
            
            // 异步等待进入信号量，如果没有线程被授予对信号量的访问权限，则进入执行保护代码；否则此线程将在此处等待，直到信号量被释放为止                               
            try
            {
                // 把主機索引存去queue
                //queue.Enqueue(idleIndex);

                // 做服務!!! 印到CONSOLE上 做完後=> 在釋放資源           
                result = await allServiceData[idleIndex].GetAsync(reportObj);

                // 把主機從queue裡面移除 queue
                //queue.TryDequeue(out int res);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            };

            // 增加semaphoreSlim內的信號空間 RELEASE +1 池+1    
            semaphores.Release();

            return result;                  
        }

        /// <summary>
        /// 取得誰是閒置主機
        /// </summary>
        /// <returns>主機編號</returns>
        private int GetIdleServiceIndex()
        {
            // 只要有閒置
            if (queue.Count != allServiceData.Count)
            {
                // 找閒置主機
                return getidleServer();
            }
            else // 如果沒有閒置server 看哪台先完成就先回傳哪台 WhenAny => 需要找到忙碌到閒置的任務
            {
                //var result = 0;

                Task t = Task.Run(() =>
                {
                    // 都在忙碌
                    while (queue.Count != allServiceData.Count)
                    {
                        return;
                    }

                });

                t.Wait();

                // 回傳主機編號
                return getidleServer();
            }
        }

        private int getidleServer()
        {
            while (true)
            {
                var server = Random.Next(0, allServiceData.Count);

                if (!queue.Contains(server))
                {
                    return server;
                }
            }
        }
    }
}