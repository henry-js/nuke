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
    public static partial class EnvironmentInfo
    {
        public static string NewLine => Environment.NewLine;
        public static string MachineName => Environment.MachineName;

        public static AbsolutePath WorkingDirectory
        {
#if NETCORE
            get => (AbsolutePath) Directory.GetCurrentDirectory();
            set => Directory.SetCurrentDirectory(value);
#else
            get => Environment.CurrentDirectory;
            set => Environment.CurrentDirectory = value;
#endif
        }

        public static IDisposable SwitchWorkingDirectory(string workingDirectory)
        {
            var previousWorkingDirectory = WorkingDirectory;
            return DelegateDisposable.CreateBracket(
                () => WorkingDirectory = workingDirectory,
                () => WorkingDirectory = previousWorkingDirectory);
        }
    }
}
