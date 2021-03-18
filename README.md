# Package Manifest Editor

<p align="center">
	<img alt="GitHub package.json version" src ="https://img.shields.io/github/package-json/v/Thundernerd/Unity3D-PackageManifestEditor" />
	<a href="https://github.com/Thundernerd/Unity3D-PackageManifestEditor/issues">
		<img alt="GitHub issues" src ="https://img.shields.io/github/issues/Thundernerd/Unity3D-PackageManifestEditor" />
	</a>
	<a href="https://github.com/Thundernerd/Unity3D-PackageManifestEditor/pulls">
		<img alt="GitHub pull requests" src ="https://img.shields.io/github/issues-pr/Thundernerd/Unity3D-PackageManifestEditor" />
	</a>
	<a href="https://github.com/Thundernerd/Unity3D-PackageManifestEditor/blob/master/LICENSE.md">
		<img alt="GitHub license" src ="https://img.shields.io/github/license/Thundernerd/Unity3D-PackageManifestEditor" />
	</a>
	<img alt="GitHub last commit" src ="https://img.shields.io/github/last-commit/Thundernerd/Unity3D-PackageManifestEditor" />
</p>

Package Manifest Editor is a utility to edit package manifests through code.

## Installation
1. The package is available on the [openupm registry](https://openupm.com). You can install it via [openupm-cli](https://github.com/openupm/openupm-cli).
```
openupm add net.tnrd.packagemanifesteditor
```
2. Installing through a [Unity Package](http://package-installer.glitch.me/v1/installer/package.openupm.com/net.tnrd.packagemanifesteditor?registry=https://package.openupm.com) created by the [Package Installer Creator](https://package-installer.glitch.me) from [Needle](https://needle.tools)

[<img src="https://img.shields.io/badge/-Download-success?style=for-the-badge"/>](http://package-installer.glitch.me/v1/installer/package.openupm.com/net.tnrd.packagemanifesteditor?registry=https://package.openupm.com)

## Usage

Before you can do anything you'll have to open a manifest for editing like so
```c#
private void Foo()
{
    ManifestEditor editor = ManifestEditor.OpenById("Package Folder Name");
}
```
I personally put my packages in folders with the reverse domain name notation so I would use something along the lines of 
```ManifestEditor.OpenById("tld.domain.packagename");```

### Supported attributes
This package follows Unity's manifest definition which can be found here: [https://docs.unity3d.com/Manual/upm-manifestPkg.html](https://docs.unity3d.com/Manual/upm-manifestPkg.html)

The odd one out are is the dependencies attribute. This one can only be read by using the `Dependencies` property on the `ManifestEditor`.

To add a dependency one can use the `AddDependency` method like so
```c#
private void Foo()
{
    ManifestEditor editor = ManifestEditor.OpenById("tld.domain.packagename");
    editor.AddDependency("tld.domain.packagedependency", "1.0.0");
}
```
The second parameter requires a version number according to the Semantic Versioning spec using [Artees' SemVer](https://github.com/Artees/Unity-SemVer).

Removing a dependency is done using the `RemoveDependency` method like so
```c#
private void Foo()
{
    ManifestEditor editor = ManifestEditor.OpenById("tld.domain.packagename");
    editor.RemoveDependency("tld.domain.packagedependency");
}
```

### Saving
Once you're done with your changes you can save the manifest file. It will overwrite the existing one on disk.
```c#
private void Foo()
{
    ManifestEditor editor = ManifestEditor.OpenById("tld.domain.packagename");
    // Do some changes
    editor.Save();
}
```

### Undoing your changes
If you want to undo your changes you can simply reload the manifest from disk like so
```c#
private void Foo()
{
    ManifestEditor editor = ManifestEditor.OpenById("tld.domain.packagename");
    // Some changes applied that you want to undo
    editor.Reload();
}
```


## Support
**Package Manifest Editor** is a small and open-source utility that I hope helps other people. It is by no means necessary but if you feel generous you can support me by donating.

[![ko-fi](https://www.ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/J3J11GEYY)

## Contributing
Pull requests are welcomed. Please feel free to fix any issues you find, or add new features.
