using System;
using System.Collections.Generic;

namespace Zuora.Services
{
    public interface IZuoraService
    {
        void CheckTime();
        ResponseHolder Login();
        ResponseHolder Query(string queryString);
        List<ResponseHolder> Create(List<zObject> input, bool singleTransaction);
        List<ResponseHolder> Update(List<zObject> input);
        List<ResponseHolder> Delete(List<string> input, string type);
        List<AmendResponseHolder> Amend(List<AmendRequest> input);
        List<SubscribeResponseHolder> Subscribe(List<SubscribeRequest> input);
    }
}
