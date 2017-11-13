using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Ipfs.Api
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Peer
    {
        #region Nested

        public enum AddressType
        {
            DnsAddr,
            IP4,
            IP6
        }

        #endregion

        #region Variables

        private AddressType _RemoteAddressType;
        private string _RemoteAddress;
        private string _RemotePeerId;

        #endregion

        #region Ctor

        public Peer(AddressType addrType, string remoteAddr, string remotePeerId)
        {
            _RemoteAddressType = addrType;
            _RemoteAddress = remoteAddr;
            _RemotePeerId = remotePeerId;
        }

        #endregion

        #region Properties

        [JsonProperty]
        public AddressType RemoteAddressType
        {
            get { return _RemoteAddressType; }
            set { _RemoteAddressType = value; }
        }

        [JsonProperty]
        public string RemoteAddress
        {
            get { return _RemoteAddress; }
            set { _RemoteAddress = value; }
        }

        [JsonProperty]
        public string RemotePeerId
        {
            get { return _RemotePeerId; }
            set { _RemotePeerId = value; }
        }

        #endregion

        #region Methods

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

        #endregion
    }
}