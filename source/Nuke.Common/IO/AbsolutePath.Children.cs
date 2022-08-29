// Copyright 2022 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Nuke.Common.IO
{
    partial class AbsolutePathExtensions
    {
        public static IEnumerable<AbsolutePath> GetFiles(
            this AbsolutePath path,
            string pattern = "*",
            SearchOption options = SearchOption.TopDirectoryOnly)
        {
            return Directory.GetFiles(path, pattern, options).Select(AbsolutePath.Create);
        }

        public static IEnumerable<AbsolutePath> GetDirectories(
            this AbsolutePath path,
            string pattern = "*",
            SearchOption option = SearchOption.TopDirectoryOnly)
        {
            return Directory.GetDirectories(path, pattern, option).Select(AbsolutePath.Create);
        }
    }
}
