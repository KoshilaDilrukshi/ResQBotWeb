using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ResQBotWeb.Models.ResQ.Business;
using System.Data;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Web.Configuration;

namespace ResQBotWeb.Controllers.ResQ
{
    public class DefectRateController : ApiController
    {
       
        List<DefectBusiness> returnList1 = new List<DefectBusiness>();
        string URL_DefectRate = WebConfigurationManager.AppSettings["UrlCommen"];

        public List<DefectBusiness> getDefectRate(string TenantId, string CompanyId, string SiteId, string BandId)
        {
            using (WebClient webClient = new WebClient())
            {
                WebClient n = new WebClient();

                //consume the url & pass all details into data
                // var data = n.DownloadString(@"http://essmobileapiqa.azurewebsites.net/BOT/GetBandWiseDefectRateAPI?TenantId=" + TenantId + "&CompanyId=" + CompanyId + "&SiteId=" + SiteId + "&BandId=" + BandId);

                var data = n.DownloadString(@URL_DefectRate + @"BOT/GetBandWiseDefectRateAPI?TenantId=" + TenantId + "&CompanyId=" + CompanyId + "&SiteId=" + SiteId + "&BandId=" + BandId);

                JavaScriptSerializer ser = new JavaScriptSerializer();
                List<DefectBusiness> item1 = ser.Deserialize<List<DefectBusiness>>(data);

               
                returnList1 = item1;
                
            }
            return returnList1;
        }
    }
}
