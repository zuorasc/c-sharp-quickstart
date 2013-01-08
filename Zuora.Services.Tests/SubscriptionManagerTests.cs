using System;
using System.Collections.Generic;
using Zuora.Services;
using Xunit;

namespace Zuora.Services.Tests
{
    public class SubscriptionManagerTests : IDisposable
    {
        ZuoraService zs;
        SubscriptionManager sm;
        ZuoraTestHelper zth;

        public SubscriptionManagerTests()
        {
            zs = new ZuoraService(Settings.API_USERNAME, Settings.API_PASSWORD, Settings.API_ENDPOINT);
            sm = new SubscriptionManager(zs);
            zth = new ZuoraTestHelper();
        }

        [Fact]
        public void CanWeSubscribeToMultipleRatePlans()
        {
            Account acc = zth.MakeTestAccount();
            Contact con = zth.MakeTestContact();
            PaymentMethod pay = zth.MakeTestPaymentMethod();

            String productRatePlanId = zth.CreateRatePlanToSubscribe();
            PreviewOptions po = new PreviewOptions();
            SubscribeOptions so = new SubscribeOptions();
            ProductRatePlanHolder prph = new ProductRatePlanHolder();

            ProductRatePlan prp = new ProductRatePlan();
            prp.Id = productRatePlanId;

            prph.ProductRatePlan = prp;

            ResponseHolder queryRes = zs.Query("SELECT id, ChargeModel FROM ProductRatePlanCharge WHERE productrateplanid = '" + productRatePlanId + "'");
            ProductRatePlanCharge prpc = (ProductRatePlanCharge)queryRes.Objects[0];
            prpc.DefaultQuantity = 11;

            ProductRatePlanChargeHolder prpch = new ProductRatePlanChargeHolder();
            prpch.ProductRatePlanCharge = prpc;

            prph.ProductRatePlanCharges = new List<ProductRatePlanChargeHolder> { prpch };

            SubscribeResponseHolder subResp = sm.Subscribe(acc, con, pay, new List<ProductRatePlanHolder> { prph, prph }, zth.MakeTestSubscription(), po, so);
            Assert.True(subResp.Success);
        }

        [Fact]
        public void CanWeSubscribeToMultipleRatePlansWithExistingAccount()
        {
            String accId = zth.MakeSubscription(true).SubRes.AccountId;
            String productRatePlanId = zth.CreateRatePlanToSubscribe();
            PreviewOptions po = new PreviewOptions();
            SubscribeOptions so = new SubscribeOptions();

            ProductRatePlanHolder prph = new ProductRatePlanHolder();

            ProductRatePlan prp = new ProductRatePlan();
            prp.Id = productRatePlanId;

            prph.ProductRatePlan = prp;

            ResponseHolder queryRes = zs.Query("SELECT id, ChargeModel FROM ProductRatePlanCharge WHERE productrateplanid = '" + productRatePlanId + "'");
            ProductRatePlanCharge prpc = (ProductRatePlanCharge)queryRes.Objects[0];
            prpc.DefaultQuantity = 11;

            ProductRatePlanChargeHolder prpch = new ProductRatePlanChargeHolder();
            prpch.ProductRatePlanCharge = prpc;

            prph.ProductRatePlanCharges = new List<ProductRatePlanChargeHolder> { prpch };

            SubscribeResponseHolder subResp = sm.SubscribeWithExisitingAccount(accId, new List<ProductRatePlanHolder> { prph, prph }, zth.MakeTestSubscription(), po, so);
            Assert.True(subResp.Success);
        }

