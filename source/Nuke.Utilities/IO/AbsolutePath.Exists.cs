// Copyright 2022 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Nuke.Common.Utilities;

namespace Nuke.Common.IO
{
    public static partial class AbsolutePathExtensions
    {
        /// <summary>
        /// Indicates whether a file or directory exists. The argument expression should indicate which type is expected, i.e.,
        /// <c>configFile</c>, <c>outputFolder</c>, or <c>artifactsDirectory</c>.
        /// </summary>
        [Pure]
        public static bool Exists(this AbsolutePath path, [CallerArgumentExpression("path")] string expression = null)
        {
            if (expression.EndsWithOrdinalIgnoreCase("file"))
                return path.FileExists();
            if (expression.EndsWithAnyOrdinalIgnoreCase("directory", "dir", "folder"))
                return path.DirectoryExists();

            throw new ArgumentException($"Cannot infer from argument '{expression}' if either file or directory must exist");
        }

        [Pure]
        public static bool FileExists(this AbsolutePath path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Indicates whether the directory exists.
        /// </summary>
        [Pure]
        public static bool DirectoryExists(this AbsolutePath path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// Indicates whether the directory contains a file (<c>*</c> as wildcard) using <see cref="SearchOption.TopDirectoryOnly"/>.
        /// </summary>
        [Pure]
        public static bool ContainsFile(this AbsolutePath path, string pattern, SearchOption options = SearchOption.TopDirectoryOnly)
        {
            Assert.DirectoryExists(path);
            return path.ToDirectoryInfo().GetFiles(pattern, options).Any();
        }

        /// <summary>
        /// Indicates whether the directory contains a directory (<c>*</c> as wildcard) using <see cref="SearchOption.TopDirectoryOnly"/>.
        /// </summary>
        [Pure]
        public static bool ContainsDirectory(this AbsolutePath path, string pattern, SearchOption options = SearchOption.TopDirectoryOnly)
        {
            Assert.DirectoryExists(path);
            return path.ToDirectoryInfo().GetDirectories(pattern, SearchOption.TopDirectoryOnly).Any();
        }
    }
}
