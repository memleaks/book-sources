using System;
using System.Linq;
using System.Collections.Generic;

namespace DevJourney.Scripting
{
  /// <summary>
  /// The prototypical rule containing a name,
  /// location, code, type and expected results.
  /// </summary>
  public interface IRule
  {
    /// <summary>
    /// The name of the rule which must not be null or empty.
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// The address of the rule. This could be a file name,
    /// a web service address or anything else that describes
    /// how it was obtained. It must not be null or empty.
    /// </summary>
    string Address { get; set; }
    /// <summary>
    /// The body (code) of the rule expressed in any DLR
    /// hosting-compliant programming language.
    /// </summary>
    string Body { get; set; }
    /// <summary>
    /// The type of code in the Body, e.g. Python, Ruby, etc.
    /// The ScriptRuntime that eventually runs the code must
    /// have a LanguageSetup that exposes this value as one
    /// of its Name property members.
    /// </summary>
    string ContentType { get; set; }
    /// <summary>
    /// A collection of names to fetch from the ScriptScope
    /// after the execution of the script has completed.
    /// </summary>
    string[] ExpectedReturnValueNames { get; set; }
  }

  /// <summary>
  /// An abstract base class types which implements
  /// the IRule interface for safety and convenience.
  /// </summary>
  public abstract class RuleBase : IRule
  {
    private string _name;
    private string _address;
    private string _body;
    private string _contentType;
    private List<string> _expectedReturnValueNames;

    /// <summary>
    /// The name of the rule which must not be null or empty.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the mutator argument is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the mutator argument is empty.
    /// </exception>
    public string Name
    {
      get { return _name; }
      set
      {
        ValidateString("Name", ref value);
        _name = value;
      }
    }

    /// <summary>
    /// The address of the rule. This could be a file name,
    /// a web service address or anything else that describes
    /// how it was obtained. It must not be null or empty.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the mutator argument is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the mutator argument is empty.
    /// </exception>
    public string Address
    {
      get { return _address; }
      set
      {
        ValidateString("Address", ref value);
        _address = value;
      }
    }

    /// <summary>
    /// The body (code) of the rule expressed in any DLR
    /// hosting-compliant programming language.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the mutator argument is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the mutator argument is empty.
    /// </exception>
    public string Body
    {
      get { return _body; }
      set
      {
        ValidateString("Body", ref value);
        _body = value;
      }
    }

    /// <summary>
    /// The type of code in the Body, e.g. Python, Ruby, etc.
    /// The ScriptRuntime that eventually runs the code must
    /// have a LanguageSetup that exposes this value as one
    /// of its Name property members.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the mutator argument is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the mutator argument is empty.
    /// </exception>
    public string ContentType
    {
      get { return _contentType; }
      set
      {
        ValidateString("ContentType", ref value);
        _contentType = value;
      }
    }

    /// <summary>
    /// A list of names of variables to fetch from
    /// the ScriptScope after executing the rule.
    /// </summary>
    public string[] ExpectedReturnValueNames
    {
      get
      {
        return (_expectedReturnValueNames != null)
          ? _expectedReturnValueNames.ToArray()
          : null;
      }
      set
      {
        if (value == null)
        {
          _expectedReturnValueNames = null;
          return;
        }
        List<string> newNames =
          (from name in value
           let tname = (name == null)
             ? String.Empty
             : name.Trim()
           where tname.Length > 0
           select tname).ToList();
        _expectedReturnValueNames =
          (newNames.Count > 0)
            ? newNames
            : null;
      }
    }

    private void ValidateString(
      string name, ref string value)
    {
      if (value == null)
        throw new ArgumentNullException(name);
      value = value.Trim();
      if (value.Length == 0)
        throw new ArgumentException(name,
          String.Format("The {0} property " +
            "may not be empty.", name));
    }
  }

  /// <summary>
  /// An implementation of IRule that reads rule code
  /// from a disk-based file, assigning the Name, Address,
  /// ContentType and Body based on file properties.
  /// </summary>
  public class RuleFromFile : RuleBase
  {
    /// <summary>
    /// Construct the rule from a file path.
    /// </summary>
    /// <param name="filePath">
    /// The full path to the script file to be loaded.
    /// </param>
    /// <param name="expectedReturnValueNames">
    /// A list of named variables to fetch from the
    /// ScriptScope when the execution of the script
    /// is complete. This parameter may be null or
    /// an empty array.
    /// </param>
    public RuleFromFile(string filePath,
      string[] expectedReturnValueNames = null)
    {
      Name = System.IO.Path
        .GetFileNameWithoutExtension(filePath);
      Address = filePath;
      ContentType = System.IO.Path
        .GetExtension(filePath).Remove(0, 1);
      ExpectedReturnValueNames =
        expectedReturnValueNames;
      using (System.IO.StreamReader stream =
        System.IO.File.OpenText(filePath))
      {
        Body = stream.ReadToEnd();
      }
    }
  }
}
