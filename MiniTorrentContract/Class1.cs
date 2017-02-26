using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace MiniTorrentContract
{
    [ServiceContract]
    public interface IMiniTorrentContract
    {
        [OperationContract]
        string GetName();
        [OperationContract]
        int GetCount();
    }
}
