using ClassLibraryAsync;
using ClassLibraryAsync.Handler;
using ClassLibraryAsync.Interface;
using ClassLibraryAsync.Model;
using ClassLibraryAsync.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace webApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // 服務帶入參數
            ReportObj products = new ReportObj()
            {
                dtno = 5493,
                ftno = 0,
                all = new Assign { AssignID = "00878", AssignDate = "20200101-99999999", DTOrder = 2, MTPeriod = 2 },
                keyMap = string.Empty,
                assign = string.Empty,
            };
            
            List<AllServerObj> test = new List<AllServerObj>()
            {
                new AllServerObj(){maxRequests = 3, reports = new ReportServices("1")},
                new AllServerObj(){maxRequests = 3, reports = new ReportServices("2")},
                new AllServerObj(){maxRequests = 3, reports = new ReportServices("3")},
            };

            MaxRequestHandler maxRequestHandler = new MaxRequestHandler(test);

            // 釋放初始要求數目
            maxRequestHandler.ReleaseInitialSource();

            // 發送30次請求 要有隨機分配請求給三個service 超過server請求次數限制後 非同步等待請求釋放
            for (int i = 1; i <= 15; i++)
            {
                var i1 = i;
                _ = Task.Run(async () =>
                {
                    try
                    {
                        // 這邊碰到await 就進行下一次request
                        var result = await maxRequestHandler.GetAsync(products, i1);
                        Console.WriteLine(result.signature);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex);
                    }                       
                });               
            }
            Console.ReadLine();
        }
    }
}
