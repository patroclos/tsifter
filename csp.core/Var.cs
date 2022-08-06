using System;
using System.Collections.Generic;
using System.Linq;

namespace csp; 

public class Var<T> : IVariable<T> {
	public readonly IEnumerable<T> Domain;
	public readonly string? Name;

	Type IVariable.ValueType => typeof(T);

	private List<IVariable>? _scope;
	public List<IVariable> Scope => _scope ??= new List<IVariable>{this};

	IEnumerable<T> IVariable<T>.Domain => Domain;

	IEnumerable<object> IVariable.Domain => Domain.ToList().Cast<object>();

	public Var(IEnumerable<T> domain, string? name = null) {
		Domain = domain;
		Name = name;
	}

	object? ITerm.Evaluate(Problem _, Assignment ass) => ass[this];

	public override string ToString() => $"Var<{typeof(T).FullName}>" + (string.IsNullOrEmpty(Name) ? string.Empty : $" ('{Name}')");
}

public static class VarExtensions {
	public static Dictionary<A, IVariable<V>> SelectVariableTable<A, V>(this IEnumerable<A> self, Func<A, IVariable<V>> map)
		=> self.ToDictionary(a => a, map);
}