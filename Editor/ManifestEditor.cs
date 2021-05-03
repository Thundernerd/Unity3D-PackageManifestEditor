using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Artees.UnitySemVer;
using JetBrains.Annotations;
using SimpleJSON;

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
        /// The officially registered package name.
        /// This name must conform to the Unity Package Manager naming convention, which uses reverse domain name notation.
        /// </summary>
        [PublicAPI]
        public string Name
        {
            get => root["name"].Value;
            set => root["name"].Value = value;
        }

        /// <summary>
        /// The package version number (MAJOR.MINOR.PATCH).
        /// </summary>
        [PublicAPI]
        public SemVer Version
        {
            get => root["version"].Value;
            set => root["version"].Value = value;
        }

        /// <summary>
        /// A user-friendly name to appear in the Unity Editor
        /// For example, in the Project Browser, the Package Manager window, etc.
        /// </summary>
        [PublicAPI]
        public string DisplayName
        {
            get => root["displayName"].Value;
            set => root["displayName"].Value = value;
        }

        /// <summary>
        /// A brief description of the package. This is the text that appears in the details view of the Package Manager window.
        /// Any UTF–8 character code is supported.
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
        [PublicAPI]
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

        [PublicAPI]
        public Sample[] Samples
        {
            get
            {
                if (!root.HasKey("samples"))
                    return new Sample[0];

                JSONArray samplesArray = root["samples"].AsArray;
                if (samplesArray == null)
                    return new Sample[0];

                List<Sample> samples = new List<Sample>();
                foreach (KeyValuePair<string, JSONNode> kvp in samplesArray)
                {
                    Sample sample = new Sample((JSONObject) kvp.Value);
                    samples.Add(sample);
                }

                return samples.ToArray();
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
        /// Adds a sample to the manifest
        /// </summary>
        /// <param name="displayName">The name that should be displayed for this sample</param>
        /// <param name="path">The path of the sample relative to the package's root folder</param>
        /// <param name="description">Optional description for the sample</param>
        [PublicAPI]
        public void AddSample([NotNull] string displayName, [NotNull] string path, [CanBeNull] string description = "")
        {
            if (!root.HasKey("samples"))
            {
                root.Add("samples", new JSONArray());
            }

            Sample sample = new Sample(new JSONObject())
            {
                DisplayName = displayName,
                Path = path,
                Description = description
            };

            JSONArray samples = root["samples"].AsArray;

            if (samples == null)
            {
                samples = new JSONArray();
            }

            samples.Add(sample.Root);
            root["samples"] = samples;
        }

        /// <summary>
        /// Tries to remove a sample. Can be done with either or all parameters
        /// </summary>
        /// <param name="displayName">The displayName to search for</param>
        /// <param name="path">The path to search for</param>
        /// <returns>True if it removed a sample, false if it didn't</returns>
        [PublicAPI]
        public bool RemoveSample([CanBeNull] string displayName, [CanBeNull] string path)
        {
            if (!root.HasKey("samples"))
            {
                return false;
            }

            bool hasDisplayName = !string.IsNullOrEmpty(displayName);
            bool hasPath = !string.IsNullOrEmpty(path);

            if (!hasDisplayName && !hasPath)
            {
                return false;
            }

            Sample[] existingSamples = Samples;
            Sample sample;

            if (hasDisplayName && hasPath)
            {
                sample = existingSamples.FirstOrDefault(x => x.DisplayName == displayName && x.Path == path);
            }
            else if (hasDisplayName)
            {
                sample = existingSamples.FirstOrDefault(x => x.DisplayName == displayName);
            }
            else
            {
                sample = existingSamples.FirstOrDefault(x => x.Path == path);
            }

            if (sample == null)
            {
                return false;
            }

            int index = Array.IndexOf(existingSamples, sample);
            JSONArray samples = root["samples"].AsArray;
            samples.Remove(index);
            root["samples"] = samples;

            return true;
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
            dependencies.Add(id, new JSONString(version));
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
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Opens a manifest file for editing
        /// </summary>
        /// <param name="id">The name of the folder the package is located in</param>
        /// <returns>
        ///     <see cref="ManifestEditor" />
        /// </returns>
        /// <exception cref="PackageNotFoundException">Thrown when the package cannot be found</exception>
        [PublicAPI, Obsolete("Use ManifestEditor.OpenById instead")]
        public static ManifestEditor Open(string id)
        {
            return OpenById(id);
        }

        /// <summary>
        /// Opens a manifest file for editing
        /// </summary>
        /// <param name="id">The name of the folder the package is located in</param>
        /// <returns>
        ///     <see cref="ManifestEditor" />
        /// </returns>
        /// <exception cref="PackageNotFoundException">Thrown when the package cannot be found</exception>
        [PublicAPI]
        public static ManifestEditor OpenById(string id)
        {
            string path = $"Packages/{id}/package.json";
            return OpenByPath(path);
        }

        /// <summary>
        /// Opens a manifest file for editing
        /// </summary>
        /// <param name="path">The name of the folder the package is located in</param>
        /// <returns>
        ///     <see cref="ManifestEditor" />
        /// </returns>
        /// <exception cref="PackageNotFoundException">Thrown when the package cannot be found</exception>
        [PublicAPI]
        public static ManifestEditor OpenByPath(string path)
        {
            bool isDirectory = Directory.Exists(path) && !File.Exists(path);

            if (isDirectory)
            {
                string combinedPath = Path.Combine(path, "package.json");
                if (File.Exists(combinedPath))
                {
                    return new ManifestEditor(combinedPath);
                }

                throw new PackageNotFoundException("Given path is a directory", path);
            }

            if (!File.Exists(path))
            {
                throw new PackageNotFoundException("Package not found!", path);
            }

            return new ManifestEditor(path);
        }
    }
}
