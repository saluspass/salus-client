using Ipfs.Api;

namespace Salus
{
    public static class ApiExtensions
    {
        public static KeyApi KeyApi(this IpfsClient client)
        {
            return new KeyApi(client);
        }

        public static NameApi NameApi(this IpfsClient client)
        {
            return new NameApi(client);
        }

        public static BootstrapApi BootstrapApi(this IpfsClient client)
        {
            return new BootstrapApi(client);
        }
    }
}