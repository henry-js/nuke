// Copyright 2022 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.IO;
using System.Linq;
using Nuke.Common.IO;
using Xunit;
using static Nuke.Common.Tests.TestUtils;

namespace Nuke.Common.Tests.New.Generation;

public class GeneratorTests
{
    [Fact]
    public void Test()
    {
        var dataObject = Loader.LoadDataObjects(ThisDirectory / "CliOptions.data.yml").Single();
        var generatedFile1 = dataObject.File.Parent / $"{dataObject.Name}.Generated.cs";
        File.WriteAllText(generatedFile1, Generator.Generate(dataObject));
    }
}
