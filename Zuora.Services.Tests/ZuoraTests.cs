using System;
using System.Collections.Generic;

using Zuora.Services;
using Xunit;

namespace Zuora.Services.Tests
{
 
    public class ZuoraTests : IDisposable
    {
        ZuoraService zs;
        ZuoraTestHelper zth;
        public ZuoraTests()
        {
            zs = new ZuoraService(Settings.API_USERNAME, Settings.API_PASSWORD, Settings.API_ENDPOINT);
            zth = new ZuoraTestHelper();
        }
        /*
         * This works but takes 5 minutes
        [Fact]
        public void CanWeQueryMoreThan2000Objects()
        {
            Account tempAcc = MakeTestAccount();
            try
            {
                List<zObject> accs = new List<zObject>();
                for (int i = 0; i < 2050; i++)
                {
                    tempAcc.Name = tempAcc.Name;
                    accs.Add(tempAcc);
                }
                List<ResponseHolder> createResp = zs.Create(accs, false);
                foreach (ResponseHolder cr in createResp)
                {
                    AddToCleanup(cr.Id, "Account");
                }
                ResponseHolder qRes = zs.Query("SELECT id FROM Account");
                Assert.True(qRes.Success && qRes.Objects.Count > 2000);
            }
            finally
            {
                CleanUp();
            }
        }
        */
        [Fact]
        public void CanWeUpdateMoreThanFiftyObjectsAtOnce()
        {
            Account tempAcc = zth.MakeTestAccount();

            List<zObject> accs = new List<zObject>();
            for (int i = 0; i < 55; i++)
            {
                tempAcc.Name = tempAcc.Name;
                accs.Add(tempAcc);
            }
            List<ResponseHolder> createResp = zs.Create(accs, false);

            List<zObject> updatedAccs = new List<zObject>();
            foreach (ResponseHolder rh in createResp)
            {
                Account updateAccount = new Account();
                updateAccount.Id = rh.Id;
                updateAccount.Notes += updateAccount.Name + " updated";
                updatedAccs.Add(updateAccount);
            }
            List<ResponseHolder> updateResp = zs.Update(updatedAccs);
            foreach (ResponseHolder rh in updateResp)
            {
                Assert.True(rh.Success);
            }
        }

        [Fact]
        public void CanWeCreateMoreThanFiftyObjectsAtOnce()
        {
            try
            {
                List<zObject> accs = new List<zObject>();
                for (int i = 0; i < 105; i++)
                {
                    Account tempAcc = zth.MakeTestAccount();
                    tempAcc.Name = tempAcc.Name + i;
                    accs.Add(tempAcc);
                }
                List<ResponseHolder> createResp = zs.Create(accs, false);
                foreach (ResponseHolder cr in createResp)
                {
                    Assert.True(cr.Success);
                    zth.AddToCleanup(cr.Id, "Account");
                }
            }
            finally
            {
                zth.CleanUp();
            }
        }
        /* This works but takes 1 minute
        [Fact]
        public void CanWeSubscribeToMoreThanFiftySubscriptionsAtOnce()
        {
            try
            {
                SubscribeRequest tempsr = CreatSubscriptionRequest();
                List<SubscribeRequest> subRequestList = new List<SubscribeRequest>();
                for (int i = 0; i < 55; i++)
                {
                    subRequestList.Add(tempsr); 
                }
                List<ResponseHolder> subRes = zs.Subscribe(subRequestList);
                foreach (ResponseHolder sr in subRes)
                {
                    AddToCleanup(sr.SubRes.AccountId, "Account");
                    Assert.True(sr.SubRes.Success);
                }
            }
            finally
            {
                CleanUp();
            }
        }
        */

        [Fact]
        public void CanWeAmendInZuora()
        {
            List<SubscribeResponseHolder> subRes = zs.Subscribe(new List<SubscribeRequest> { zth.CreatSubscriptionRequest() });
            if (subRes[0].Success)
            {
                zth.AddToCleanup(subRes[0].SubRes.AccountId, "Account");
            }
            List<AmendResponseHolder> amendRes = zs.Amend(new List<AmendRequest>() { zth.CreateAmendRequest(subRes[0].SubRes.SubscriptionId, "tsandcs") });
            Assert.True(amendRes[0].Success);
        }

        [Fact]
        public void CanWeSubscribeInZuora()
        {
            List<SubscribeResponseHolder> subRes = zs.Subscribe(new List<SubscribeRequest> { zth.CreatSubscriptionRequest() });
            if (subRes[0].Success)
            {
                zth.AddToCleanup(subRes[0].SubRes.AccountId, "Account");
            }
            Assert.True(subRes[0].Success);
        }

        [Fact]
        public void CanWeUpdateInZuora()
        {
            List<ResponseHolder> results = zs.Create(new List<zObject> { zth.MakeTestAccount() }, false);
            foreach (ResponseHolder res in results)
            {
                if (res.Success)
                {
                    Account acc = new Account();
                    acc.Notes = "Updating Notes";
                    acc.Id = res.Id;
                    //update
                    List<ResponseHolder> temp = zs.Update(new List<zObject> { acc });
                    foreach (ResponseHolder resp in temp)
                    {
                        Assert.True(resp.Success);
                        //get the account from z
                        if (resp.Success)
                        {
                            ResponseHolder qRes = zs.Query("SELECT id, name FROM Account WHERE id='" + resp.Id + "'");
                            Account tempAcc = (Account)qRes.Objects[0];
                            Assert.Equal(tempAcc.Name, "ApiTestingAccountUpdate");
                            zth.AddToCleanup(resp.Id, "Account");
                        }
                    }
                }
            }
        }

        [Fact]
        public void CanWeDeleteFromZuora()
        {
            List<ResponseHolder> results = zs.Create(new List<zObject> { zth.MakeTestAccount() }, false);
            foreach (ResponseHolder res in results)
            {
                if (res.Success)
                {
                    List<ResponseHolder> temp = zs.Delete(new List<String> { res.Id }, "Account");
                    foreach (ResponseHolder resp in temp)
                    {
                        Assert.True(resp.Success);
                    }
                }
            }
        }

        [Fact]
        public void CanWeCreateInZuora()
        {
            List<ResponseHolder> results = zs.Create(new List<zObject> { zth.MakeTestAccount() }, false);
            foreach (ResponseHolder res in results)
            {
                Assert.True(res.Success);
                zth.AddToCleanup(res.Id, "Account");
                ResponseHolder qRes = zs.Query("SELECT id FROM Account WHERE id='" + res.Id + "'");
                Assert.True(qRes.Success);
                Assert.True(qRes.Objects.Count == 1);
            }
        }

        [Fact]
        public void CanWeConnectToZuora()
        {
            ResponseHolder res = zs.Login();
            Assert.True(res.Success);
        }

        [Fact]
        public void CanWeQueryZuora()
        {
            //theres always default payment methods in z
            ResponseHolder res = zs.Query("SELECT Id FROM PaymentMethod");
            Assert.Null(res.Message);
            Assert.True(res.Success);
            
            Assert.NotNull(res.Objects);
        }

        
        public  void Dispose()
        {
            ZuoraTestHelper zth = new ZuoraTestHelper();
            zth.CleanUp();
        }
    }
}
