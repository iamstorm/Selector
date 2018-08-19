using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronPython.Hosting;
using System.Reflection;
using Microsoft.Scripting.Hosting;
using System.Collections;

namespace SelectImpl
{
  public class PythonOper
  {
    public readonly ScriptEngine Engine = Python.CreateEngine();

    public readonly IDictionary<string, object> ValueDict = new Dictionary<string, object>();

    private ScriptScope resultScope;

    public PythonOper()
    {
        Engine.Runtime.LoadAssembly(Assembly.LoadFrom("SelectImpl.dll"));
    }

    public void Execute(String scriptFilePath)
    {
      var scope = Engine.CreateScope();
      foreach (var key in ValueDict.Keys)
      {
        scope.SetVariable(key, ValueDict[key]);
      }
      resultScope = Engine.ExecuteFile(scriptFilePath, scope);
    }

    public object GetValue(string key)
    {
      return resultScope.GetVariable(key);
    }
  }
}
