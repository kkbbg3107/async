using ClassLibraryAsync.Interface;
using ClassLibraryAsync.Model;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryAsync.Services
{
    public class ReportServices : IReport
    {       
        private static readonly HttpClient client = new HttpClient();

        private static string UrlPost = "http://192.168.99.95:5000/api/customreport";

        private string Name;

        public ReportServices(string name)
        {
            Name = name;
        }
        /// <summary>
        /// 被呼叫的服務 串接URL
        /// </summary>
        /// <param name="reportObj">帶入服務的參數</param>
        /// <returns>response物件</returns>
        public async Task<Result> GetAsync(ReportObj reportObj)
        {                       
            string json = JsonConvert.SerializeObject(reportObj);
            //HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            //HttpResponseMessage response = await client.PostAsync(UrlPost, content);
            //response.EnsureSuccessStatusCode();
            //string responseBody = await response.Content.ReadAsStringAsync();
            //var result = JsonConvert.DeserializeObject<Result>(responseBody);
            //result.result = "reportService\n";
            Result result = new Result();
            result.signature = Name;
            return result;
        }
    }
}
