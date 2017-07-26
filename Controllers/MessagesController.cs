using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using InsuranceBoT;
using InsuranceBOT;
using Autofac;
using Microsoft.Bot.Builder.Autofac.Base;
using Microsoft.Bot.Builder.Dialogs.Internals;
using System.Net.Http.Headers;
using System.Xml.Linq;
using System.Diagnostics;

namespace InsuranceBOTDemo
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        /// 
        string TextTranslatorApiKey = "646ac41b10124fe18b84f9272b3ef4bb";
        string targetLang = "en";

        internal static IDialog<LossForm> BuildInsuranceDialog()
        {
            return Chain.From(() => FormDialog.FromForm(LossForm.BuildForm));
        }

        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {          
            if (activity.Type == ActivityTypes.Message)
            {
                var input = activity.Text;
                Task.Run(async () =>
                {
                    var accessToken = await GetAuthenticationToken(TextTranslatorApiKey);
                    var output = await TranslateText(input, targetLang, accessToken);
                    Debug.WriteLine(output);
                    activity.Text = output;
                }).Wait();

                /// without LUIS 
                /// await Conversation.SendAsync(activity, BuildInsuranceDialog);
                /// 
                /// with LUIS
                await Conversation.SendAsync(activity, () => new BuildInsuranceLUISDialog());
                return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        //For TextTranslator
        static async Task<string> GetAuthenticationToken(string key)
        {
            string endpoint = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
                var response = await client.PostAsync(endpoint, null);
                var token = await response.Content.ReadAsStringAsync();
                return token;
            }
        }

        static async Task<string> TranslateText(string inputText, string language, string accessToken)
        {
            string url = "http://api.microsofttranslator.com/v2/Http.svc/Translate";
            //turn category using neural network
            string query = $"?text={System.Net.WebUtility.UrlEncode(inputText)}&to={language}&contentType=text/plain&category=generalnn";

            string urlDetectLang = "http://api.microsofttranslator.com/v2/Http.svc/detect";
            //turn category using neural network
            string queryDetectLang = $"?text={System.Net.WebUtility.UrlEncode(inputText)}&contentType=text/plain";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                //Lets detect the user input lang
                var responseDetectLang = await client.GetAsync(urlDetectLang + queryDetectLang);
                var resultDetectLang = await responseDetectLang.Content.ReadAsStringAsync();
                var TextDetectLang = XElement.Parse(resultDetectLang).Value;
                Debug.WriteLine("Detect Lang: " + TextDetectLang);

                //Automatic translate wiht NN turn on
                var response = await client.GetAsync(url + query);
                var result = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    return "Hata: " + result;            
                var translatedText = XElement.Parse(result).Value;
                return translatedText;
            }
        }


        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}