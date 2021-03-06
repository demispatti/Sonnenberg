// <copyright file="PexAssemblyInfo.cs">Copyright © Demis Patti 2019</copyright>

using Microsoft.Pex.Framework.Coverage;
using Microsoft.Pex.Framework.Creatable;
using Microsoft.Pex.Framework.Instrumentation;
using Microsoft.Pex.Framework.Settings;
using Microsoft.Pex.Framework.Validation;

// Microsoft.Pex.Framework.Settings
[assembly: PexAssemblySettings(TestFramework = "NUnit3")]

// Microsoft.Pex.Framework.Instrumentation
[assembly: PexAssemblyUnderTest("ServerManager")]
[assembly: PexInstrumentAssembly("Common")]
[assembly: PexInstrumentAssembly("Language")]
[assembly: PexInstrumentAssembly("ShellServer")]
[assembly: PexInstrumentAssembly("log4net")]
[assembly: PexInstrumentAssembly("SharpShell")]

// Microsoft.Pex.Framework.Creatable
[assembly: PexCreatableFactoryForDelegates]

// Microsoft.Pex.Framework.Validation
[assembly: PexAllowedContractRequiresFailureAtTypeUnderTestSurface]
[assembly: PexAllowedXmlDocumentedException]

// Microsoft.Pex.Framework.Coverage
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Common")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Language")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "ShellServer")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "log4net")]
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "SharpShell")]