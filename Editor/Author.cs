using JetBrains.Annotations;
using SimpleJSON;

namespace TNRD.PackageManifestEditor
{
    public class Author
    {
        private readonly JSONObject root;

        internal Author(JSONObject root)
        {
            this.root = root;
        }

        [PublicAPI]
        public string Name
        {
            get => root["name"].Value;
            set => root.Add("name", new JSONString(value));
        }

        [PublicAPI]
        public string Email
        {
            get => root["email"].Value;
            set => root.Add("email", new JSONString(value));
        }

        [PublicAPI]
        public string Url
        {
            get => root["url"].Value;
            set => root.Add("url", new JSONString(value));
        }
    }
}
