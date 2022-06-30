// Copyright 2022 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;

namespace Nuke.Common.Tests.New;

public class DotNetTestResult
{
    public void Deconstruct(out int passed)
    {
        passed = Passed;
        // failed = Failed;
    }

    public static DotNetTestResult Create()
    {
        return null;
    }

    public int Passed { get; set; }
    public int Failed { get; set; }
}


[Cli(Executable = "dotnet")]
class FakeCli
{
    [Command(OptionsType = typeof(DotNetOptions))]
    public static (IReadOnlyCollection<Output> Output, int ExitCode) FakeWithoutArguments(Configure<DotNetOptions> configurator)
    {
        // return Processing.Execute(arguments)
        return (null, 0);
    }

    [Command(Arguments = "restore", OptionsType = typeof(DotNetOptions))]
    public static (IReadOnlyCollection<Output> Output, int ExitCode) FakeWithArguments(Configure<DotNetOptions> configurator)
    {
        // return Processing.Execute(arguments)
        return (null, 0);
    }
}

partial class DotNetOptions
{
    internal partial string Foo(string value) => null;
}

[CommandOptions(CliType = typeof(FakeCli), Command = nameof(FakeCli.FakeWithArguments))]
public partial class DotNetOptions : CliOptions
{
    internal partial string Foo(string value);


    [Argument(Format = "--boolean {value}")]  public bool? Boolean => GetScalar<bool?>(() => Boolean);
    [Argument(Format = "--flag")]  public bool? Flag => GetScalar<bool?>(() => Flag);
    [Argument(Format = "--verbosity {value}")]  public DotNetVerbosity Verbosity => GetScalar<DotNetVerbosity>(() => Verbosity);



    [Argument(Format = "--integer {value}")]
    public int? Integer => GetScalar<int?>(() => Integer);

    [Argument(Format = "--string {value}")]
    public string String => GetScalar<string>(() => String);
    [Argument(Format = "--secret {value}")]
    public string Secret => GetScalar<string>(() => Secret);
    [Argument(Format = "/p:{key}={value}")]
    public IReadOnlyDictionary<string, object> Properties => GetComplex<Dictionary<string, object>>(() => Properties);
    [Argument(Format = "--flags {value}", CollectionSeparator = ",")]
    public IReadOnlyCollection<BindingFlags> Flags => GetComplex<Collection<BindingFlags>>(() => Flags);
    [Argument(Format = "--verbosities {value}", CollectionSeparator = ",")]
    public IReadOnlyCollection<DotNetVerbosity> VerbosityLists => GetComplex<Collection<DotNetVerbosity>>(() => VerbosityLists);
    public ILookup<string, int> Traits => GetComplex<LookupTable<string, int>>(() => Traits);

    public DotNetOptions Nested => GetComplex<DotNetOptions>(() => Nested);
    public IReadOnlyCollection<DotNetOptions> NestedList => GetComplex<Collection<DotNetOptions>>(() => NestedList);
}



public static class SettingsExtensions
{
    /// <summary><inheritdoc cref="DotNetOptions.Integer"/></summary>
    [OptionsModificator(OptionsType = typeof(DotNetOptions), Property = nameof(DotNetOptions.Integer))]
    public static T SetInteger<T>(this T o, int? value) where T : DotNetOptions => o.Copy(b => b.Set(() => o.Integer, value));
    public static T SetVerbosity<T>(this T o, DotNetVerbosity value) where T : DotNetOptions => o.Copy(b => b.Set(() => o.Verbosity, value));
    public static T ResetVerbosity<T>(this T o) where T : DotNetOptions => o.Copy(b => b.Remove(() => o.Verbosity));
    /// <summary><inheritdoc cref="DotNetOptions.Integer"/></summary>
    [OptionsModificator(OptionsType = typeof(DotNetOptions), Property = nameof(DotNetOptions.Integer))]
    public static T SetNoRestore<T>(this T o, bool? value) where T : DotNetOptions => o.Copy(b => b.Set(() => o.Flag, value));
    public static T ResetInteger<T>(this T o) where T : DotNetOptions => o.Copy(b => b.Remove(() => o.Integer));
    public static T SetString<T>(this T o, [Secret] string value) where T : DotNetOptions => o.Copy(b => b.Set(() => o.String, value));
    public static T ResetString<T>(this T o) where T : DotNetOptions => o.Copy(b => b.Remove(() => o.String));
    public static T SetSecret<T>(this T o, [Secret] string value) where T : DotNetOptions => o.Copy(b => b.Set(() => o.Secret, value));
    public static T ResetSecret<T>(this T o) where T : DotNetOptions => o.Copy(b => b.Remove(() => o.Secret));

