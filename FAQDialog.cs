using System;
using QnAMakerDialog;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

//Note: https://github.com/garypretty/botframework/tree/master/QnAMakerDialog

namespace InsuranceBOT
{
    [Serializable]
    [QnAMakerService("21014d33da9a4762be6858acb4d4b335", "29ab0dd8-cab0-43e6-8128-fb5bcb41fa99")]
    public class FAQDialog : QnAMakerDialog<object>
    {
        //falling back to QnAMakerDialog from a LUIS dialog if no intents match
        public override async Task NoMatchHandler(IDialogContext context, string originalQueryText)
        {
            await context.PostAsync($"Sorry, I couldn't find an answer for '{originalQueryText}'.");
            context.Done(false);
        }

        public override async Task DefaultMatchHandler(IDialogContext context, string originalQueryText, QnAMakerResult result)
        {
            await context.PostAsync($"I found an answer that might help...{result.Answer}.");
            context.Done(true);
        }

        }  //FAQDialog

    }  //InsuranceBot