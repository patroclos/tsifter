using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace csp; 

public class Assignment {
	public readonly ImmutableDictionary<IVariable, object> Values;

	public object this[IVariable v] => Values[v];

	public Assignment(IDictionary<IVariable, object> values) {
		Values = values as ImmutableDictionary<IVariable, object> ?? values.ToImmutableDictionary();
	}

	public T Read<T>(IVariable<T> variable) {
		return (T)Values[variable];
	}

	public Assignment Rebuild(Func<ImmutableDictionary<IVariable, object>, ImmutableDictionary<IVariable, object>> map) {
		return new Assignment(map(Values));
	}

	public bool IsCompleteFor(Problem p) => p.Variables.All(Values.ContainsKey);

	public override string ToString() => "{ " + string.Join(", ", Values.Select(kv => $"{kv.Key} => {kv.Value}")) + "}";
}
