using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.InteropServices;
using IctBaden.Framework.Types;

namespace IctBaden.Scripting;

[ComVisible(true)]
public class ScriptContext
{
    private readonly Dictionary<string, object?> _valueList = new();
    private QueryValue? _queryValue;
    public dynamic Var = new ExpandoObject(); 
    public dynamic Result = new ExpandoObject(); 

    public delegate object QueryValue(string key);

    public void Clear()
    {
        _valueList.Clear();
    }

    public void SetQuery(QueryValue query)
    {
        _queryValue = query;
    }

    public object? GetValue(string key) => this[key];
    public T? GetValue<T>(string key) => UniversalConverter.ConvertTo<T>(_valueList[key]) ?? default(T);
        
    public object? this[string key]
    {
        set => _valueList[key] = value;
        get
        {
            var val = _queryValue?.Invoke(key);
            return val ?? _valueList.GetValueOrDefault(key);
        }
    }

    public IEnumerable<string> ValueNames
    {
        get
        {
            foreach (var keyValue in _valueList)
            {
                yield return keyValue.Key;
            }
        }
    }

}