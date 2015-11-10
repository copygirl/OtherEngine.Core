using System;
using System.Reflection;

[assembly: AssemblyProduct("OtherEngine")]
[assembly: AssemblyTitle("OtherEngine.Core")]
[assembly: AssemblyCopyright("copygirl")]

[assembly: AssemblyVersion("3.0")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: CLSCompliant(true)]

