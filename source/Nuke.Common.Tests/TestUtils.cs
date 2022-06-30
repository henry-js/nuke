// Copyright 2022 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Nuke.Common.IO;

namespace Nuke.Common.Tests;

public static class TestUtils
{
    public static AbsolutePath RootDirectory => Constants.TryGetRootDirectoryFrom(Directory.GetCurrentDirectory()).NotNull();

    public static AbsolutePath ThisDirectory => (AbsolutePath)new StackTrace(fNeedFileInfo: true)
        .GetFrames()
        .Skip(1)
        .Select(x => Path.GetDirectoryName(x.GetFileName()))
        .First(x => RootDirectory.Contains(x));
}
