using System;
using System.Collections.Generic;

namespace csp {

	public interface IVariable<T> : IVariable { }

	public interface IVariable {
		Type ValueType { get; }
	}

	public class Var<T> : IVariable<T> {
		public readonly IEnumerable<T> Domain;
		public readonly string? Name;

		public Var(IEnumerable<T> domain, string? name = null) {
			Domain = domain;
			Name = name;
		}

		Type IVariable.ValueType => typeof(T);
	}

}
