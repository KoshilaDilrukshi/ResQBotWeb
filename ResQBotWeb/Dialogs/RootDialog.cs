using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using ResQDbBotWeb.Controllers.ResQ;
using System.Collections.Generic;
using ResQBotWeb.Models.ResQ.Business;
using ResQBotWeb.Controllers.ResQ;
using System.Text;
using System.Linq;

namespace ResQBotWeb.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private string LoginId;
        private string UserId;
        private string BandId;
        private static float DR;
        private static string DR2;
        private static string DefectRate;
        private static List<TestBusiness> returnData;        
        private static List<DefectBusiness> returnDefectRate;
        public int dataCount;
       
        

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }



        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            UserId = "";
            BandId = "";

            
            var activity = await result as Activity;

            if (activity.Text.ToLower().Contains("hi") || activity.Text.ToLower().Contains("hello"))
            {
                LoginId = activity.From.Id;
                await context.PostAsync("hi.."+LoginId+ "\U0001F642"+ "\U0001F642"+ "\U0001F642");

                // return our reply to the user
                await context.PostAsync($"Please enter the User Id:"+ "\U0001F642");
                context.Wait(getUserIdAsync);
            }
           
        }

        private  async Task getUserIdAsync(IDialogContext context, IAwaitable<object> userid)
        {
            var activity = await userid as Activity;
            UserId = activity.Text;

            //create the object of UserIdController to get  data from batabase
            UserIdController userIdCon = new UserIdController();
            returnData = userIdCon.GetUserId(UserId);         // pass value of UserId as a parameter
            dataCount = userIdCon.getItems();                 //get list of Tenant id, Comapny Id, Site Id and all Band Ids & Names     

            await context.PostAsync(String.Format("User Id = {0}", UserId));    //show entered User Id            

                     
            if (dataCount == 0) {

                //Validate User Id
                await context.PostAsync($"Incorrect User Id..! Please re-enter. \U0001F61F \U0001F61F \U0001F61F");
                context.Wait(getUserIdAsync);
            }
            else {

                //  Display  all data in a card view
                var userData = context.MakeMessage();
                var bandsAttachment = bandsCard(returnData);
                userData.Attachments.Add(bandsAttachment);
                await context.PostAsync(userData);
                context.Wait(getBandDetailsAsync);
            }

        }

        private async Task getBandDetailsAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            BandId = activity.Text;

            string TenantId = returnData[0].TenantId.ToString();
            string CompanyId = returnData[0].CompanyId.ToString();
            string SiteId = returnData[0].SiteId.ToString();
           // string BandId = returnData[0].BandId.ToString();


            //create the object of DefectRateController to get  data from batabase
            DefectRateController defectrate = new DefectRateController();
            returnDefectRate = defectrate.getDefectRate(TenantId,CompanyId,SiteId,BandId);  //pass parameters
            
            if (returnDefectRate[0].DefectRate.ToString() !="")
            {
                DefectRate = returnDefectRate[0].DefectRate.ToString();

                var selectedcard = await result;
                var message = context.MakeMessage();

                //calling the card to show Defect rate
                var attachment = defectrateCard();
                message.Attachments.Add(attachment);
                await context.PostAsync(message);
                context.Wait(MessageReceivedAsync);
                //context.Wait(getBandDetailsAsync);

                

            }
            else
            {
                // suggest retry if list doesn't contain any useful values
                await context.PostAsync($"No available band details for " + SiteId + "...!  Please try again. \U0001F61F \U0001F61F \U0001F61F");
                context.Wait(getBandDetailsAsync);
            }
        }

        private static Attachment bandsCard(List<TestBusiness> bands)
        {
            string TenantId = returnData[0].TenantId.ToString();
            string CompanyId = returnData[0].CompanyId.ToString();
            string SiteId = returnData[0].SiteId.ToString();            

            var bandsCardActions = new List<CardAction>();

            foreach (TestBusiness band in bands)
            {
                //get all Band Names as buttons
                bandsCardActions.Add(new CardAction(ActionTypes.ImBack, band.BandName, value: band.BandId));
            }

            var searchField = new ThumbnailCard
            {
                                
                Title = $"<u><font color=\"#800000\">These are the Bands that are available for you... </font></u>",
                Images = new List<CardImage> { new CardImage("http://www.hirdaramani.com/Modify/Logo.png") },
                Subtitle = "Please select the Band to get status..",
                Text = "<b>Tenant Id     :-     </b>" + TenantId + "</br>\n\n<b>Company Id   :-      </b>" + CompanyId + "<br><b>Site Id       :-   </b>" + SiteId,
                Buttons = bandsCardActions,
            };
            return searchField.ToAttachment();
        }

        private static Attachment defectrateCard()
        {
            //truncate the value of Defect Rate
            DR = float.Parse(returnDefectRate[0].DefectRate.ToString());        
            DR2 = DR.ToString("##,##0.##");

            var defectDetails = new ThumbnailCard {

                Title = "<u><font color=\"#800000\"> ResQBot</font></u>",
                Images = new List<CardImage> { new CardImage("http://www.hirdaramani.com/Modify/Logo.png") },
                Text = "<font color=\"#11b92f\"><b>Defect Rate         :-      </b></font>" + DR2+"%",
                Buttons = new List<CardAction> {                
                new CardAction(ActionTypes.ImBack,"Thank you",value:"\U0001F642") },

            };
            return defectDetails.ToAttachment();
        }
        
    }
}