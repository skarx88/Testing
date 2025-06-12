# About Build-Configurations

- Debug:
	* The default development configuration. Used mainly for testing and development.
	* This configuration uses project references with full debug symbols enabled and obfuscation is disabled.
	* CAUTION: Project References are not used for MFC (c++) projects to speed up the build process! If you want a full source build, use Debug_MFC configurations, see next for more info.

- Debug_MFC:
	* All projects are used as project references to create a build.
	* This configuration can be used when the c++/mfc references need also be tested.
	* This build is _slow and ugly_ because of all the c++ stuff that needs to be recompiled. Do not use for common development and testing.

- Release:
	* The default release configuration. Uses nuget references.
	* This configuration should be mainly used for creating public releases.
	* Obfuscation is activated and no debug symbols are created. Debugging is not possible!

- Release_DEV:
	* A special _Release-like-testing-configuration_, f.e. for testing the obfuscation.
	* Obfuscation is activated, Nuget references are used and Debugging (with breakpoints) is possible but restricted only to the main solution projects.
	* Debugging throu references is only possible but restricted when Setting [Just my Code](https://learn.microsoft.com/en-us/visualstudio/debugger/just-my-code?view=vs-2022) is disabled.

# Build HowTos
## Create a ci-build locally
 - Use the "ci.bat" file. 
 - This file tries to parse & execute the commands in the ci.yml file from the build_sh - block

## Licensing: how to use in build process
### Eazfuscator
1. Installation of Nuget Package <br><br>
Add Gapotchenko.Eazfuscator.NET NuGet package to a project you want to obfuscate. <br>
Once the package is added, the project will be automatically obfuscated in Release configuration as usual.<br>
You can specify a different active configuration or pass command-line arguments to Eazfuscator.NET using [available MSBuild properties](https://learn.gapotchenko.com/eazfuscator.net/docs/integration/msbuild-integration#Eazfuscator_MSBuild_Properties). <br>
To remove the obfuscation, uninstall Gapotchenko.Eazfuscator.NET NuGet package from the corresponding project. 

2. Licensing <br><br>
Eazfuscator.NET uses a locally installed license when it works as a NuGet package. <br>
If the license is not installed, it looks for `EAZFUSCATOR_NET_LICENSE` environment variable and gets the license key from there. <br>
To license Eazfuscator.NET at a build server, configure `EAZFUSCATOR_NET_LICENSE` environment variable to contain a corresponding license key.<br><br>
Alternatively, `EazfuscatorLicense` MSBuild property may be used to specify a license for Eazfuscator.NET. <br>
This approach may be useful in environments that could not propagate the environment variables.

### VectorDraw
1. CI-Build (authorize machine)<br><br>
The VectorDraw licensing system needs to Authorize a machine before it can create a license file for the build. <br>
See issue #1 for more detail about this problem. <br/>
As a solution, Vectordraw has provided a special ci-build-key that should be used to only authorize this machine. <br><br>
_the vdAuthorizeApp.exe may not work for every case, but you can use the vdLic.exe instead with the same commands_

	<details> 
	<summary>VectorDraw 10 CI Serial (Encrypted)</summary>
	j6qVBEiPJ4MiK7rl1zenplYe6M/fMx4hCUuN50lIajtXhYg7alR/z2xIgRgvDweCeTqQVZu7CdIL9QmOTR8uGfOiF10h94qCMOQAXG8bh9/bmtIEoHVFUgt3G5LmiB5H+uquFBoLdVY5iObKEtpGCzo08N+0LqFw0POzQp3NZSI=
	</details> 

2. MsBuild.NET (create license*.lic file)<br><br>
If you are using version 10+ nugets you also need to create a .lic file on the post publish event so the license file will be distributed among with all the other neccessary files of your application.<br>

    ```
    <Target Name = "PostBuild" AfterTargets="PostBuildEvent"><br>
        <Exec Command = "&quot;$(VDRAWDEV)vdlic.exe&quot;  &quot;$(TargetPath)&quot;" /><br>
    </Target ><br>
    <Target Name="AddPayloadsFolder" AfterTargets="Publish"><br>
        <Exec Command = "&quot;$(VDRAWDEV)vdlic.exe&quot;  &quot;$(PublishDir)$(targetfilename)&quot;" /><br>
    </Target ><br>
    ```
	[source on vdraw.com](https://www.vdraw.com/articles/general/70002172-new-license-managment/)
    
# MSBuild
- [Reference for .NET SDK](https://learn.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props#assemblyinfo-properties)
- [Command Macros](https://learn.microsoft.com/en-us/cpp/build/reference/common-macros-for-build-commands-and-properties?view=msvc-170)
- [well-known Properties](https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-reserved-and-well-known-properties?view=vs-2022)
- [Common Properties](http://msdn.microsoft.com/en-us/library/bb629394.aspx)
- [Microsoft Documentation on Github](https://github.com/MicrosoftDocs) -> [visualstudio-docs](https://github.com/MicrosoftDocs/visualstudio-docs) -> [msbuild-reserved-and-well-known-properties](https://github.com/MicrosoftDocs/visualstudio-docs/blob/main/docs/msbuild/msbuild-reserved-and-well-known-properties.md)
- [MsBuild property functions](https://devblogs.microsoft.com/visualstudio/msbuild-property-functions)
- [Difference between .props and .target files](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-your-build?view=vs-2022#choose-between-adding-properties-to-a-props-or-targets-file)
    
# Build Tools
## Create a file-hash
### How to create a file hash with powershell

Usage:
To use Get-Filehash, click Start and type PowerShell to launch a Powershell console. 
Then go to the desired directory and run `Get-Filehash` to create a SHA256 checksum:

`Get-Filehash [FileName]`

[source](https://www.thomas-krenn.com/de/wiki/Get-Filehash_-_sha256sum_Windows)

### Ho to create hashes for Wix-Burn (SHA512)

- For wix-burn-installer payloads you need to use the parameter -Algorithm SHA512 otheriwse it will not work: 

- `Get-FileHash -Algorithm sha512 [FileName] | Select-Object Hash | Out-File hash.txt`

- After executing this cmdlet, the calculated hash should be obtainable in the file "hash.txt"

An easier alternative is now available by using the [ToolBox](https://ulm-dev.zuken.com/Team-Erlangen/scripts)

## BinLog files

- BinLog files are special binary log files [created by msbuild](https://github.com/dotnet/msbuild/blob/main/documentation/wiki/Providing-Binary-Logs.md)
- You can open these files with the [MsBuild Structured Log Viewer Tool](https://msbuildlog.com/)
- (Optional) Use the [ToolBox](https://ulm-dev.zuken.com/Team-Erlangen/scripts) for automatic installation when checking software/system
