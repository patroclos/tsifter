using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace csp {

	public interface ITerm {
		// TODO: use ImmutableSortedSet
		ImmutableList<IVariable> Scope { get; }

		object Evaluate(Problem p, Assignment a);
	}

	public interface ITerm<T> : ITerm {
		new T Evaluate(Problem p, Assignment a);

		object ITerm.Evaluate(Problem p, Assignment a) => (object)Evaluate(p, a);
	}


	public interface IVariable : ITerm {
		Type ValueType { get; }
		IEnumerable<object> Domain { get; }

		// ImmutableList<IVariable> ITerm.Scope => new []{this}.ToImmutableList();

		object ITerm.Evaluate(Problem _, Assignment a) {
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
