using System;
using System.Collections.Generic;
using System.Linq;

namespace csp; 

public class Var<T> : IVariable<T> {
	public readonly IEnumerable<T> Domain;
	private readonly string _name;
	public string Name => _name;

	Type IVariable.ValueType => typeof(T);

	private HashSet<IVariable>? _scope;
	public HashSet<IVariable> Scope => _scope ??= new() {this};

	IEnumerable<T> IVariable<T>.Domain => Domain;

	IEnumerable<object> IVariable.Domain => Domain.ToList().Cast<object>();

	public Var(IEnumerable<T> domain, string? name = $"variable({nameof(T)})") {
		Domain = domain;
		_name = name;
	}

	object? ITerm.Evaluate(Problem _, Assignment ass) => ass[this];

	public override string ToString() => $"Var<{typeof(T).FullName}>" + (string.IsNullOrEmpty(_name) ? string.Empty : $" ('{_name}')");
}

public static class VarExtensions {
	public static Dictionary<A, IVariable<V>> SelectVariableTable<A, V>(this IEnumerable<A> self, Func<A, IVariable<V>> map)
		=> self.ToDictionary(a => a, map);
}
