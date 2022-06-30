// Copyright 2022 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Humanizer;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;

namespace Nuke.Common.Tests.New.Generation;

public static class Generator
{
    private static readonly string[] CommonNamespaces =
    {
        "System.Collections.Generic",
        "System.Collections.ObjectModel",
        "System.Linq",
        "JetBrains.Annotations",
        "Nuke.Common.Tests",
        "Nuke.Common.Utilities.Collections"
    };

    public static string Generate(Data dataObject)
    {
        var output = new StringWriter();
        var writer = new Writer(output);

        writer.WriteHeader(dataObject);
        writer.WriteLine();
        writer.GenerateClass(dataObject);
        writer.WriteLine();
        writer.GenerateExtensions(dataObject);

        return output.ToString();
    }

    private static void WriteHeader(this Writer writer, Data dataObject)
    {
        var namespaces = CommonNamespaces.Concat(dataObject.Namespaces)
            .OrderBy(x => x.StartsWith("System"))
            .ThenBy(x => x);

        writer.WriteLine("// ReSharper disable ArrangeMethodOrOperatorBody");
        writer.WriteLine();
        writer.WriteLine(namespaces, x => $"using {x};");
        writer.WriteLine();
        writer.WriteLine($"namespace {dataObject.GetNamespace()};");
    }

    private static void GenerateClass(this Writer writer, Data dataObject)
    {
        // writer.WriteLine($"[CommandOptions(CliType = typeof({cliType}), Command = nameof({cliType}.{commandName}))]");
        writer.WriteLine("[PublicAPI]");
        writer.WriteLine($"public partial class {dataObject.Name} : {dataObject.BaseClass ?? "CliOptions"}");

        using (writer.WriteBlock())
        {
            foreach (var option in dataObject.Options)
            {
                // [Argument(Format = "--boolean {value}")]  public bool? Boolean => GetScalar<bool?>(() => Boolean);

                string GetAttribute()
                {
                    var properties = new Dictionary<string, string>()
                        .AddPairWhenValueNotNull(nameof(Option.Format), option.Format)
                        .AddPairWhenValueNotNull(nameof(Option.AlternativeFormat), option.AlternativeFormat)
                        .Select(x => $"{x.Key} = {x.Value.DoubleQuote()}")
                        .JoinCommaSpace();

                    return $"[Argument({properties})]";
                }

                var attribute = option.Format != null
                    ? GetAttribute()
                    : string.Empty;

                var getter = option.IsValueType()
                    ? "GetScalar"
                    : "GetComplex";

                writer.WriteLineParts(
                    attribute,
                    "public",
                    option.GetReturnType(),
                    option.Name,
                    "=>",
                    $"{getter}<{option.GetInternalType()}>(() => {option.Name});");
            }
        }
    }

