using System;
using System.Collections;
using System.Collections.Generic;

namespace csp {

	public interface ITerm {
		IEnumerable<IVariable> Dependencies { get; }

		object Evaluate(Problem p, Assignment a);
	}

	public interface ITerm<T> : ITerm {
		new T Evaluate(Problem p, Assignment a);

		object ITerm.Evaluate(Problem p, Assignment a) => (object)Evaluate(p, a);
	}

	public interface IVariable<T> : IVariable, ITerm<T> {
		new IEnumerable<T> Domain { get; }

		T ITerm<T>.Evaluate(Problem _, Assignment a) {
			var val = a[this];
			return (T)a[this];
		}
	}

	public interface IVariable : ITerm {
		Type ValueType { get; }
		IEnumerable Domain { get; }

		IEnumerable<IVariable> ITerm.Dependencies => new[] { this };

		object ITerm.Evaluate(Problem _, Assignment a) {
			return a[this];
		}
	}

	public class Var<T> : IVariable<T> {
		public readonly IEnumerable<T> Domain;
		public readonly string? Name;

		Type IVariable.ValueType => typeof(T);

		IEnumerable<T> IVariable<T>.Domain => Domain;

		IEnumerable IVariable.Domain => Domain;

		public Var(IEnumerable<T> domain, string? name = null) {
			Domain = domain;
			Name = name;
		}

		object ITerm.Evaluate(Problem _, Assignment ass) => ass[this];
	}

}
