using System.Collections.Generic;
using System.IO;
using Artees.UnitySemVer;
using JetBrains.Annotations;
using SimpleJSON;
using UnityEngine;

namespace TNRD.PackageManifestEditor
{
    public class ManifestEditor
    {
        private readonly string path;

        private JSONObject root;

        private ManifestEditor(string path)
        {
            this.path = path;
            Reload();
        }

        /// <summary>
        /// The officially registered package name. This name must conform to the Unity Package Manager naming convention, which uses reverse domain name notation.
        /// </summary>
        [PublicAPI]
        public string Name
        {
            get => root["name"].Value;
            set => root["name"].Value = value;
        }

        /// <summary>
        /// A user-friendly name to appear in the Unity Editor (for example, in the Project Browser, the Package Manager window, etc.).
        /// </summary>
        [PublicAPI]
        public string DisplayName
        {
            get => root["displayName"].Value;
            set => root["displayName"].Value = value;
        }

        /// <summary>
        /// A brief description of the package. This is the text that appears in the details view of the Package Manager window. Any UTF–8 character code is supported.
        /// </summary>
        [PublicAPI]
        public string Description
        {
            get => root["description"].Value;
            set => root["description"].Value = value ?? "";
        }

        /// <summary>
        /// Part of a Unity version
        /// Format: <MAJOR>.<MINOR>
        /// </summary>
        public string UnityVersion
        {
            get => root["unity"].Value;
            set => root["unity"].Value = value;
        }

        /// <summary>
        /// Part of a Unity version
        /// Format: <UPDATE><RELEASE>
        /// </summary>
        [PublicAPI]
        public string UnityReleaseVersion
        {
            get => root["unityRelease"].Value;
            set
            {
                if (value == null)
                {
                    root.Remove("unityRelease");
                }
                else
                {
                    root["unityRelease"].Value = value;
                }
            }
        }

        /// <summary>
        /// An array of package dependencies
        /// </summary>
        [PublicAPI]
        public Dependency[] Dependencies
        {
            get
            {
                if (!root.HasKey("dependencies"))
                    return new Dependency[0];

                List<Dependency> dependencies = new List<Dependency>();

                JSONObject dependenciesObj = root["dependencies"].AsObject;
                foreach (KeyValuePair<string, JSONNode> kvp in dependenciesObj)
                {
                    Dependency dependency = new Dependency
                    {
                        Id = kvp.Key,
                        Version = SemVer.Parse(kvp.Value.Value)
                    };

                    dependencies.Add(dependency);
                }

                return dependencies.ToArray();
            }
        }

        /// <summary>
        /// An array of keywords used by the Package Manager search APIs. This helps users find relevant packages.
        /// </summary>
        [PublicAPI]
        public string[] Keywords
        {
            get => root["keywords"].AsStringArray;
            set => root["keywords"].AsStringArray = value;
        }

        /// <summary>
        /// Author of the package
        /// </summary>
        [PublicAPI]
        public Author Author
        {
            get
            {
                if (!root.HasKey("author"))
                {
                    root.Add("author", new JSONObject());
                }

                return new Author(root["author"].AsObject);
            }
        }

        /// <summary>
        /// Adds a dependency to the manifest
        /// Updates the version if it already exists
        /// </summary>
        /// <param name="id">The id of the dependency</param>
        /// <param name="version">The version of the dependency</param>
        [PublicAPI]
        public void AddDependency(string id, SemVer version)
        {
            if (!root.HasKey("dependencies"))
            {
                root.Add("dependencies", new JSONObject());
            }

            JSONObject dependencies = root["dependencies"].AsObject;

            if (dependencies.HasKey("id"))
            {
                dependencies[id].Value = version;
            }
            else
            {
                dependencies.Add(id, new JSONString(version));
            }
        }

        /// <summary>
        /// Removes a dependency from the manifest
        /// </summary>
        /// <param name="id">The id of the dependency</param>
        [PublicAPI]
        public void RemoveDependency(string id)
        {
            if (!root.HasKey("dependencies"))
                return;

            JSONObject dependencies = root["dependencies"].AsObject;
            dependencies.Remove(id);
        }

        /// <summary>
        /// Reloads the manifest from disk
        /// </summary>
        [PublicAPI]
        public void Reload()
        {
            string json = File.ReadAllText(path);
            root = JSON.Parse(json).AsObject;
        }

        /// <summary>
        /// Saves the current state of the package to disk
        /// </summary>
        [PublicAPI]
        public void Save()
        {
            string json = root.ToString(2);
            Debug.Log(json);
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Opens a manifest file for editing
        /// </summary>
        /// <param name="id">The name of the folder the package is located in</param>
        /// <returns><see cref="ManifestEditor"/></returns>
        /// <exception cref="PackageNotFoundException">Thrown when the package cannot be found</exception>
        [PublicAPI]
        public static ManifestEditor Open(string id)
        {
            string path = $"Packages/{id}/package.json";

            if (!File.Exists(path))
            {
                throw new PackageNotFoundException("Package not found!", path);
            }

            return new ManifestEditor(path);
        }
    }
}