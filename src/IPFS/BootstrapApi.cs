using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ipfs.Api
{
    public class BootstrapApi
    {
        IpfsClient ipfs;

        internal BootstrapApi(IpfsClient ipfs)
        {
            this.ipfs = ipfs;
        }

        /// <summary>
        ///   Will explictly add a peer for ipfs to connect with
        /// </summary>
        /// <param name="peer">
        ///   Peer information to be added
        /// </param>
        /// <param name="cancel">
        ///   Is used to stop the task.  When cancelled, the <see cref="TaskCanceledException"/> is raised.
        /// </param>
        /// <returns></returns>
        public async Task AddPeer(Peer peer, CancellationToken cancel = default(CancellationToken))
        {
            await ipfs.PostCommandAsync("bootstrap/add", cancel, peer.ToString());
        }

        /// <summary>
        ///   Adds a peer into the bootstrap peer list
        /// </summary>
        /// <param name="peer">
        ///   Peer information to be added
        /// </param>
        /// <param name="cancel">
        ///   Is used to stop the task.  When cancelled, the <see cref="TaskCanceledException"/> is raised.
        /// </param>
        public async Task AddDefaultPeer(Peer peer, CancellationToken cancel = default(CancellationToken))
        {
            await ipfs.PostCommandAsync("bootstrap/add/default", cancel, peer.ToString());
        }

        /// <summary>
        ///   Will list all of the peers
        /// </summary>
        /// <param name="cancel">
        ///   Is used to stop the task.  When cancelled, the <see cref="TaskCanceledException"/> is raised.
        /// </param>
        /// <returns>A list of <see cref="Peer"/> objects</returns>
        public async Task<List<Peer>> ListAllPeers(CancellationToken cancel = default(CancellationToken))
        {
            string json = await ipfs.PostCommandAsync("bootstrap/list", cancel);
            return JObject.Parse(json)["Peers"]
                .Select(o => Peer.Create(o))
                .ToList();
        }

        /// <summary>
        ///   Will remove all of the peers in the bootstrap list
        /// </summary>
        /// <param name="cancel">
        ///   Is used to stop the task.  When cancelled, the <see cref="TaskCanceledException"/> is raised.
        /// </param>
        public async Task RemoveAllDefaultPeers(CancellationToken cancel = default(CancellationToken))
        {
            await ipfs.PostCommandAsync("bootstrap/rm/all", cancel);
        }
    }
}
