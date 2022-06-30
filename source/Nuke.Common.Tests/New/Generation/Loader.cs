// Copyright 2022 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nuke.Common.IO;
using Nuke.Common.Utilities.Collections;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Nuke.Common.Tests.New.Generation;

public static class Loader
{
    public static IEnumerable<Data> LoadDataObjects(string file)
    {
        var dataObjects = LoadMultiple<Data>(file).ToList();
        if (dataObjects.Count == 1 && dataObjects[0].Name.IsNullOrEmpty())
            dataObjects[0].Name = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(file));

        var directory = Path.GetDirectoryName(file).NotNull();
        foreach (var dataObject in dataObjects)
        {
            dataObject.Options ??= new[] { $"{dataObject.Name}.options.yml" }
                .Concat(dataObject.AdditionalOptions?.Select(x => $"{x}.options.yml") ?? new string[0])
                .Select(x => Path.Combine(directory, x))
                .Where(File.Exists)
                .SelectMany(LoadMultiple<Option>).ToList();

            dataObject.Description = dataObject.Description?.Trim();
            dataObject.Options.ForEach(x =>
            {
                x.DataObject = dataObject;
                x.Description = x.Description?.Trim();
            });
        }

        return dataObjects;
    }

    public static IEnumerable<T> LoadMultiple<T>(string file)
        where T : ICommonObject
    {
        var yaml = File.ReadAllText(file);
        var reader = new StringReader(yaml);
        var parser = new Parser(reader);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        parser.Consume<StreamStart>();
        while (parser.Accept<DocumentStart>(out _))
        {
            var item = deserializer.Deserialize<T>(parser);
            item.File = (AbsolutePath) file;
            yield return item;
        }
    }
}
