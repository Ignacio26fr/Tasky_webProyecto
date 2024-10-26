using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Tasky.Logica
{
    public class SmsService 
    {
        private readonly string _accountSid;
        private readonly string _authToken;
        private readonly string _fromPhoneNumber;

        public SmsService(string accountSid, string authToken, string fromPhoneNumber)
        {
            _accountSid = accountSid;
            _authToken = authToken;
            _fromPhoneNumber = fromPhoneNumber;
        }

        public async Task EnviarSms(string toPhoneNumber, string message)
        {
            var msjOptions = new CreateMessageOptions(new PhoneNumber(toPhoneNumber))
            {
                From = new PhoneNumber(_fromPhoneNumber),
                Body = message,
            };

            await MessageResource.CreateAsync(msjOptions);
        }

    }
}