        [Fact]
        public void CanWeSubscribeWithPreview()
        {
            Account acc = zth.MakeTestAccount();
            Contact con = zth.MakeTestContact();
            PaymentMethod pay = zth.MakeTestPaymentMethod();

            String productRatePlanId = zth.CreateRatePlanToSubscribe();
            PreviewOptions po = new PreviewOptions();
            po.EnablePreviewModeSpecified = true;
            po.EnablePreviewMode = true;
            po.NumberOfPeriodsSpecified = true;
            po.NumberOfPeriods = 1;
            SubscribeOptions so = new SubscribeOptions();
            ProductRatePlanHolder prph = new ProductRatePlanHolder();

            ProductRatePlan prp = new ProductRatePlan();
            prp.Id = productRatePlanId;

            prph.ProductRatePlan = prp;

            ResponseHolder queryRes = zs.Query("SELECT id, ChargeModel FROM ProductRatePlanCharge WHERE productrateplanid = '" + productRatePlanId + "'");
            ProductRatePlanCharge prpc = (ProductRatePlanCharge)queryRes.Objects[0];
            prpc.DefaultQuantity = 11;

            ProductRatePlanChargeHolder prpch = new ProductRatePlanChargeHolder();
            prpch.ProductRatePlanCharge = prpc;


            prph.ProductRatePlanCharges = new List<ProductRatePlanChargeHolder> { prpch };

            SubscribeResponseHolder subResp = sm.Subscribe(acc, con, pay, new List<ProductRatePlanHolder> { prph }, zth.MakeTestSubscription(), po, so);
            Assert.True(subResp.Success);
            //it should not create an account
            Assert.Null(subResp.SubRes.AccountId);
        }
        [Fact]
        public void CanWeSubscribeWithoutChargeInfo()
        {
            Account acc = zth.MakeTestAccount();
            Contact con = zth.MakeTestContact();
            PaymentMethod pay = zth.MakeTestPaymentMethod();

            String productRatePlanId = zth.CreateRatePlanToSubscribe();
            PreviewOptions po = new PreviewOptions();
            SubscribeOptions so = new SubscribeOptions();
            ProductRatePlanHolder prph = new ProductRatePlanHolder();

            ProductRatePlan prp = new ProductRatePlan();
            prp.Id = productRatePlanId;

            prph.ProductRatePlan = prp;

            SubscribeResponseHolder subResp = sm.Subscribe(acc, con, pay, new List<ProductRatePlanHolder> { prph }, zth.MakeTestSubscription(), po, so);
            Assert.True(subResp.Success);
        }
        [Fact]
        public void CanWeSubscribe()
        {
            Account acc = zth.MakeTestAccount();
            Contact con = zth.MakeTestContact();
            PaymentMethod pay = zth.MakeTestPaymentMethod();

            String productRatePlanId = zth.CreateRatePlanToSubscribe();
            PreviewOptions po = new PreviewOptions();
            SubscribeOptions so = new SubscribeOptions();
            ProductRatePlanHolder prph = new ProductRatePlanHolder();

            ProductRatePlan prp = new ProductRatePlan();
            prp.Id = productRatePlanId;

            prph.ProductRatePlan = prp;

            ResponseHolder queryRes = zs.Query("SELECT id, ChargeModel FROM ProductRatePlanCharge WHERE productrateplanid = '" + productRatePlanId + "'");
            ProductRatePlanCharge prpc = (ProductRatePlanCharge)queryRes.Objects[0];
            prpc.DefaultQuantity = 11;

            ProductRatePlanChargeHolder prpch = new ProductRatePlanChargeHolder();
            prpch.ProductRatePlanCharge = prpc;


            prph.ProductRatePlanCharges = new List<ProductRatePlanChargeHolder> { prpch };

            SubscribeResponseHolder subResp = sm.Subscribe(acc, con, pay, new List<ProductRatePlanHolder> { prph }, zth.MakeTestSubscription(), po, so);
            Assert.True(subResp.Success);
        }

        [Fact]
        public void CanWeSubscribeWithExistingAccount()
        {
            String accId = zth.MakeSubscription(true).SubRes.AccountId;
            String productRatePlanId = zth.CreateRatePlanToSubscribe();
            PreviewOptions po = new PreviewOptions();
            SubscribeOptions so = new SubscribeOptions();

            ProductRatePlanHolder prph = new ProductRatePlanHolder();

            ProductRatePlan prp = new ProductRatePlan();
            prp.Id = productRatePlanId;

            prph.ProductRatePlan = prp;

            ResponseHolder queryRes = zs.Query("SELECT id, ChargeModel FROM ProductRatePlanCharge WHERE productrateplanid = '" + productRatePlanId + "'");
            ProductRatePlanCharge prpc = (ProductRatePlanCharge)queryRes.Objects[0];
            prpc.DefaultQuantity = 11;

            ProductRatePlanChargeHolder prpch = new ProductRatePlanChargeHolder();
            prpch.ProductRatePlanCharge = prpc;


            prph.ProductRatePlanCharges = new List<ProductRatePlanChargeHolder> { prpch };


            SubscribeResponseHolder subResp = sm.SubscribeWithExisitingAccount(accId, new List<ProductRatePlanHolder> { prph }, zth.MakeTestSubscription(), po, so);
            Assert.True(subResp.Success);
        }

        [Fact]
        public void CanWeDoAddProductAmendment()
        {
            AmendOptions ao = new AmendOptions();
            ao.GenerateInvoiceSpecified = true;
            ao.GenerateInvoice = false;
            String subId = zth.MakeSubscription(true).SubRes.SubscriptionId;
            String productRatePlanId = zth.CreateRatePlanToSubscribe();
            AmendResponseHolder amendRes = sm.DoAddProductAmendment(subId, DateTime.Now, productRatePlanId, ao);
            Assert.True(amendRes.Success);
            
        }

        [Fact]
        public void CanWeDoRenewalAmenedment()
        {
            //the subscription has to be termed
            String subId = zth.MakeSubscription(true).SubRes.SubscriptionId;
            AmendResponseHolder amendRes = sm.DoRenewalAmendment(subId, DateTime.Now);
            Assert.True(amendRes.Success);
        }

        [Fact]
        public void CanWeDoATermsAndConditionsAmendment()
        {
            String subId = zth.MakeSubscription(true).SubRes.SubscriptionId;
            AmendResponseHolder amendRes = sm.DoTermsAndConditionsAmendment(subId, DateTime.Now, "TERMED", 24, 12);
            Assert.True(amendRes.Success);
        }

        [Fact]
        public void CanWeGetSubscriptionAndChargeInfo()
        {
            String subId = zth.MakeSubscription(true).SubRes.SubscriptionId;
            SubscriptionInfoHolder sih = sm.GetSubscriptionAndChargeInfo(subId);
            Assert.True(sih.Success);
            Assert.NotNull(sih.Subscription);
            Assert.True(sih.RatePlanList.Count > 0);
            Assert.True(sih.RatePlanChargeList.Count > 0);
            Assert.True(sih.RatePlanChargeTierList.Count > 0);
        }

        public void Dispose()
        {
            ZuoraTestHelper zth = new ZuoraTestHelper();
            zth.CleanUp();
        }
    }
}
