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
        /// <summary>
        /// 建立semaphoreslim鎖
        /// </summary>
        private readonly SemaphoreSlim Semaphores;

        /// <summary>
        /// 存放semaphore的信號量
        /// </summary>
        private readonly ConcurrentQueue<IReport> Queue = new ConcurrentQueue<IReport>();      
        
        /// <summary>
        /// 建立建構式
        /// </summary>
        /// <param name="reports">各式服務</param>
        public MaxRequestHandler(List<IReport> allServerObjs, int maxRequetCount)
        {               
            // 信號量為所有主機的request最大總和
            Semaphores = new SemaphoreSlim(maxRequetCount* allServerObjs.Count);
            var random = new Random();

            // queue管理semaphoreslim的信號量
            for (int i = 1; i <= maxRequetCount*allServerObjs.Count; i++)
            {
                // 每一個請求隨機給主機 這邊指定主機
                var server = random.Next(0, allServerObjs.Count);             
                Queue.Enqueue(allServerObjs[server]);                
            }
        }
       
        /// <summary>
        /// 假如每個service 的請求數量上限為3 主程式寄發30個請求
        /// 第10個請求時 3台server都忙碌中 請求需要非同步等待
        /// </summary>
        /// <param name="reportObj">帶入服務的物件</param>
        /// <returns>response物件</returns>
        public async Task<Result> GetAsync(ReportObj reportObj)
        {        
            // 异步等待进入信号量，如果没有线程被授予对信号量的访问权限，则进入执行保护代码；否则此线程将在此处等待，直到信号量被释放为止
            await Semaphores.WaitAsync();
                               
            // 拿佇列開頭的資源 並釋放掉
            Queue.TryDequeue(out IReport res); // 釋放掉的請求

            try
            {                
                // 做服務!!! 印到CONSOLE上 做完後=> 再釋放資源           
                return await res.GetAsync(reportObj);       
            }       
            finally
            {
                // 把資源放回佇列上
                Queue.Enqueue(res);

                // 增加semaphoreSlim內的信號空間 RELEASE +1 池+1    
                Semaphores.Release();
            }           
        }
    }
}