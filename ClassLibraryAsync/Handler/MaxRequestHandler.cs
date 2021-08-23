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
        /// 假如每個service 的請求數量上限為5 主程式寄發100個請求 
        /// 第16個請求時 3台server都忙碌中 請求需要非同步等待
        /// </summary>
        /// <param name="reportObj">帶入服務的物件</param>
        /// <returns>response物件</returns>
        public  Task<Result> GetAsync(ReportObj reportObj)
        {
            // 隨機取主機
            var num = Random.Next(0, allServiceData.Count);
            
            // 請求進來 計數+1    
            var count = Interlocked.Increment(ref allServiceData[num].RequestNum);    
                        
            // 如果這台server的當前請求數 > 他的最大請求數 
            if (count > allServiceData[num].maxRequests)
            {
                // 狀態變為忙碌中 => 滿載 => 下一個進來要等待 => 等待完釋放一個RequestNum
                allServiceData[num].IsBusy = true;

              
                Interlocked.Decrement(ref allServiceData[num].RequestNum); // (非同步等待)釋放一個RequestsNum
                
                                               
                allServiceData[num].IsBusy = false; // 等待完變回閒置中     
            }            

            return  allServiceData[num].reports.GetAsync(reportObj);
        }
    }

    public class AllServerObj
    {
        /// <summary>
        ///賦予狀態 忙碌 OR 閒置 
        /// </summary>
        public bool IsBusy;

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
