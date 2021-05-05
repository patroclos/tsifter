using System;
using System.Linq;
using System.Collections.Generic;

namespace csp {
	public sealed class STR2Solver {
		private Problem _problem;

		private Dictionary<IVariable, List<object>> _domains;

		public STR2Solver(Problem problem) {
			_problem = problem;
			_domains = problem.Variables.ToDictionary(v => v, v => v.Domain.ToList());
		}

		// prune values from variable domains that violate some constraint
		private void EnsureGeneralizedArcConsistency() {
			// for every constraint
			// find an r-tuple that satisfies the constraint
			// store that r-tuple in a `Dict<IConstraint, Assignment> Supports`
			throw new NotImplementedException();
		}

		// create a partial assignment that is consistent with the given constraints and spans their combined scope
		private Assignment? SearchSolutionSpace(params IConstraint[] constraints) {
			throw new NotImplementedException();
		}


		public IEnumerable<Solution> GetSolutions() {
			throw new NotImplementedException();
			yield break;
			// build a relations table for the constraint scopes of the problem
			// ensure arc consistency (for every variable of the scope of every constraint, remove all values from the domain that have no support in the rel table)
			//	remove all entries from the rel table that are no longer valid
		}

	}
}