    private static void GenerateExtensions(this Writer writer, Data dataObject)
    {
        writer.WriteLine("[PublicAPI]");
        writer.WriteLine($"public static partial class {dataObject.Name}Extensions");

        using (writer.WriteBlock())
        {
            // /// <summary><inheritdoc cref="FakeOptions.Integer"/></summary>
            // [OptionsModificator(OptionsType = typeof(FakeOptions), Property = nameof(FakeOptions.Integer))]
            // public static T SetInteger<T>(this T o, int? value) where T : FakeOptions => o.Copy(b => b.Set(() => o.Integer, value));

            foreach (var option in dataObject.Options)
            {
                writer.WriteLine($"#region {dataObject.Name}.{option.Name}");
                foreach (var (extension, modification) in GetExtensionsWithModifications(option))
                {
                    writer.WriteLine($"/// <summary>{option.GetSummary()}</summary>");
                    writer.WriteLine($"[OptionsModificator(OptionsType = typeof({dataObject.Name}), Property = nameof({dataObject.Name}.{option.Name}))]");
                    writer.WriteLineParts($"public static T {extension} where T : {dataObject.Name} => o.Copy(b => b.{modification});");
                }

                writer.WriteLine("#endregion");
            }
        }

        IEnumerable<(string Extension, string Modification)> GetExtensionsWithModifications(Option option)
        {
            if (option.Type != "list" && option.Type != "dictionary" && option.Type != "lookup")
            {
                yield return (
                    $"Set{option.Name}<T>(this T o, {option.GetReturnType()} value)",
                    $"Set(() => o.{option.Name}, value)");
            }

            var singular = option.Name.Singularize(inputIsKnownToBePlural: false);
            switch (option.Type)
            {
                case "bool":
                    yield return (
                        $"Enable{option.Name}<T>(this T o)",
                        $"Set(() => o.{option.Name}, true)");
                    yield return (
                        $"Disable{option.Name}<T>(this T o)",
                        $"Set(() => o.{option.Name}, false)");
                    break;

                case "list":
                    yield return (
                        $"Set{option.Name}<T>(this T o, params {option.Value}[] values)",
                        $"Set(() => o.{option.Name}, values)");
                    yield return (
                        $"Set{option.Name}<T>(this T o, IEnumerable<{option.Value}> values)",
                        $"Set(() => o.{option.Name}, values)");
                    yield return (
                        $"Add{option.Name}<T>(this T o, params {option.Value}[] values)",
                        $"AddCollection(() => o.{option.Name}, values)");
                    yield return (
                        $"Add{option.Name}<T>(this T o, IEnumerable<{option.Value}> values)",
                        $"AddCollection(() => o.{option.Name}, values)");
                    yield return (
                        $"Remove{option.Name}<T>(this T o, params {option.Value}[] values)",
                        $"RemoveCollection(() => o.{option.Name}, values)");
                    yield return (
                        $"Remove{option.Name}<T>(this T o, IEnumerable<{option.Value}> values)",
                        $"RemoveCollection(() => o.{option.Name}, values)");
                    yield return (
                        $"Clear{option.Name}<T>(this T o)",
                        $"ClearCollection(() => o.{option.Name})");
                    break;

                case "dictionary":
                    yield return (
                        $"Set{option.Name}<T>(this T o, IReadOnlyDictionary<{option.Key}, {option.Value}> values)",
                        $"Set(() => o.{option.Name}, values)");
                    yield return (
                        $"Set{option.Name}<T>(this T o, IDictionary<{option.Key}, {option.Value}> values)",
                        $"Set(() => o.{option.Name}, values)");
                    yield return (
                        $"Add{option.Name}<T>(this T o, IReadOnlyDictionary<{option.Key}, {option.Value}> values)",
                        $"AddDictionary(() => o.{option.Name}, values)");
                    yield return (
                        $"Add{option.Name}<T>(this T o, IDictionary<{option.Key}, {option.Value}> values)",
                        $"AddDictionary(() => o.{option.Name}, values)");
                    yield return (
                        $"Add{singular}<T>(this T o, {option.Key} key, {option.Value} value)",
                        $"AddDictionary(() => o.{option.Name}, key, value)");
                    yield return (
                        $"Set{singular}<T>(this T o, {option.Key} key, {option.Value} value)",
                        $"SetDictionary(() => o.{option.Name}, key, value)");
                    yield return (
                        $"Remove{singular}<T>(this T o, {option.Key} key)",
                        $"RemoveDictionary(() => o.{option.Name}, key)");
                    yield return (
                        $"Clear{option.Name}<T>(this T o)",
                        $"ClearDictionary(() => o.{option.Name})");
                    break;

                case "lookup":
                    yield return (
                        $"Set{option.Name}<T>(this T o, ILookup<{option.Key}, {option.Value}> values)",
                        $"Set(() => o.{option.Name}, values)");
                    yield return (
                        $"Add{singular}<T>(this T o, {option.Key} key, params {option.Value}[] values)",
                        $"AddLookup(() => o.{option.Name}, key, values)");
                    yield return (
                        $"Add{singular}<T>(this T o, {option.Key} key, IEnumerable<{option.Value}> values)",
                        $"AddLookup(() => o.{option.Name}, key, values)");
                    yield return (
                        $"Set{singular}<T>(this T o, {option.Key} key, params {option.Value}[] values)",
                        $"SetLookup(() => o.{option.Name}, key, values)");
                    yield return (
                        $"Set{singular}<T>(this T o, {option.Key} key, IEnumerable<{option.Value}> values)",
                        $"SetLookup(() => o.{option.Name}, key, values)");
                    yield return (
                        $"Remove{singular}<T>(this T o, {option.Key} key)",
                        $"AddLookup(() => o.{option.Name}, key)");
                    yield return (
                        $"Remove{singular}<T>(this T o, {option.Key} key, {option.Value} value)",
                        $"AddLookup(() => o.{option.Name}, key, value)");
                    yield return (
                        $"Clear{option.Name}<T>(this T o)",
                        $"ClearLookup(() => o.{option.Name})");
                    break;
            }

            yield return (
                $"Reset{option.Name}<T>(this T o)",
                $"Remove(() => o.{option.Name})");
        }
    }
}
