// Copyright 2021 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Linq;
using JetBrains.Annotations;

namespace Nuke.Common.Tooling
{
    [PublicAPI]
    public static class ToolPathResolver
    {
        [Obsolete($"Use {nameof(NuGetToolResolver)} instead")]
        public static string EmbeddedPackagesDirectory
        {
            set => NuGetToolResolver.EmbeddedPackagesDirectory = value;
        }

        [Obsolete($"Use {nameof(NuGetToolResolver)} instead")]
        public static string NuGetPackagesConfigFile
        {
            set => NuGetToolResolver.NuGetPackagesConfigFile = value;
        }

        [Obsolete($"Use {nameof(NuGetToolResolver)} instead")]
        public static string NuGetAssetsConfigFile
        {
            set => NuGetToolResolver.NuGetAssetsConfigFile = value;
        }

        [Obsolete($"Use {nameof(NuGetToolResolver)} instead")]
        public static string PaketPackagesConfigFile
        {
            set => NuGetToolResolver.PaketPackagesConfigFile = value;
        }

        [Obsolete($"Use {nameof(ToolResolver)} instead")]
        [CanBeNull]
        public static string TryGetEnvironmentExecutable(string environmentExecutable)
        {
            return ToolResolver.TryGetEnvironmentExecutable(environmentExecutable);
        }

        [Obsolete($"Use {nameof(NuGetToolResolver)} instead")]
        public static string GetPackageExecutable(string packageId, string packageExecutable, string version = null, string framework = null)
        {
            return NuGetToolResolver.GetPackageExecutable(packageId, packageExecutable, version, framework);
        }

        [Obsolete($"Use {nameof(ToolResolver)} instead")]
        public static string GetPathExecutable(string pathExecutable)
        {
            return ToolResolver.GetPathExecutable(pathExecutable);
        }
    }
}
