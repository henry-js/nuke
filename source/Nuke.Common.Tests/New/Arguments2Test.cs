// Copyright 2022 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.EventStream;
using FluentAssertions;
using Nuke.Common.Tests.New;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using Serilog.Events;
using Xunit;
using Xunit.Abstractions;

namespace Nuke.Common.Tests;

public class Settings2Test
{
    private readonly ITestOutputHelper _testOutputHelper;
    private DotNetOptions Options = new DotNetOptions();

    public Settings2Test(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    private void Modify(Configure<DotNetOptions> configurator)
    {
        Options = configurator.Invoke(Options).Copy();
    }

    [Fact]
    public void IntegerScalarTest()
    {
        Modify(x => x.SetInteger(1));
        Options.Integer.Should().Be(1);

        Modify(x => x.ResetInteger());
        Options.Integer.Should().BeNull();
    }

    [Fact]
    public void EnumerationScalarTest()
    {
        Modify(x => x.SetVerbosity(DotNetVerbosity.Detailed));
        Options.Verbosity.Should().Be(DotNetVerbosity.Detailed);

        Modify(x => x.ResetVerbosity());
        Options.Verbosity.Should().BeNull();
    }

    [Fact]
    public void StringScalarTest()
    {
        Modify(x => x.SetString("foo"));
        Options.String.Should().Be("foo");

        Modify(x => x.ResetString());
        Options.String.Should().BeNull();
    }

    [Fact]
    public void DictionaryTest()
    {
        var properties = new Dictionary<string, object>
        {
            {"foo", "bar"},
            {"baz", "qux"}
        };

        Modify(x => x.SetProperties(properties));
        Options.Properties.Should().BeEquivalentTo(properties);

        Modify(x => x.SetProperty("foo", "buzz"));
        Options.Properties["foo"].Should().Be("buzz");

        Modify(x => x.RemoveProperty("baz"));
        Options.Properties.Should().NotContainKey("baz");

        Action action = () => Options.AddProperty("foo", "existing");
        action.Should().Throw<Exception>();

        Modify(x => x.EnableSpecialProperty());
        Options.Properties["SpecialProperty"].As<bool>().Should().BeTrue();

        Modify(x => x.ClearProperties());
        Options.Properties.Should().BeEmpty();

        Modify(x => x.ResetProperties());
        Options.Properties.Should().BeNull();
    }

    [Fact]
    public void ListTest()
    {
        var flags = new List<BindingFlags>
        {
            BindingFlags.Static,
            BindingFlags.DeclaredOnly
        };

        Modify(x => x.SetFlags(flags));
        Options.Flags.Should().BeEquivalentTo(flags);

        Modify(x => x.AddFlag(BindingFlags.Instance));
        Options.Flags.Should().Contain(BindingFlags.Instance);

        Modify(x => x.RemoveFlag(BindingFlags.Static));
        Options.Flags.Should().NotContain(BindingFlags.Static);

        Modify(x => x.ClearFlags());
        Options.Flags.Should().BeEmpty();

        Modify(x => x.ResetFlags());
        Options.Flags.Should().BeNull();
    }

    [Fact]
    public void EnumListTest()
    {
        var verbosities = new List<DotNetVerbosity>
        {
            DotNetVerbosity.Diagnostic,
            DotNetVerbosity.Minimal
        };

        Modify(x => x.SetVerbosityLists(verbosities));
        Options.VerbosityLists.Should().BeEquivalentTo(verbosities);

        Modify(x => x.AddVerbosityList(DotNetVerbosity.Quiet));
        Options.VerbosityLists.Should().Contain(DotNetVerbosity.Quiet);

        Modify(x => x.RemoveVerbosityList(DotNetVerbosity.Quiet));
        Options.VerbosityLists.Should().NotContain(DotNetVerbosity.Quiet);

        Modify(x => x.ClearVerbosityLists());
        Options.VerbosityLists.Should().BeEmpty();

        Modify(x => x.ResetVerbosityLists());
        Options.VerbosityLists.Should().BeNull();
    }

    [Fact]
    public void LookupTest()
    {
        var traits = new LookupTable<string, int>
        {
            ["foo"] = new[] { 1, 2, 3 },
            ["bar"] = new[] { 3, 4, 5 },
        };

        Modify(x => x.SetTraits(traits));
        Options.Traits.Should().BeEquivalentTo(traits);

        Modify(x => x.SetTrait("buzz", 1000));
        Options.Traits["buzz"].Should().Equal(1000);

        Modify(x => x.AddTrait("foo", 4, 5));
        Options.Traits["foo"].Should().Equal(1, 2, 3, 4, 5);

        Modify(x => x.RemoveTrait("foo", 2));
        Options.Traits["foo"].Should().Equal(1, 3, 4, 5);

        Modify(x => x.RemoveTrait("foo"));
        Options.Traits["foo"].Should().BeEmpty();

        Modify(x => x.SetTrait("buzz", 9));
        Options.Traits["buzz"].Should().Equal(9);

        Modify(x => x.ClearTraits());
        Options.Traits.Should().BeEmpty();

        Modify(x => x.ResetTraits());
        Options.Traits.Should().BeNull();
    }

    [Fact]
    public void NestedTest()
    {
        var innerOptions = new DotNetOptions().SetInteger(1);
        Modify(x => x.SetNested(innerOptions));
        Options.Nested.Integer.Should().Be(1);

        Modify(x => x.AddNestedList(new DotNetOptions().SetInteger(5), new DotNetOptions().SetInteger(1)));
        Options.NestedList.Should().HaveCount(2);
    }

    [Fact]
    public void RenderTest()
    {
        var options = new DotNetOptions()
            .SetNoRestore(true)
            .SetInteger(5)
            .SetString("spacy value")
            .AddProperty("key", "value");

        var arguments = options.GetArguments();

        arguments.Should().BeEquivalentTo(
            "--flag",
            "--integer",
            "5",
            "--string",
            "spacy value",
            "/p:key=value");
    }
}

class SecretAttribute : Attribute
{
}

class Processing
{
    public static string ParseArguments(CliOptions options)
    {
        var commandOptionsAttribute = options.GetType().GetCustomAttribute<CommandOptionsAttribute>().NotNull();
        var commandMethod = commandOptionsAttribute.CliType.GetMethod(commandOptionsAttribute.Command).NotNull();
        var commandAttribute = commandMethod.GetCustomAttribute<CommandAttribute>().NotNull();
        var arguments = commandAttribute.Arguments;

        return new[]{arguments}.Concat(options.GetArguments()).JoinSpace();
    }

