// at least one of these languages must be defined
#define ENABLE_PYTHON
//#define ENABLE_RUBY

using System;
using System.Dynamic;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;

namespace DevJourney.Scripting
{
  /// <summary>
  /// Describe the isolation mode of an AppDomain,
  /// a ScriptRuntime or a ScriptScope.
  /// </summary>
  public enum IsolationMode
  {
    Shared,
    Private
  }

  /// <summary>
  /// An encapsulation of a Dynamic Language Runtime
  /// scripting engine that simplifies runtime setup
  /// and execution. This class also affords AppDomain,
  /// ScriptRuntime and ScriptScope isolation for
  /// establishing separation between host and script
  /// code.
  /// </summary>
  public class RuleEngine
  {
    private readonly ScriptRuntimeSetup _runtimeSetup;
    private readonly ScriptRuntime _sharedRuntime;
    private readonly Dictionary<int, RuleContext>
      _rulesContexts;
    private readonly AppDomain _remoteAppDomain;
    private static int _nextHandle = 0;

    private struct RuleContext
    {
      internal IRule Rule;
      internal CompiledCode Code;
      internal ScriptScope SharedScope;
      internal bool IsIsolatedRuntime;
    }

    /// <summary>
    /// Create a new engine for executing rules.
    /// </summary>
    /// <param name="appDomainMode">
    /// Set to IsolationMode.Private to force all rules
    /// in this engine to run in a separate application
    /// domain. Set to IsolationMode.Shared to allow the
    /// rules to run in the current application domain.
    /// </param>
    public RuleEngine(IsolationMode appDomainMode)
    {
      _runtimeSetup = new ScriptRuntimeSetup();

#if ENABLE_PYTHON
      // Make sure that the IronPython.dll assembly
      // is available to the application at runtime.
      _runtimeSetup.LanguageSetups.Add(
        new LanguageSetup(
          "IronPython.Runtime.PythonContext, IronPython",
          "IronPython",
          new[] { "IronPython", "Python", "py" },
          new[] { ".py" }));
#endif

#if ENABLE_RUBY
      // Make sure that the IronRuby.dll assembly
      // is available to the application at runtime.
      _runtimeSetup.LanguageSetups.Add(
        new LanguageSetup(
          "IronRuby.Runtime.RubyContext, IronRuby",
          "IronRuby",
          new[] { "IronRuby", "Ruby", "rb" },
          new[] { ".rb" }));
#endif

      if (appDomainMode == IsolationMode.Private)
        _remoteAppDomain = AppDomain.CreateDomain(
          DateTime.UtcNow.ToString("s"));
      _sharedRuntime =
        (_remoteAppDomain != null)
          ? ScriptRuntime.CreateRemote(
            _remoteAppDomain, _runtimeSetup)
          : new ScriptRuntime(_runtimeSetup);
      _rulesContexts =
        new Dictionary<int, RuleContext>();
    }

    /// <summary>
    /// Fetch a rule from the rule cache by handle.
    /// </summary>
    /// <param name="handle">
    /// A handle previously obtained by calling InsertRule or UpdateRule.
    /// </param>
    /// <returns>
    /// A reference to the rule object.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the specified handle cannot be found in the rule cache.
    /// </exception>
    public IRule SelectRule(int handle)
    {
      lock (_rulesContexts)
      {
        if (!_rulesContexts.ContainsKey(handle))
        throw new ArgumentOutOfRangeException(
          "handle", String.Format("The rule " +
          "context with handle {0} cannot be " +
          "selected from the cache because it " +
          "does not exist.", handle));

        return _rulesContexts[handle].Rule;
      }
    }

    /// <summary>
    /// Insert a rule into the rule cache. This method also
    /// compiles the rule within the cache to improve performance.
    /// </summary>
    /// <param name="rule">
    /// The rule to insert. This parameter must not be null.
    /// </param>
    /// <param name="runtimeMode">
    /// Set to IsolationMode.Private to force the rule engine to
    /// create a separate ScriptRuntime for the rule. Set to
    /// IsolationMode.Shared to allow the rule to run in a shared
    /// ScriptRuntime managed by the rule engine.
    /// </param>
    /// <returns>
    /// The unique cache handle of the rule within this scope of
    /// this rule engine.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the specified rule is null.
    /// </exception>
    public int InsertRule(IRule rule,
      IsolationMode runtimeMode)
    {
      lock (_rulesContexts)
      {
        int handle = -1;
        UpsertRule(rule, ref handle,
            runtimeMode);
        return handle;
      }
    }

