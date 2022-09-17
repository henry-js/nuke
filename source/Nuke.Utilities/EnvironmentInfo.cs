﻿// Copyright 2021 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.IO;
using System.Linq;
using Nuke.Common.IO;
using Nuke.Common.Utilities;

namespace Nuke.Common
{
    /// <summary>
    /// Provides access to environment relevant information.
    /// </summary>
    public static partial class EnvironmentInfo
    {
        public static string NewLine => Environment.NewLine;
        public static string MachineName => Environment.MachineName;

        /// <summary>
        /// Returns the working directory for the current process.
        /// </summary>
        public static AbsolutePath WorkingDirectory
        {
            get => Environment.CurrentDirectory;
            set => Environment.CurrentDirectory = value;
        }

        /// <summary>
        /// Switches to a new working directory. The previous working directory is restored once the <see cref="IDisposable"/> is disposed.
        /// </summary>
        public static IDisposable SwitchWorkingDirectory(string workingDirectory)
        {
            var previousWorkingDirectory = WorkingDirectory;
            return DelegateDisposable.CreateBracket(
                () => WorkingDirectory = workingDirectory,
                () => WorkingDirectory = previousWorkingDirectory);
        }
    }
}
