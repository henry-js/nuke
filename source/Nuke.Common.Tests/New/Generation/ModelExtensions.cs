// Copyright 2022 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.IO;
using System.Linq;
using Markdig;
using Nuke.Common.IO;
using Nuke.Common.Utilities;

namespace Nuke.Common.Tests.New.Generation;

public static class CommonObjectExtensions
{
    public static string GetNamespace(this ICommonObject obj)
    {
        var fileDirectory = obj.File.Parent;
        var projectDirectory = FileSystemTasks.FindParentDirectory(
            fileDirectory,
            x => x.GetFiles("*.csproj").Any()).NotNull();
        var projectFile = Directory.GetFiles(projectDirectory, "*.csproj").Single();
        var rootNamespace = Path.GetFileNameWithoutExtension(projectFile);
        var namespaces = PathConstruction.GetRelativePath(projectDirectory, fileDirectory).Split('/', '\\');
        return $"{rootNamespace}.{namespaces.JoinDot()}";
    }
}
public static class OptionsExtensions
{
    public static bool IsValueType(this Option option)
    {
        return option.Type.EqualsAnyOrdinalIgnoreCase(
            "int",
            "bool",
            "sbyte",
            "short",
            "long",
            "byte",
            "ushort",
            "uint",
            "ulong",
            "float",
            "double",
            "char",
            "decimal",
            "DateTime",
            "TimeSpan");
    }

    public static string GetNullableType(this Option option)
    {
        return option.IsValueType() ? option.Type + "?" : option.Type.Trim('!');
    }

    public static string GetInternalType(this Option option)
    {
        return option.Type switch
        {
            "list" => $"List<{option.Value}>",
            "dictionary" => $"Dictionary<{option.Key}, {option.Value}>",
            "lookup" => $"LookupTable<{option.Key}, {option.Value}>",
            _ => option.GetNullableType()
        };
    }

    public static string GetReturnType(this Option option)
    {
        return option.Type switch
        {
            "list" => $"IReadOnlyList<{option.Value}>",
            "dictionary" => $"IReadOnlyDictionary<{option.Key}, {option.Value}>",
            "lookup" => $"ILookup<{option.Key}, {option.Value}>",
            _ => option.GetNullableType()
        };
    }

    public static string GetSummary(this Option option)
    {
        // https://learn-the-web.algonquindesign.ca/topics/markdown-yaml-cheat-sheet/
        return Markdown.ToHtml(option.Description)
            .Replace("code>", "c>")
            .Replace(Environment.NewLine, string.Empty)
            .Trim();
    }
}
