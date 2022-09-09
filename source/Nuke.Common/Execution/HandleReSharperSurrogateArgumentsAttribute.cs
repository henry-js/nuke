﻿// Copyright 2022 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Serilog;

namespace Nuke.Common.Execution
{
    internal class HandleReSharperSurrogateArgumentsAttribute : BuildExtensionAttributeBase, IOnBuildCreated
    {
        private const string SurrogateFileName = "nuke.tmp";

        public void OnBuildCreated(IReadOnlyCollection<ExecutableTarget> executableTargets)
        {
            var surrogateFile = Build.BuildAssemblyDirectory / SurrogateFileName;
            if (!File.Exists(surrogateFile))
                return;

            var argumentLines = File.ReadAllLines(surrogateFile);
            var lastWriteTime = File.GetLastWriteTime(surrogateFile);

            Assert.HasSingleItem(argumentLines, $"{SurrogateFileName} must have only one single line");
            File.Delete(surrogateFile);
            if (lastWriteTime.AddMinutes(value: 1) < DateTime.Now)
            {
                Log.Warning("Last write time of {File} was {LastWriteTime}. Skipping ...", SurrogateFileName, lastWriteTime);
                return;
            }

            var arguments = argumentLines.Single();
            EnvironmentInfo.ArgumentParser = new ArgumentParser(arguments);
        }
    }
}