    public static (IReadOnlyCollection<Output> Output, int ExitCode) Execute(string toolPath, string arguments, int? timeout)
    {
        return ExecuteAsync(toolPath, arguments, captureStandardOutput: true, captureErrorOutput: true, timeout).GetAwaiter().GetResult();
    }

    public static async Task<(IReadOnlyCollection<Output> Output, int ExitCode)> ExecuteAsync(
        string toolPath,
        string arguments,
        bool captureStandardOutput,
        bool captureErrorOutput,
        int? timeout)
    {
        var output = new List<Output>();
        var exitCode = 0;
        var cmd = Cli.Wrap(toolPath)
            .WithArguments(arguments);

        var cancellationTokenSource = timeout.HasValue ? new CancellationTokenSource() : null;
        cancellationTokenSource?.CancelAfter(timeout.Value);
        var cancellationToken = cancellationTokenSource?.Token ?? default(CancellationToken);

        await foreach (var cmdEvent in cmd.ListenAsync(cancellationToken))
        {
            switch (cmdEvent)
            {
                case StandardOutputCommandEvent std when captureStandardOutput:
                    output.Add(new Output { Type = OutputType.Std, Text = std.Text });
                    break;
                case StandardErrorCommandEvent err when captureErrorOutput:
                    output.Add(new Output { Type = OutputType.Std, Text = err.Text });
                    break;
                case ExitedCommandEvent exited:
                    exitCode = exited.ExitCode;
                    break;
            }
        }
        return (output, exitCode);
    }

    public static New.Generation.Command ExecuteCommand()
    {
        return null;
    }
}

// TODO: allow custom tool path using env var
