using System.Reflection;
// This source file is shared across both .NET projects in the solution.

[assembly: AssemblyVersion( "1.1.0.0" )]

[assembly: AssemblyCompany( "const.me" )]
[assembly: AssemblyProduct( "Vrmac Graphics" )]
[assembly: AssemblyCopyright( "Copyright © const.me, 2020" )]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration( "Release" )]
#endif