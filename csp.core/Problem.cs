using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace csp {
	public readonly struct Problem {
		public readonly ImmutableHashSet<IVariable> Variables;
		public readonly ImmutableHashSet<IConstraint> Constraints;

		public Problem(ImmutableHashSet<IVariable> variables, ImmutableHashSet<IConstraint> constraints) {
			Variables = variables;
			Constraints = constraints;
		}

		public static ProblemBuilder Build() => new ProblemBuilder {
			Variables = new List<IVariable>(),
			Constraints = new List<IConstraint>()
		};

		public bool IsSatisfiedBy(Assignment a) {
			var self = this;
			return Constraints.All(c => c.IsSatisfiedBy(self, a));
		}
	}

	public struct ProblemBuilder {
		public List<IVariable> Variables;
		public List<IConstraint> Constraints;

		public IVariable<T> AddVariable<T>(IEnumerable<T> domain, string? name = null) {
			var v = new Var<T>(domain, name);
			Variables.Add(v);

			return v;
		}

		public Problem Build() => new Problem(Variables.ToImmutableHashSet(), Constraints.ToImmutableHashSet());
	}
}
