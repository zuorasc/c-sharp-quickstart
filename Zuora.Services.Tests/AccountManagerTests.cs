using System;
using System.Collections.Generic;

using Zuora.Services;
using Xunit;

namespace Zuora.Services.Tests
{
 
    public class AccountManagerTests : IDisposable
    {
        AccountManager am;
        ZuoraService zs;
        ZuoraTestHelper zth;

        public AccountManagerTests()
        {
            zs = new ZuoraService(Settings.API_USERNAME, Settings.API_PASSWORD, Settings.API_ENDPOINT);
            am = new AccountManager(zs);
            zth = new ZuoraTestHelper();
        }
        [Fact]
        public void CanWeCreateAnAccountAndContactInZuora()
        {
            Account acc = zth.MakeTestAccount();
            Contact con = zth.MakeTestContact();
            var res = am.CreateAccount(acc, con);
            Assert.True(res.Success);
        }
        [Fact]
        public void CanWeCheckToSeeIfAnAccountExistsInZuora()
        {
            zth.CleanUp();
            bool firstRes = am.DoesAccountNameExistInZuora("ApiTestAccount");
            Assert.False(firstRes);
            String parentAccId = zth.MakeSubscription(true).SubRes.AccountId;

            bool secondRes = am.DoesAccountNameExistInZuora("ApiTestAccount");
            Assert.True(secondRes);
        }

        [Fact]
        public void CanWeGetTheChildAccountForAParent()
        {
            String parentAccId = zth.MakeSubscription(true).SubRes.AccountId;
            Account acc = zth.MakeTestAccount();
            acc.Name = "ChildApiTestAccount";
            Contact con = zth.MakeTestContact();
            con.FirstName = "child";
            ResponseHolder createRes = am.CreateChildAccountAndIncreaseCreditBalance(acc, parentAccId, con, 100);

            ResponseHolder qRes = am.GetChildAccount(parentAccId);
            Assert.True(qRes.Objects[0].Id == createRes.Id);

        }
        [Fact]
        public void CanWeCreateChildAccountAndIncreaseCreditBalance()
        {
            
            String parentAccId = zth.MakeSubscription(true).SubRes.AccountId;
            
            Account acc = zth.MakeTestAccount();
            acc.Name = "ChildApiTestAccount";
            Contact con = zth.MakeTestContact();
            con.FirstName = "child";

            ResponseHolder createRes = am.CreateChildAccountAndIncreaseCreditBalance(acc, parentAccId, con, 100);

            ResponseHolder qRes = am.GetAccount(createRes.Id);
            Assert.True(((Account)qRes.Objects[0]).CreditBalance == 100);
            Assert.True(((Account)qRes.Objects[0]).ParentId == parentAccId);
        }
        [Fact]
        public void CanWeSetTheParentAccount()
        {
            String accId1 = zth.MakeSubscription(true).SubRes.AccountId;
            String accId2 = zth.MakeSubscription(true).SubRes.AccountId;
            ResponseHolder updateRes = am.SetParentAccount(accId1, accId2);

            ResponseHolder qRes = am.GetAccount(accId2);
            Assert.Equal(((Account)qRes.Objects[0]).ParentId, accId1);
        }

        [Fact]
        public void CanWeGetTheInvoiceBodyForASingleInvoice()
        {
            String invId = zth.MakeSubscription(true).SubRes.InvoiceId;
            ResponseHolder qRes = am.GetSingleInvoice(invId);
            Assert.NotNull(((Invoice)qRes.Objects[0]).Body);
        }

        [Fact]
        public void CanWeChangeInvoiceTemplateIdForAccount()
        {
            String accId = zth.MakeSubscription(true).SubRes.AccountId;
            //this has to be a real template id in zuora
            String invoiceTemlateId = "2c92c0f83994fbe8013998c1e10216e7";
            if (!invoiceTemlateId.Equals(""))
            {
                ResponseHolder updateRes = am.ChangeInvoiceTemplate(accId, invoiceTemlateId);

                Assert.True(updateRes.Success);
                
            }
            ResponseHolder qRes = am.GetAccount(accId);
            Assert.Equal(((Account)qRes.Objects[0]).InvoiceTemplateId, invoiceTemlateId);
        }
        [Fact]
        public void CanWeGetContact()
        {
            String accId = zth.MakeSubscription(true).SubRes.AccountId;
            ResponseHolder qRes = am.GetContact(accId);
            Assert.True(qRes.Success && qRes.Objects.Count > 0);

        }
        [Fact]
        public void CanWeGetAccount()
        {
            String accId = zth.MakeSubscription(true).SubRes.AccountId;
            ResponseHolder qRes = am.GetAccount(accId);
            Assert.True(qRes.Success && qRes.Objects.Count > 0);
        }
        [Fact]
        public void CanWeGetPOForAccount()
        {
            Account acc = zth.MakeTestAccount();
            acc.PurchaseOrderNumber = "123";
            List<ResponseHolder> createRes = zs.Create(new List<zObject>{acc}, false);
            String PO = am.GetPOForAccount(createRes[0].Id);
            Assert.Equal(PO, "123");
        }
        [Fact]
        public void CanWeGetCreditCardsForAccount()
        {
            String accId = zth.MakeSubscription(true).SubRes.AccountId;
            ResponseHolder queryRes = am.GetCreditCardsForAccount(accId);
            Assert.True(queryRes.Success && queryRes.Objects.Count > 0);
        }
        [Fact]
        public void CanWeGetCreditBalanceAdjustmentsForAnAccount()
        {
            String accId = zth.MakeSubscription(true).SubRes.AccountId;
            //create an overpayment for the account
            List<ResponseHolder> createRes = zth.MakeOverPayment(accId);
            //query the Credit Balance Adjustments
            ResponseHolder queryRes = am.GetCreditBalanceAdjustmentsForAccount(accId);
            Assert.True(queryRes.Success && queryRes.Objects.Count > 0);
        }
        [Fact]
        public void CanWeGetRefundsForAnAccount()
        {
            SubscribeResponseHolder sub = zth.MakeSubscription(true);
            ResponseHolder invRes = am.GetInvoicesForAccount(sub.SubRes.AccountId);
            Invoice inv = (Invoice)invRes.Objects[0];
            zth.MakeRefund(sub, inv);
            //query the refund
            ResponseHolder queryRes = am.GetRefundsForAccount(sub.SubRes.AccountId);
            Assert.True(queryRes.Success && queryRes.Objects.Count > 0);
        }
        [Fact]
        public void CanWeGetInvoicesForAnAccount()
        {
            SubscribeResponseHolder sub = zth.MakeSubscription(true);
            ResponseHolder queryRes = am.GetInvoicesForAccount(sub.SubRes.AccountId);
            Assert.True(queryRes.Success && queryRes.Objects.Count > 0);
        }

      
        public void Dispose()
        {
            ZuoraTestHelper zth = new ZuoraTestHelper();
            zth.CleanUp();
        }
    }
}


