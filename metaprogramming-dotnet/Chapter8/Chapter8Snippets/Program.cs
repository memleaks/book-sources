using System;
using Microsoft.Scripting.Hosting;
using System.Dynamic;
using System.Reflection;
using Microsoft.Scripting;

namespace Chapter8Snippets
{
  [AttributeUsage(AttributeTargets.Method)]
  public class DemoMethodAttribute : Attribute { }

  class Program
  {
    static void Main()
    {
      RunDemoMethod("MultiLanguageLoad");

      //RunDemoMethod("ReturnScalarFromScript");

      /*
      RunDemoMethod("PassingVariablesToCompiledCode",
        "Platinum has 6 naturally-occuring " +
        "iostopes. True or False? ", true);
      RunDemoMethod("PassingVariablesToCompiledCode",
        "By ascending rank, where does the mass " +
        "of calcium in the Earth's\r\ncrust fall " +
        "as compared to the other elements? ", 5);
      */

      /*
      RunDemoMethod("PoseQuizQuestion",
        "Platinum has 6 naturally-occuring " +
        "iostopes. True or False? ", true);
      RunDemoMethod("PoseQuizQuestion",
        "By ascending rank, where does the mass " +
        "of calcium in the Earth's\r\ncrust fall " +
        "as compared to the other elements? ", 5);
      */
    }

    [DemoMethod]
    public static void MultiLanguageLoad()
    {
      var runtimeSetup = new ScriptRuntimeSetup();
      var pythonSetup = new LanguageSetup(
        typeName: "IronPython.Runtime.PythonContext, IronPython",
        displayName: "IronPython",
        names: new[] { "IronPython", "Python", "py" },
        fileExtensions: new[] { ".py" });
      runtimeSetup.LanguageSetups.Add(pythonSetup);
      var rubySetup = new LanguageSetup(
        typeName: "IronRuby.Runtime.RubyContext, IronRuby",
        displayName: "IronRuby",
        names: new[] { "IronRuby", "Ruby", "rb" },
        fileExtensions: new[] { ".rb" });
      runtimeSetup.LanguageSetups.Add(rubySetup);
      ScriptRuntime runtimeObject =
        new ScriptRuntime(runtimeSetup);
      ScriptEngine pythonEngine =
        runtimeObject.GetEngine("Python");
      ScriptEngine rubyEngine =
        runtimeObject.GetEngineByFileExtension(".rb");

      pythonEngine.Execute("print 'Hello from Python!'");
      rubyEngine.Execute("puts 'Hello from Ruby!'");
    }

    [DemoMethod]
    public static void ReturnScalarFromScript()
    {
      var runtimeSetup = new ScriptRuntimeSetup();
      var languageSetup = new LanguageSetup(
        "IronPython.Runtime.PythonContext, IronPython",
        "IronPython", new[] { "Python" }, new[] { ".py" });
      runtimeSetup.LanguageSetups.Add(languageSetup);
      var runtime = new ScriptRuntime(runtimeSetup);
      ScriptEngine engine = runtime.GetEngine("Python");

      string name = engine.Execute(
        "raw_input('What is your name? ')");
      int age = engine.Execute<int>(
        "input('How old are you? ')");

      Console.WriteLine(
        "Wow, {0} is only {1} years old!",
        name, age);
    }

    [DemoMethod]
    public static void PassingVariablesToCompiledCode(
      string question, object correctResponse)
    {
      var runtimeSetup = new ScriptRuntimeSetup();
      var languageSetup = new LanguageSetup(
        "IronPython.Runtime.PythonContext, IronPython",
        "IronPython", new[] { "Python" }, new[] { ".py" });
      runtimeSetup.LanguageSetups.Add(languageSetup);
      var runtime = new ScriptRuntime(runtimeSetup);
      ScriptEngine engine = runtime.GetEngine("Python");

      ScriptSource source =
        engine.CreateScriptSourceFromString(@"
import Question
import CorrectResponse
input(Question) == CorrectResponse
");

      CompiledCode AskQuestion = source.Compile();

      runtime.Globals.SetVariable("Question", question);
      runtime.Globals.SetVariable(
        "CorrectResponse", correctResponse);

      Console.WriteLine("You chose... {0}",
        AskQuestion.Execute<bool>()
          ? "wisely."
          : "poorly");
    }

    private static ScriptEngine _pythonEngine = null;
    private static ScriptEngine PythonEngine
    {
      get
      {
        if (_pythonEngine == null)
        {
          var runtimeSetup = new ScriptRuntimeSetup();
          var languageSetup = new LanguageSetup(
            "IronPython.Runtime.PythonContext, IronPython",
            "IronPython", new[] { "Python" }, new[] { ".py" });
          runtimeSetup.LanguageSetups.Add(languageSetup);
          var runtime = new ScriptRuntime(runtimeSetup);
          _pythonEngine = runtime.GetEngine("Python");
        }
        return _pythonEngine;
      }
    }

    private static CompiledCode _askQuestion = null;
    private static CompiledCode AskQuestion
    {
      get
      {
        if (_askQuestion == null)
        {
          ScriptSource source =
            PythonEngine.CreateScriptSourceFromString(
            "input(Question) == CorrectResponse");

          _askQuestion = source.Compile();
        }
        return _askQuestion;
      }
    }

    private static ScriptScope _questionScope = null;
    private static ScriptScope QuestionScope
    {
      get
      {
        if (_questionScope == null)
        {
          _questionScope =
            PythonEngine.CreateScope();
        }
        return _questionScope;
      }
    }

    [DemoMethod]
    public static void PoseQuizQuestion(
      string question, object correctResponse)
    {
      QuestionScope.SetVariable("Question", question);
      QuestionScope.SetVariable("CorrectResponse",
        correctResponse);

      Console.WriteLine("You chose... {0}",
        AskQuestion.Execute<bool>(QuestionScope)
          ? "wisely."
          : "poorly");
    }

    private static void RunDemoMethod(string name,
      params object[] parameters)
    {
      MethodInfo method =
        typeof(Program).GetMethod(name,
          BindingFlags.Public
          | BindingFlags.Static);
      if (method == null)
        return;
      Console.Title = name;
      method.Invoke(null, parameters);
      Console.ReadLine();
    }
  }
}
