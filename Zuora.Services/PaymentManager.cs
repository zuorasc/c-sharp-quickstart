using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Lower credit balance: would this be just deleting the credit balance created by the overpayment?

namespace Zuora.Services
{
    public class PaymentManager
    {
        ZuoraService zs;

        public PaymentManager(ZuoraService zs)
        {
            this.zs = zs;
        }
        public ResponseHolder CancelCreditBalanceAdjustmentOnInvoice(String invoiceId)
        {
            ResponseHolder queryRes = zs.Query("SELECT id FROM CreditBalanceAdjustment WHERE SourceTransactionId = '" + invoiceId + "'");
            CreditBalanceAdjustment cba = (CreditBalanceAdjustment)queryRes.Objects[0];
            cba.Status = "Canceled";
            return zs.Update(new List<zObject> { cba })[0];
        }
        /// <summary>
        /// Decrease credit balance for an account by an amount by creating an external refund
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="amount"></param>
        /// <param name="methodType"></param>
        /// <param name="refundDate"></param>
        /// <returns></returns>
        public ResponseHolder DecreaseCreditBalance(String accountId, Decimal amount)
        {
            //refund payment
            Refund refund = new Refund();
            refund.AccountId = accountId;
            refund.AmountSpecified = true;
            refund.Amount = amount;
            refund.SourceType = "CreditBalance";
            refund.Type = "External";
            refund.MethodType = "Other";

            List<ResponseHolder> createRefRes = zs.Create(new List<zObject> { refund }, false);
            return createRefRes[0];
        }
        /// <summary>
        /// Applies a single payment to multiple invoices
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="invoiceIds"></param>
        /// <param name="paymentMethodId"></param>
        /// <param name="amount"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ResponseHolder ApplyPaymentToMultipleInvoices(String accountId, String[] invoiceIds, String paymentMethodId, Decimal amount, String type)
        {
            //create payment in draft
            Payment payment = new Payment();
            payment.Amount = amount;
            payment.AmountSpecified = true;
            payment.EffectiveDate = DateTime.Now;
            payment.EffectiveDateSpecified = true;
            payment.AccountId = accountId;
            payment.PaymentMethodId = paymentMethodId;
            payment.Type = type;
            payment.Status = "Draft";

            List<ResponseHolder> payCreateRes =  zs.Create(new List<zObject> { payment }, false);
            foreach (ResponseHolder rh in payCreateRes)
            {
                if (!rh.Success)
                {
                    return rh;
                }
            }
            List<zObject> ipList = new List<zObject>();
            //create invoice payment objects for the invoice amount
            foreach(String invId in invoiceIds)
            {
                ResponseHolder qRes = zs.Query("SELECT id, Amount FROM Invoice WHERE id = '" + invId + "'");
                Invoice inv = (Invoice)qRes.Objects[0];
                InvoicePayment ip = new InvoicePayment();
                ip.AmountSpecified = true;
                ip.Amount = inv.Amount;
                ip.InvoiceId = inv.Id;
                ip.PaymentId = payCreateRes[0].Id;

                ipList.Add(ip);
            }
            List<ResponseHolder> ipCreateRes = zs.Create(ipList, false);
            foreach (ResponseHolder rh in ipCreateRes)
            {
                if (!rh.Success)
                {
                    return rh;
                }
            }
            //update the original payment to be Proccessed
            Payment updatePayment = new Payment();
            updatePayment.Id = payCreateRes[0].Id;
            updatePayment.Status = "Processed";
            List<ResponseHolder> updateRes = zs.Update(new List<zObject>{ updatePayment });

            return updateRes[0];
        }

        /// <summary>
        /// Pay an invoice with an external payment
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="invoiceId"></param>
        /// <param name="paymentMethodId"></param>
        /// <param name="amount"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ResponseHolder ApplyPaymentToInvoice(String accountId, String invoiceId, String paymentMethodId, Decimal amount, String type)
        {
            Payment payment = new Payment();

            payment.Amount = amount;
            payment.AmountSpecified = true;
            payment.AppliedInvoiceAmount = amount;
            payment.AppliedInvoiceAmountSpecified = true;
            payment.EffectiveDate = DateTime.Now;
            payment.EffectiveDateSpecified = true;
            payment.AccountId = accountId;
            payment.InvoiceId = invoiceId;
            payment.PaymentMethodId = paymentMethodId;
            payment.Type = type;
            payment.Status = "Processed";

            return zs.Create(new List<zObject> { payment }, false)[0];
        }

