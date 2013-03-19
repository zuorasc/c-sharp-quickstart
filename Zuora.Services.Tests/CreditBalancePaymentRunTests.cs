using System;

using Zuora.Services;
using System.Collections.Generic;
using Xunit;

namespace Zuora.Services.Tests
{
 
    public class CreditBalancePaymentRunTests : IDisposable
    {


        ZuoraService zs;
        ZuoraTestHelper zth;
        CreditBalancePaymentRun cbpr;
        PaymentManager pm;
        public CreditBalancePaymentRunTests()
        {
            zs = new ZuoraService(Settings.API_USERNAME, Settings.API_PASSWORD, Settings.API_ENDPOINT);
            zth = new ZuoraTestHelper();
            cbpr = new CreditBalancePaymentRun(zs);
            pm = new PaymentManager(zs);
        }
        /* these can be run one at a time
        [Fact]
        public void CanWeDoCreditBalancePaymentRun()
        {
            //have at least one account that has an invoice posted but not paid and the account had a credit balance
            SubscribeResponseHolder subResp = zth.MakeSubscription(false);
            pm.IncreaseCreditBalance(subResp.SubRes.AccountId, 100);
            Boolean res = cbpr.DoCreditBalancePaymentRun();
            ResponseHolder qRes = zs.Query("SELECT Balance From Invoice Where Id = '" + subResp.SubRes.InvoiceId + "'");
            Invoice inv = (Invoice)qRes.Objects[0];

            Assert.True(inv.Balance == 0);
            Assert.True(res);
        }
         * /
        /* these can be run one at a time
        [Fact]
        public void CanWeDoCreditBalancePaymentRunWithMultipleInvoices()
        {
            DateTime inThePast = DateTime.Now.AddMonths(-2);
            DateTime aMontheLater = inThePast.AddMonths(1);
            //have at least one account that has multiple due invoices
            SubscribeResponseHolder subResp = zth.MakeSubscription(false, false, inThePast);

            Invoice pastInv = MakeInvoice(inThePast, subResp.SubRes.AccountId);
            List<ResponseHolder> invCreateResp = zs.Create(new List<zObject> { pastInv }, false);
            Invoice newInv = new Invoice();
            newInv.Id = invCreateResp[0].Id;
            newInv.Status = "Posted";
            List<ResponseHolder> updateRes = zs.Update(new List<zObject> { newInv } );
            Invoice aMontheLaterInv = MakeInvoice(aMontheLater, subResp.SubRes.AccountId);
            List<ResponseHolder> inv2CreateResp = zs.Create(new List<zObject> { aMontheLaterInv }, false);
            Invoice newInv2 = new Invoice();
            newInv2.Id = inv2CreateResp[0].Id;
            newInv2.Status = "Posted";
            List<ResponseHolder> update2Res = zs.Update(new List<zObject> { newInv2 });

            pm.IncreaseCreditBalance(subResp.SubRes.AccountId, new Decimal(5.5));
            Boolean res = cbpr.DoCreditBalancePaymentRun();
            ResponseHolder qRes = zs.Query("SELECT Balance From Invoice Where Id = '" + inv2CreateResp[0].Id + "'");
            Invoice inv = (Invoice)qRes.Objects[0];

            Assert.True(inv.Balance == 0);
            Assert.True(res);
        }
         */
        public Invoice MakeInvoice(DateTime date, String accountId)
        {
            //create another invoice
            Invoice newInvoice = new Invoice();
            newInvoice.AccountId = accountId;
            newInvoice.TargetDateSpecified = true;
            newInvoice.TargetDate = date;
            newInvoice.TargetDateSpecified = true;
            newInvoice.InvoiceDateSpecified = true;
            newInvoice.InvoiceDate = date;
            newInvoice.IncludesOneTimeSpecified = true;
            newInvoice.IncludesOneTime = true;
            newInvoice.IncludesRecurringSpecified = true;
            newInvoice.IncludesRecurring = true;
            newInvoice.IncludesUsageSpecified = true;
            newInvoice.IncludesUsage = true;
            return newInvoice;
        }
        [Fact]
        public void CanWeDoCreditBalancePaymentRunAndApplyPartialPaymentToInvoice()
        {
            //have at least one account that has an invoice posted but not paid and the account had a credit balance
            SubscribeResponseHolder subResp = zth.MakeSubscription(false);
            pm.IncreaseCreditBalance(subResp.SubRes.AccountId, new Decimal(0.01));
            Boolean res = cbpr.DoCreditBalancePaymentRun();
            ResponseHolder qRes = zs.Query("SELECT Balance From Invoice Where Id = '" + subResp.SubRes.InvoiceId + "'");
            Invoice inv = (Invoice)qRes.Objects[0];

            Assert.True(inv.Balance > 0);
            Assert.True(res);
        }
        /*
         * these only work one at a time
        [Fact]
        public void CanWeDoCreditBalancePaymentRunWithMultipleAccounts()
        {
            SubscribeResponseHolder subResp1 = zth.MakeSubscription(false);
            pm.IncreaseCreditBalance(subResp1.SubRes.AccountId, 100);
            //have at least one account that has an invoice posted but not paid and the account had a credit balance
            SubscribeResponseHolder subResp = zth.MakeSubscription(false);
            pm.IncreaseCreditBalance(subResp.SubRes.AccountId, new Decimal(0.01));
            Boolean res = cbpr.DoCreditBalancePaymentRun();
            ResponseHolder qRes = zs.Query("SELECT Balance From Invoice Where Id = '" + subResp.SubRes.InvoiceId + "'");
            Invoice inv = (Invoice)qRes.Objects[0];

            ResponseHolder qRes1 = zs.Query("SELECT Balance From Invoice Where Id = '" + subResp1.SubRes.InvoiceId + "'");
            Invoice inv1 = (Invoice)qRes1.Objects[0];

            Assert.True(inv1.Balance == 0);
            Assert.True(inv.Balance > 0);
            Assert.True(res);
        }
        */
        
        public  void Dispose()
        {
            ZuoraTestHelper zth = new ZuoraTestHelper();
            zth.CleanUp();
        }
    }
}
