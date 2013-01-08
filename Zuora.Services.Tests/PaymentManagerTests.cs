using System;
using System.Collections.Generic;
using Zuora.Services;
using Xunit;

namespace Zuora.Services.Tests
{
    public class PaymentManagerTests : IDisposable
    {
        PaymentManager pm;
        AccountManager am;
        ZuoraService zs;
        ZuoraTestHelper zth;
        SubscriptionManager sm;

        public PaymentManagerTests()
        {
            zs = new ZuoraService(Settings.API_USERNAME, Settings.API_PASSWORD, Settings.API_ENDPOINT);
            pm = new PaymentManager(zs);
            zth = new ZuoraTestHelper();
            am = new AccountManager(zs);
            sm = new SubscriptionManager(zs);
        }
        [Fact]
        public void CanWeCancelCreditBalanceAdjustmentOnInvoice()
        {
            SubscribeResponseHolder subRes = zth.MakeSubscription(false);
            ResponseHolder overpaymentRes = pm.IncreaseCreditBalance(subRes.SubRes.AccountId, 120);
            ResponseHolder createRes = pm.ApplyCreditBalanceToInvoice(subRes.SubRes.InvoiceId, 5);
            ResponseHolder cancellRes = pm.CancelCreditBalanceAdjustmentOnInvoice(subRes.SubRes.InvoiceId);

            Assert.True(cancellRes.Success);
        }
        [Fact]
        public void CanWeDecreaseCreditBalance()
        {
            SubscribeResponseHolder subRes = zth.MakeSubscription(false);
            ResponseHolder overpaymentRes = pm.IncreaseCreditBalance(subRes.SubRes.AccountId, 120);
            ResponseHolder cbaQRes = zs.Query("SELECT id, amount FROM CreditBalanceAdjustment WHERE AccountId = '" + subRes.SubRes.AccountId + "'");
            ResponseHolder refundRes = pm.DecreaseCreditBalance(subRes.SubRes.AccountId, 110);
            Assert.True(refundRes.Success);
        }
        [Fact]
        public void CanWeApplyExternalPaymentToMultipleInvoices()
        {
            AmendOptions ao = new AmendOptions();
            ao.GenerateInvoiceSpecified = true;
            ao.GenerateInvoice = true;
            //make an account with multiple invoices
            SubscribeResponseHolder subRes = zth.MakeSubscription(false);
            AmendResponseHolder amendRes = sm.DoAddProductAmendment(subRes.SubRes.SubscriptionId, DateTime.Now, zth.CreateRatePlanToSubscribe(), ao);
            ResponseHolder invList = am.GetInvoicesForAccount(subRes.SubRes.AccountId);
            List<String> invIdsList = new List<String>();

            Decimal invAmount = 0;

            foreach (zObject z in invList.Objects)
            {
                Invoice inv = (Invoice)z;
                invAmount += (Decimal)inv.Amount;
                invIdsList.Add(inv.Id);
            }
            //get the payment method id
            ResponseHolder pmRes = zs.Query("SELECT id FROM PaymentMethod where Name='Check'");

            ResponseHolder createRes = pm.ApplyPaymentToMultipleInvoices(subRes.SubRes.AccountId, invIdsList.ToArray(), pmRes.Objects[0].Id, invAmount, "External");

            Assert.True(createRes.Success);
        }

        [Fact]
        public void CanWeApplyElectronicPaymentToMultipleInvoices()
        {
            AmendOptions ao = new AmendOptions();
            ao.GenerateInvoiceSpecified = true;
            ao.GenerateInvoice = true;
            //make an account with multiple invoices
            SubscribeResponseHolder subRes = zth.MakeSubscription(false);
            AmendResponseHolder amendRes = sm.DoAddProductAmendment(subRes.SubRes.SubscriptionId, DateTime.Now, zth.CreateRatePlanToSubscribe(), ao);
            ResponseHolder invList = am.GetInvoicesForAccount(subRes.SubRes.AccountId);
            List<String> invIdsList = new List<String>();

            Decimal invAmount = 0;

            foreach (zObject z in invList.Objects)
            {
                Invoice inv = (Invoice)z;
                invAmount += (Decimal)inv.Amount;
                invIdsList.Add(inv.Id);
            }
            //get the payment method id
            ResponseHolder pmRes = am.GetCreditCardsForAccount(subRes.SubRes.AccountId);

            ResponseHolder createRes =  pm.ApplyPaymentToMultipleInvoices(subRes.SubRes.AccountId, invIdsList.ToArray(), pmRes.Objects[0].Id, invAmount, "Electronic");

            Assert.True(createRes.Success);
        }

