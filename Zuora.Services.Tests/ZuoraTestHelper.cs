using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zuora.Services;

namespace Zuora.Services.Tests
{
    class ZuoraTestHelper
    {

        ZuoraService zs;
        Dictionary<String, List<String>> cleanUpMap = new Dictionary<string, List<string>>();

        public ZuoraTestHelper()
        {
            this.zs = new ZuoraService(Settings.API_USERNAME, Settings.API_PASSWORD, Settings.API_ENDPOINT);
        }

        public List<ResponseHolder> MakeOverPayment(String accountId)
        {
            Payment payment = new Payment();
            payment.AccountId = accountId;
            payment.Type = "External";
            payment.Status = "Processed";
            payment.Amount = 5;
            payment.AppliedCreditBalanceAmountSpecified = true;
            payment.AppliedCreditBalanceAmount = 5;
            payment.EffectiveDate = DateTime.Now;
            payment.EffectiveDateSpecified = true;
            ResponseHolder qa = zs.Query("SELECT id FROM PaymentMethod WHERE name = 'Other'");
            payment.PaymentMethodId = qa.Objects[0].Id;

            return zs.Create(new List<zObject> { payment }, false);
        }

        public void MakeRefund(SubscribeResponseHolder sub, Invoice inv)
        {
            Refund refund = new Refund();
            refund.AccountId = sub.SubRes.AccountId;
            refund.Amount = inv.Amount;
            refund.AmountSpecified = true;
            refund.Type = "Electronic";
            refund.PaymentId = sub.SubRes.PaymentId;
            List<ResponseHolder> createRes = zs.Create(new List<zObject> { refund }, false);
        }
        public SubscribeResponseHolder MakeSubscription(bool processPayment)
        {
            SubscribeRequest sr = CreatSubscriptionRequest();
            SubscribeOptions so = new SubscribeOptions();
            so.GenerateInvoiceSpecified = true;
            so.GenerateInvoice = true;
            so.ProcessPaymentsSpecified = true;
            so.ProcessPayments = processPayment;
            sr.SubscribeOptions = so;
            SubscribeResponseHolder sub = zs.Subscribe(new List<SubscribeRequest> { sr })[0];
            AddToCleanup(sub.SubRes.AccountId, "Account");
            return sub;
        }
        public SubscribeResponseHolder MakeSubscription(bool processPayment, bool generateInvoice, DateTime date)
        {
            SubscribeRequest sr = CreatSubscriptionRequest(date);
            SubscribeOptions so = new SubscribeOptions();
            so.GenerateInvoiceSpecified = true;
            so.GenerateInvoice = generateInvoice;
            so.ProcessPaymentsSpecified = true;
            so.ProcessPayments = processPayment;
            sr.SubscribeOptions = so;
            SubscribeResponseHolder sub = zs.Subscribe(new List<SubscribeRequest> { sr })[0];
            AddToCleanup(sub.SubRes.AccountId, "Account");
            return sub;
        }
        public AmendRequest CreateAmendRequest(String subscriptionId, String type)
        {
            AmendRequest amendRequest = new AmendRequest();

            Amendment amendment = new Amendment();
            if (type == "tsandcs")
            {
                amendment.Name = "t's and c's amendment";
                amendment.Type = "TermsAndConditions";
                amendment.ContractEffectiveDate = DateTime.Now;
                amendment.ContractEffectiveDateSpecified = true;
                amendment.TermType = "TERMED";
                amendment.InitialTerm = 12;
                amendment.InitialTermSpecified = true;
                amendment.RenewalTerm = 24;
                amendment.RenewalTermSpecified = true;
                amendment.SubscriptionId = subscriptionId;
            }

            amendRequest.Amendments = new Amendment[] { amendment };

            return amendRequest;
        }
        public SubscribeRequest CreatSubscriptionRequest()
        {
            String rateplanid = CreateRatePlanToSubscribe();

            SubscribeRequest subrequest = new SubscribeRequest();

            subrequest.Account = MakeTestAccount();
            subrequest.BillToContact = MakeTestContact();
            subrequest.PaymentMethod = MakeTestPaymentMethod();
            SubscriptionData sd = new SubscriptionData();

            sd.Subscription = MakeTestSubscription();
            RatePlanData rpd = new RatePlanData();
            RatePlan rp = new RatePlan();
            rp.ProductRatePlanId = rateplanid;
            rpd.RatePlan = rp;
            sd.RatePlanData = new RatePlanData[] { rpd };
            subrequest.SubscriptionData = sd;

            return subrequest;
        }
        public SubscribeRequest CreatSubscriptionRequest(DateTime date)
        {
            String rateplanid = CreateRatePlanToSubscribe();

            SubscribeRequest subrequest = new SubscribeRequest();

            subrequest.Account = MakeTestAccount();
            subrequest.BillToContact = MakeTestContact();
            subrequest.PaymentMethod = MakeTestPaymentMethod();
            SubscriptionData sd = new SubscriptionData();

            sd.Subscription = MakeTestSubscription(date);
            RatePlanData rpd = new RatePlanData();
            RatePlan rp = new RatePlan();
            rp.ProductRatePlanId = rateplanid;
            rpd.RatePlan = rp;
            sd.RatePlanData = new RatePlanData[] { rpd };
            subrequest.SubscriptionData = sd;

            return subrequest;
        }
        public String CreateRatePlanToSubscribe()
        {
            //create product
            List<ResponseHolder> product = zs.Create(new List<zObject> { MakeTestProduct() }, false);
            List<ResponseHolder> productrateplan = zs.Create(new List<zObject> { MakeTestProductRatePlan(product[0].Id) }, false);
            List<ResponseHolder> productrateplancharge = zs.Create(new List<zObject> { MakeTestProductRatePlanCharge(productrateplan[0].Id) }, false);
            AddToCleanup(product[0].Id, "Product");
            return productrateplan[0].Id;

        }

