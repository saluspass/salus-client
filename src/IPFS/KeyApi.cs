using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ipfs.Api
{
    public class KeyApi
    {
        IpfsClient ipfs;

        internal KeyApi(IpfsClient ipfs)
        {
            this.ipfs = ipfs;
        }

        /// <summary>
        /// Will generate and store a new key pair in the local keystore
        /// </summary>
        /// <param name="keyName">
        ///     Name of the key
        /// </param>
        /// <param name="keyType">
        ///     Type of the key to create (ex. rsa, ed25519)
        /// </param>
        /// <param name="size">
        ///     Size of the key to generate
        /// </param>
        /// <param name="cancel">
        ///   Is used to stop the task.  When cancelled, the <see cref="TaskCanceledException"/> is raised.
        /// </param>
        public async Task<string> Generate(string keyName, string keyType = "rsa", int size = 2048, CancellationToken cancel = default(CancellationToken))
        {
            string json = await ipfs.PostCommandAsync("key/gen", cancel, keyName, $"type={keyType}", $"size={size}");
            return JObject.Parse(json)
                .Value<string>("ID");
        }

        /// <summary>
        /// Will return the public hash associated with key
        /// </summary>
        /// <param name="keyName">
        ///     The key who's public hash to obtain
        /// </param>
        /// <param name="cancel">
        ///   Is used to stop the task.  When cancelled, the <see cref="TaskCanceledException"/> is raised.
        /// </param>
        public async Task<string> GetAdditionalInformation(string keyName, CancellationToken cancel = default(CancellationToken))
        {
            string json = await ipfs.PostCommandAsync("key/list", cancel, null, $"l=true");
            return JObject.Parse(json)["Keys"]
                .FirstOrDefault(p => p.Value<string>("Name") == keyName)
                ?.Value<string>("Id");
        }
    }
}
