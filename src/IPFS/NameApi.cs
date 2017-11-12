using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ipfs.Api
{
    /// <summary>
    ///   Helps resolve information from ipns
    /// </summary>
    /// <remarks>
    ///   This API is accessed via the <see cref="IpfsClient.Name"/> property.
    /// </remarks>
    public class NameApi
    {
        IpfsClient ipfs;

        internal NameApi(IpfsClient ipfs)
        {
            this.ipfs = ipfs;
        }

        /// <summary>
        ///   Will resolve the ipfs path for the ipns entry passed in hash
        /// </summary>
        /// <param name="hash">
        ///   The <see cref="string"/> representation of a ipns entry
        /// </param>
        /// <param name="cancel"></param>
        /// <param name="cancel">
        ///   Is used to stop the task.  When cancelled, the <see cref="TaskCanceledException"/> is raised.
        /// </param>
        public async Task<string> ResolveAsync(string hash = null, CancellationToken cancel = default(CancellationToken))
        {
            string json = await ipfs.DoCommandAsync("name/resolve", cancel, hash);

            JObject jObject = JObject.Parse(json);
            return jObject.Value<string>("Path");
        }

        /// <summary>
        /// This will publish a file hash to IPNS
        /// </summary>
        /// <param name="hashFilename">
        ///   Hash of the already uploaded file to publish
        /// </param>
        /// <param name="cancel">
        ///   Is used to stop the task.  When cancelled, the <see cref="TaskCanceledException"/> is raised.
        /// </param>
        /// <returns>True if success</returns>
        public async Task<bool> PublishAsync(string hashFilename, CancellationToken cancel = default(CancellationToken))
        {
            return await PublishAsync(Encoding.UTF8.GetBytes(hashFilename), cancel);
        }

        /// <summary>
        /// This will publish a file hash to IPNS
        /// </summary>
        /// <param name="hashFilename">
        ///   Hash of the already uploaded file to publish
        /// </param>
        /// <param name="cancel">
        ///   Is used to stop the task.  When cancelled, the <see cref="TaskCanceledException"/> is raised.
        /// </param>
        /// <returns>True if success</returns>
        public async Task<bool> PublishAsync(byte[] hashFilename, CancellationToken cancel = default(CancellationToken))
        {
            string json = await ipfs.UploadAsync("name/publish", cancel, hashFilename);
            JObject jObject = JObject.Parse(json);
            return jObject["Name"] != null && jObject["Value"] != null;
        }

        /// <summary>
        /// This will publish a file hash to IPNS
        /// </summary>
        /// <param name="hashFilename">
        ///   Hash of the already uploaded file to publish
        /// </param>
        /// <param name="ownerHash">
        ///   The peer id or key to use as the owner
        /// </param>
        /// <param name="cancel">
        ///   Is used to stop the task.  When cancelled, the <see cref="TaskCanceledException"/> is raised.
        /// </param>
        /// <returns>True if success</returns>
        public async Task<bool> PublishAsync(string hashFilename, string ownerHash, CancellationToken cancel = default(CancellationToken))
        {
            return await PublishAsync(Encoding.UTF8.GetBytes(hashFilename), $"key={ownerHash}", cancel);
        }

        /// <summary>
        /// This will publish a file hash to IPNS
        /// </summary>
        /// <param name="hashFilename">
        ///   Hash of the already uploaded file to publish
        /// </param>
        /// <param name="ownerHash">
        ///   The peer id or key to use as the owner
        /// </param>
        /// <param name="cancel">
        ///   Is used to stop the task.  When cancelled, the <see cref="TaskCanceledException"/> is raised.
        /// </param>
        /// <returns>True if success</returns>
        public async Task<bool> PublishAsync(byte[] hashFilename, string ownerHash, CancellationToken cancel = default(CancellationToken))
        {
            string json = await ipfs.UploadAsync("name/publish", cancel, hashFilename, !ownerHash.Contains("key=") ? $"key={ownerHash}" : ownerHash);
            JObject jObject = JObject.Parse(json);
            return jObject["Name"] != null && jObject["Value"] != null;
        }
    }
}
