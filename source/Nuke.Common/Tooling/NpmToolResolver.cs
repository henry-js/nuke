// Copyright 2022 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Linq;
using JetBrains.Annotations;

namespace Nuke.Common.Tooling
{
    [PublicAPI]
    public static class NpmToolResolver
    {
        public static Tool GetNpmTool(string name, bool allowEnvironmentOverride = true)
        {
            var toolPath = GetNpmExecutable(name, allowEnvironmentOverride);
            return new ToolExecutor(toolPath).Execute;
        }

        public static string GetNpmExecutable(string name, bool allowEnvironmentOverride = true)
        {
            if (allowEnvironmentOverride && ToolResolver.TryGetEnvironmentExecutable($"{name.ToUpperInvariant()}_EXE") is { } environmentToolPath)
                return environmentToolPath;

            var npx = ToolResolver.GetPathTool("npx");
            return npx.Invoke(
                    arguments: $"which {name}",
                    workingDirectory: NukeBuild.BuildProjectDirectory.NotNull() / "node_modules",
                    logInvocation: false,
                    logOutput: false)
                .StdToText();
        }
    }
}
