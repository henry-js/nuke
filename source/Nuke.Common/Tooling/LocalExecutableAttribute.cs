// Copyright 2021 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Nuke.Common.IO;
using Nuke.Common.ValueInjection;

namespace Nuke.Common.Tooling
{
    /// <summary>
    ///     Injects a delegate for process execution. The path relative to the root directory is passed as constructor argument.
    /// </summary>
    /// <example>
    ///     <code>
    /// [LocalExecutable("./tools/custom.exe")] readonly Tool Custom;
    /// Target FooBar => _ => _
    ///     .Executes(() =>
    ///     {
    ///         var output = Custom("test");
    ///     });
    ///     </code>
    /// </example>
    public class LocalExecutableAttribute : ValueInjectionAttributeBase
    {
        private readonly string _absoluteOrRelativePath;

        public LocalExecutableAttribute(string absoluteOrRelativePath)
        {
            _absoluteOrRelativePath = absoluteOrRelativePath;
        }

        public override object GetValue(MemberInfo member, object instance)
        {
            var toolPath = PathConstruction.HasPathRoot(_absoluteOrRelativePath)
                ? _absoluteOrRelativePath
                : Path.Combine(Build.RootDirectory, _absoluteOrRelativePath);
            Assert.FileExists(toolPath);
            return ToolResolver.GetTool(_absoluteOrRelativePath);
        }
    }
}
