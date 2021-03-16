using NUnit.Framework;

namespace TNRD.PackageManifestEditor
{
    public class PackageManifestEditorTests
    {
        private const string PACKAGE_KEY = "net.tnrd.packagemanifesteditor";

        // A Test behaves as an ordinary method
        [Test]
        public void CanOpenManifest()
        {
            ManifestEditor manifestEditor = ManifestEditor.Open(PACKAGE_KEY);
            Assert.IsNotNull(manifestEditor);
            // Use the Assert class to test conditions
        }

        [Test]
        public void CanReadName()
        {
            ManifestEditor manifestEditor = ManifestEditor.Open(PACKAGE_KEY);
            Assert.AreEqual(PACKAGE_KEY, manifestEditor.Name);
        }

        [Test]
        public void CanWriteName()
        {
            ManifestEditor manifestEditor = ManifestEditor.Open(PACKAGE_KEY);
            Assert.AreEqual(PACKAGE_KEY, manifestEditor.Name);
            manifestEditor.Name = "test";
            Assert.AreEqual("test", manifestEditor.Name);
        }

        [Test]
        public void CanReload()
        {
            ManifestEditor manifestEditor = ManifestEditor.Open(PACKAGE_KEY);
            Assert.AreEqual(PACKAGE_KEY, manifestEditor.Name);
            manifestEditor.Name = "test";
            Assert.AreEqual("test", manifestEditor.Name);
            manifestEditor.Reload();
            Assert.AreEqual(PACKAGE_KEY, manifestEditor.Name);
            Assert.AreNotEqual(manifestEditor, "test");
        }

        [Test]
        public void AddSample()
        {
            ManifestEditor manifestEditor = ManifestEditor.Open(PACKAGE_KEY);
            Assert.AreEqual(0, manifestEditor.Samples.Length);
            manifestEditor.AddSample("TestSample", "Samples~//TestSample", "TestSampleDescription");
            Assert.AreEqual(1, manifestEditor.Samples.Length);
        }

        [Test]
        public void RemoveSample()
        {
            static void AddDummySample(ManifestEditor manifestEditor)
            {
                Assert.AreEqual(0, manifestEditor.Samples.Length);
                manifestEditor.AddSample("TestSample", "Samples~//TestSample", "TestSampleDescription");
                Assert.AreEqual(1, manifestEditor.Samples.Length);
            }

            ManifestEditor manifestEditor = ManifestEditor.Open(PACKAGE_KEY);

            AddDummySample(manifestEditor);
            bool result = manifestEditor.RemoveSample("TestSample", null);
            Assert.AreEqual(true, result);
            Assert.AreEqual(0, manifestEditor.Samples.Length);

            AddDummySample(manifestEditor);
            result = manifestEditor.RemoveSample(null, "Samples~//TestSample");
            Assert.AreEqual(true, result);
            Assert.AreEqual(0, manifestEditor.Samples.Length);

            AddDummySample(manifestEditor);
            result = manifestEditor.RemoveSample("TestSample", "Samples~//TestSample");
            Assert.AreEqual(true, result);
            Assert.AreEqual(0, manifestEditor.Samples.Length);
        }
    }
}