    public static T SetProperties<T>(this T o, IDictionary<string, object> dictionary) where T : DotNetOptions => o.Copy(b => b.Set(() => o.Properties, dictionary.AsReadOnly()));
    public static T SetProperty<T>(this T o, string key, object value) where T : DotNetOptions => o.Copy(b => b.SetDictionary(() => o.Properties, key, value));
    public static T AddProperty<T>(this T o, string key, object value) where T : DotNetOptions => o.Copy(b => b.AddDictionary(() => o.Properties, key, value));
    public static T RemoveProperty<T>(this T o, string key) where T : DotNetOptions => o.Copy(b => b.RemoveDictionary(() => o.Properties, key));
    public static T ClearProperties<T>(this T o) where T : DotNetOptions => o.Copy(b => b.ClearDictionary(() => o.Properties));
    public static T ResetProperties<T>(this T o) where T : DotNetOptions => o.Copy(b => b.Remove(() => o.Properties));

    /// <summary>Sets <c>SpecialProperty</c> in <see cref="DotNetOptions.Properties"/>.</summary>
    public static T SetSpecialProperty<T>(this T o, string key, object value) where T : DotNetOptions => o.Copy(b => b.SetDictionary(() => o.Properties, "SpecialProperty", value));
    public static T RemoveSpecialProperty<T>(this T o, string key) where T : DotNetOptions => o.Copy(b => b.RemoveDictionary(() => o.Properties, "SpecialProperty"));
    public static T EnableSpecialProperty<T>(this T o) where T : DotNetOptions => o.Copy(b => b.SetDictionary(() => o.Properties, "SpecialProperty", true));
    public static T DisableSpecialProperty<T>(this T o) where T : DotNetOptions => o.Copy(b => b.SetDictionary(() => o.Properties, "SpecialProperty", false));

    public static T SetFlags<T>(this T o, IEnumerable<BindingFlags> collection) where T : DotNetOptions => o.Copy(b => b.Set(() => o.Flags, collection.ToList().AsReadOnly()));
    public static T AddFlag<T>(this T o, BindingFlags value) where T : DotNetOptions => o.Copy(b => b.AddCollection(() => o.Flags, value));
    public static T RemoveFlag<T>(this T o, BindingFlags value) where T : DotNetOptions => o.Copy(b => b.RemoveCollection(() => o.Flags, value));
    public static T ClearFlags<T>(this T o) where T : DotNetOptions => o.Copy(b => b.ClearCollection(() => o.Flags));
    public static T ResetFlags<T>(this T o) where T : DotNetOptions => o.Copy(b => b.Remove(() => o.Flags));

    public static T SetVerbosityLists<T>(this T o, IEnumerable<DotNetVerbosity> collection) where T : DotNetOptions => o.Copy(b => b.Set(() => o.VerbosityLists, collection.ToList().AsReadOnly()));
    public static T AddVerbosityList<T>(this T o, DotNetVerbosity value) where T : DotNetOptions => o.Copy(b => b.AddCollection(() => o.VerbosityLists, value));
    public static T RemoveVerbosityList<T>(this T o, DotNetVerbosity value) where T : DotNetOptions => o.Copy(b => b.RemoveCollection(() => o.VerbosityLists, value));
    public static T ClearVerbosityLists<T>(this T o) where T : DotNetOptions => o.Copy(b => b.ClearCollection(() => o.VerbosityLists));
    public static T ResetVerbosityLists<T>(this T o) where T : DotNetOptions => o.Copy(b => b.Remove(() => o.VerbosityLists));

    public static T SetNestedList<T>(this T o, IEnumerable<DotNetOptions> collection) where T : DotNetOptions => o.Copy(b => b.Set(() => o.NestedList, collection.ToList().AsReadOnly()));
    public static T AddNestedList<T>(this T o, params DotNetOptions[] value) where T : DotNetOptions => o.Copy(b => b.AddCollection(() => o.NestedList, value));
    public static T RemoveNestedList<T>(this T o, DotNetOptions value) where T : DotNetOptions => o.Copy(b => b.RemoveCollection(() => o.NestedList, value));
    public static T ClearNestedList<T>(this T o) where T : DotNetOptions => o.Copy(b => b.ClearCollection(() => o.NestedList));
    public static T ResetNestedList<T>(this T o) where T : DotNetOptions => o.Copy(b => b.Remove(() => o.NestedList));

    public static T SetTraits<T>(this T o, ILookup<string, int> lookup) where T : DotNetOptions => o.Copy(b => b.Set(() => o.Traits, lookup));
    public static T SetTrait<T>(this T o, string key, params int[] values) where T : DotNetOptions => o.Copy(b => b.SetLookup(() => o.Traits, key, values));
    public static T AddTrait<T>(this T o, string key, params int[] values) where T : DotNetOptions => o.Copy(b => b.AddLookup(() => o.Traits, key, values));
    public static T RemoveTrait<T>(this T o, string key, int value) where T : DotNetOptions => o.Copy(b => b.RemoveLookup(() => o.Traits, key, value));
    public static T RemoveTrait<T>(this T o, string key) where T : DotNetOptions => o.Copy(b => b.RemoveLookup(() => o.Traits, key));
    public static T ClearTraits<T>(this T o) where T : DotNetOptions => o.Copy(b => b.ClearLookup(() => o.Traits));
    public static T ResetTraits<T>(this T o) where T : DotNetOptions => o.Copy(b => b.Remove(() => o.Traits));

    public static T SetNested<T>(this T o, DotNetOptions value) where T : DotNetOptions => o.Copy(b => b.Set(() => o.Nested, value));
}
