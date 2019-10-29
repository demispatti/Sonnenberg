using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using log4net.Config;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ShellServer")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ShellServer")]
[assembly: AssemblyCopyright("Copyright © Demis Patti 2019")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("FA91A3F5-271C-421B-AE48-7963167A1A19")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.0.0.0")]
// Log4Net
[assembly: XmlConfigurator(Watch = true)]
[assembly:
    InternalsVisibleTo(
        "ShellServer, PublicKey=0024000004800000140100000602000000240000525341310008000001000100056d3f92afccc69fb67f6f1fe7e230916ac38cca6638989ef58ef32e99b6695063c42d0d2bbd80b2d6dce73220f327ba817a075e87c7af4feec7ccaf94b759c24f9260bef960162266682dd2e54155afff0636da4eaecf5d94692afb8b81f41dcb7510e675591e33cd073f83a0f59849a8df70e5856dcff7613039b6583c6f1b9ce0a242d921f32b97c3f850e348ea14a1445545dc34f51f2ef90b5c0ab224db5c48795640a84a62f03538382bbb6841c4ff4ef49e811906922a86cb19bbc53b6225b19de808a54ef71eee9a4b559a9bd47721f9d004f6dd59258060ce5a4a1447bf63c6e6335a2d302ad3135a97dd46dd2aa2e45049b64aeb71f03933f000e1")]
[assembly:
    InternalsVisibleTo(
        "ShellServer.Tests, PublicKey=002400000480000014010000060200000024000052534131000800000100010041aab028e722b319755c3f012049e5d2804b6e57a8bab72291fbc9e2ff612f8f89e2a932f69250ba621ed1658cecd702d3b8665669d65046c887d2177bd9a2875cdb57c81265ed65b076a2c0a2cf50bd9008a5498860c1456d3c3fd7c5aa51e487c4fd0160b835aecd375818b1b9af7929db7924a143fd920221b377ad663a25df19300dc269cc9989331f5043118044c50e6c8d54f013cca87d94ce33e028afff93e604f5d58330231b62f9a3f12a44caaa70601aa13042b2deef5ad76584efea97c68a33d21fe1337a673e47ffbd9808af173c7d7c3a9665f5da8f640eb6fd082a36c6aad95e0f8955df2d8609a75f1e1ebc9e2c974fe7e9fd365e5b9c22c9")]
[assembly:
    InternalsVisibleTo(
        "ShellServer.Explorables, PublicKey=002400000480000014010000060200000024000052534131000800000100010041aab028e722b319755c3f012049e5d2804b6e57a8bab72291fbc9e2ff612f8f89e2a932f69250ba621ed1658cecd702d3b8665669d65046c887d2177bd9a2875cdb57c81265ed65b076a2c0a2cf50bd9008a5498860c1456d3c3fd7c5aa51e487c4fd0160b835aecd375818b1b9af7929db7924a143fd920221b377ad663a25df19300dc269cc9989331f5043118044c50e6c8d54f013cca87d94ce33e028afff93e604f5d58330231b62f9a3f12a44caaa70601aa13042b2deef5ad76584efea97c68a33d21fe1337a673e47ffbd9808af173c7d7c3a9665f5da8f640eb6fd082a36c6aad95e0f8955df2d8609a75f1e1ebc9e2c974fe7e9fd365e5b9c22c9")]