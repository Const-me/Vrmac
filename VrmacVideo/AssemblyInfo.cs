using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle( "Vrmac Graphics native interop assembly" )]
[assembly: AssemblyDescription( ".NET projection of COM API implemented by the native DLL, Vrmac.dll on Windows, or libVrmac.so on Linux" )]

[assembly: ComVisible( false )]

[assembly: Guid( "494b2840-cc8e-40b5-9f12-bebfa5bf6811" )]
// For the tests
[assembly: InternalsVisibleTo( "RenderSamples" )]