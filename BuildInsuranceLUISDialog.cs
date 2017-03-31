using InsuranceBoT;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

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
            await context.PostAsync("XXX Insurance (LUIS): Sorry to know about your loss, I just need few piece of info to get your report file");
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
                var message = $"Thanks! for using our Bot service.";
                await context.PostAsync(message);
            }
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("None")]
        public async Task NoneHandler(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I'm sorry, I don't understand");
            //pass to human agent?...........
            context.Wait(MessageReceived);

        }
    }
}