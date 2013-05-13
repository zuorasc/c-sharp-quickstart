using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zuora.Services
{
    public class AccountManager
    {
        ZuoraService zs;
        PaymentManager pm;
        public AccountManager(ZuoraService zs)
        {
            this.pm = new PaymentManager(zs);
            this.zs = zs;
        }

        public ResponseHolder CreateAccount(Account acc, Contact con)
        {

            acc.Status = "Draft";
            ResponseHolder accResp = zs.Create(acc);
            if (accResp.Success)
            {
                con.AccountId = accResp.Id;
                ResponseHolder conResp = zs.Create(con);
                if (conResp.Success)
                {
                    Account newAcc = new Account();
                    newAcc.Id = accResp.Id;
                    newAcc.Status = "Active";
                    newAcc.SoldToId = conResp.Id;
                    newAcc.BillToId = conResp.Id;

                    return zs.Update(new List<zObject>{newAcc})[0];

                }
                else
                {
                    return conResp;
                }
            }
            else
            {
                return accResp;
            }

        }
        public Boolean DoesAccountNameExistInZuora(String name)
        {
            String accQueryString = "SELECT id FROM account WHERE name = '" + name + "'";
            ResponseHolder qRes = zs.Query(accQueryString);

            List<zObject> tempList = qRes.Objects;

            if (tempList == null || tempList.Count == 0)
            {
                   return false;
            }
            return true;
        }

        /// <summary>
        /// Return the child account of a parent given the parents id
        /// </summary>
        /// <param name="parentAccountId"></param>
        /// <returns></returns>
        public ResponseHolder GetChildAccount(String parentAccountId)
        {
            String accQueryString = "SELECT id, ParentId, Name, purchaseordernumber, AutoPay, Balance, BillCycleDay, BillToId, CreditBalance, SoldToId, TaxExemptStatus, TotalInvoiceBalance, InvoiceTemplateId FROM account WHERE parentId = '" + parentAccountId + "'";
            return zs.Query(accQueryString);
        }
        /// <summary>
        /// Create a child account, set the parent id, create the contact, update the account and increase the credit balance
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="parentAccountId"></param>
        /// <param name="con"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public ResponseHolder CreateChildAccountAndIncreaseCreditBalance(Account acc, String parentAccountId, Contact con, Decimal amount)
        {
            acc.ParentId = parentAccountId;
            acc.Status = "Draft";
            //create account
            ResponseHolder accRes = zs.Create(new List<zObject>{acc}, false)[0];
            if (!accRes.Success)
            {
                ResponseHolder error = new ResponseHolder();
                error.Success = false;
                error.Message = accRes.Message;
                return error;
            }
            //set the account id on the contact
            con.AccountId = accRes.Id;
            //create contact
            ResponseHolder conRes = zs.Create(new List<zObject>{con}, false)[0];
            if (!conRes.Success)
            {
                ResponseHolder error = new ResponseHolder();
                error.Success = false;
                error.Message = conRes.Message;
                return error;
            }
            //update to active
            Account draftAcc = new Account();
            draftAcc.Id = accRes.Id;
            draftAcc.Status = "Active";
            draftAcc.SoldToId = conRes.Id;
            draftAcc.BillToId = conRes.Id;
            ResponseHolder createRes = zs.Update(new List<zObject>{draftAcc})[0];
            if (!createRes.Success)
            {
                ResponseHolder error = new ResponseHolder();
                error.Success = false;
                error.Message = createRes.Message;
                return error;
            }
            //increase credit balance
            ResponseHolder cRes = pm.IncreaseCreditBalance(accRes.Id, amount);
            if (!cRes.Success)
            {
                ResponseHolder error = new ResponseHolder();
                error.Success = false;
                error.Message = cRes.Message;
                return error;
            }

            return accRes;
        }
        /// <summary>
        /// Set the parent account of an account
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="childId"></param>
        /// <returns></returns>
        public ResponseHolder SetParentAccount(String parentId, String accountId)
        {
            Account account = new Account();
            account.Id = accountId;
            account.ParentId = parentId;
            
            return zs.Update(new List<zObject>{account})[0];
        }
        /// <summary>
        /// Get a single invoice and Base64 encoded pdf body
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public ResponseHolder GetSingleInvoice(String invoiceId)
        {
            String singleInvoiceQueryString = "SELECT Id, InvoiceNumber, Amount, DueDate, Body, Balance, InvoiceDate FROM Invoice WHERE Id = '" + invoiceId + "'";
            return zs.Query(singleInvoiceQueryString);
        }
        /// <summary>
        /// Get the invoices for an account based on accountId
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public ResponseHolder GetInvoicesForAccount(String accountId)
        {
            String invoiceQueryString = "SELECT Id, InvoiceNumber, Amount, DueDate, Balance, InvoiceDate, Status FROM Invoice WHERE AccountId = '" + accountId + "'";
            return zs.Query(invoiceQueryString);
        }
        /// <summary>
        /// Get the pdf version for an invoice
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public ResponseHolder GetInvoicePDFForAccount(String invoiceId)
        {
            String invoiceBodyQueryString = "SELECT Id, AccountId, Amount, Balance, DueDate, InvoiceDate, InvoiceNumber, Status, TargetDate, Body FROM Invoice WHERE Id= '" + invoiceId + "'";
            return zs.Query(invoiceBodyQueryString);
        }
        /// <summary>
        /// Get Refunds based on accountId
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public ResponseHolder GetRefundsForAccount(String accountId)
        {
            String refundQueryString = "SELECT id, amount FROM Refund WHERE accountid = '" + accountId + "'";
            return zs.Query(refundQueryString);
        }
        /// <summary>
        /// Get Credit balance based on accountId
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public ResponseHolder GetCreditBalanceAdjustmentsForAccount(String accountId)
        {
            String creditBalanceQueryString = "SELECT id, accountId, amount, type FROM CreditBalanceAdjustment WHERE accountId = '" + accountId + "'";
            return zs.Query(creditBalanceQueryString);
        }
        /// <summary>
        /// Get Credit Card based on parent account id, only the parent account will have Credit Cards
        /// </summary>
        /// <param name="parentAccountId"></param>
        /// <returns></returns>
        public ResponseHolder GetCreditCardsForAccount(String parentAccountId)
        {
            String paymentMethodQueryString = "SELECT Id, Type, CreditCardHolderName, CreditCardExpirationYear, CreditCardExpirationMonth FROM PaymentMethod WHERE AccountId = '" + parentAccountId + "' AND Type = 'CreditCard'";
            return zs.Query(paymentMethodQueryString);
        }
        /// <summary>
        /// Get P.O. on file, only the parent account will have Credit Cards
        /// </summary>
        /// <param name="parentAccountId"></param>
        /// <returns></returns>
        public String GetPOForAccount(String parentAccountId)
        {
            String poQueryString = "SELECT id, purchaseordernumber FROM account WHERE Id = '" + parentAccountId + "'";
            ResponseHolder qRes = zs.Query(poQueryString);
            if(qRes.Success)
            {
                Account acc = (Account) qRes.Objects[0];
                if(acc.PurchaseOrderNumber != null)
                {
                    return acc.PurchaseOrderNumber;
                }
            }
            return null;
        }
        /// <summary>
        /// Get Account information based on accountId
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public ResponseHolder GetAccount(String accountId)
        {
            String accQueryString = "SELECT id, ParentId, Name, purchaseordernumber, AutoPay, Balance, BillCycleDay, BillToId, CreditBalance, SoldToId, TaxExemptStatus, TotalInvoiceBalance, InvoiceTemplateId FROM account WHERE Id = '" + accountId + "'";
            return zs.Query(accQueryString);
        }
        /// <summary>
        /// Get Contacts based on accountId
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public ResponseHolder GetContact(String accountId)
        {
            String contactQueryString = "SELECT id, FirstName, LastName, Address1, Address2, City, WorkEmail, WorkPhone, " +
            "PostalCode, State, Country, County, Description, Fax, HomePhone, PostalCode FROM Contact WHERE AccountId='" + accountId + "'";
            return zs.Query(contactQueryString);
        }
        /// <summary>
        /// Upade Contact on account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="lastName"></param>
        /// <param name="firstName"></param>
        /// <param name="address"></param>
        /// <param name="state"></param>
        /// <param name="city"></param>
        /// <param name="country"></param>
        /// <param name="postalCode"></param>
        /// <returns></returns>
        public ResponseHolder UpdateContact(String accountId, String lastName, String firstName, String address, String city, String state, String postalCode, String country)
        {
            ResponseHolder qRes = GetContact(accountId);
            
            if(qRes.Success)
            {
                Contact contact = new Contact();
                   
                if((Contact) qRes.Objects[0] != null)
                {
                    contact.Id = (String) qRes.Objects[0].Id;
                    contact.LastName = lastName;
                    contact.FirstName = firstName;
                    contact.Address1 = address;
                    contact.City = city;
                    contact.State = state;
                    contact.PostalCode = postalCode;
                    contact.Country = country;

                    return zs.Update(new List<zObject> { contact })[0];
                }
            }
            return null;
         }
        /// <summary>
        /// Change the invoice template id for an account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="templateId"></param>
        /// <returns></returns>
        public ResponseHolder ChangeInvoiceTemplate(String accountId, String templateId)
        {
            Account acc = new Account();
            acc.Id = accountId;
            acc.InvoiceTemplateId = templateId;
            return zs.Update(new List<zObject> { acc })[0];
        }
        /// <summary>
        /// Get Credit Cards based on accountId
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public ResponseHolder GetCreditCardsPaymentMethods(String accountId)
        {
            String creditCardsQueryString = "Select Id,CreditCardHolderName,CreditCardMaskNumber,CreditCardExpirationYear,CreditCardExpirationMonth,CreditCardType from PaymentMethod WHERE AccountId='" + accountId + "'";
            return zs.Query(creditCardsQueryString);
        }

        /// <summary>
        /// Delete Payment Method
        /// </summary>
        /// <param name="paymentMethodId"></param>
        /// <returns></returns>
        public List<ResponseHolder> DeletePaymentMethod(String paymentMethodId)
        {
            return zs.Delete(new List<String>{paymentMethodId}, "PaymentMethod");           
        }
    }
}
