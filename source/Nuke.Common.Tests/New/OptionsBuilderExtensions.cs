// Copyright 2022 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Linq;
using Newtonsoft.Json;

namespace Nuke.Common.Tests.New;

public static class OptionsBuilderExtensions
{
    internal static T Copy<T>(this T builder, Action<T> modification = null)
    {
        var newOptions = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(builder));
        modification?.Invoke(newOptions);
        return newOptions;
    }
}