        public String CreateProductRatePlanChargeToUpdate(){
            //create product
            List<ResponseHolder> product = zs.Create(new List<zObject> { MakeTestProduct() }, false);
            List<ResponseHolder> productrateplan = zs.Create(new List<zObject> { MakeTestProductRatePlan(product[0].Id) }, false);
            List<ResponseHolder> productrateplancharge = zs.Create(new List<zObject> { MakeTestProductRatePlanCharge(productrateplan[0].Id) }, false);
            AddToCleanup(product[0].Id, "Product");
            return productrateplancharge[0].Id;
        }

        public String CreateTieredProductRatePlanChargeToUpdate()
        {
            List<ResponseHolder> product = zs.Create(new List<zObject> { MakeTestProduct() }, false);
            List<ResponseHolder> productrateplan = zs.Create(new List<zObject> { MakeTestProductRatePlan(product[0].Id) }, false);
            List<ResponseHolder> productrateplancharge = zs.Create(new List<zObject> { MakeTestTieredProductRatePlanCharge(productrateplan[0].Id) }, false);
            return productrateplancharge[0].Id;
        }
        public Account MakeTestAccount()
        {
            Account acc = new Account();
            acc.Name = "ApiTestAccount";
            acc.Currency = "USD";
            acc.PaymentTerm = "Due Upon Receipt";
            acc.BillCycleDay = 1;
            acc.BillCycleDaySpecified = true;
            acc.Status = "Draft";
            acc.Batch = "Batch1";

            return acc;
        }

        public Contact MakeTestContact()
        {
            Contact con = new Contact();
            con.FirstName = "ApiTestFirstName";
            con.LastName = "ApiTestFirstName";
            con.Country = "USA";
            con.State = "CA";

            return con;
        }

