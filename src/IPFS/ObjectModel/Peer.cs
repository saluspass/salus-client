using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Ipfs.Api
{
    public class Peer
    {
        public enum AddressType
        {
            DnsAddr,
            IP4,
            IP6
        }

        private AddressType _RemoteAddressType;
        private string _RemoteAddress;
        private string _RemotePeerId;

        public Peer(AddressType addrType, string remoteAddr, string remotePeerId)
        {
            _RemoteAddressType = addrType;
            _RemoteAddress = remoteAddr;
            _RemotePeerId = remotePeerId;
        }


        public static Peer Create(JToken jObject)
        {
            string[] strSeg = jObject.Value<string>().Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            AddressType addrType = Enum.GetValues(typeof(AddressType))
                .Cast<AddressType>()
                .First(o => o.ToString().ToLower() == strSeg[0]);
            return new Peer(addrType, strSeg[1], strSeg[3]);
        }

        public override string ToString()
        {
            return $"/{_RemoteAddressType.ToString().ToLower()}/{_RemoteAddress}/ipfs/{_RemotePeerId}";
        }
    }
}