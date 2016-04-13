using System;
using System.Dynamic;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;

namespace DevJourney.Scripting
{
  public class UnsafeRuleEngine
  {
    private readonly ScriptRuntimeSetup _runtimeSetup;
    private readonly ScriptRuntime _sharedRuntime;
    private readonly Dictionary<int, RuleContext>
      _rulesContexts;
    private static int _nextHandle = 0;

    private struct RuleContext
    {
      internal IRule Rule;
      internal CompiledCode Code;
      internal ScriptScope SharedScope;
    }

    public UnsafeRuleEngine()
    {
      _runtimeSetup = new ScriptRuntimeSetup();
      _runtimeSetup.LanguageSetups.Add(
        new LanguageSetup(
          "IronPython.Runtime.PythonContext, IronPython",
          "IronPython",
          new[] { "IronPython", "Python", "py" },
          new[] { ".py" }));
      _sharedRuntime =
        new ScriptRuntime(_runtimeSetup);
      _rulesContexts =
        new Dictionary<int, RuleContext>();
    }

    public IRule SelectRule(int handle)
    {
      return _rulesContexts[handle].Rule;
    }

    public int InsertRule(IRule rule)
    {
      int handle = -1;
      UpsertRule(rule, ref handle);
      return handle;
    }

    public void UpdateRule(int handle, IRule rule)
    {
      UpsertRule(rule, ref handle);
    }

    public void DeleteRule(int handle)
    {
      _rulesContexts.Remove(handle);
    }

    public IDictionary<string, dynamic> Execute(
      int handle,
      IDictionary<string, object> parameters)
    {
      RuleContext context =
        _rulesContexts[handle];
      ScriptScope scope = context.SharedScope;

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
      ref int handle)
    {
      CompiledCode compilation = null;
      ScriptScope sharedScope = null;
      ScriptEngine engine = _sharedRuntime
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
        };
    }
  }
}