        public PaymentMethod MakeTestPaymentMethod()
        {
            PaymentMethod pm = new PaymentMethod();
            pm.CreditCardAddress1 = "somewhere";
            pm.CreditCardCity = "someplace";
            pm.CreditCardCountry = "USA";
            pm.CreditCardExpirationMonthSpecified = true;
            pm.CreditCardExpirationMonth = 1;
            pm.CreditCardExpirationYearSpecified = true;
            pm.CreditCardExpirationYear = 2020;
            pm.CreditCardHolderName = "Test Test";
            pm.CreditCardNumber = "4111111111111111";
            pm.CreditCardPostalCode = "95050";
            pm.CreditCardSecurityCode = "123";
            pm.CreditCardState = "CA";
            pm.CreditCardType = "Visa";
            pm.Type = "CreditCard";

            return pm;
        }

        public Subscription MakeTestSubscription()
        {
            Subscription sub = new Subscription();
            sub.ContractAcceptanceDateSpecified = true;
            sub.ContractAcceptanceDate = DateTime.Now;
            sub.ServiceActivationDateSpecified = true;
            sub.ServiceActivationDate = DateTime.Now;
            sub.ContractEffectiveDateSpecified = true;
            sub.ContractEffectiveDate = DateTime.Now;

            sub.TermType = "TERMED";
            sub.InitialTermSpecified = true;
            sub.InitialTerm = 12;
            sub.RenewalTermSpecified = true;
            sub.RenewalTerm = 12;

            return sub;
        }
        public Subscription MakeTestSubscription(DateTime date)
        {
            Subscription sub = new Subscription();
            sub.ContractAcceptanceDateSpecified = true;
            sub.ContractAcceptanceDate = date;
            sub.ServiceActivationDateSpecified = true;
            sub.ServiceActivationDate = date;
            sub.ContractEffectiveDateSpecified = true;
            sub.ContractEffectiveDate = date;

            sub.TermType = "TERMED";
            sub.InitialTermSpecified = true;
            sub.InitialTerm = 12;
            sub.RenewalTermSpecified = true;
            sub.RenewalTerm = 12;

            return sub;
        }
        public Product MakeTestProduct()
        {
            Product prod = new Product();
            prod.Name = "ApiTestProduct";
            prod.EffectiveStartDateSpecified = true;
            prod.EffectiveStartDate = new DateTime(2000, 1, 1);
            prod.EffectiveEndDateSpecified = true;
            prod.EffectiveEndDate = new DateTime(2050, 1, 1);
            prod.ProductType__c = "iCredits";
            return prod;
        }

        public ProductRatePlan MakeTestProductRatePlan(String pid)
        {
            ProductRatePlan prp = new ProductRatePlan();
            prp.Name = "ApiTestRatePlan";
            prp.ProductId = pid;
            prp.EffectiveStartDateSpecified = true;
            prp.EffectiveStartDate = new DateTime(2000, 1, 1);
            prp.EffectiveEndDateSpecified = true;
            prp.EffectiveEndDate = new DateTime(2050, 1, 1);

            return prp;
        }

        public ProductRatePlanCharge MakeTestProductRatePlanCharge(String prpid)
        {
            ProductRatePlanCharge prpc = new ProductRatePlanCharge();
            prpc.Name = "ApiTestRatePlanCharge";
            prpc.TriggerEvent = "ContractEffective";
            prpc.ProductRatePlanId = prpid;
            prpc.BillingPeriod = "Month";
            prpc.BillCycleDaySpecified = true;
            prpc.BillCycleDay = 1;
            prpc.BillingPeriodAlignment = "AlignToCharge";
            prpc.ProductRatePlanChargeTierData = MakeTestProductRatePlanChargeTierData();
            prpc.ChargeType = "Recurring";
            prpc.ChargeModel = "Per Unit Pricing";
            prpc.DefaultQuantitySpecified = true;
            prpc.DefaultQuantity = 10;
            prpc.UOM = "Each";

            return prpc;
        }

        public ProductRatePlanCharge MakeTestTieredProductRatePlanCharge(String prpid)
        {
            ProductRatePlanCharge prpc = new ProductRatePlanCharge();
            prpc.Name = "ApiTestRatePlanCharge";
            prpc.TriggerEvent = "ContractEffective";
            prpc.ProductRatePlanId = prpid;
            prpc.BillingPeriod = "Month";
            prpc.BillCycleDaySpecified = true;
            prpc.BillCycleDay = 1;
            prpc.BillingPeriodAlignment = "AlignToCharge";
            prpc.ProductRatePlanChargeTierData = MakeTestTieredProductRatePlanChargeTierData();
            prpc.ChargeType = "Recurring";
            prpc.ChargeModel = "Volume Pricing";
            prpc.UOM = "Each";
            return prpc;
        }

