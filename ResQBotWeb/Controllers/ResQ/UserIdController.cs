using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using ResQBotWeb.Models.ResQ.Business;

using Newtonsoft.Json;
using System.Data;
using System;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using System.Web.Configuration;

namespace ResQDbBotWeb.Controllers.ResQ
{
    //[BotAuthentication(MicrosoftAppId = "_appIdValue_", MicrosoftAppPassword = "_passwordValue_")]

    public class UserIdController : ApiController
    {
        //[HttpGet]

        List<TestBusiness> returnList = new List<TestBusiness>();
        string URL_UserId = WebConfigurationManager.AppSettings["UrlCommen"];

        public List<TestBusiness> GetUserId(string UserId)
        {
            using (WebClient webClient = new WebClient())
            {
                WebClient n = new WebClient();

                //consume the url & pass all details into data
                //var data = n.DownloadString(@"http://essmobileapiqa.azurewebsites.net/BOT/GetUserWiseTransBandDetailsAPI?UserId=" + UserId);
               
                var data = n.DownloadString(URL_UserId+ @"BOT/GetUserWiseTransBandDetailsAPI?UserId=" + UserId);
                

                //deserialize all data               
                JavaScriptSerializer ser = new JavaScriptSerializer();
                List<TestBusiness> lst = ser.Deserialize<List<TestBusiness>>(data);  // get as a list
                               

                returnList = lst;
                return returnList;   // return the list
            }

        }

        public int getItems()
        {
            return returnList.Count();
        }

    }

}