        [Fact]
        public void CanWeGenerateIframeUrl()
        {
            //Need some kind of config file to put this in along with the password and stuff
            String apiSecKey  = "Y46yy3LMIBRIeqwzk_u4-4YvBGU_HHs79PCHcoihq90=";
            String tenantId = "10717";
            String pageId = "2c92c0f83a569221013a709dc0c2455c";
            String appUrl = "https://apisandbox.zuora.com";
            String iframeUrl = pm.GenerateIframeUrl(pageId, tenantId, apiSecKey, appUrl);
            //Is there a better way to test this? I debug and then drop the iframe src into an html page as a check to see if the iframe is being generated correctly
            Assert.NotNull(iframeUrl);
        }

        [Fact]
        public void CanWeApplyCreditBalanceToAnInvoice()
        {
            SubscribeResponseHolder subRes = zth.MakeSubscription(false);
            ResponseHolder createRes = pm.IncreaseCreditBalance(subRes.SubRes.AccountId, 100);
            ResponseHolder invRes = zs.Query("SELECT id, amount FROM Invoice where id = '" + subRes.SubRes.InvoiceId + "'");
            ResponseHolder adjustRes = pm.ApplyCreditBalanceToInvoice(subRes.SubRes.InvoiceId, (Decimal)((Invoice)invRes.Objects[0]).Amount);
            Assert.True(adjustRes.Success);
        }

        [Fact]
        public void CanWeApplyElectronicPaymentToInvoice()
        {
            SubscribeResponseHolder sub = zth.MakeSubscription(false);
            ResponseHolder invRes = zs.Query("SELECT id, amount FROM Invoice where id = '" + sub.SubRes.InvoiceId + "'");
            ResponseHolder pmRes = am.GetCreditCardsForAccount(sub.SubRes.AccountId);
            String paymentMethodId = pmRes.Objects[0].Id;

            ResponseHolder payRes = pm.ApplyPaymentToInvoice(sub.SubRes.AccountId, invRes.Objects[0].Id, paymentMethodId, (Decimal)((Invoice)invRes.Objects[0]).Amount, "Electronic");

            Assert.True(payRes.Success);
            
        }

        [Fact]
        public void CanWeApplyExternalPaymentToInvoice()
        {
            SubscribeResponseHolder sub = zth.MakeSubscription(false);
            ResponseHolder invRes = zs.Query("SELECT id, amount FROM Invoice where id = '" + sub.SubRes.InvoiceId + "'");
            ResponseHolder checkPaymentMethodId = zs.Query("SELECT id FROM PaymentMethod where Name='Check'");
            ResponseHolder payRes = pm.ApplyPaymentToInvoice(sub.SubRes.AccountId, invRes.Objects[0].Id, checkPaymentMethodId.Objects[0].Id, (Decimal)((Invoice)invRes.Objects[0]).Amount, "External");
            Assert.True(payRes.Success);
        }

        [Fact]
        public void CanWeRefundExternalPayment()
        {
            SubscribeResponseHolder sub = zth.MakeSubscription(false);
            ResponseHolder invRes = zs.Query("SELECT id, amount FROM Invoice where id = '" + sub.SubRes.InvoiceId + "'");
            ResponseHolder checkPaymentMethodId = zs.Query("SELECT id FROM PaymentMethod where Name='Check'");
            ResponseHolder payRes = pm.ApplyPaymentToInvoice(sub.SubRes.AccountId, invRes.Objects[0].Id, checkPaymentMethodId.Objects[0].Id, (Decimal)((Invoice)invRes.Objects[0]).Amount, "External");
            ResponseHolder qRes = zs.Query("SELECT id, amount FROM payment where id = '" + payRes.Id + "'");
            ResponseHolder createRes = pm.RefundExternalPayment(payRes.Id, (Decimal)((Payment)qRes.Objects[0]).Amount, "Check", DateTime.Now);
            Assert.True(createRes.Success);
        }

        [Fact]
        public void CanWeRefundElectronicPayment()
        {
            SubscribeResponseHolder subRes = zth.MakeSubscription(true);
            ResponseHolder payRes = zs.Query("SELECT id, amount FROM payment where id = '" + subRes.SubRes.PaymentId + "'");
            Payment pay = (Payment)payRes.Objects[0];
            ResponseHolder createRes = pm.RefundElectronicPayment(pay.Id, (Decimal)pay.Amount);
            Assert.True(createRes.Success);        
        }

        [Fact]
        public void CanWeIncreaseTheCreditBalanceOfAnAccount()
        {
            SubscribeResponseHolder subRes = zth.MakeSubscription(true);
            pm.IncreaseCreditBalance(subRes.SubRes.AccountId, 10);

            ResponseHolder accRes = am.GetAccount(subRes.SubRes.AccountId);
            Assert.True(((Account)accRes.Objects[0]).CreditBalance == 10);

        }

        public void Dispose()
        {
            ZuoraTestHelper zth = new ZuoraTestHelper();
            zth.CleanUp();
        }
    }
}