        public ProductRatePlanChargeTier[] MakeTestProductRatePlanChargeTierData()
        {
            List<ProductRatePlanChargeTier> tiers = new List<ProductRatePlanChargeTier> { };
            ProductRatePlanChargeTier tier = new ProductRatePlanChargeTier();
            tier.ActiveSpecified = true;
            tier.Active = true;
            tier.Currency = "USD";
            tier.Price = 5;
            tier.PriceSpecified = true;
            tiers.Add(tier);

            return tiers.ToArray();
        }

        public ProductRatePlanChargeTier[] MakeTestTieredProductRatePlanChargeTierData()
        {
            List<ProductRatePlanChargeTier> tiers = new List<ProductRatePlanChargeTier> { };
            ProductRatePlanChargeTier tier = new ProductRatePlanChargeTier();
            tier.ActiveSpecified = true;
            tier.Active = true;
            tier.Currency = "USD";
            tier.Price = 5;
            tier.PriceSpecified = true;
            tier.StartingUnitSpecified = true;
            tier.StartingUnit = 0;
            tier.EndingUnitSpecified = true;
            tier.EndingUnit = 10;
            tiers.Add(tier);

            ProductRatePlanChargeTier tier2 = new ProductRatePlanChargeTier();
            tier2.ActiveSpecified = true;
            tier2.Active = true;
            tier2.Currency = "USD";
            tier2.Price = 10;
            tier2.PriceSpecified = true;
            tier2.StartingUnitSpecified = true;
            tier2.StartingUnit = 11;
            tier2.EndingUnitSpecified = true;
            tier2.EndingUnit = 20;
            tiers.Add(tier2);

            return tiers.ToArray();
        }

        public void CleanUp()
        {
            if (cleanUpMap.ContainsKey("Account"))
            {
                List<String> delIds = new List<String>();
                cleanUpMap.TryGetValue("Account", out delIds);
                zs.Delete(delIds, "Account");
                cleanUpMap.Remove("Account");
            }

            foreach (String k in cleanUpMap.Keys)
            {
                List<String> delIds = new List<String>();
                cleanUpMap.TryGetValue(k, out delIds);
                zs.Delete(delIds, k);
            }

            //query for account with test api name
            String accQueryString = "SELECT Id FROM Account WHERE name='ApiTestAccount' OR name='ChildApiTestAccount'";
            ResponseHolder accQResp = zs.Query(accQueryString);
            List<String> deleteIds = new List<String>();
            if(accQResp.Objects != null)
            foreach (zObject acc in accQResp.Objects)
            {
                if (acc != null)
                    deleteIds.Add(acc.Id);
            }
            List<ResponseHolder> delResp = zs.Delete(deleteIds, "Account");
            //query for the products with api test name
            String prodQueryString = "SELECT Id FROM Product WHERE name='ApiTestProduct'";
            ResponseHolder prodQResp = zs.Query(prodQueryString);
            List<String> prodDeleteIds = new List<String>();

            if (prodQResp.Objects != null)
            {
            foreach (zObject prod in prodQResp.Objects)
            {
                if (prod != null)
                    prodDeleteIds.Add(prod.Id);
            }
            }
            List<ResponseHolder> prodDelResp = zs.Delete(prodDeleteIds, "Product");
        }

        public void AddToCleanup(String id, String type)
        {
            if (!cleanUpMap.ContainsKey(type))
            {
                cleanUpMap.Add(type, new List<String>() { id });
            }
            else
            {
                List<String> tempList = new List<String>();
                cleanUpMap.TryGetValue(type, out tempList);
                tempList.Add(id);
                cleanUpMap[type] = tempList;
            }
        }
    }
}
