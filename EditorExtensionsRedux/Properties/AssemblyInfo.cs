using System.Reflection;
using System.Runtime.CompilerServices;

#if KSP11x
	[assembly: AssemblyTitle("EditorExtensions (for KSP 1.1.x)")]
	[assembly: AssemblyDescription("Kerbal Space Program Plugin : Editor Extensions (for KSP 1.1.x)")]
#else
	[assembly: AssemblyTitle("EditorExtensions")]
	[assembly: AssemblyDescription("Kerbal Space Program Plugin : Editor Extensions")]
#endif
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Editor Extensions")]
[assembly: AssemblyCopyright("MachXXV 2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

#if KSP11x
	[assembly: AssemblyVersion("3.3.3.1")]
#else
	[assembly: AssemblyVersion("3.3.3.*")]
#endif
