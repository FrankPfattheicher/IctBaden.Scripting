using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.InteropServices;

namespace IctBaden.Scripting;

[ComVisible(true)]
public class ScriptContext
{
    private readonly Dictionary<string, object?> _valueList = new();
    private QueryValue? _queryValue;
    public dynamic Var = new ExpandoObject(); 

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
        
    public object? this[string key]
    {
        set => _valueList[key] = value;
        get
        {
            var val = _queryValue?.Invoke(key);
            if (val != null)
                return val;
            return _valueList.TryGetValue(key, out var value) ? value : null;
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