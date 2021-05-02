using System;
using System.Collections.Generic;

namespace csp {
	public sealed class STR2Solver {
		private Problem _problem;

		private Dictionary<IVariable, IEnumerable<object>> Domains = new();

		public STR2Solver(Problem problem) {
			_problem = problem;
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


		public void Run() {
			// initialize domains to original domain of declared variable
			// ensure arc consistency (for every variable of the scope of every constraint, remove all values from the domain that have no support in the rel table)
			//	remove all entries from the rel table that are no longer valid
		}


		// TODO: need a ArcN class that can handle arcs that entangle 1,2,3,... variables
		/*
		private class Arc {
			// public 
		}
		*/
	}
}
