// Copyright 2021 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;

namespace Nuke.Common.Execution
{
    internal class HandleVisualStudioDebuggingAttribute : BuildExtensionAttributeBase, IOnBuildCreated
    {
        private const int TimeoutInMilliseconds = 10_000;

        public void OnBuildCreated(IReadOnlyCollection<ExecutableTarget> executableTargets)
        {
            if (!ParameterService.GetParameter<bool>(Constants.VisualStudioDebugParameterName))
                return;

            File.WriteAllText(Constants.GetVisualStudioDebugFile(Build.RootDirectory),
                Process.GetCurrentProcess().Id.ToString());
            Assert.True(SpinWait.SpinUntil(() => Debugger.IsAttached, millisecondsTimeout: TimeoutInMilliseconds),
                $"VisualStudio debugger was not attached within {TimeoutInMilliseconds} milliseconds");
        }
    }
}