    /// <summary>
    /// Update an existing rule in the rule cache by handle. This
    /// method also compiles the rule within the cache to improve
    /// performance.
    /// </summary>
    /// <param name="handle">
    /// The handle of the existing rule to update.
    /// </param>
    /// <param name="rule">
    /// The rule to update. This parameter must not be null.
    /// </param>
    /// <param name="runtimeMode">
    /// Set to IsolationMode.Private to force the rule engine to
    /// create a separate ScriptRuntime for the rule. Set to
    /// IsolationMode.Shared to allow the rule to run in a shared
    /// ScriptRuntime managed by the rule engine.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the specified rule is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the specified handle cannot be found in the rule cache.
    /// </exception>
    public void UpdateRule(int handle, IRule rule,
      IsolationMode runtimeMode)
    {
      lock (_rulesContexts)
      {
        if (!_rulesContexts.ContainsKey(handle))
          throw new ArgumentOutOfRangeException(
            "handle", String.Format("The rule " +
            "context with handle {0} cannot be " +
            "updated in the cache because it " +
            "does not exist.", handle));

        UpsertRule(rule, ref handle,
          runtimeMode);
      }
    }

    /// <summary>
    /// Delete a rule from the rule cache by handle. This method
    /// also shuts down the associated ScriptRuntime if the rule
    /// was set to IsolationMode.Private.
    /// </summary>
    /// <param name="handle">
    /// The handle of the rule to delete.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the specified handle cannot be found in the rule cache.
    /// </exception>
    public void DeleteRule(int handle)
    {
      lock (_rulesContexts)
      {
        if (!_rulesContexts.ContainsKey(handle))
          throw new ArgumentOutOfRangeException(
            "handle", String.Format("The rule " +
            "context with handle {0} cannot be " +
            "deleted from the cache because it " +
            "does not exist.", handle));

        if (_rulesContexts[handle].IsIsolatedRuntime)
        {
          _rulesContexts[handle].Code.Engine
            .Runtime.Shutdown();
        }
        _rulesContexts.Remove(handle);
      }
    }

    /// <summary>
    /// Execute a rule from the cache by handle.
    /// </summary>
    /// <param name="handle">
    /// The handle of the rule to execute.
    /// </param>
    /// <param name="parameters">
    /// A dictionary of name value pairs to insert into the
    /// ScriptScope before executing the rule.
    /// </param>
    /// <param name="scopeMode">
    /// Set to IsolationMode.Private to force a new
    /// ScriptScope to be created before the execution. Set
    /// to IsolationMode.Shared to allow the use of a shared
    /// ScriptScope.
    /// </param>
    /// <returns>
    /// A dictionary of name value pairs according to the
    /// names in the ExpectedReturnValueNames of the associated
    /// rule object.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the specified handle cannot be found in the rule cache.
    /// </exception>
    public IDictionary<string, dynamic> Execute(
      int handle,
      IDictionary<string, object> parameters,
      IsolationMode scopeMode)
    {
      RuleContext context;
      lock (_rulesContexts)
      {
        if (!_rulesContexts.ContainsKey(handle))
          throw new ArgumentOutOfRangeException(
            "handle", String.Format("Rule handle " +
            "{0} was not found in the rule cache.",
            handle));

        context = _rulesContexts[handle];
      }

      ScriptScope scope =
        (scopeMode == IsolationMode.Private)
          ? context.Code.Engine.CreateScope()
          : context.SharedScope;
      foreach (var kvp in parameters)
        scope.SetVariable(kvp.Key, kvp.Value);

      context.Code.Execute(scope);

      var results = new Dictionary<string, dynamic>();
      if (context.Rule.ExpectedReturnValueNames != null
        && context.Rule
            .ExpectedReturnValueNames.Length > 0)
      {
        dynamic result;
        foreach (var valueName in
          context.Rule.ExpectedReturnValueNames)
        {
          if (valueName == null
            || valueName.Trim().Length == 0)
          {
            continue;
          }
          if (scope.TryGetVariable(
            valueName.Trim(), out result))
          {
            results.Add(valueName, result);
          }
        }
      }

      return results;
    }

    private void UpsertRule(IRule rule,
      ref int handle, IsolationMode runtimeMode)
    {
      if (rule == null)
        throw new ArgumentNullException("rule");

      lock (_rulesContexts)
      {
        CompiledCode compilation = null;
        ScriptScope sharedScope = null;
        ScriptRuntime runtime =
          (runtimeMode == IsolationMode.Private)
            ? (_remoteAppDomain != null)
              ? ScriptRuntime.CreateRemote(
                  _remoteAppDomain,
                  _runtimeSetup)
              : new ScriptRuntime(_runtimeSetup)
            : _sharedRuntime;
        ScriptEngine engine = runtime
          .GetEngineByFileExtension(
            rule.ContentType);
        sharedScope = engine.CreateScope();
        ScriptSource source = engine
          .CreateScriptSourceFromString(
            rule.Body);
        compilation = source.Compile();

        if (_rulesContexts.ContainsKey(handle))
          DeleteRule(handle);
        else
          handle = System.Threading
            .Interlocked.Increment(
              ref _nextHandle);

        _rulesContexts[handle] =
          new RuleContext()
          {
            Rule = rule,
            Code = compilation,
            SharedScope = sharedScope,
            IsIsolatedRuntime =
              (runtimeMode == IsolationMode.Private)
          };
      }
    }
  }
}