using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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
            Assert.AreEqual(manifestEditor.Name, PACKAGE_KEY);
        }

        [Test]
        public void CanWriteName()
        {
            ManifestEditor manifestEditor = ManifestEditor.Open(PACKAGE_KEY);
            Assert.AreEqual(manifestEditor.Name, PACKAGE_KEY);
            manifestEditor.Name = "test";
            Assert.AreEqual(manifestEditor.Name, "test");
        }

        [Test]
        public void CanReload()
        {
            ManifestEditor manifestEditor = ManifestEditor.Open(PACKAGE_KEY);
            Assert.AreEqual(manifestEditor.Name, PACKAGE_KEY);
            manifestEditor.Name = "test";
            Assert.AreEqual(manifestEditor.Name, "test");
            manifestEditor.Reload();
            Assert.AreEqual(manifestEditor.Name, PACKAGE_KEY);
            Assert.AreNotEqual(manifestEditor, "test");
        }
    }
}