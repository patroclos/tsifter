using System.Collections.Generic;

namespace csp; 

public class TermConstraint : IConstraint {
	public readonly ITerm ConstraintTerm;

	public TermConstraint(ITerm term) {
		ConstraintTerm = term;
	}

	public List<IVariable> Scope => ConstraintTerm.Scope;

	public bool Evaluate(Problem p, Assignment a) => (bool)ConstraintTerm.Evaluate(p, a)!;

	public bool IsSatisfiedBy(Problem p, Assignment a) => Evaluate(p, a);
}
