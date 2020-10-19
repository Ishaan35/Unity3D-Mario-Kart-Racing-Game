# com.unity.ads Packman Package

## Local Development

#### Getting Started

1. To get started with this repo, you need the following tools:
- An empty Unity 2018.3 project (Can be created from Unity Hub, a standalone version of Unity, or the command line):
  ```sh
  /Applications/Unity/Hub/Editor/2018.3.3f1/Unity.app/Contents/MacOS/Unity -quit -createProject UnityAdsTest
  ```
- This repo, cloned to your local file system (We will assume its cloned at `~/git/com.unity.ads` going forward)

2. Open the empty Unity project created and navigate to: `<ProjectRoot>/Packages/`
2. Create a symlink to the git repo you cloned
	```ln -s ~/git/com.unity.ads com.unity.ads```
2. Update the manifest.json file located in `<ProjectRoot>/Packages/` with the following change:
	```"com.unity.ads": "3.0.3"` -> `"com.unity.ads": "file:com.unity.ads"```

Thats it.  Open the empty project you created in step 1 and if you look in the file explorer under the Packages directory you will see the local package source code and its fully editable with whatever default editor you use with Unity.


#### Compiling

The Package manager source code is automatically built when you first open a project, and anytime any of the source code managed by the assembly definition files changes.  Any time you modify any of the source code in our project, unity will rebuild the package dlls immediately upon returning focus to unity.  You can then immediately begin testing your changes in the editor without needing to invoke any build scripts or move assets around.

#### .NET Framework version

*more details to follow*

.NET 3.5 should be used when developing the package source code in order for us to maintain backwards compatability to Unity 5.6.6.  We will solidify this decision with the 3.1 release, and for now this is mearly a guideline that should be adheared to until we decide to officially swap to .NET 4.6 for development.  As a side note, this restricts us to C#4 constructs as Unity pairs .NET versions with C# versions and currently there is no way to alter this.

## Testing 

Inside Unity, navigate to `Window => General => Test Runner` to open the test runner dialog. Inside, you can then click `Run All` to run tests.

After setting up the local environment, all tests in the package should be visible in the Test Runner window.  If you want to be able to run the tests for a given package without going to the effort of setting up the development environment above, you can simply add the following to your package manager manifest file after the dependencies section: `"testables": [ "com.unity.ads" ]`.  You should only need to add the testables json when you want to run the tests for a given package that you are not setup to locally develop for.  This is useful when you are testing a package on staging and want to run its tests locally.

## Staging

Deploying the package to Staging happens by creating a tag using the following format: `v3.0.3-preview.1` or `v3.0.3`.  Once the tag is created, the yamato build system will run all the test builds and if they pass, it will automatically deploy the package to bintray staging.  You can monitor the status of the build jobs [here](https://yamato.prd.cds.internal.unity3d.com/jobs/36-Ads%2520Package)

#### Testing Staged Packages

In order to test a package that has been deployed to staging, you must update your project's package manifest to look at staging instead of production.  Navigate to: `<ProjectRoot>/Packages/` and open `manifest.json`.  Add the following line as a sibling element to dependencies (before or after this json element): `"registry": "https://artifactory.prd.cds.internal.unity3d.com/artifactory/api/npm/upm-candidates"`

## Production

Refer to [this](https://docs.google.com/document/d/1yN5cEnba4kJEU_hKjwnpl4h0TioWS7l2bo5cQSG9UOc/edit) doc on how to release the package to production.  Please note that you should do all your testing as a preview package, and *ONLY* once you are *COMPLETELY* confident the package is fully tested and ready to be merged to production should you cut the final non preview version, deploy that to staging, and finally deploy that to production.  FYI, once a version is pushed to staging, you cannot delete it, and so if your not ready to push that version to production as is, you will have forever lost that version number.

## Asset Store

The Asset Store (.unitypackage) is build via a shell script in the AssetStoreTemplate~ folder.  It compiles platform specific dll's using mcs (Mono Compiler).  This compiler is capable of converting the code from C#7 ([*most language features supported*](https://www.mono-project.com/docs/about-mono/languages/csharp/)) down to .NET 2.0 compliant code.  By doing this we are then capatible with Unity 5.6.6 running on .NET 3.5. 
