using InsuranceBoT;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using System.Threading;
using System.Diagnostics;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using System.Collections.Generic;
using System.Web;

// Important resources
// https://github.com/richdizz/BotAuth/tree/master/CSharp 

// https://blog.mastykarz.nl/configuring-multi-tenant-authentication-azure-app-service-authentication-options/
// http://www.matvelloso.com/2015/01/30/troubleshooting-common-azure-active-directory-errors/
using BotAuth.AADv2;
using BotAuth.Dialogs;
using BotAuth.Models;
using BotAuth;
using System.Configuration;
using System.Net.Http;

//Ref: https://github.com/Microsoft/BotBuilder-CognitiveServices/tree/master/CSharp/Samples/QnAMaker

namespace InsuranceBOT
{
    [Serializable]
    //[LuisModel("YourModelId", "YourSubscriptionKey")]
    //LUIS APP: InsuranceBOT
    [LuisModel("7f5f9306-17a5-45ef-9daf-6eac1b816da5", "f5a1c2fc5ea0407ba30aed8e2e27187c")]
    public class BuildInsuranceLUISDialog : LuisDialog<object>
    {
       
        [LuisIntent("LossReportForm")]
        public async Task LossReportHandler(IDialogContext context, LuisResult result)
        {   
     
            await context.PostAsync("XXX Insurance (LUIS): Loss Form");
            var form = new FormDialog<LossForm>(
                new LossForm(),
                LossForm.BuildForm,
                FormOptions.PromptInStart,
                result.Entities);
            context.Call<LossForm>(form, LossFormComplete);
            //context.Wait(this.MessageReceived);
        }

        private async Task LossFormComplete(IDialogContext context, IAwaitable<LossForm> result)
        {
            LossForm form = null;
            try
            {
                form = await result;
            }
            catch (OperationCanceledException)
            {
            }
            if (form == null)
            {
                await context.PostAsync("You cancelled the form.");
            }
            else
            {
                //call the LossForm service to complete the form fill
                var message = $"Thanks! for using our Bot to submit Form Services.";
                await context.PostAsync(message);
            }
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("InvestmentEnquiry")]
        public async Task InvestmentEnquiryHandler(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result)
        {
            await context.PostAsync("LUIS Invsetment Enquiry: ........");
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            ///var message = await item;
            var message = (Activity)context.Activity;

            Debug.WriteLine("am I here");
            // Initialize AuthenticationOptions and forward to AuthDialog for token
            AuthenticationOptions options = new AuthenticationOptions()
            {
                Authority = ConfigurationManager.AppSettings["aad:Authority"],
                ClientId = ConfigurationManager.AppSettings["aad:ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["aad:ClientSecret"],
                Scopes = new string[] { "User.Read" },
                RedirectUrl = ConfigurationManager.AppSettings["aad:Callback"]
            }; 
            var loginMicrosoftOnlineCom = new MSALAuthProvider();
            ////await context.Forward(new AuthDialog(new MSALAuthProvider(), options), async (IDialogContext authContext, IAwaitable<AuthResult> authResult) =>
            await context.Forward(new AuthDialog(loginMicrosoftOnlineCom, options), async (IDialogContext authContext, IAwaitable<AuthResult> authResult) =>
            {
                var result = await authResult;
                // Use token to call into service
                var json = await new HttpClient().GetWithAuthAsync(result.AccessToken, "https://graph.microsoft.com/v1.0/me");
                await authContext.PostAsync($"Welcome back {json.Value<string>("displayName")} , you've login as {json.Value<string>("userPrincipalName")}. Account/transaction can operation now....");             
            }, message, CancellationToken.None);

            //pass back to root dialog
            await loginMicrosoftOnlineCom.Logout(options, context);
            context.Done(true);
        }

        [LuisIntent("AccountOperation")]
        public async Task AccountOperationHandler(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result)
        {
            await context.PostAsync("LUIS Account Operation:");
            var replyToConversation = context.MakeMessage();
            replyToConversation.Attachments = new List<Attachment>();
            List<CardAction> cardButtons = new List<CardAction>();
            CardAction plButton = new CardAction()
            {
                ///Value = $"{System.Configuration.ConfigurationManager.AppSettings["AppWebSite"]}/Home/Login?userid={HttpUtility.UrlEncode(context.Activity.From.Id)}",
                Value = $"{System.Configuration.ConfigurationManager.AppSettings["AppWebSite"]}",
                Type = "signin",
                Title = "Authentication Required"
            };
            cardButtons.Add(plButton);
            SigninCard plCard = new SigninCard("Login to proceed", new List<CardAction>() { plButton });
            Attachment plAttachment = plCard.ToAttachment();
            replyToConversation.Attachments.Add(plAttachment);
            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceivedAsync);
        }

        // Everthing else, ask FAQ
        [LuisIntent("None")]
        public async Task NoneHandler(IDialogContext context, IAwaitable<IMessageActivity> message, LuisResult result)
        {
            //falling back to QnAMakerDialog from a LUIS dialog if no intents match
            var qnadialog = new FAQDialog();
            var messageToForward = await message;
            await context.Forward(qnadialog, AfterQnADialog, messageToForward, CancellationToken.None);
        }

        //private async Task AfterQnADialog(IDialogContext context, IAwaitable<object> result)
        private async Task AfterQnADialog(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            //var answerFound = await result;
            Debug.WriteLine("AfterQnADialog: ");
            // we might want to send a message or take some action if no answer was found (false returned)
            if (result.Equals(null))
            {
                Debug.WriteLine("AfterQnADialog: no answer found");
                await context.PostAsync("I’m not sure what you want. Try another query.");
            }          
            context.Wait(MessageReceived);
            //throw new NotImplementedException();

        }
    }   
}