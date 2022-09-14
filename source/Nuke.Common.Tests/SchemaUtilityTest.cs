﻿// Copyright 2021 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Nuke.Common.Execution;
using VerifyXunit;
using Xunit;
using static Nuke.Common.Constants;

namespace Nuke.Common.Tests
{
    [UsesVerify]
    public class SchemaUtilityTest
    {
        [Fact]
        public Task TestGetBuildSchema()
        {
            var schema = SchemaUtility.GetBuildSchema(new TestBuild());
            var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            var json = JsonSerializer.Serialize(schema, options);
            return Verifier.Verify(json);
        }

        [Fact]
        public void TestGetCompletionItemsFromSchema()
        {
            var schema = JsonDocument.Parse(@"
{
  ""$schema"": ""http://json-schema.org/draft-04/schema#"",
  ""title"": ""Build Schema"",
  ""definitions"": {
    ""build"": {
      ""type"": ""object"",
      ""properties"": {
        ""NoLogo"": {
          ""type"": ""boolean"",
          ""description"": ""Disables displaying the NUKE logo""
        },
        ""Configuration"": {
          ""type"": ""string"",
          ""description"": ""Configuration to build - Default is 'Debug' (local) or 'Release' (server)"",
          ""enum"": [
            ""Debug"",
            ""Release""
          ]
        },
        ""Target"": {
          ""type"": ""array"",
          ""description"": ""List of targets to be invoked. Default is '{default_target}'"",
          ""items"": {
            ""type"": ""string"",
            ""enum"": [
              ""Restore"",
              ""Compile""
            ]
          }
        }
      }
    }
  }
}
");
            var profileNames = new[] { "dev" };
            var items = SchemaUtility.GetCompletionItems(schema, profileNames);
            items.Should().BeEquivalentTo(
                new Dictionary<string, string[]>
                {
                    ["NoLogo"] = null,
                    ["Configuration"] = new[] { "Debug", "Release" },
                    ["Target"] = new[] { "Restore", "Compile" },
                    [LoadedLocalProfilesParameterName] = profileNames
                });
        }

#pragma warning disable CS0649
        private class TestBuild : NukeBuild, ITestComponent
        {
            [Parameter] public string Param;
            [Parameter] public bool? NullableBool;
            [Parameter] public int? NullableInteger;
            [Parameter] public Verbosity[] Verbosities;
            [Parameter] [Secret] public string Secret;
            public string Param2 => "";
            string ITestComponent.Param3 => "";

            Target ITestComponent.Bar => _ => _;
            public Target Zoo => _ => _;
        }

        [ParameterPrefix("Component")]
        private interface ITestComponent : INukeBuild
        {
            [Parameter] string Param1 => TryGetValue(() => Param1);
            [Parameter] string Param2 => TryGetValue(() => Param2);
            [Parameter] string Param3 => TryGetValue(() => Param3);

            Target Foo => _ => _;
            Target Bar => _ => _;
            Target Zoo => _ => _;
        }
#pragma warning restore CS0649
    }
}
