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
            
            List<IReport> server = new List<IReport>()
            {
                new FakeServices(10, 1, "Server1"),
                new FakeServices(10, 2, "Server2"),
                new FakeServices(10, 3, "Server3"),
            };

            MaxRequestHandler maxRequestHandler = new MaxRequestHandler(server, 5);

            // 發送50次請求 要有隨機分配請求給三個service 超過server請求次數限制後 非同步等待請求釋放
            for (int i = 1; i <= 50; i++)
            {                              
                _ = Task.Run(async () =>
                {                               
                    try
                    {                                        
                        // 這邊碰到await 就進行下一次request
                        var result = await maxRequestHandler.GetAsync(products);
                        
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
