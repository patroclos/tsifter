using System.Linq;
using System.Collections.Immutable;

namespace csp; 

public class Problem {
	public readonly ImmutableHashSet<IVariable> Variables;
	public readonly ImmutableHashSet<IConstraint> Constraints;

	public Problem(ImmutableHashSet<IVariable> variables, ImmutableHashSet<IConstraint> constraints) {
		Variables = variables;
		Constraints = constraints;
	}

	public bool IsSatisfiedBy(Assignment a) {
		var self = this;
		return Constraints.All(c => c.IsSatisfiedBy(self, a));
	}

}
