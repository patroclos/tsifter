using System.Collections.Generic;
using System.Collections.Immutable;

namespace csp {
	public class ProblemBuilder {
		public HashSet<IVariable> Variables = new HashSet<IVariable>();
		public HashSet<IConstraint> Constraints = new HashSet<IConstraint>();

		public IVariable<T> AddVariable<T>(IEnumerable<T> domain, string? name = null) {
			var v = new Var<T>(domain, name);
			Variables.Add(v);

			return v;
		}

		/// add constraint and all dependency variables to the problem
		public IConstraint AddConstraint(ITerm<bool> term) {
			var constraint = term is IConstraint c ? c : new TermConstraint(term);

			foreach(var v in term.Scope)
				Variables.Add(v);

			Constraints.Add(constraint);
			return constraint;
		}

		public Problem Build() {
			return new Problem(Variables.ToImmutableHashSet(), Constraints.ToImmutableHashSet());
		}
	}
}

