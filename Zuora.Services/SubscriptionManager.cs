using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zuora.Services
{  
    public class SubscriptionManager
    {
        ZuoraService zs;
        public SubscriptionManager(ZuoraService zs)
        {
            this.zs = zs;
        }

        /*public ResponseHolder GetCurrentSubscription(String subscriptionId)
        {
            String subscriptionQuery = "SELECT Id,Name,Status,Version,PreviousSubscriptionId,ContractEffectiveDate,TermStartDate FROM Subscription WHERE AccountId='" + subscriptionId + "' AND Status='Active'AND Status='Active'";
            Subscription activeSubscription = (Subscription)zs.Query(subscriptionQuery).Objects[0];

            String RatePlanString = "SELECT Id,Name,ProductRatePlanId FROM RatePlan WHERE SubscriptionId='" + activeSubscription.Id + "'";
            ResponseHolder ratePlans = zs.Query(RatePlanString);

            foreach (zObject ratePlan in ratePlans.Objects)
            {

            }
        }*/

        public SubscribeResponseHolder Subscribe(Account acc, Contact con, PaymentMethod pay, List<ProductRatePlanHolder> productRatePlans, Subscription sub, PreviewOptions po, SubscribeOptions so)
        {
            SubscribeRequest subscribeRequest = new SubscribeRequest();
            subscribeRequest.SubscribeOptions = so;
            subscribeRequest.PreviewOptions = po;

            subscribeRequest.Account = acc;
            subscribeRequest.BillToContact = con;
            //Payment Method isn't required to subscribe
            if (pay != null)
            {
                subscribeRequest.PaymentMethod = pay;
            }

            SubscriptionData subData = new SubscriptionData();
            sub.AccountId = acc.Id;
            subData.Subscription = sub;
            List<RatePlanData> rpdList = new List<RatePlanData>();

            foreach (ProductRatePlanHolder prph in productRatePlans)
            {

                RatePlanData ratePlanData = new RatePlanData();
                RatePlan ratePlan = new RatePlan();
                ratePlan.ProductRatePlanId = prph.ProductRatePlan.Id;
                ratePlanData.RatePlan = ratePlan;
                List<RatePlanChargeData> rpcData = new List<RatePlanChargeData>();
                if (prph.ProductRatePlanCharges != null)
                {
                    foreach (ProductRatePlanChargeHolder prpch in prph.ProductRatePlanCharges)
                    {
                        ProductRatePlanCharge prpc = prpch.ProductRatePlanCharge;
                        //set quantity and add chargeData if applicable
                        if ((prpc.ChargeModel.Equals("Per Unit Pricing") || prpc.ChargeModel.Equals("Tiered Pricing") || prpc.ChargeModel.Equals("Volume Pricing")))
                        {
                            RatePlanChargeData ratePlanChargeData = new RatePlanChargeData();
                            RatePlanCharge ratePlanCharge = new RatePlanCharge();
                            ratePlanCharge.ProductRatePlanChargeId = prpc.Id;
                            ratePlanCharge.QuantitySpecified = true;
                            ratePlanCharge.Quantity = prpc.DefaultQuantity;
                            ratePlanChargeData.RatePlanCharge = ratePlanCharge;
                            rpcData.Add(ratePlanChargeData);
                        }
                    }
                }
                if (rpcData.Count != 0)
                {
                    ratePlanData.RatePlanChargeData = rpcData.ToArray();
                }
                rpdList.Add(ratePlanData);

            }

            subData.RatePlanData = rpdList.ToArray();
            subscribeRequest.SubscriptionData = subData;
            return zs.Subscribe(new List<SubscribeRequest> { subscribeRequest })[0];
        }

        public SubscribeResponseHolder SubscribeWithExisitingAccount(String accountId, List<ProductRatePlanHolder> productRatePlans, Subscription sub, PreviewOptions po, SubscribeOptions so)
        {
            SubscribeRequest subscribeRequest = new SubscribeRequest();
            subscribeRequest.SubscribeOptions = so;
            subscribeRequest.PreviewOptions = po;

            Account acc = new Account();
            acc.Id = accountId;
            subscribeRequest.Account = acc;

            SubscriptionData subData = new SubscriptionData();
            sub.AccountId = accountId;
            subData.Subscription = sub;

            List<RatePlanData> rpdList = new List<RatePlanData>();

            foreach (ProductRatePlanHolder prph in productRatePlans)
            {
                
                RatePlanData ratePlanData = new RatePlanData();
                RatePlan ratePlan = new RatePlan();
                ratePlan.ProductRatePlanId = prph.ProductRatePlan.Id;
                ratePlanData.RatePlan = ratePlan;
                List<RatePlanChargeData> rpcData = new List<RatePlanChargeData>();
                foreach(ProductRatePlanChargeHolder prpch in prph.ProductRatePlanCharges)
                {
                    ProductRatePlanCharge prpc = prpch.ProductRatePlanCharge;
                    //set quantity and add chargeData if applicable
                    if ((prpc.ChargeModel.Equals("Per Unit Pricing") || prpc.ChargeModel.Equals("Tiered Pricing") || prpc.ChargeModel.Equals("Volume Pricing")))
                    {
                        RatePlanChargeData ratePlanChargeData = new RatePlanChargeData();
                        RatePlanCharge ratePlanCharge = new RatePlanCharge();
                        ratePlanCharge.ProductRatePlanChargeId = prpc.Id;
                        ratePlanCharge.QuantitySpecified = true;
                        ratePlanCharge.Quantity = prpc.DefaultQuantity;
                        ratePlanChargeData.RatePlanCharge = ratePlanCharge;
                        rpcData.Add(ratePlanChargeData);
                    }
                }
                if (rpcData.Count != 0)
                {
                    ratePlanData.RatePlanChargeData = rpcData.ToArray();
                }
                rpdList.Add(ratePlanData);

            }

            subData.RatePlanData = rpdList.ToArray();
            subscribeRequest.SubscriptionData = subData;
            return zs.Subscribe(new List<SubscribeRequest>{subscribeRequest})[0];
        }

        public AmendResponseHolder DoAddProductAmendment(String subscriptionId, DateTime effectiveDate, String productRatePlanId, AmendOptions ao = null, PreviewOptions po = null)
        {
            AmendRequest amendRequest = new AmendRequest();
            Amendment amendment = new Amendment();

            amendment.Name = "Add Product Amendment";
            amendment.Type = "NewProduct";
            amendment.ContractEffectiveDate = effectiveDate;
            amendment.ContractEffectiveDateSpecified = true;
            amendment.SubscriptionId = subscriptionId;

            RatePlanData ratePlanData = new RatePlanData();
            RatePlan ratePlan = new RatePlan();
            ratePlan.ProductRatePlanId = productRatePlanId;
            ratePlanData.RatePlan = ratePlan;

            amendment.RatePlanData = ratePlanData;
            if (ao != null)
            {
                amendRequest.AmendOptions = ao;
            }
            if (po != null)
            {
                amendRequest.PreviewOptions = po;
            }
            amendRequest.Amendments = new Amendment[] { amendment };
            return zs.Amend(new List<AmendRequest> { amendRequest })[0];
        }

        public AmendResponseHolder DoRenewalAmendment(String subscriptionId, DateTime effectiveDate, AmendOptions ao = null, PreviewOptions po = null)
        {
            AmendRequest amendRequest = new AmendRequest();
            Amendment amendment = new Amendment();
            if (ao != null)
            {
                amendRequest.AmendOptions = ao;
            }
            if (po != null)
            {
                amendRequest.PreviewOptions = po;
            }

            amendment.Name = "Renewal amendment";
            amendment.Type = "Renewal";
            amendment.ContractEffectiveDate = effectiveDate;
            amendment.ContractEffectiveDateSpecified = true;
            amendment.SubscriptionId = subscriptionId;

            amendRequest.Amendments = new Amendment[] { amendment };
            return zs.Amend(new List<AmendRequest> { amendRequest })[0];
        }

        public AmendResponseHolder DoTermsAndConditionsAmendment(String subscriptionId, DateTime effectiveDate, String termType, int initialTerm, int renewalTerm, AmendOptions ao = null, PreviewOptions po = null)
        {
            AmendRequest amendRequest = new AmendRequest();
            Amendment amendment = new Amendment();
            if (ao != null)
            {
                amendRequest.AmendOptions = ao;
            }
            if (po != null)
            {
                amendRequest.PreviewOptions = po;
            }

            amendment.Name = "T's and C's amendment";
            amendment.Type = "TermsAndConditions";
            amendment.ContractEffectiveDate = effectiveDate;
            amendment.ContractEffectiveDateSpecified = true;
            if(termType != null)
                amendment.TermType = termType;
            amendment.InitialTerm = initialTerm;
            amendment.InitialTermSpecified = true;
            amendment.RenewalTerm = renewalTerm;
            amendment.RenewalTermSpecified = true;
            amendment.SubscriptionId = subscriptionId;
            
            amendRequest.Amendments = new Amendment[] { amendment };
            return zs.Amend(new List<AmendRequest> { amendRequest })[0];

        }

        public SubscriptionInfoHolder GetSubscriptionAndChargeInfo(String subscriptionId)
        {
            SubscriptionInfoHolder sih = new SubscriptionInfoHolder();
            sih.RatePlanList = new List<RatePlan>();
            sih.RatePlanChargeList = new List<RatePlanCharge>();
            sih.RatePlanChargeTierList = new List<RatePlanChargeTier>();
            sih.Success = true;
            //get the subscription
            String subQueryString = "SELECT id, AutoRenew, ContractEffectiveDate, ContractAcceptanceDate, InitialTerm, RenewalTerm, ServiceActivationDate, Status,"
            + "SubscriptionStartDate, SubscriptionEndDate, TermEndDate, TermStartDate, TermType, Version" 
            + " FROM Subscription WHERE id = '" + subscriptionId + "' AND Status = 'Active'";
            ResponseHolder qSubRes = zs.Query(subQueryString);
            if (qSubRes.Success && qSubRes.Objects.Count != 0)
            {
                sih.Subscription = (Subscription)qSubRes.Objects[0];
            }
            else
            {
                sih.Success = false;
                sih.Message = qSubRes.Message;
                return sih;
            }
            //get the rate plans for the subscription
            String ratePlanQueryString = "SELECT id, ProductRatePlanId, Name FROM RatePlan WHERE SubscriptionId='" + subscriptionId + "'";
            ResponseHolder qRpRes = zs.Query(ratePlanQueryString);
            if (qRpRes.Success)
            {
                foreach (zObject zo in qRpRes.Objects)
                {
                    RatePlan rp = (RatePlan)zo;
                    sih.RatePlanList.Add(rp);
                }
            }
            else
            {
                sih.Success = false;
                sih.Message = qRpRes.Message;
                return sih;
            }
            //get the charges for each rate plan
            foreach (zObject zo in qRpRes.Objects)
            {
                RatePlan rp = (RatePlan) zo;
                String ratePlanChargeQueryString = "SELECT id, ChargedThroughDate, Description, EffectiveEndDate, EffectiveStartDate, Name, Price, Quantity, UOM FROM RatePlanCharge where RatePlanId = '" + rp.Id + "'";
                ResponseHolder rpcRes = zs.Query(ratePlanChargeQueryString);
                if (rpcRes.Success)
                {
                    foreach (zObject rpc in rpcRes.Objects)
                    {
                        RatePlanCharge temprpc = (RatePlanCharge)rpc;
                        sih.RatePlanChargeList.Add(temprpc);
                    }
                }
                else
                {
                    sih.Success = false;
                    sih.Message = rpcRes.Message;
                    return sih;
                }
                //get the tiers for each charge
                foreach (zObject trpc in rpcRes.Objects)
                {
                    RatePlanCharge rpc = (RatePlanCharge)trpc;
                    String tierQueryString = "SELECT Id, EndingUnit, IsOveragePrice, Price, PriceFormat, RatePlanChargeId, StartingUnit, Tier FROM RatePlanChargeTier WHERE RatePlanChargeId='" + rpc.Id + "'";
                    ResponseHolder tierRes = zs.Query(tierQueryString);
                    if (tierRes.Success)
                    {
                        foreach (zObject tier in tierRes.Objects)
                        {
                            RatePlanChargeTier rpct = (RatePlanChargeTier)tier;
                            sih.RatePlanChargeTierList.Add(rpct);
                        }
                    }
                    else
                    {
                        sih.Success = false;
                        sih.Message = tierRes.Message;
                        return sih;
                    }
                }
            }
            return sih;
        }
    }
}
