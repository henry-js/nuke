// Copyright 2021 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Nuke.Common.Utilities;

namespace Nuke.Common.Tooling
{
    [PublicAPI]
    public static class ToolResolver
    {
        [Obsolete($"Use {nameof(NuGetToolResolver)}")]
        public static Tool GetPackageTool(string packageId, string packageExecutable, string version = null, string framework = null)
        {
            return NuGetToolResolver.GetPackageTool(packageId, packageExecutable, version, framework);
        }

        [Obsolete($"Use {nameof(GetTool)} with absolute path")]
        public static Tool GetLocalTool(string absoluteOrRelativePath)
        {
            return GetTool(absoluteOrRelativePath);
        }

        public static Tool GetTool(string path)
        {
            return new ToolExecutor(path).Execute;
        }

        [CanBeNull]
        public static Tool TryGetEnvironmentTool(string variable)
        {
            var toolPath = TryGetEnvironmentExecutable($"{variable.ToUpperInvariant()}_EXE");
            if (toolPath == null)
                return null;

            return new ToolExecutor(toolPath).Execute;
        }

        [CanBeNull]
        public static string TryGetEnvironmentExecutable(string variable)
        {
            var environmentExecutablePath = EnvironmentInfo.GetVariable<string>(variable);
            if (environmentExecutablePath == null)
                return null;

            Assert.FileExists(environmentExecutablePath,
                $"Path '{environmentExecutablePath}' from environment variable '{variable}' does not exist");
            return environmentExecutablePath;
        }

        public static Tool GetPathTool(string name, bool allowEnvironmentOverride = true)
        {
            var toolPath = GetPathExecutable(name, allowEnvironmentOverride);
            return new ToolExecutor(toolPath).Execute;
        }

        [CanBeNull]
        public static Tool TryGetPathTool(string name, bool allowEnvironmentOverride = true)
        {
            var toolPath = TryGetPathExecutable(name, allowEnvironmentOverride);
            if (toolPath == null)
                return null;

            return new ToolExecutor(toolPath).Execute;
        }

        public static string GetPathExecutable(string name, bool allowEnvironmentOverride = true)
        {
            return TryGetPathExecutable(name, allowEnvironmentOverride).NotNull($"Could not find '{name}' on PATH.");
        }

        [CanBeNull]
        public static string TryGetPathExecutable(string name, bool allowEnvironmentOverride = true)
        {
            if (allowEnvironmentOverride && TryGetEnvironmentExecutable($"{name.ToUpperInvariant()}_EXE") is { } environmentToolPath)
                return environmentToolPath;

            if (File.Exists(name))
                return Path.GetFullPath(name);

            var locateExecutable = EnvironmentInfo.IsWin
                ? @"C:\Windows\System32\where.exe"
                : "/usr/bin/which";

            if (!File.Exists(locateExecutable))
            {
                string GetExecutableFullPath(string path)
                    => Path.Combine(path,
                        Path.GetExtension(name).IsNullOrEmpty() && EnvironmentInfo.IsWin
                            ? $"{name}.exe"
                            : name);

                var environmentVariable = Environment.GetEnvironmentVariable("PATH").NotNullOrEmpty("PATH variable not available");
                var paths = environmentVariable.Split(new[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries);
                return paths.Select(GetExecutableFullPath).FirstOrDefault(File.Exists);
            }

            var locateProcess = ProcessTasks.StartProcess(
                locateExecutable,
                name,
                logOutput: false,
                logInvocation: false);
            locateProcess.AssertWaitForExit();

            return locateProcess.Output
                .Select(x => x.Text)
                .Where(x => EnvironmentInfo.IsWin && Path.HasExtension(x) || EnvironmentInfo.IsUnix)
                .FirstOrDefault(File.Exists);
        }
    }
}
