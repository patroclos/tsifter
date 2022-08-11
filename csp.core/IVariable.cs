using System;
using System.Collections.Generic;

namespace csp; 

public interface IVariable : ITerm {
	string Name { get; }
	Type ValueType { get; }
	IEnumerable<object> Domain { get; }

	object ITerm.Evaluate(Problem _, Assignment a) {
		return a[this];
	}
}

public interface IVariable<out T> : IVariable, ITerm<T> {
	new IEnumerable<T> Domain { get; }

	T ITerm<T>.Evaluate(Problem _, Assignment a) {
		return (T)a[this];
	}
}
