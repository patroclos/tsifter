using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace csp; 

public sealed class STR2Solver {
	private Problem _problem;

	private Dictionary<IVariable, List<object>> _domains;
	private Dictionary<IConstraint, IEnumerable<IDictionary<IVariable, object>>> _rel = new();

	public STR2Solver(Problem problem) {
		_problem = problem;
		_domains = problem.Variables.ToDictionary(v => v, v => v.Domain.ToList());
	}

	private void BuildRelationTable(IConstraint c) {
		var table = new[] { ImmutableDictionary<IVariable, object>.Empty } as IEnumerable<ImmutableDictionary<IVariable, object>>;

		foreach (var v in c.Scope)
			table = from dict in table
				from i in v.Domain
				select dict.Add(v, i);

		table = table.Where(dict => c.IsSatisfiedBy(_problem, new Assignment(dict)));

		_rel.Add(c, table);
	}

	// prune values from variable domains that violate some constraint
	// TODO: return a result?
	private void PruneUnsupportedDomainValues() {
		foreach (var c in _problem.Constraints) {
			Console.WriteLine($"Pruning constraint {c}");
			foreach (var v in c.Scope.ToArray()) {
				var domain = _domains[v];
				for (var i = domain.Count - 1; i >= 0; i--) {
					var val = domain[i];
					if (!HasSupport(c, v, val)) {
						domain.RemoveAt(i);
						Console.WriteLine(
							$"Removing {val} from the domain of {v}, because it has no support in {c}.");
					}
					else
						Console.WriteLine($"{val} from domain of {v} is supported!");
				}
			}
		}
	}

	private bool HasSupport(IConstraint c, IVariable v, object val)
		=> _rel[c].Any(tab => tab[v].Equals(val));

	private void PruneRelTables() {
		foreach (var kv in _rel.ToArray()) {
			var supports = kv.Value;

			supports = supports.Where(x => x.All(y => _domains[y.Key].Contains(y.Value)));
			_rel[kv.Key] = supports;
		}
	}

	public IEnumerable<Solution> GetSolutions() {
		foreach (var c in _problem.Constraints)
			BuildRelationTable(c);

		foreach (var rep in Enumerable.Range(1, 20)) {
			Console.WriteLine($"PASS {rep:00}");
			PruneUnsupportedDomainValues();
			PruneRelTables();
		}

		Console.WriteLine($"Bruteforcing with {_domains.Values.Select(d=>d.Count()).Aggregate(1, (a,b)=>a*b)}");

		return Bruteforce();
		// build a relations table for the constraint scopes of the problem
		// ensure arc consistency (for every variable of the scope of every constraint, remove all values from the domain that have no support in the rel table)
		//	remove all entries from the rel table that are no longer valid
	}

	private IEnumerable<Solution> Bruteforce() {
		var combos = new[] { ImmutableDictionary<IVariable, object>.Empty } as IEnumerable<ImmutableDictionary<IVariable, object>>;
		foreach (var v in _problem.Variables)
			combos = from dict in combos from val in _domains[v] select dict.Add(v, val);

		foreach (var assign in combos.Select(c => new Assignment(c)).Where(_problem.IsSatisfiedBy))
			yield return new Solution(_problem, assign);
	}

}