        /// <summary>
        /// Increase the credit balance of an account by an amount
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public ResponseHolder IncreaseCreditBalance(String accountId, Decimal amount)
        {
            Payment payment = new Payment();
            payment.AccountId = accountId;
            payment.Type = "External";
            payment.Status = "Processed";
            payment.Amount = amount;
            payment.AppliedCreditBalanceAmountSpecified = true;
            payment.AppliedCreditBalanceAmount = amount;
            payment.EffectiveDate = DateTime.Now;
            payment.EffectiveDateSpecified = true;
            ResponseHolder otherPaymentMethod = zs.Query("SELECT id FROM PaymentMethod WHERE Name = 'Other'");
            payment.PaymentMethodId = ((PaymentMethod)otherPaymentMethod.Objects[0]).Id;

            return zs.Create(new List<zObject> {payment}, false)[0];
        }

        /// <summary>
        /// Apply credit balance to an invoice
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="paymentId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public ResponseHolder ApplyCreditBalanceToInvoice(String invoiceId, Decimal amount)
        {
            CreditBalanceAdjustment cba = new CreditBalanceAdjustment();
            cba.AmountSpecified = true;
            cba.Amount = amount;
            //cba.ReferenceId = paymentId;
            
            cba.SourceTransactionId = invoiceId;
            cba.Type = "Decrease";

            return zs.Create(new List<zObject> { cba }, false)[0];
        }

        /// <summary>
        /// Refund an external payment
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="amount"></param>
        /// <param name="methodType"></param>
        /// <param name="refundDate"></param>
        /// <returns></returns>
        public ResponseHolder RefundExternalPayment(String paymentId, Decimal amount, String methodType, DateTime refundDate)
        {
            //refund payment
            Refund refund = new Refund();
            refund.PaymentId = paymentId;
            refund.AmountSpecified = true;
            refund.Amount = amount;
            refund.SourceType = "Payment";
            refund.Type = "External";
            refund.MethodType = methodType;
            refund.RefundDateSpecified = true;
            refund.RefundDate = refundDate;

            List<ResponseHolder> createRefRes = zs.Create(new List<zObject> { refund }, false);
            return createRefRes[0];
        }

        /// <summary>
        /// Refund an electronic payment
        /// </summary>
        /// <param name="paymentId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public ResponseHolder RefundElectronicPayment(String paymentId, Decimal amount)
        {
            //refund payment
            Refund refund = new Refund();
            refund.PaymentId = paymentId;
            refund.AmountSpecified = true;
            refund.Amount = amount;
            refund.SourceType = "Payment";
            refund.Type = "Electronic";

            List<ResponseHolder> createRefRes = zs.Create(new List<zObject> { refund }, false);
            return createRefRes[0];
        }

        //Generate the url to use with the Zuora Hosted Payment Pages
        public String GenerateIframeUrl(String pageId, String tenantId, String apiSecurityKey, String appUrl)
        {
            String randomtoken = Guid.NewGuid().ToString();  // 32 char alphanumeric random token
            randomtoken = randomtoken.Replace("-", "");

            long timestamp = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000; // UTC(GMT) timetamp in milliseconds.
            String query = "id=" + pageId + "&tenantId=" + tenantId + "&timestamp=" + timestamp + "&token=" + randomtoken + apiSecurityKey;
            String signature = EncodeTo64(CreateMD5Hash(query)); // Signature Genarated from hashing
            signature = signature.Replace("+", "-");
            signature = signature.Replace('/', '_');
            String querystring = "id=" + pageId + "&tenantId=" + tenantId + "&timestamp=" + timestamp + "&token=" + randomtoken;
            String iframeurl = appUrl + "/apps/PublicHostedPaymentMethodPage.do?" + "method=requestPage&" + querystring + "&" + "signature=" + signature;

            return iframeurl;
        }
        /// <summary>
        /// Take string convert it to UTF8 then genrate MD5 Hash then Convert to Base 16
        /// </summary>
        /// <param name="RawData"></param>
        /// <returns></returns>
        public String CreateMD5Hash(String RawData)
        {
            byte[] hash = System.Security.Cryptography.MD5.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(RawData));
            String str = "";
            byte[] numArray = hash;
            int index = 0;
            while (index < numArray.Length)
            {
                byte num = numArray[index];
                str = str + num.ToString("x2");
                checked { ++index; }
            }
            return str;
        }

        /// <summary>
        /// Encode string to Base64
        /// </summary>
        /// <param name="toEncode"></param>
        /// <returns></returns>
        public String EncodeTo64(String toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.Encoding.UTF8.GetBytes(toEncode);
            String returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }
    }
}
