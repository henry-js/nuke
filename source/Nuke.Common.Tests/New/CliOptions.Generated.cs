// ReSharper disable ArrangeMethodOrOperatorBody

using JetBrains.Annotations;
using Nuke.Common.Tests;
using Nuke.Common.Utilities.Collections;
using Serilog.Events;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Nuke.Common.Tests.New;

[PublicAPI]
public partial class CliOptions : OptionsBuilder
{
    public string ProcessToolPath => GetComplex<string>(() => ProcessToolPath);
    public string ProcessWorkingDirectory => GetComplex<string>(() => ProcessWorkingDirectory);
    public IReadOnlyDictionary<string, object> ProcessEnvironmentVariables => GetComplex<Dictionary<string, object>>(() => ProcessEnvironmentVariables);
    public int? ProcessExecutionTimeout => GetScalar<int?>(() => ProcessExecutionTimeout);
    public LogEventLevel? ProcessOutputLogging => GetComplex<LogEventLevel?>(() => ProcessOutputLogging);
    public LogEventLevel? ProcessInvocationLogging => GetComplex<LogEventLevel?>(() => ProcessInvocationLogging);
}

[PublicAPI]
public static partial class CliOptionsExtensions
{
    #region CliOptions.ProcessToolPath
    /// <summary><p>Defines the path of the tool to be invoked. In most cases, the tool path is automatically resolved from the PATH environment variable or a NuGet package.</p></summary>
    [OptionsModificator(OptionsType = typeof(CliOptions), Property = nameof(CliOptions.ProcessToolPath))]
    public static T SetProcessToolPath<T>(this T o, string value) where T : CliOptions => o.Copy(b => b.Set(() => o.ProcessToolPath, value));
    /// <summary><p>Defines the path of the tool to be invoked. In most cases, the tool path is automatically resolved from the PATH environment variable or a NuGet package.</p></summary>
    [OptionsModificator(OptionsType = typeof(CliOptions), Property = nameof(CliOptions.ProcessToolPath))]
    public static T ResetProcessToolPath<T>(this T o) where T : CliOptions => o.Copy(b => b.Remove(() => o.ProcessToolPath));
    #endregion
    #region CliOptions.ProcessWorkingDirectory
    /// <summary><p>Defines the working directory for the process.</p></summary>
    [OptionsModificator(OptionsType = typeof(CliOptions), Property = nameof(CliOptions.ProcessWorkingDirectory))]
    public static T SetProcessWorkingDirectory<T>(this T o, string value) where T : CliOptions => o.Copy(b => b.Set(() => o.ProcessWorkingDirectory, value));
    /// <summary><p>Defines the working directory for the process.</p></summary>
    [OptionsModificator(OptionsType = typeof(CliOptions), Property = nameof(CliOptions.ProcessWorkingDirectory))]
    public static T ResetProcessWorkingDirectory<T>(this T o) where T : CliOptions => o.Copy(b => b.Remove(() => o.ProcessWorkingDirectory));
    #endregion
    #region CliOptions.ProcessEnvironmentVariables
    /// <summary><p>Defines the environment variables to be passed to the process. By default, the environment variablesof the current process are used.</p></summary>
    [OptionsModificator(OptionsType = typeof(CliOptions), Property = nameof(CliOptions.ProcessEnvironmentVariables))]
    public static T SetProcessEnvironmentVariables<T>(this T o, IReadOnlyDictionary<string, object> values) where T : CliOptions => o.Copy(b => b.Set(() => o.ProcessEnvironmentVariables, values));
    /// <summary><p>Defines the environment variables to be passed to the process. By default, the environment variablesof the current process are used.</p></summary>
    [OptionsModificator(OptionsType = typeof(CliOptions), Property = nameof(CliOptions.ProcessEnvironmentVariables))]
    public static T SetProcessEnvironmentVariables<T>(this T o, IDictionary<string, object> values) where T : CliOptions => o.Copy(b => b.Set(() => o.ProcessEnvironmentVariables, values));
    /// <summary><p>Defines the environment variables to be passed to the process. By default, the environment variablesof the current process are used.</p></summary>
    [OptionsModificator(OptionsType = typeof(CliOptions), Property = nameof(CliOptions.ProcessEnvironmentVariables))]
    public static T AddProcessEnvironmentVariables<T>(this T o, IReadOnlyDictionary<string, object> values) where T : CliOptions => o.Copy(b => b.AddDictionary(() => o.ProcessEnvironmentVariables, values));
    /// <summary><p>Defines the environment variables to be passed to the process. By default, the environment variablesof the current process are used.</p></summary>
    [OptionsModificator(OptionsType = typeof(CliOptions), Property = nameof(CliOptions.ProcessEnvironmentVariables))]
    public static T AddProcessEnvironmentVariables<T>(this T o, IDictionary<string, object> values) where T : CliOptions => o.Copy(b => b.AddDictionary(() => o.ProcessEnvironmentVariables, values));
    /// <summary><p>Defines the environment variables to be passed to the process. By default, the environment variablesof the current process are used.</p></summary>
    [OptionsModificator(OptionsType = typeof(CliOptions), Property = nameof(CliOptions.ProcessEnvironmentVariables))]
    public static T AddProcessEnvironmentVariable<T>(this T o, string key, object value) where T : CliOptions => o.Copy(b => b.AddDictionary(() => o.ProcessEnvironmentVariables, key, value));
    /// <summary><p>Defines the environment variables to be passed to the process. By default, the environment variablesof the current process are used.</p></summary>
    [OptionsModificator(OptionsType = typeof(CliOptions), Property = nameof(CliOptions.ProcessEnvironmentVariables))]
    public static T SetProcessEnvironmentVariable<T>(this T o, string key, object value) where T : CliOptions => o.Copy(b => b.SetDictionary(() => o.ProcessEnvironmentVariables, key, value));
    /// <summary><p>Defines the environment variables to be passed to the process. By default, the environment variablesof the current process are used.</p></summary>
    [OptionsModificator(OptionsType = typeof(CliOptions), Property = nameof(CliOptions.ProcessEnvironmentVariables))]
    public static T RemoveProcessEnvironmentVariable<T>(this T o, string key) where T : CliOptions => o.Copy(b => b.RemoveDictionary(() => o.ProcessEnvironmentVariables, key));
    /// <summary><p>Defines the environment variables to be passed to the process. By default, the environment variablesof the current process are used.</p></summary>
    [OptionsModificator(OptionsType = typeof(CliOptions), Property = nameof(CliOptions.ProcessEnvironmentVariables))]
    public static T ClearProcessEnvironmentVariables<T>(this T o) where T : CliOptions => o.Copy(b => b.ClearDictionary(() => o.ProcessEnvironmentVariables));
    /// <summary><p>Defines the environment variables to be passed to the process. By default, the environment variablesof the current process are used.</p></summary>
    [OptionsModificator(OptionsType = typeof(CliOptions), Property = nameof(CliOptions.ProcessEnvironmentVariables))]
    public static T ResetProcessEnvironmentVariables<T>(this T o) where T : CliOptions => o.Copy(b => b.Remove(() => o.ProcessEnvironmentVariables));
    #endregion
    #region CliOptions.ProcessExecutionTimeout
    /// <summary><p>Defines the execution timeout of the invoked process.</p></summary>
    [OptionsModificator(OptionsType = typeof(CliOptions), Property = nameof(CliOptions.ProcessExecutionTimeout))]
    public static T SetProcessExecutionTimeout<T>(this T o, int? value) where T : CliOptions => o.Copy(b => b.Set(() => o.ProcessExecutionTimeout, value));
    /// <summary><p>Defines the execution timeout of the invoked process.</p></summary>
    [OptionsModificator(OptionsType = typeof(CliOptions), Property = nameof(CliOptions.ProcessExecutionTimeout))]
    public static T ResetProcessExecutionTimeout<T>(this T o) where T : CliOptions => o.Copy(b => b.Remove(() => o.ProcessExecutionTimeout));
    #endregion
    #region CliOptions.ProcessOutputLogging
    /// <summary><p>Defines the log-level for standard output.</p></summary>
    [OptionsModificator(OptionsType = typeof(CliOptions), Property = nameof(CliOptions.ProcessOutputLogging))]
    public static T SetProcessOutputLogging<T>(this T o, LogEventLevel? value) where T : CliOptions => o.Copy(b => b.Set(() => o.ProcessOutputLogging, value));
    /// <summary><p>Defines the log-level for standard output.</p></summary>
    [OptionsModificator(OptionsType = typeof(CliOptions), Property = nameof(CliOptions.ProcessOutputLogging))]
    public static T ResetProcessOutputLogging<T>(this T o) where T : CliOptions => o.Copy(b => b.Remove(() => o.ProcessOutputLogging));
    #endregion
    #region CliOptions.ProcessInvocationLogging
    /// <summary><p>Defines the log-level for the process invocation.</p></summary>
    [OptionsModificator(OptionsType = typeof(CliOptions), Property = nameof(CliOptions.ProcessInvocationLogging))]
    public static T SetProcessInvocationLogging<T>(this T o, LogEventLevel? value) where T : CliOptions => o.Copy(b => b.Set(() => o.ProcessInvocationLogging, value));
    /// <summary><p>Defines the log-level for the process invocation.</p></summary>
    [OptionsModificator(OptionsType = typeof(CliOptions), Property = nameof(CliOptions.ProcessInvocationLogging))]
    public static T ResetProcessInvocationLogging<T>(this T o) where T : CliOptions => o.Copy(b => b.Remove(() => o.ProcessInvocationLogging));
    #endregion
}
