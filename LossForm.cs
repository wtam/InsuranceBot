using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/* Source: https://www.codeproject.com/articles/1110794/building-a-bot-using-csharp-and-debugging-using-bo */

namespace InsuranceBoT
{
    public enum LossTypeOptions
    {
        Stolen = 1,
        Accident,
        NaturalCalamity
    }
    [Serializable]
    public class LossForm
    {
        //Prompt the Question in part of the FormFlow
        [Prompt("Please give me your policy number")]
        public string PolicyNumber;

        [Prompt("What happened to your vehicle {||}")]
        public LossTypeOptions LossType;

        [Prompt("When did this happen")]
        public DateTime? LossDate;

        [Prompt("Where did this happen")]
        public string WhereDidThisHappen;

        [Prompt("How this happened in detail")]
        public string LossDetail;

        public static IForm<LossForm> BuildForm()
        {
            OnCompletionAsyncDelegate<LossForm> wrapUpRequest = async (context, state) =>
            {
                string wrapUpMessage = "Your loss[" + state.LossType + "] on "+ state.LossDate.Value.ToShortDateString() + " against Policy number " + state.PolicyNumber + @"is being processed. You will also receive a mail in your registered mail ID once this is initiated.Thank you for using our Bot service.";
                await context.PostAsync(wrapUpMessage);

            };
            return new FormBuilder<LossForm>().Message
            ("XXX Insurance (FormFlow): start filling the form.")
                .Field(nameof(PolicyNumber))
                .Field(nameof(LossType))
                .Field(nameof(LossDate))
                .Field(nameof(WhereDidThisHappen))
                .Field(nameof(LossDetail))
                .OnCompletion(wrapUpRequest)
                .Build();
        }
    }
}