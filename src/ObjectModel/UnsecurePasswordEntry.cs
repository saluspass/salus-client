using Newtonsoft.Json;

namespace Salus
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal class UnsecurePasswordEntry
    {
        [JsonProperty]
        public string Username;

        [JsonProperty]
        public string Password;

        [JsonProperty]
        public string Website;

        internal UnsecurePasswordEntry(PasswordEntry entry)
        {
            Username = entry.Username;
            Password = entry.Password;
            Website = entry.Website;
        }
    }
}
