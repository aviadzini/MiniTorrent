using MiniTorrentService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MiniTorrentClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IWCFMiniTorrentService> channelFactory = 
                new ChannelFactory<IWCFMiniTorrentService>("MiniTorrentEndpoint");

            IWCFMiniTorrentService proxy = channelFactory.CreateChannel();

            //proxy.signIn("uhuh");
        }
    }
}
