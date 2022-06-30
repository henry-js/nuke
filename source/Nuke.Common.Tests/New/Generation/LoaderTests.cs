// Copyright 2022 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using VerifyTests;
using VerifyXunit;
using Xunit;
using static Nuke.Common.Tests.TestUtils;

namespace Nuke.Common.Tests.New.Generation;

[UsesVerify]
public class LoaderTests
{
    [Fact]
    public async Task TestInline()
    {
        var dataObjects = Loader.LoadDataObjects(ThisDirectory / "Inline.data.yml");
        await Verifier.Verify(dataObjects);
    }

    [Fact]
    public async Task TestDistributed()
    {
        var dataObjects = Loader.LoadDataObjects(ThisDirectory / "Distributed.data.yml");
        await Verifier.Verify(dataObjects);
    }
}
