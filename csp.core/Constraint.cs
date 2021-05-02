using System.Collections.Generic;
using System.Collections.Immutable;

namespace csp {
	public interface IConstraint : ITerm<bool> {
		bool IsSatisfiedBy(Problem p, Assignment a);
	}

	public class TermConstraint : IConstraint {
		public readonly ITerm Term;

		public TermConstraint(ITerm term) {
			Term = term;
		}

		public ImmutableList<IVariable> Scope => Term.Scope;

		public bool Evaluate(Problem p, Assignment a) => (bool)Term.Evaluate(p, a);

		public bool IsSatisfiedBy(Problem p, Assignment a) => Evaluate(p, a);
	}
}
