# EmbeddedDIFx
DIFx API wrapper which simplifies driver installation and uninstallation.

Goals:
* Simplify using DIFx.
* Reduce bitness concerns.
* Reduce DLL dependencies.
* Accessible through NuGet.

## Simplify using DIFx
You won't need to write your own messy interop declarations to use the library.
The Disposable pattern is used for automatic cleanup.

Example:
```
using (var difx = new DIFx())
{
	difx.DriverInstallPackage(extractedInf, DriverPackageFlags.ONLY_IF_DEVICE_PRESENT | DriverPackageFlags.FORCE)
}
```

## Reduce bitness concerns
EmbeddedDIFx contains both the x86 and x64 versions of DIFxAPI, and automatically detects and utilizes the appropriate version at runtime. This can simplify production of a single installer rather than having to make x86 and x64 versions or writing and maintaining similar runtime logic in your installer code.

## Reduce DLL dependencies
You only need to include EmbeddedDIFx.dll, instead of the x86 and x64 versions of DIFxAPI, to use DIFx.
If your goal is to reach a single-file driver installer with zero loose file dependencies, check out how the installer at [ScpDriverInterface](https://github.com/DavidRieman/ScpDriverInterface) works for inspiration.
(It extracts the .inf and related driver files on the fly as well.)

## Accessible through NuGet
TODO: Hopefully this will be "EmbeddedDIFx" on NuGet soon.
