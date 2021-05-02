using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace csp {
	public class Var<T> : IVariable<T> {
		public readonly IEnumerable<T> Domain;
		public readonly string? Name;

		Type IVariable.ValueType => typeof(T);

		private ImmutableList<IVariable>? _scope;
		public ImmutableList<IVariable> Scope => _scope ?? (_scope = new IVariable[]{this}.ToImmutableList());

		IEnumerable<T> IVariable<T>.Domain => Domain;

		IEnumerable<object> IVariable.Domain => (IEnumerable<object>)Domain.ToList().Cast<object>();

		public Var(IEnumerable<T> domain, string? name = null) {
			Domain = domain;
			Name = name;
		}

		object ITerm.Evaluate(Problem _, Assignment ass) => ass[this];

		public override string ToString() => $"Var<{typeof(T).FullName}>" + (string.IsNullOrEmpty(Name) ? string.Empty : $" ('{Name}')");
	}

}

