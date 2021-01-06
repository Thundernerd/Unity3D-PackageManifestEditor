using System.IO;

namespace TNRD.PackageManifestEditor
{
    public class PackageNotFoundException : FileNotFoundException
    {
        public PackageNotFoundException(string message, string fileName) : base(message, fileName)
        {
        }
    }
}