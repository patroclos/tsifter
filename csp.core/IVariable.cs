using System;
using System.Collections.Generic;

namespace csp {

	public interface IVariable : ITerm {
		Type ValueType { get; }
		IEnumerable<object> Domain { get; }

		// ImmutableList<IVariable> ITerm.Scope => new []{this}.ToImmutableList();

		object? ITerm.Evaluate(Problem _, Assignment a) {
			return a[this];
		}
	}

	public interface IVariable<T> : IVariable, ITerm<T> {
		new IEnumerable<T> Domain { get; }

		T ITerm<T>.Evaluate(Problem _, Assignment a) {
			var val = a[this];
			return (T)a[this];
		}
	}
}
