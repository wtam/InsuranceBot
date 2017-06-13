using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using System.Linq;

//Note: https://github.com/garypretty/botframework/tree/master/QnAMakerDialog

namespace InsuranceBOT
{
    [Serializable]
    //[QnAMaker("21014d33da9a4762be6858acb4d4b335", "29ab0dd8-cab0-43e6-8128-fb5bcb41fa99","I don't understand this right now! Try another query!", 0.50, 3)]
    [QnAMaker("21014d33da9a4762be6858acb4d4b335", "29ab0dd8-cab0-43e6-8128-fb5bcb41fa99")]
    public class FAQDialog : QnAMakerDialog
    {
        //falling back to QnAMakerDialog from a LUIS dialog if no intents match
        /*
        public override async Task NoMatchHandler(IDialogContext context, string originalQueryText)
        {
            await context.PostAsync($"Sorry, I couldn't find an answer for '{originalQueryText}'.");
            context.Done(false);
        }

        
        public override async Task DefaultMatchHandler(IDialogContext context, string originalQueryText, QnAMakerResult result)
        {
            await context.PostAsync($"I found an answer that might help...{result.Answer}.");
            context.Done(true);
        } */

        protected override async Task QnAFeedbackStepAsync(IDialogContext context, QnAMakerResults qnaMakerResults)
        {
            // responding with the top answer when score is above some threshold
            if (qnaMakerResults.Answers.Count > 0 && qnaMakerResults.Answers.FirstOrDefault().Score > 0.75)
            {
                await context.PostAsync(qnaMakerResults.Answers.FirstOrDefault().Answer);
            }
            else
            {
                await base.QnAFeedbackStepAsync(context, qnaMakerResults);
            }
        } 

        // Override to also include the knowledgebase question with the answer on confident matches
        protected override async Task RespondFromQnAMakerResultAsync(IDialogContext context, IMessageActivity message, QnAMakerResults results)
        {
            if (results.Answers.Count > 0)
            {
                var response = "Here is the match from FAQ:  \r\n  Q: " + results.Answers.First().Questions.First() + "  \r\n A: " + results.Answers.First().Answer;
                await context.PostAsync(response);
                //context.Done(true);
            } else
            {
                await context.PostAsync("No match found from FAQ");
            }
            context.Done(true);
        } 

        // Override to log matched Q&A before ending the dialog
        /*
        protected override async Task DefaultWaitNextMessageAsync(IDialogContext context, IMessageActivity message, QnAMakerResults results)
        {
            Console.WriteLine("KB Question: " + results.Answers.First().Questions.First());
            Console.WriteLine("KB Answer: " + results.Answers.First().Answer);
            await base.DefaultWaitNextMessageAsync(context, message, results);
            context.Done(true);
        } */

    }  //FAQDialog
}  //InsuranceBot