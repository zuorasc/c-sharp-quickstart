using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Zuora.Services
{
    class CreditBalancePaymentRun
    {
        ZuoraService zs;
        PaymentManager pm;
        int attempts = 0;
        int maxAttempts = 3;
        List<Account> accountList = new List<Account>();
        List<Invoice> invoiceList = new List<Invoice>();
        private string csvPath = "C:\\dev\\Illumina.iCloud\\CloudWeb\\branches\\v1.2-zuora\\src\\Zuora.Services\\exportCsv.csv";
        private string paymentRunLogPath = "C:\\dev\\Illumina.iCloud\\CloudWeb\\branches\\v1.2-zuora\\src\\Zuora.Services\\paymentrunlog.txt";
        TextWriter tw;

        public CreditBalancePaymentRun(ZuoraService zs)
        {
            tw = new StreamWriter(paymentRunLogPath);
            this.zs = zs;
            pm = new PaymentManager(zs);
        }
        public Boolean DoCreditBalancePaymentRun()
        {
            DateTime today = DateTime.Now;
            string format = "yyyy-MM-ddTHH:mm:ss";
            string curDate = today.ToString(format);

            
            WriteLine("Starting new credit balance run...");

            //query for open invoices in z
            String queryString = "SELECT Id FROM Invoice WHERE Status='Posted' AND Balance > 0 AND DueDate <= '" + curDate + "'";
            ResponseHolder qResp = zs.Query(queryString);
            //do export
            //create export
            String exportZoqlQueryString = "SELECT Account.Id, Account.CreditBalance, Invoice.InvoiceDate ,Invoice.Id, Invoice.Balance, Invoice.Status, Invoice.DueDate, Invoice.Amount FROM Invoice WHERE Invoice.Status = 'Posted' AND Invoice.Balance > 0 AND Invoice.DueDate <= 'today'";
            ResponseHolder exportRespose = CreateExport(exportZoqlQueryString);
            String exportId = exportRespose.Id;         
            //get export file id
            String exportFileId = zs.GetExportFileId(exportId);
            if (exportFileId == null)
            {
                WriteLine("Export failed");
                return false;
            }           
            zs.GetAndWriteExportToFile(exportFileId);
            //readCsvFile i.e. create objects
            List<Dictionary<String, String>> exportLines = ReadCSVFileAndPopulateLists();
            if (exportLines.Count == 0)
            {
                WriteLine("Nothing returned in export");
                return false;
            }
            foreach (Dictionary<String, String> exportLine in exportLines)
            {
                Account acc = new Account();
                Invoice inv = new Invoice();
                String accId, accCreditBalance, invId, invBalance, invDate; 
                exportLine.TryGetValue("Account.Id", out accId);
                exportLine.TryGetValue("Account.CreditBalance", out accCreditBalance);
                exportLine.TryGetValue("Invoice.Id", out invId);
                exportLine.TryGetValue("Invoice.Balance", out invBalance);
                exportLine.TryGetValue("Invoice.InvoiceDate", out invDate);

                acc.Id = accId;
                acc.CreditBalance = Convert.ToDecimal(accCreditBalance);

                inv.Id = invId;
                inv.AccountId = accId;
                inv.Balance = Convert.ToDecimal(invBalance);
                inv.InvoiceDate = Convert.ToDateTime(invDate);

                accountList.Add(acc);
                invoiceList.Add(inv);
            }
            if (invoiceList.Count != qResp.Objects.Count)
            {
                //retry x time and after x attempts
                if (attempts > maxAttempts)
                {
                    WriteLine("Error getting export after " + maxAttempts + " tries. Please try again later" );
                    return false;
                }
                attempts++;
                DoCreditBalancePaymentRun();
            }
            //deal with export i.e separate into account and related invoices
            List<AccountInvoiceInfoHolder> accInvHolders = MakeAccountInvHolders(accountList, invoiceList);
            //process accounts
            foreach (AccountInvoiceInfoHolder aiih in accInvHolders)
            {
                Account acc = aiih.InvoiceAccount;
                List<Invoice> invList = aiih.InvoiceList;
                decimal? invTotal = aiih.InvoiceTotal;
                decimal? creditBalance = acc.CreditBalance;
                WriteLine("Processing "+ invList.Count +" invoice(s) for account id: " + acc.Id);
                if (invTotal > 0)
                {
                    if (creditBalance > 0)
                    {
                        //pay each invoice by applying credit balance to each open invoice, oldest first, 
                        //sort by date
                        invList.Sort(delegate(Invoice i1, Invoice i2){ return Convert.ToDateTime(i1.InvoiceDate).CompareTo(Convert.ToDateTime(i2.InvoiceDate)); });
                        foreach (Invoice inv in invList)
                        {
                            WriteLine("Invoice Total: " + invTotal + " CreditBalance on account: " + creditBalance);
                            WriteLine("Current invoice balance: " + inv.Balance + " Invoice Date: " + inv.InvoiceDate);
                            //Theres enought credit balance to cover the whole invoice
                            if (creditBalance >= inv.Balance)
                            {                             
                                //pay invoice total
                                ResponseHolder resp = pm.ApplyCreditBalanceToInvoice(inv.Id, (Decimal)inv.Balance);
                                if (resp.Success)
                                {
                                    WriteLine("Success applied " + inv.Balance + " to invoice id: " + inv.Id);
                                    creditBalance -= inv.Balance;
                                    invTotal -= inv.Balance;
                                }
                                else
                                {
                                    WriteLine("Failure in applying credit balance to invoice id: " + inv.Id + " Error: " + resp.Message);
                                }
                                
                            }
                            //theres some credit balance but not enough to cover the invoice
                            else if (creditBalance < inv.Balance && creditBalance > 0)
                            {
                                //apply remaing credit balance to the invoice
                                ResponseHolder resp = pm.ApplyCreditBalanceToInvoice(inv.Id, (Decimal)creditBalance);
                                if (resp.Success)
                                {
                                    WriteLine("Success applied " + creditBalance + " to invoice id: " + inv.Id);
                                    creditBalance -= creditBalance;
                                    invTotal -= creditBalance;
                                }
                                else
                                {
                                    WriteLine("Failure in applying credit balance to invoice id: " + inv.Id + " Error: " + resp.Message);
                                }
                            }
                            else
                            {
                                WriteLine("Ran out of credit balance on account skipping rest of the invoices");
                                break;
                            }                         
                            if (invTotal > 0)
                            {
                                //all the invoices were not payed
                                WriteLine("All invoice(s) for account " + acc.Id + " were not paid");
                            }
                        }
                    }
                    else
                    {
                        WriteLine("Account " + acc.Id + " does not have credit balance skipping");
                    }
                }
            }
            // close the stream
            tw.Close();
            return true;
        }
        public bool ExistsInList(Account inputAcc, List<Account> accounts)
        {
            foreach (Account acc in accounts)
            {
                if (acc.Id == inputAcc.Id)
                    return true;
            }
            return false;
        }
        public List<Account> GetUniqueAccounts(List<Account> input)
        {
            List<Account> output = new List<Account>();
            foreach (Account acc in input)
            {
                if(!ExistsInList(acc, output))
                {
                    output.Add(acc);
                }
            }
            return output;
        }
        public List<AccountInvoiceInfoHolder> MakeAccountInvHolders(List<Account> accList, List<Invoice> invList)
        {
            List<AccountInvoiceInfoHolder> output = new List<AccountInvoiceInfoHolder>();
            //get unique accounts
            List<Account> uniqueList = GetUniqueAccounts(accList);

            foreach (Account acc in uniqueList)
            {
                List<Invoice> tempInvList = new List<Invoice>();
                decimal? invTotal = 0;
                foreach (Invoice inv in invList)
                {
                    if (inv.AccountId == acc.Id)
                    {
                        tempInvList.Add(inv);
                        invTotal += inv.Balance;
                    }
                }
                AccountInvoiceInfoHolder aiih = new AccountInvoiceInfoHolder();
                aiih.InvoiceAccount = acc;
                aiih.InvoiceList = tempInvList;
                aiih.InvoiceTotal = invTotal;
                output.Add(aiih);
            }
            return output;
        }
        public List<Dictionary<String, String>> ReadCSVFileAndPopulateLists()
        {
            List<Dictionary<String, String>> objects = new List<Dictionary<String, String>>();
            StreamReader readFile = new StreamReader(csvPath);

            List<String[]> parsedData = new List<String[]>();
            String line;
            String[] row;

            while ((line = readFile.ReadLine()) != null)
            {
                row = line.Split(',');
                parsedData.Add(row);
            }
            String[] headers = null;
            Boolean headersFlag = true; 
            foreach (String[] tRow in parsedData)
            {
                Dictionary<String, String> temp = new Dictionary<String, String>();
                if (headersFlag)
                {
                    headers = tRow;
                    headersFlag = false;
                }
                else
                {
                    for (int i = 0; i < headers.Length; i++)
                    {
                        temp.Add(headers[i], tRow[i]);
                    }
                    objects.Add(temp);
                }
            }
            return objects;
        }

        public void WriteLine(String s)
        {
            // write a line of text to the file
            tw.WriteLine(DateTime.Now + " : " + s);
        }

        public ResponseHolder CreateExport(String queryString)
        {
            Export exp = new Export();
            exp.Name = "API Export " + DateTime.Now;
            exp.Query = queryString;
            exp.Format = "csv";

            List<ResponseHolder> rh = zs.Create(new List<zObject> { exp }, false);

            return rh[0];
        }
    }
}
