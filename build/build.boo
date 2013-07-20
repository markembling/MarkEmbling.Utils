solution = "MarkEmbling.Utils.sln"
configuration = "release"
test_assembly = "MarkEmbling.Utils.Tests/bin/${configuration}/MarkEmbling.Utils.Tests.dll"
bin_path = "build/bin"
package_path = "build/nuget"

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
  rmdir(bin_path)
  
  with FileList("MarkEmbling.Utils/bin/${configuration}"):
    .Include("*.{dll,exe,config}")
    .ForEach def(file):
      file.CopyToDirectory(bin_path)

  with FileList("MarkEmbling.Utils.Forms/bin/${configuration}"):
    .Include("*.{dll,exe,config}")
    .ForEach def(file):
      file.CopyToDirectory(bin_path)

desc "Creates the NuGet packages"
target package, (prep):
  with nuget_pack():
    .toolPath = "packages/NuGet.CommandLine.2.6.1/tools/NuGet.exe"
    .nuspecFile = "build/nuget/MarkEmbling.Utils.nuspec"
    .outputDirectory = package_path

  with nuget_pack():
    .toolPath = "packages/NuGet.CommandLine.2.6.1/tools/NuGet.exe"
    .nuspecFile = "build/nuget/MarkEmbling.Utils.Forms.nuspec"
    .outputDirectory = package_path

