using JetBrains.Annotations;
using SimpleJSON;

namespace TNRD.PackageManifestEditor
{
    public class Sample
    {
        internal JSONObject Root { get; }

        internal Sample(JSONObject root)
        {
            Root = root;
        }

        [PublicAPI]
        public string DisplayName
        {
            get => Root["displayName"].Value;
            set => Root.Add("displayName", new JSONString(value));
        }

        [PublicAPI]
        public string Description
        {
            get => Root["description"].Value;
            set => Root.Add("description", new JSONString(value));
        }

        [PublicAPI]
        public string Path
        {
            get => Root["path"].Value;
            set => Root.Add("path", new JSONString(value));
        }
    }
}
