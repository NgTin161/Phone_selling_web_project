using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Crmf;
using Server.Models;
using Server.Models.Momo;
using System.Security.Cryptography;
using System.Text;
using RestSharp;
using Microsoft.Extensions.Primitives;

namespace Server.Services
{
    public class MomoService : IMomoService
    {
        private readonly IOptions<MomoOptionModel> _options;

        public MomoService(IOptions<MomoOptionModel> options)
        {
            _options = options;
        }
        public async Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(string fullName, double amount)
        {
            string OrderId = DateTime.UtcNow.Ticks.ToString();
            string OrderInfo = "Khách hàng: " + fullName + " thanh toán đơn hàng qua MoMo" ;
            var rawData =
                $"partnerCode={_options.Value.PartnerCode}&accessKey={_options.Value.AccessKey}&requestId={OrderId}&amount={amount}&orderId={OrderId}&orderInfo={OrderInfo}&returnUrl={_options.Value.ReturnUrl}&notifyUrl={_options.Value.NotifyUrl}&extraData=";

            var signature = ComputeHmacSha256(rawData, _options.Value.SecretKey);

            var client = new RestClient(_options.Value.MomoApiUrl);
            var request = new RestRequest() { Method = RestSharp.Method.Post };
            request.AddHeader("Content-Type", "application/json; charset=UTF-8");

            // Create an object representing the request data
            var requestData = new
            {
                accessKey = _options.Value.AccessKey,
                partnerCode = _options.Value.PartnerCode,
                requestType = _options.Value.RequestType,
                notifyUrl = _options.Value.NotifyUrl,
                returnUrl = _options.Value.ReturnUrl,
                orderId = OrderId,
                amount = amount.ToString(),
                orderInfo = OrderInfo,
                requestId = OrderId,
                extraData = "",
                signature = signature
            };

            request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);

            var response = await client.ExecuteAsync(request);

            return JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);
        }

        public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection)
        {
            string amount = collection["amount"];
            string orderInfo = collection["orderInfo"];
            string orderId = collection["orderId"];

            var responseModel = new MomoExecuteResponseModel
            {
                amount = amount,
                orderId = orderId,
                orderInfo = orderInfo
            };

            return responseModel;
        }

        private string ComputeHmacSha256(string message, string secretKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] hashBytes;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                hashBytes = hmac.ComputeHash(messageBytes);
            }

            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            return hashString;
        }
    }
}
