using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.IO;

namespace Zuora.Services
{
    public class ZuoraService : Zuora.Services.IZuoraService
    {

        private string csvPath = "C:\\dev\\Illumina.iCloud\\CloudWeb\\branches\\v1.2-zuora\\src\\Zuora.Services\\exportCsv.csv";
        private String endpoint;
        private String username;
        private String password;
        SoapClient svc;
        SessionHeader sh = new SessionHeader();
        DateTime starttime = DateTime.Now;
        TimeSpan elapsedtime;
        //relogin in this often
        TimeSpan timeoutlength = new TimeSpan(0, 9, 0);

        public ZuoraService(String username, String password, String endpoint)
        {
            this.username = username;
            this.password = password;
            this.endpoint = endpoint;
            this.svc = CreateClient(endpoint);
        }

        public String GetExportFileId(String exportId)
        {
            int sleepTime = 5000;
            int maxSleepTime = 60000;

            Boolean isCompleted = false;
            Export exp = new Export();
            while (!isCompleted)
            {
                String exportFileIdQueryString = "SELECT status, fileId, query, size FROM Export WHERE Id='" + exportId + "'";
                ResponseHolder queryRes = Query(exportFileIdQueryString);
                //check the status
                if (queryRes.Objects.Count != 0)
                {
                    exp = (Export)queryRes.Objects[0];
                    if (exp.Status == "Completed")
                    {
                        return exp.FileId;
                    }
                    else if (exp.Status == "Processing" || exp.Status == "Pending")
                    {
                        int totalWaitTime = 0;
                        System.Threading.Thread.Sleep(sleepTime);
                        totalWaitTime += sleepTime;
                        if (totalWaitTime >= maxSleepTime)
                        {
                            //should notify illumina that export timeout happened here
                            Console.WriteLine("Export timeout");
                            return null;
                        }
                    }
                    else if (exp.Status == "Failed")
                    {
                        //export failed
                        Console.WriteLine("Export timeout");
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        public HttpWebResponse GetAndWriteExportToFile(String exportFileId)
        {
            

            String zendpoint = "";
            if (endpoint.Contains("api"))
            {
                //look in api sandbox
                zendpoint = "https://apisandbox.zuora.com/apps/api/file/" + exportFileId + "/";
            }
            else
            {
                //look in production
                zendpoint = "https://www.zuora.com/apps/api/file/" + exportFileId + "/";
            }

            HttpWebRequest request = WebRequest.Create(zendpoint) as HttpWebRequest;
            WebHeaderCollection authenticationHeader = new WebHeaderCollection();
            authenticationHeader.Add("Authorization", "ZSession " + sh.session);
            request.Headers = authenticationHeader;
            
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            StreamReader sr = new StreamReader(response.GetResponseStream());
            string myFile = csvPath;
            System.IO.StreamWriter fh = new System.IO.StreamWriter(myFile);
            
            while (!sr.EndOfStream)
            {
                fh.WriteLine(sr.ReadLine());
            }
            fh.Close();

            return response;
        }

        public List<AmendResponseHolder> Amend(List<AmendRequest> input)
        {
            List<AmendResponseHolder> results = new List<AmendResponseHolder>();
            AmendResult[] res = new AmendResult[] { };
            bool done = false;
            //do amendments one at a time
            int amendNum = 1;
            while (!done)
            {
                List<AmendRequest> doWork = new List<AmendRequest>() { };

                if (input.Count > amendNum)
                {
                    //first 3 of input, the sub id's prolly have to be the same...
                    List<AmendRequest> amendList = input.GetRange(0, amendNum);
                    for (int i = 0; i < amendNum; i++)
                    {
                        AmendRequest temp = amendList[i];
                        doWork.Add(temp);
                        input.Remove(temp);
                    } 
                }
                else
                {
                    done = true;
                    doWork = input;
                }

                CheckTime();
                try
                {  
                    res = svc.amend(sh, doWork.ToArray());   
                }
                catch (FaultException e)
                {
                    AmendResponseHolder error = new AmendResponseHolder();
                    error.Message = e.Message;
                    error.Success = false;
                    results.Add(error);
                    return results;
                }
                foreach (AmendResult ar in res)
                {
                    AmendResponseHolder temp = new AmendResponseHolder();
                    if (ar.Success)
                    {
                        temp.Success = true;
                        temp.AmendRes = ar;
                    }
                    else
                    {
                        temp.Success = false;
                        String error = "";
                        foreach (Error e in ar.Errors)
                        {
                            if (e.Message == null)
                                error += e.Code;
                            else
                                error += e.Message;                           
                        }
                        temp.Message = error;
                    }
                    results.Add(temp);
                }
            }
            return results;
        }

        public List<SubscribeResponseHolder> Subscribe(List<SubscribeRequest> input)
        {
            List<SubscribeResponseHolder> results = new List<SubscribeResponseHolder>();
            SubscribeResult[] res = new SubscribeResult[]{};
            bool done = false;
            int subscribeNum = 50;
            while (!done)
            {
                List<SubscribeRequest> doWork = new List<SubscribeRequest>() { };

                if (input.Count > subscribeNum)
                {
                    List<SubscribeRequest> subList = input.GetRange(0, subscribeNum);
                    for (int i = 0; i < subscribeNum; i++)
                    {
                        SubscribeRequest temp = subList[i];
                        doWork.Add(temp);
                        input.Remove(temp);
                    }
                }
                else
                {
                    done = true;
                    doWork = input;
                }
                CheckTime();
                try
                {
                    res = svc.subscribe(sh, doWork.ToArray());
                }
                catch (FaultException e)
                {
                    SubscribeResponseHolder error = new SubscribeResponseHolder();
                    error.Message = e.Message;
                    error.Success = false;
                    results.Add(error);
                    return results;
                }
                foreach (SubscribeResult sr in res)
                {
                    SubscribeResponseHolder temp = new SubscribeResponseHolder();
                    if (sr.Success)
                    {
                        temp.Success = true;
                        temp.SubRes = sr;
                    }
                    else
                    {
                        temp.Success = false;
                        temp.Message = sr.Errors[0].Message;
                    }
                    results.Add(temp);
                }
            }
            return results;
        }

        //update a list of objects in zuora
        public List<ResponseHolder> Update(List<zObject> input)
        {
            List<ResponseHolder> results = new List<ResponseHolder>();
            SaveResult[] res =  new SaveResult[]{};
            bool done = false;
            int updateNum = 50;
            while (!done)
            {
                List<zObject> doWork = new List<zObject>() { };

                if (input.Count > updateNum)
                {
                    List<zObject> updateList = input.GetRange(0, updateNum);
                    for (int i = 0; i < updateNum; i++)
                    {
                        zObject temp = updateList[i];
                        doWork.Add(temp);
                        input.Remove(temp);
                    }
                }
                else
                {
                    done = true;
                    doWork = input;
                }
                CheckTime();
                try
                {
                    res = svc.update(sh, doWork.ToArray());
                }
                catch (FaultException e)
                {
                    ResponseHolder error = new ResponseHolder();
                    error.Message = e.Message;
                    error.Success = false;
                    results.Add(error);
                    return results;
                }
                foreach (SaveResult sr in res)
                {
                    ResponseHolder temp = new ResponseHolder();
                    if (sr.Success)
                    {
                        temp.Id = sr.Id;
                        temp.Success = true;
                    }
                    else
                    {
                        temp.Success = false;
                        temp.Message = sr.Errors[0].Message;
                    }
                    results.Add(temp);
                }
            }
            return results;
        }

        //delete a list of objects from zuora
        public List<ResponseHolder> Delete(List<String> input, String type)
        {
            List<ResponseHolder> results = new List<ResponseHolder>();
            bool done = false;
            int deleteNum = 50;
            while (!done)
            {
                List<String> doWork = new List<String>() { };

                if (input.Count > deleteNum)
                {
                    List<String> delList = input.GetRange(0, deleteNum);
                    for (int i = 0; i < deleteNum; i++)
                    {
                        String temp = delList[i];
                        doWork.Add(temp);
                        input.Remove(temp);
                    }
                }
                else
                {
                    done = true;
                    doWork = input;
                }

                CheckTime();
                DeleteResult[] drs = new DeleteResult[] { };
                try
                {
                    drs = svc.delete(sh, type, doWork.ToArray());
                }
                catch (FaultException e)
                {
                    ResponseHolder error = new ResponseHolder();
                    error.Message = e.Message;
                    error.Success = false;
                    results.Add(error);
                    return results;
                }
                foreach (DeleteResult dr in drs)
                {
                    ResponseHolder res = new ResponseHolder();
                    if (dr.success)
                    {
                        res.Success = true;
                        res.Id = dr.id;
                    }
                    else
                    {
                        res.Success = false;
                        res.Message = dr.errors[0].Message;
                    }
                    results.Add(res);
                }
            }
            return results; 
        }

        //create a list of objects in zuora
        public List<ResponseHolder> Create(List<zObject> input, bool singleTransaction)
        {
            List<ResponseHolder> results = new List<ResponseHolder>();
            
            CallOptions co = new CallOptions();
            SaveResult[] sr = new SaveResult[]{};
            bool done = false;
            int createNum = 50;
            if (singleTransaction)
            {
                co.useSingleTransaction = true;
                co.useSingleTransactionSpecified = true;
            }

            while (!done)
            {
                List<zObject> doWork = new List<zObject>() { };

                if (input.Count > createNum)
                {
                    List<zObject> createList = input.GetRange(0, createNum);
                    for (int i = 0; i < createNum; i++)
                    {
                        zObject temp = createList[i];
                        doWork.Add(temp);
                        input.Remove(temp);
                    }
                }
                else
                {
                    done = true;
                    doWork = input;
                }

                CheckTime();
                try
                {
                    sr = svc.create(co, sh, doWork.ToArray());
                }
                catch (FaultException e)
                {
                    ResponseHolder error = new ResponseHolder();
                    error.Message = e.Message;
                    error.Success = false;
                    results.Add(error);
                    return results;
                }
                foreach (SaveResult s in sr)
                {
                    ResponseHolder temp = new ResponseHolder();
                    if (s.Success)
                    {
                        temp.Success = true;
                        temp.Id = s.Id;
                    }
                    else
                    {
                        temp.Success = false;
                        temp.Message = s.Errors[0].Message;
                        if(s.Errors[0].CodeSpecified)
                        {
                            temp.ErrorCode = s.Errors[0].Code.ToString();
                        }
                    }
                    results.Add(temp);
                }
            }
            return results;
        }

        //query request to zuora
        public ResponseHolder Query(String queryString)
        {
            ResponseHolder res = new ResponseHolder();
            List<zObject> output = new List<zObject>();
            QueryResult qs = new QueryResult();
            QueryOptions qo = new QueryOptions();
            CheckTime(); 
            try
            {               
                qs = svc.query(qo, sh, queryString);
            }
            catch(Exception e){
                res.Success = false;
                res.Message = e.Message;
                return res;
            }
            foreach (zObject zo in qs.records)
            {
                if(zo != null)
                    output.Add(zo);
            }
            
            while (!qs.done)
            {
                CheckTime();
                try
                {
                    qs = svc.queryMore(qo, sh, qs.queryLocator);
                }
                catch (Exception e)
                {
                    res.Success = false;
                    res.Message = e.Message;
                    return res;
                }
                foreach (zObject zo in qs.records)
                {
                    if (zo != null)
                        output.Add(zo);
                }
            }
            
            if (output.Count >= 1)
            {
                res.Success = true;
                res.Objects = output;
            }
            else if (output.Count == 0)
            {
                res.Success = true;
                res.Message = "No objects were returned by query.";
                res.Objects = output;
            }
            return res;
        }

        public ResponseHolder Create(zObject input)
        {
            return Create(new List<zObject>() { input }, false).FirstOrDefault();
        }

        public ResponseHolder Delete(zObject input)
        {
            var type = input.GetType();
            return Delete(new List<string>() { input.Id }, type.Name).FirstOrDefault();
        }

        //login to zuora
        public ResponseHolder Login()
        {
            ResponseHolder res = new ResponseHolder();
            LoginResult loginRes = new LoginResult();
            try
            {
                loginRes = svc.login(username, password);
                
            }
            catch (FaultException e)
            {
                res.Message = e.Message;
                res.Success = false;
            }
            if (loginRes.Session != null)
            {
                sh.session = loginRes.Session;
                res.Success = true;
            }
            return res;             
        }
        //checks the elapsed time and does a login() if it has elapsed
        public void CheckTime()
        {
            elapsedtime = DateTime.Now - starttime;
            if (sh.session == null || elapsedtime > timeoutlength)
            {
                Login();
            }
        }

        private SoapClient CreateClient(String endpoint)
        {
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);

            binding.MaxBufferSize = 5242880;
            binding.MaxBufferPoolSize = 524288;
            binding.MaxReceivedMessageSize = 5242880;
            binding.ReaderQuotas.MaxArrayLength = 16384;
            binding.ReaderQuotas.MaxDepth = 128;
            binding.ReaderQuotas.MaxStringContentLength = 5242880;
            binding.ReaderQuotas.MaxArrayLength = 16384;
            binding.ReaderQuotas.MaxBytesPerRead = 4096;
            binding.ReaderQuotas.MaxNameTableCharCount = 16384;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.None;
            TimeSpan s = new TimeSpan(TimeSpan.TicksPerMinute * 2);          
            binding.SendTimeout = s;

            var address = new EndpointAddress(endpoint);
            return new SoapClient(binding, address);

        }
    }
}
