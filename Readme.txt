Summary:
This is to illustrate LUIS + FormFlow
use LUIS to identify the LossForm intent than call the LossForm FormFlow

[Don't forget to fill this into in the Webconfig in order to allow your registerd bot webapp to be connected]
Bot registration:
AppName: InsuranceBotDemo
App ID: 2883500a-e72c-4096-8a3d-f4a1b8a5f6a2
App Password: f5bSOLqPXEjBbjsDmUbtMdB


 1. MessageController
	/// without LUIS , just FormFlow
    await Conversation.SendAsync(activity, BuildInsuranceDialog);

	/// with LUIS
    await Conversation.SendAsync(activity, () => new BuildInsuranceLUISDialog());
 
 2. Create the FormFlow 
	- create the LossForm class 
	- call  initiate the from from MessageController Chain.From(() => FormDialog.FromForm(LossForm.BuildForm))

 3. Create the LUISDialog, BuildInsuranceLUISDialog
	- Create LUIS APP: LUIS APP: InsuranceBOT 
		- Create an LossReportForm intent : E.g. I like to file a lost report
	-Create an LossReportHandler
		- for initiate the from from from LossReportFrom(LUIS intent)
		Note : comment out MessageController the without LUIS

 4. Run the code and use "Hi" and "I like to file a lost report" to test LUIS is able to understand and pass to formflow