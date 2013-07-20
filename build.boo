solution = "MarkEmbling.Utils.sln"
configuration = "release"
test_assembly = "MarkEmbling.Utils.Tests/bin/${configuration}/MarkEmbling.Utils.Tests.dll"

target default, (compile, test, prep):
  pass

desc "Compiles the solution"
target compile:
  msbuild(file: solution, configuration: configuration, version: "4.0")

desc "Executes the tests"
target test, (compile):
  with nunit(assembly: test_assembly, toolPath: "packages/NUnit.Runners.2.6.2/tools/nunit-console.exe")
  
desc "Copies the binaries to the 'build/bin' directory"
target prep, (compile, test):
  rmdir("build/bin")
  
  with FileList("MarkEmbling.Utils/bin/${configuration}"):
    .Include("*.{dll,exe,config}")
    .ForEach def(file):
      file.CopyToDirectory("build/bin")

  with FileList("MarkEmbling.Utils.Forms/bin/${configuration}"):
    .Include("*.{dll,exe,config}")
    .ForEach def(file):
      file.CopyToDirectory("build/bin")

desc "Creates the NuGet packages"
target package, (prep):
  with nuget_pack():
    .toolPath = "packages/NuGet.CommandLine.2.6.1/tools/NuGet.exe"
    .nuspecFile = "build/nuget/MarkEmbling.Utils.nuspec"
    .outputDirectory = "build/nuget"

  with nuget_pack():
    .toolPath = "packages/NuGet.CommandLine.2.6.1/tools/NuGet.exe"
    .nuspecFile = "build/nuget/MarkEmbling.Utils.Forms.nuspec"
    .outputDirectory = "build/nuget"

