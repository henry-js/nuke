// Copyright 2022 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;

namespace Nuke.Common.Tests.New;

partial class CliOptionsExtensions
{
    /// <summary><p>Defines the execution timeout of the invoked process.</p></summary>
    [OptionsModificator(OptionsType = typeof(CliOptions), Property = nameof(CliOptions.ProcessExecutionTimeout))]
    public static T SetProcessExecutionTimeout<T>(this T o, TimeSpan? value) where T : CliOptions
        => o.Copy(b => b.Set(() => o.ProcessExecutionTimeout, value?.TotalMilliseconds));
}
