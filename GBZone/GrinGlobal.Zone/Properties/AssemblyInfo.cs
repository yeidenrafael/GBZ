using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("GrinGlobal.Zone")]
[assembly: AssemblyDescription("Reponsive Design using bootstrap")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Cimmyt")]
[assembly: AssemblyProduct("GrinGlobal.Zone")]
[assembly: AssemblyCopyright("Copyright©2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]


//seting log4net

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Web.config", Watch = true)]
// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("1462c90c-1d65-49dd-86a8-081e131375c8")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
