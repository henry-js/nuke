// Copyright 2022 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common.IO;

namespace Nuke.Common.Tests.New.Generation;

public interface ICommonObject
{
    public AbsolutePath File { get; set; }
    public string Description { get; set; }
}

public class Command : Data
{
    public Command()
    {
        GenerateExtensions = true;
    }
}

public class Data : ICommonObject
{
    public AbsolutePath File { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public string OfficialUrl { get; set; }

    public string BaseClass { get; set; }
    public string[] Namespaces { get; set; }
    public bool GenerateClass { get; set; } = true;
    public bool GenerateExtensions { get; set; }
    public string[] AdditionalOptions { get; set; }

    public List<Option> Options { get; set; }
}

public class Option : ICommonObject
{
    public Data DataObject { get; set; }
    public AbsolutePath File { get; set; }

    public string Name { get; set; }
    public string Type { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
    public string Format { get; set; }
    public string AlternativeFormat { get; set; }
    public string Description { get; set; }
    public int? Index { get; set; }
}
