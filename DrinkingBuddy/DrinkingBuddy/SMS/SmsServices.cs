using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;

namespace DrinkingBuddy.SMS
{
    public class SmsServices
    {

        //static void Main(string[] args)
        //{
        //    Twilio.Clients.TwilioRestClient client;

        //    // ACCOUNT_SID and ACCOUNT_TOKEN are from your Twilio account
        //   // client = new TwilioRestClient(ACCOUNT_SID, ACCOUNT_TOKEN);


        //    var result = client.SendMessage(CALLER_ID, "PHONE NUMBER TO SEND TO", "The answer is 42");
        //    if (result.RestException != null)
        //    {

        //    }
        //}


        public string SendMessage(string ReciverNumber)
        {
            // Find your Account Sid and Token at twilio.com/console
            const string accountSid = "AC5ef153fbe9bc874f51c8aa15cc3dd092";
            const string authToken = "8aa80f02f6ad9a1cbbec04e55bd548c6";

            TwilioClient.Init(accountSid, authToken);

            try { 
            var message = MessageResource.Create(
                body: "This is a Text Message For Testing Puspose?",
                from: new Twilio.Types.PhoneNumber("+61488855893"),
                to: new Twilio.Types.PhoneNumber(ReciverNumber)
            );
               return message.Sid;
            }
            catch (ApiException e)
            {
                return e.Message;
            }
           
        }

    }
}
