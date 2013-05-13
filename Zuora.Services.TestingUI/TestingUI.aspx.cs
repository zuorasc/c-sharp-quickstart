using Zuora.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Zuora.Services.TestingUI
{
    public partial class TestingUI : System.Web.UI.Page
    {
        ZuoraService zs;
        String loginResStr;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ListItem l3 = new ListItem("-", "-", true);
                ListItem l = new ListItem("Create Child Account And Increase Credit Balance", "Create Child Account And Increase Credit Balance", true);
                ListItem l2 = new ListItem("Create Account", "Create Account", true);
                ListItem l4 = new ListItem("Cancel Credit Balance Adjustment On Invoice", "Cancel Credit Balance Adjustment On Invoice", true);
                ListItem l5 = new ListItem("Get Invoices For Account", "Get Invoices For Account", true);
                ListItem l6 = new ListItem("Get Refunds For Account", "Get Refunds For Account", true);
                ListItem l7 = new ListItem("Get Credit Balance Adjustments For Account", "Get Credit Balance Adjustments For Account", true);
                ListItem l8 = new ListItem("Get Credit Cards For Account", "Get Credit Cards For Account", true);
                ListItem l9 = new ListItem("Get PO For Account", "Get PO For Account", true);
                ListItem l10 = new ListItem("Get Account", "Get Account", true);
                ListItem l11 = new ListItem("Get Contact", "Get Contact", true);
                ListItem l12 = new ListItem("Change Invoice Template", "Change Invoice Template", true);
                ListItem l13 = new ListItem("Does Account Name Exist In Zuora", "Does Account Name Exist In Zuora", true);
                ListItem l14 = new ListItem("Get Child Account", "Get Child Account", true);
                ListItem l15 = new ListItem("Set Parent Account", "Set Parent Account", true);
                ListItem l16 = new ListItem("Get Single Invoice", "Get Single Invoice", true);
                ListItem l17 = new ListItem("Increase Credit Balance", "Increase Credit Balance", true);
                ListItem l18 = new ListItem("Apply Credit Balance To Invoice", "Apply Credit Balance To Invoice", true);
                ListItem l19 = new ListItem("Refund Electronic Payment", "Refund Electronic Payment", true);
                ListItem l20 = new ListItem("Decrease Credit Balance", "Decrease Credit Balance", true);
                ListItem l21 = new ListItem("Apply Payment To Invoice", "Apply Payment To Invoice", true);
                ListItem l22 = new ListItem("Refund External Payment", "Refund External Payment", true);
                ListItem l23 = new ListItem("Get Product Catalog", "Get Product Catalog", true);
                ListItem l24 = new ListItem("Get Rate Plan By Name", "Get Rate Plan By Name", true);
                ListItem l25 = new ListItem("Disable Rate Plan", "Disable Rate Plan", true);
                ListItem l26 = new ListItem("Get Product By Name", "Get Product By Name", true);
                ListItem l27 = new ListItem("Create Product", "Create Product", true);
                ListItem l28 = new ListItem("Create Rate Plan With One Time Charge", "Create Rate Plan With One Time Charge", true);
                ListItem l29 = new ListItem("Delete Product", "Delete Product", true);
                ListItem l30 = new ListItem("Delete Rate Plan", "Delete Rate Plan", true);
                ListItem l31 = new ListItem("Change Price For Product", "Change Price For Product", true);
                ListItem l32 = new ListItem("Subscribe", "Subscribe", true);
                ListItem l33 = new ListItem("Subscribe With Existing Account", "Subscribe With Existing Account", true);
                ListItem l34 = new ListItem("Do Add Product Amendment", "Do Add Product Amendment", true);
                ListItem l35 = new ListItem("Do Renewal Amendment", "Do Renewal Amendment", true);
                ListItem l36 = new ListItem("Do Terms And Conditions Amendment", "Do Terms And Conditions Amendment", true);
                ListItem l37 = new ListItem("Get Subscription And Charge Info", "Get Subscription And Charge Info", true);
                ListItem l38 = new ListItem("Update Contact", "Update Contact", true);
                ListItem l39 = new ListItem("Get Credit Cards", "Get Credit Cards", true);
                ListItem l40 = new ListItem("New Credit Card", "New Credit Card", true);
                ListItem l41 = new ListItem("Get Invoices PDF For Account", "Get Invoices PDF For Account", true);
                //ListItem l42 = new ListItem("Get Current Subscription", "Get Current Subscription", true);
                ListItem l43 = new ListItem("Delete Payment Method", "Delete Payment Method");
                
                ddl1.Items.Add(l3);
                ddl1.Items.Add(l2);
                ddl1.Items.Add(l);
                ddl1.Items.Add(l4);
                ddl1.Items.Add(l5);
                ddl1.Items.Add(l6);
                ddl1.Items.Add(l7);
                ddl1.Items.Add(l8);
                ddl1.Items.Add(l9);
                ddl1.Items.Add(l10);
                ddl1.Items.Add(l11);
                ddl1.Items.Add(l12);
                ddl1.Items.Add(l13);
                ddl1.Items.Add(l14);
                ddl1.Items.Add(l15);
                ddl1.Items.Add(l16);
                ddl1.Items.Add(l17);
                ddl1.Items.Add(l18);
                ddl1.Items.Add(l19);
                ddl1.Items.Add(l20);
                ddl1.Items.Add(l21);
                ddl1.Items.Add(l22);
                ddl1.Items.Add(l23);
                ddl1.Items.Add(l24);
                ddl1.Items.Add(l25);
                ddl1.Items.Add(l26);
                ddl1.Items.Add(l27);
                ddl1.Items.Add(l28);
                ddl1.Items.Add(l29);
                ddl1.Items.Add(l30);
                ddl1.Items.Add(l31);
                ddl1.Items.Add(l32);
                ddl1.Items.Add(l33);
                ddl1.Items.Add(l34);
                ddl1.Items.Add(l35);
                ddl1.Items.Add(l36);
                ddl1.Items.Add(l37);
                ddl1.Items.Add(l38);
                ddl1.Items.Add(l39);
                ddl1.Items.Add(l40);
                ddl1.Items.Add(l41);
                //ddl1.Items.Add(l42);
                ddl1.Items.Add(l43);
            }
        }

        protected void Login(object sender, EventArgs e)
        {
            if (uname.Text != "" && pass.Text != "" && endpoint.Text != "")
            {
                zs = new ZuoraService(uname.Text, pass.Text, endpoint.Text);
                ResponseHolder loginRes = zs.Login();
                loginResStr += "" + loginRes.Success + " " + loginRes.Message;
                result.Text += loginResStr;
                Session["zs"] = zs;
                result.Text += "<br/>";
            }
            else
            {
                result.Text += "Missing Required Information";
            }
        }

        protected void DoAction(object sender, EventArgs e)
        {
            zs = (ZuoraService)Session["zs"];
            AccountManager am = new AccountManager(zs);
            PaymentManager pm = new PaymentManager(zs);
            ProductCatalogManager pcm = new ProductCatalogManager(zs,   "C:\\localCache.txt");
            SubscriptionManager sm = new SubscriptionManager(zs);
                
            if (zs != null)
            {
                var operation = ddl1.SelectedValue;

                if (operation == "Create Account")
                {
                    result.Text += "<br/>";
                    Account acc = new Account();
                    acc.Name = AccountName.Text;
                    acc.BillCycleDaySpecified = true;
                    acc.BillCycleDay = Convert.ToInt16(BillCycleDay.Text);
                    acc.Currency = Currency.Text;
                    acc.PaymentTerm = PaymentTermDropDown.SelectedValue;
                    acc.Batch = Batch.Text;

                    Contact con = new Contact();
                    con.Address1 = Address1.Text;
                    con.Address2 = Address2.Text;
                    con.City = City.Text;
                    con.State = State.Text;
                    con.LastName = LastName.Text;
                    con.FirstName = FirstName.Text;
                    con.Country = Country.Text;
                    con.PostalCode = Zip.Text;

                    var res = am.CreateAccount(acc, con);
                    result.Text += res.Success ? res.Id : res.Message;
                }
                else if (operation == "Create Child Account And Increase Credit Balance")
                {
                    result.Text += "<br/>";

                    Account acc = new Account();
                    acc.Name = AccountName.Text;
                    acc.BillCycleDaySpecified = true;
                    acc.BillCycleDay = Convert.ToInt16(BillCycleDay.Text);
                    acc.Currency = Currency.Text;
                    acc.PaymentTerm = PaymentTermDropDown.SelectedValue;
                    acc.Batch = Batch.Text;

                    Contact con = new Contact();
                    con.Address1 = Address1.Text;
                    con.Address2 = Address2.Text;
                    con.City = City.Text;
                    con.State = State.Text;
                    con.LastName = LastName.Text;
                    con.FirstName = FirstName.Text;
                    con.Country = Country.Text;
                    con.PostalCode = Zip.Text;

                    var res = am.CreateChildAccountAndIncreaseCreditBalance(acc, ParentAccountId.Text, con, Convert.ToDecimal(AmountBox.Text));
                    result.Text += res.Success ? res.Id : res.Message;
                }
                else if (operation == "Cancel Credit Balance Adjustment On Invoice")
                {
                    result.Text += "<br/>";
                    var res = pm.CancelCreditBalanceAdjustmentOnInvoice(InvoiceId.Text);
                    result.Text += res.Success ? res.Id : res.Message;
                }
                else if (operation == "Get Invoices For Account")
                {
                    result.Text += "<br/>";
                    var res = am.GetInvoicesForAccount(AccountId.Text);
                    //string FIELDS_INVOICE = "Id, AccountId, AdjustmentAmount, Amount, AmountWithoutTax, Balance, Comments, CreatedDate, DueDate, IncludesOneTime, IncludesRecurring, IncludesUsage, InvoiceDate, InvoiceNumber, LastEmailSentDate, PaymentAmount, PostedDate, RefundAmount, Source, SourceId, Status, TargetDate, TaxAmount, TaxExemptAmount, TransferredToAccounting, UpdatedDate";
                    //var res = zs.Query("");
                    if (res.Success)
                    {
                        result.Text += "<br/>";
                        if (res.Objects != null)
                        {
                            foreach (zObject zo in res.Objects)
                            {
                                Invoice inv = (Invoice)zo;
                                result.Text += "Invoice Number: " + inv.InvoiceNumber + " Invoice Amount: " + inv.Amount + " Status: " + inv.Status + "<br/>";
                            }
                        }
                        else
                        {
                            result.Text += res.Message;
                        }
                    }
                    else
                    {
                        result.Text += res.Message;
                    }
                }
                else if (operation == "Get Refunds For Account")
                {
                    result.Text += "<br/>";
                    var res = am.GetRefundsForAccount(AccountId.Text);
                    if (res.Success)
                    {
                        result.Text += "<br/>";
                        if (res.Objects != null)
                        {
                            foreach (zObject zo in res.Objects)
                            {
                                Refund refund = (Refund)zo;
                                result.Text += "Refund Id: " + refund.Id + " Refund Amount: " + refund.Amount + "<br/>";
                            }
                        }
                        else
                        {
                            result.Text += res.Message;
                        }
                    }
                    else
                    {
                        result.Text += res.Message;
                    }
                }
                else if (operation == "Get Credit Balance Adjustments For Account")
                {
                    result.Text += "<br/>";
                    var res = am.GetCreditBalanceAdjustmentsForAccount(AccountId.Text);
                    if (res.Success)
                    {
                        result.Text += "<br/>";
                        if (res.Objects != null)
                        {
                            foreach (zObject zo in res.Objects)
                            {
                                CreditBalanceAdjustment cba = (CreditBalanceAdjustment)zo;
                                result.Text += "CBA Id: " + cba.Id + " CBA Amount: " + cba.Amount + "<br/>";
                            }
                        }
                        else
                        {
                            result.Text += res.Message;
                        }
                    }
                    else
                    {
                        result.Text += res.Message;
                    }
                }
                else if (operation == "Get Credit Cards For Account")
                {
                    result.Text += "<br/>";
                    var res = am.GetCreditCardsForAccount(AccountId.Text);
                    if (res.Success)
                    {
                        result.Text += "<br/>";
                        if (res.Objects != null)
                        {
                            foreach (zObject zo in res.Objects)
                            {
                                PaymentMethod payMethod = (PaymentMethod)zo;
                                result.Text += "PM Id: " + payMethod.Id + " PM Type: " + payMethod.Type + "<br/>";
                            }
                        }
                        else
                        {
                            result.Text += res.Message;
                        }
                    }
                    else
                    {
                        result.Text += res.Message;
                    }
                }
                else if (operation == "Get PO For Account")
                {
                    result.Text += "<br/>";
                    var res = am.GetPOForAccount(AccountId.Text);
                    result.Text += "PO: " + res;
                }
                else if (operation == "Get Account")
                {
                    result.Text += "<br/>";
                    var res = am.GetAccount(AccountId.Text);
                    if (res.Success && res.Objects != null)
                    {
                        Account acc = (Account)res.Objects[0];
                        result.Text += "Account Name: " + acc.Name + "<br/>";
                        result.Text += "Account Credit Balance: " + acc.CreditBalance + "<br/>";
                    }
                    else
                    {
                        result.Text += res.Message;
                    }
                    
                }
                else if (operation == "Get Contact")
                {
                    var res = am.GetContact(AccountId.Text);
                   
                    if (res.Success && res.Objects != null)
                    {
                        result.Text += "<br/>";
                        foreach (zObject zo in res.Objects)
                        {
                            Contact con = (Contact)zo;
                            result.Text += "Contact First Name: " + con.FirstName + "<br/>";
                            result.Text += "Contact Last Name: " + con.LastName + "<br/>";
                            result.Text += "Contact Email: " + con.WorkEmail + "<br/>";
                        }
                    }
                    else
                    {
                        result.Text += res.Message;
                    }
                }
                else if (operation == "Change Invoice Template")
                {
                    result.Text += "<br/>";
                    var res = am.ChangeInvoiceTemplate(AccountId.Text, InvoiceTemplateId.Text);
                    if (res.Success)
                    {
                        result.Text += res.Id;
                    }
                    else
                    {
                        result.Text += res.Message;
                    }
                }
                else if (operation == "Does Account Name Exist In Zuora")
                {
                    result.Text += "<br/>";
                    var res = am.DoesAccountNameExistInZuora(AccountName.Text);
                    result.Text += "Account Exists? " + res;
                }
                else if (operation == "Get Child Account")
                {
                    result.Text += "<br/>";
                    var res = am.GetChildAccount(AccountId.Text);
                    if (res.Success && res.Objects != null)
                    {
                        Account acc = (Account)res.Objects[0];
                        result.Text += "Account Name: " + acc.Name + "<br/>";
                        result.Text += "Account Credit Balance: " + acc.CreditBalance + "<br/>";
                    }
                    else
                    {
                        result.Text += res.Message;
                    }
                }
                else if (operation == "Set Parent Account")
                {
                    result.Text += "<br/>";
                    var res = am.SetParentAccount(ParentAccountId.Text, AccountId.Text);
                    if (res.Success)
                    {
                        result.Text += "Success: " + res.Id;
                    }
                    else
                    {
                        result.Text += res.Message;
                    }
                }
                else if (operation == "Get Single Invoice")
                {
                    result.Text += "<br/>";
                    //var res = am.GetSingleInvoice(InvoiceId.Text);
                    //var res = zs.Query("SELECT id, status FROM Invoice WHERE Id = '" + InvoiceId.Text + "'").FirstOrDefault<Invoice>();
                    Boolean withBody = false;
                    //string FIELDS_INVOICE = "Id, AccountId, AdjustmentAmount, Amount, AmountWithoutTax, Balance, Comments, CreatedDate, DueDate, IncludesOneTime, IncludesRecurring, IncludesUsage, InvoiceDate, InvoiceNumber, LastEmailSentDate, PaymentAmount, PostedDate, RefundAmount, Source, SourceId, Status, TargetDate, TaxAmount, TaxExemptAmount, TransferredToAccounting, UpdatedDate";
                    string FIELDS_INVOICE = "Id, AccountId, AdjustmentAmount, Amount, AmountWithoutTax, Balance, Comments, CreatedDate, DueDate, IncludesOneTime, IncludesRecurring, IncludesUsage, InvoiceDate, InvoiceNumber, LastEmailSentDate, PaymentAmount, PostedDate, RefundAmount, Status";
                    var query = string.Format("SELECT {0} FROM invoice WHERE Id = '{1}'", FIELDS_INVOICE, InvoiceId.Text);
                    var res = zs.Query(query);
                    if (res.Success && res.Objects != null)
                    {
                        result.Text += "Success: " + res.Id;
                        result.Text += "Invoice Amount: " + ((Invoice)res.Objects[0]).Amount;
                        result.Text += " Status: " + ((Invoice)res.Objects[0]).Status; 
                    }
                    else
                    {
                        result.Text += res.Message;
                    }
                }
                else if (operation == "Increase Credit Balance")
                {
                    var res = pm.IncreaseCreditBalance(AccountId.Text, Convert.ToDecimal(AmountBox.Text));
                    if (res.Success)
                    {
                        result.Text += "Success: " + res.Id;
                    }
                    else
                    {
                        result.Text += res.Message;
                    }
                }
                else if (operation == "Apply Credit Balance To Invoice")
                {
                    var res = pm.ApplyCreditBalanceToInvoice(InvoiceId.Text, Convert.ToDecimal(AmountBox.Text));
                    if (res.Success)
                    {
                        result.Text += "Success: " + res.Id;
                    }
                    else
                    {
                        result.Text += res.Message;
                    }
                }
                else if (operation == "Refund Electronic Payment")
                {
                    result.Text += "<br/>";
                    var res = pm.RefundElectronicPayment(PaymentId.Text, Convert.ToDecimal(RefundAmount.Text));
                    if (res.Success)
                    {
                        result.Text += "Success: " + res.Id;
                    }
                    else
                    {
                        result.Text += res.Message;
                    }
                }
                else if (operation == "Decrease Credit Balance")
                {
                    result.Text += "<br/>";
                    var res = pm.DecreaseCreditBalance(AccountId.Text, Convert.ToDecimal(AmountBox.Text));
                    if (res.Success)
                    {
                        result.Text += "Success: " + res.Id;
                    }
                    else
                    {
                        result.Text += res.Message;
                    }
                }
                else if (operation == "Apply Payment To Invoice")
                {
                    result.Text += "<br/>";
                    var res = pm.ApplyPaymentToInvoice(AccountId.Text, InvoiceId.Text, PaymentMethodId.Text, Convert.ToDecimal(PaymentAmount.Text),PaymentMethodType.SelectedValue );
                    if (res.Success)
                    {
                        result.Text += "Success: " + res.Id;
                    }
                    else
                    {
                        result.Text += res.Message;
                    }
                }
                else if (operation == "Refund External Payment")
                {
                    result.Text += "<br/>";
                    var res = pm.RefundExternalPayment(PaymentId.Text, Convert.ToDecimal(RefundAmount.Text), PaymentMethodType.SelectedValue, Convert.ToDateTime(RefundDate.Text) );
                    if (res.Success)
                    {
                        result.Text += "Success: " + res.Id;
                    }
                    else
                    {
                        result.Text += res.Message;
                    }
                }
                else if (operation == "Get Product Catalog")
                {
                    result.Text += "<br/>";
                    var res = pcm.GetProductCatalog();
                    if (res != null)
                    {
                        result.Text += "Success: " + "<br/>";
                        foreach (var ph in res)
                        {
                            result.Text += "<b>Product Name:</b>" + ph.Product.Name + "<br/>";
                            foreach (var prph in ph.ProductRatePlans)
                            {
                                result.Text += "<b>Product Rate Plan Name:</b>" + prph.ProductRatePlan.Name + "<br/>";
                            }
                        }
                    }
                    else
                    {
                        result.Text += "Error";
                    }
                }
                else if (operation == "Get Rate Plan By Name")
                {
                    result.Text += "<br/>";
                    var res = pcm.GetProductRatePlanByName(Name.Text);
                    if (res != null)
                    {
                        result.Text += "Success: " + res.ProductRatePlan.Name;
                    }
                    else
                    {
                        result.Text += "Didn't find";
                    }
                }
                else if (operation == "Disable Rate Plan")
                {
                    result.Text += "<br/>";
                    var res = pcm.DisableRatePlan(ProductRatePlanId.Text, Convert.ToDateTime(DisableDate.Text));
                    if (res.Success)
                    {
                        result.Text += "Success: " + res.Id;
                    }
                    else
                    {
                        result.Text += res.Message;
                    }
                }
                else if (operation == "Get Product By Name")
                {
                    result.Text += "<br/>";
                    var res = pcm.GetProductByName(Name.Text);
                    if (res != null)
                    {
                        result.Text += "Success: " + res.Product.Name;
                    }
                    else
                    {
                        result.Text += "Didn't Find";
                    }
                }
                else if (operation == "Create Product")
                {
                    result.Text += "<br/>";
                    Constants.ProductType pt = new Constants.ProductType();
                    if (ProductType.Text == "Application")
                    {
                        pt = Constants.ProductType.Application;
                    }
                    else if (ProductType.Text == "iCredits")
                    {
                        pt = Constants.ProductType.iCredits;
                    }
                    else if (ProductType.Text == "Storage")
                    {
                        pt = Constants.ProductType.Storage;
                    }
                    
                    var res = pcm.CreateProduct(Name.Text, pt, SKU.Text, Description.Text);
                    if (res != null)
                    {
                        result.Text += "Success: " + res.Product.Id;
                    }
                    else
                    {
                        result.Text += "Error";
                    }
                }
                else if (operation == "Create Rate Plan With One Time Charge")
                {
                    Product prod = new Product();
                    prod.Id = ProductId.Text;
                    result.Text += "<br/>";
                    var res = pcm.CreateRatePlanWithOneTimeCharge(prod, Name.Text, Convert.ToDecimal(Price.Text));
                    if (res != null)
                    {
                        result.Text += "Success: " + res.ProductRatePlan.Id;
                    }
                    else
                    {
                        result.Text += "Error";
                    }
                }
                else if (operation == "Delete Rate Plan")
                {
                    pcm.DeleteRatePlan(IdToDelete.Text);
                }
                else if (operation == "Delete Product")
                {
                    pcm.DeleteProduct(IdToDelete.Text);
                }
                else if (operation == "Change Price For Product")
                {
                    List<ResponseHolder> resps = pcm.ChangePriceForProduct(ProductRatePlanChargeId.Text, new decimal[] { Convert.ToDecimal(Price.Text) }, "USD");
                    result.Text += "<br/>";
                    foreach (ResponseHolder resp in resps)
                    {
                        if (resp.Success)
                        {
                            result.Text += resp.Id;
                        }
                        else
                        {
                            result.Text += resp.Message;
                        }
                    }
                }
                else if (operation == "Subscribe")
                {
                    result.Text += "<br/>";
                    Account acc = new Account();
                    acc.Name = AccountName.Text;
                    acc.BillCycleDaySpecified = true;
                    acc.BillCycleDay = Convert.ToInt16(BillCycleDay.Text);
                    acc.Currency = Currency.Text;
                    acc.PaymentTerm = PaymentTermDropDown.SelectedValue;
                    acc.Batch = Batch.Text;

                    Contact con = new Contact();
                    con.Address1 = Address1.Text;
                    con.Address2 = Address2.Text;
                    con.City = City.Text;
                    con.State = State.Text;
                    con.LastName = LastName.Text;
                    con.FirstName = FirstName.Text;
                    con.Country = Country.Text;
                    con.PostalCode = Zip.Text;

                    DateTime date = Convert.ToDateTime(SubscriptionDate.Text);

                    Subscription sub = new Subscription();
                    sub.ContractAcceptanceDateSpecified = true;
                    sub.ContractAcceptanceDate = date;
                    sub.ServiceActivationDateSpecified = true;
                    sub.ServiceActivationDate = date;
                    sub.ContractEffectiveDateSpecified = true;
                    sub.ContractEffectiveDate = date;

                    sub.TermType = SubscriptionTermType.Text;
                    if (SubscriptionTermType.Text == "TERMED")
                    {
                        sub.InitialTermSpecified = true;
                        sub.InitialTerm = 12;
                        sub.RenewalTermSpecified = true;
                        sub.RenewalTerm = 12;
                    }

                    PreviewOptions po = new PreviewOptions();
                    po.EnablePreviewModeSpecified = false;
                    po.EnablePreviewMode = false;

                    SubscribeOptions so = new SubscribeOptions();
                    so.GenerateInvoiceSpecified = false;
                    so.GenerateInvoice = false;
                    so.ProcessPaymentsSpecified = false;
                    so.ProcessPayments = false;

                    ProductRatePlanHolder prph = pcm.GetProductRatePlanByName(ProductRatePlanName.Text);

                    SubscribeResponseHolder resp = sm.Subscribe( acc, con, null, new List<ProductRatePlanHolder> { prph }, sub, po, so);

                    if (resp.Success)
                    {
                        result.Text += resp.SubRes.AccountId;
                    }
                    else
                    {
                        result.Text += resp.Message;
                    }
                    
                }
                else if (operation == "Subscribe With Existing Account")
                {
                    result.Text += "<br/>";
                    DateTime date = Convert.ToDateTime(SubscriptionDate.Text);

                    Subscription sub = new Subscription();
                    sub.ContractAcceptanceDateSpecified = true;
                    sub.ContractAcceptanceDate = date;
                    sub.ServiceActivationDateSpecified = true;
                    sub.ServiceActivationDate = date;
                    sub.ContractEffectiveDateSpecified = true;
                    sub.ContractEffectiveDate = date;

                    sub.TermType = SubscriptionTermType.Text;
                    if (SubscriptionTermType.Text == "TERMED")
                    {
                        sub.InitialTermSpecified = true;
                        sub.InitialTerm = 12;
                        sub.RenewalTermSpecified = true;
                        sub.RenewalTerm = 12;
                    }

                    PreviewOptions po = new PreviewOptions();
                    po.EnablePreviewModeSpecified = false;
                    po.EnablePreviewMode = false;

                    SubscribeOptions so = new SubscribeOptions();
                    so.GenerateInvoiceSpecified = false;
                    so.GenerateInvoice = false;
                    so.ProcessPaymentsSpecified = false;
                    so.ProcessPayments = false;

                    ProductRatePlanHolder prph = pcm.GetProductRatePlanByName(ProductRatePlanName.Text);

                    SubscribeResponseHolder resp = sm.SubscribeWithExisitingAccount(AccountId.Text, new List<ProductRatePlanHolder>{prph}, sub, po, so);
                    if (resp.Success)
                    {
                        result.Text += resp.SubRes.AccountId;
                    }
                    else
                    {
                        result.Text += resp.Message;
                    }
                }
                else if (operation == "Do Add Product Amendment")
                {
                    AmendOptions ao = new AmendOptions();
                    ao.GenerateInvoiceSpecified = false;
                    ao.GenerateInvoice = false;

                    ao.ProcessPaymentsSpecified = true;
                    ao.ProcessPayments = false;

                    PreviewOptions po = new PreviewOptions();
                    po.EnablePreviewModeSpecified = false;
                    po.EnablePreviewMode = false;

                    AmendResponseHolder resp = sm.DoAddProductAmendment(SubscriptionId.Text, Convert.ToDateTime(AmendmentStartDate.Text), ProductRatePlanId.Text, ao, po);
                    if (resp.Success)
                    {
                        result.Text += resp.AmendRes.AmendmentIds[0];
                    }
                    else
                    {
                        result.Text += resp.Message;
                    }
                }
                else if (operation == "Do Renewal Amendment")
                {
                    AmendOptions ao = new AmendOptions();
                    ao.GenerateInvoiceSpecified = true;
                    ao.GenerateInvoice = false;

                    ao.ProcessPaymentsSpecified = true;
                    ao.ProcessPayments = false;

                    PreviewOptions po = new PreviewOptions();
                    po.EnablePreviewModeSpecified = false;
                    po.EnablePreviewMode = false;

                    AmendResponseHolder resp = sm.DoRenewalAmendment(SubscriptionId.Text, Convert.ToDateTime(AmendmentStartDate.Text), ao, po);
                    if (resp.Success)
                    {
                        result.Text += resp.AmendRes.AmendmentIds[0];
                    }
                    else
                    {
                        result.Text += resp.Message;
                    }
                }
                else if (operation == "Do Terms And Conditions Amendment")
                {
                    AmendOptions ao = new AmendOptions();
                    ao.GenerateInvoiceSpecified = true;
                    ao.GenerateInvoice = false;

                    ao.ProcessPaymentsSpecified = true;
                    ao.ProcessPayments = false;

                    PreviewOptions po = new PreviewOptions();
                    po.EnablePreviewModeSpecified = false;
                    po.EnablePreviewMode = false;

                    AmendResponseHolder resp = sm.DoTermsAndConditionsAmendment(
                                                    SubscriptionId.Text, 
                                                    Convert.ToDateTime(AmendmentStartDate.Text), 
                                                    SubscriptionTermType.SelectedValue, 
                                                    Convert.ToInt32(InitialTerm.Text), 
                                                    Convert.ToInt32(RenewalTerm.Text), 
                                                    ao, 
                                                    po
                                               );
                    if (resp.Success)
                    {
                        result.Text += resp.AmendRes.AmendmentIds[0];
                    }
                    else
                    {
                        result.Text += resp.Message;
                    }
                }
                else if (operation == "Get Subscription And Charge Info")
                {
                    SubscriptionInfoHolder sih = sm.GetSubscriptionAndChargeInfo(SubscriptionId.Text);
                    if (sih.Success)
                    {
                        result.Text += "<b>SubId:</b>" + sih.Subscription.Id + "<br/>";
                        foreach (RatePlan rp in sih.RatePlanList)
                        {
                            result.Text += "<b>RP Name:</b>" + rp.Name + "<br/>";
                        }
                    }
                    else
                    {
                        result.Text += sih.Message;
                    }
                }
                else if (operation == "Update Contact")
                {
                    ResponseHolder resp = am.UpdateContact(AccountId.Text, LastName.Text, FirstName.Text, Address1.Text, City.Text, State.Text, Zip.Text, Country.Text);

                    if (resp.Success)
                    {
                        result.Text += resp.Id;
                    }
                    else
                    {
                        result.Text += resp.Message;
                    }
                }
                else if(operation == "Get Credit Cards")
                {
                    ResponseHolder resp = am.GetCreditCardsPaymentMethods(AccountId.Text);

                    if (resp.Success)
                    {
                        foreach (zObject zo in resp.Objects)
                        {
                            PaymentMethod paymentMethod = (PaymentMethod)zo;
                            result.Text += "Payment Method: " + paymentMethod.Id + "<br/>";
                        }
                    }
                    else
                    {
                        result.Text += resp.Message;
                    }
                }
                else if (operation == "Get Invoices PDF For Account")
                {
                    result.Text += "<br/>";
                    var res = am.GetInvoicesForAccount(AccountId.Text);
                    //string FIELDS_INVOICE = "Id, AccountId, AdjustmentAmount, Amount, AmountWithoutTax, Balance, Comments, CreatedDate, DueDate, IncludesOneTime, IncludesRecurring, IncludesUsage, InvoiceDate, InvoiceNumber, LastEmailSentDate, PaymentAmount, PostedDate, RefundAmount, Source, SourceId, Status, TargetDate, TaxAmount, TaxExemptAmount, TransferredToAccounting, UpdatedDate";
                    //var res = zs.Query("");
                    if (res.Success)
                    {
                        result.Text += "<br/>";
                        if (res.Objects != null)
                        {
                            Invoice invoice = (Invoice) res.Objects[0];
                            foreach (zObject zo in res.Objects)
                            {
                                Invoice inv = (Invoice)zo;
                                if (inv.CreatedDate > invoice.CreatedDate)
                                {
                                    invoice = inv;
                                }
                                result.Text += "Invoice Number: " + inv.InvoiceNumber + " Invoice Amount: " + inv.Amount + " Status: " + inv.Status + "<br/>";
                            }

                            Invoice invRes = (Invoice)am.GetInvoicePDFForAccount(invoice.Id).Objects[0];
                            var invoiceBody = invRes.Body;

                            Byte[] bytes = System.Convert.FromBase64String(invoiceBody.ToString());

                            string pathString = @"C:\Users\sxuereb\Documents\Invoices";
                            string fileName = invRes.InvoiceNumber +".pdf";
                            pathString = System.IO.Path.Combine(pathString, fileName);

                            if (!System.IO.File.Exists(pathString))
                            {
                                using (System.IO.FileStream fs = System.IO.File.Create(pathString))
                                {
                                    for (int i = 0; i < bytes.Length; i++)
                                    {
                                        fs.WriteByte(bytes[i]);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("File \"{0}\" already exists.", fileName);
                                return;
                            }
                            
                        }
                        else
                        {
                            result.Text += res.Message;
                        }
                    }
                    else
                    {
                        result.Text += res.Message;
                    }
                }
                /*else if(operation == "Get Current Subscription")
                {
                    ResponseHolder resp = sm.GetCurrentSubscription(AccountId.Text);

                    
                    if (resp.Success)
                    {
                        
                    }
                    else
                    {
                        result.Text += resp.Message;
                    }
                }
                else if(operation == "New Credit Card")
                {

                }*/
                else if (operation == "Delete Payment Method")
                {
                    List<ResponseHolder> resp = am.DeletePaymentMethod(PaymentMethodId.Text);

                    if (resp[0].Success)
                    {
                        result.Text += "Successfully Deleted Payment Method.";
                    }
                    else
                    {
                        result.Text += resp[0].Message;
                    }
                }
                
            }
            else
            {
                result.Text += "Please Login" + "<br/>";
            }

            ddl1.SelectedIndex = 0;
        }
    }
}