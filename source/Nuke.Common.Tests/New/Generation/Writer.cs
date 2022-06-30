// Copyright 2022 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;

namespace Nuke.Common.Tests.New.Generation;

public class Writer
{
    private readonly IndentedTextWriter _writer;

    public Writer(StringWriter writer)
    {
        _writer = new IndentedTextWriter(writer);
    }

    public IDisposable Indent()
    {
        return DelegateDisposable.CreateBracket(
            () => _writer.Indent++,
            () => _writer.Indent--);
    }

    public IDisposable WriteBlock()
    {
        return DelegateDisposable.CreateBracket(
                () => WriteLine("{"),
                () => WriteLine("}"))
            .CombineWith(Indent());
    }

    public Writer WriteLine()
    {
        _writer.WriteLine();
        return this;
    }

    public Writer WriteLineParts(params string[] parts)
    {
        return WriteLine(parts.Where(x => !x.IsNullOrWhiteSpace()).JoinSpace());
    }

    public Writer WriteLine(params string[] lines)
    {
        foreach (var line in lines)
        {
            if (line.IsNullOrWhiteSpace())
                continue;

            _writer.WriteLine(line.Trim());
        }

        return this;
    }

    public Writer WriteLine(IEnumerable<string> items, Func<string, string> formatter)
    {
        items.ForEach(x => WriteLine(formatter.Invoke(x)));
        return this;
    }
}
