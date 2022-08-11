using System;
using System.Collections.Generic;
using System.Linq;

namespace csp;

using C = IConstraint;

// see gacschema papaer
public class GacSolver {
	private readonly Problem _problem;
	private readonly Stack<GacProblem> _q = new();

	public GacSolver(Problem problem) {
		_problem = problem;
	}

	public IEnumerable<Solution> Solutions() {
		var supports = new Dictionary<(IVariable, object), IEnumerator<object>>();
		_q.Push(new GacProblem(_problem));

		while (_q.Any()) {
			var p = _q.Pop()!;

			EnsureArcConsistency(p);


			// all domains have single values?
			if (p.Domains.All(d => d.Value.Count == 1)) {
				var assign = new Assignment(p.Domains.ToDictionary(kv => kv.Key, kv => kv.Value.First()));
				yield return new Solution(_problem, assign);
				continue;
			}

			// split variable domain to provoce further acr-inconsistencies
			var pivotVar = LowestMulivalueDomain(p);
			if (pivotVar == null) {
				continue;
			}

			var (a, b) = SplitOn(p, pivotVar);
			_q.Push(a);
			_q.Push(b);
		}
	}

	private void EnsureArcConsistency(GacProblem problem) {
		// TODO: find initial supports
		var todo = (from c in _problem.Constraints
				from v in c.Scope
				select problem.G.Arcs.Where(a => a.From.IsVariable()).First(
					a => a.From.Variable == v && a.To.Constraint == c
				)
			).ToList();

		while (todo.Any()) {
			Console.WriteLine($"EAC {todo.Count}");
			var space = problem.Domains.Select(d => d.Value.Count).Aggregate(1L, (x, y) => x * y);
			Console.WriteLine($"GacSolver: space:{space} q:{_q.Count}");
			
			var arc = todo[0];
			todo.RemoveAt(0);

			var var = arc.From.Variable!;
			var con = arc.To.Constraint!;
			var other = problem
				.G.Arcs.FindAll(o => {
					if (!o.From.IsConstraint())
						return false;
					if (o.From.Constraint != con)
						return false;
					return o.To.Variable != var;
				})
				.Select(a => a.To)
				.ToArray();
			

			// check all entries in problem.Domains[var] have at least one valid counterpart in other domains.  if not remove the value from var's domain and add dependent arcs to the todolist
			foreach (var val in problem.Domains[var]) {
				IEnumerable<(IVariable var, object val)[]> combos = new[] { Array.Empty<(IVariable, object)>() };
				foreach (var var2 in other.Where(o=>o.Variable != var)) {
					combos = combos.SelectMany(combo =>
						problem.Domains[var2.Variable!].Select(v =>
							combo.Append((var2.Variable!, v)).ToArray()));
				}

				combos = combos.Select(combo => combo.Append((var, val)).ToArray());
				Assignment? support = null;

				foreach (var combo in combos) {
					var assign = new Assignment(combo.GroupBy(x=>x.var).ToDictionary(kv => kv.First().var, kv => kv.First().val));
					if (!con.IsSatisfiedBy(_problem, assign)) {
						continue;
					}

					support = assign;
					break;
					// write down support information (assignment, variables
					// break from outer for loop, this domain is done
				}

				if (support != null) {
					continue;
				}

				todo.AddRange(DeleteValue(problem, con, var, val));
				// no support found for val, remove from domain and add dependent arcs to todo
			}
		}
	}

	private Arc[] DeleteValue(GacProblem problem, IConstraint constraint, IVariable variable, object value) {
		problem.Domains[variable].Remove(value);
		return problem.G.Arcs
			.Where(a => a.From.IsVariable() && a.From.Variable != variable && a.To.Constraint == constraint).ToArray();
	}

	// get the variable with the lowest amount of domain-values. null if no variable has more than one value
	private IVariable? LowestMulivalueDomain(GacProblem problem) {
		return problem.Domains
			.Where(kv => kv.Value.Count > 1)
			.OrderBy(kv => kv.Value.Count)
			.Select(kv => kv.Key)
			.FirstOrDefault();
	}

	private (GacProblem, GacProblem) SplitOn(GacProblem problem, IVariable on) {
		var domain = problem.Domains[on];
		var split = Enumerable.Range(0, 2).Select(i => domain.Where((_, j) => j % 2 == i)).ToArray();
		var problemSplit = Enumerable.Range(0, 2)
			.Select(i =>
				new GacProblem(
					problem.Domains.ToDictionary(kv => kv.Key, kv => kv.Key == on ? split[i].ToHashSet() : kv.Value),
					problem.G
				)
			)
			.ToArray();

		return (problemSplit[0], problemSplit[1]);
	}

	private class GacProblem {
		public readonly Dictionary<IVariable, HashSet<object>> Domains;
		public readonly Graph G;

		public GacProblem(Problem p) {
			Domains = p.Variables.ToDictionary(v => v, v => v.Domain.ToHashSet());
			G = new Graph();
			G.Nodes = p.Constraints.Select(c => new Node { Constraint = c })
				.Concat(p.Variables.Select(v => new Node { Variable = v }))
				.ToArray();

			G.Arcs = new List<Arc>();
			foreach (var c in p.Constraints) {
				var nodeC = G.Nodes.First(n => n.Constraint == c);
				foreach (var v in c.Scope) {
					var nodeV = G.Nodes.First(n => n.Variable == v);

					G.Arcs.Add(new Arc { From = nodeC, To = nodeV });
					G.Arcs.Add(new Arc { From = nodeV, To = nodeC });
				}
			}
		}

		public GacProblem(Dictionary<IVariable, HashSet<object>> domains, Graph g) {
			Domains = domains;
			G = g;
		}
	}

	private struct Graph {
		public Node[] Nodes { get; set; }
		public List<Arc> Arcs { get; set; }
	}

	private struct Arc {
		public Node From { get; set; }
		public Node To { get; set; }
	}

	private class Node {
		public IVariable? Variable { get; set; }
		public C? Constraint { get; set; }

		public bool IsVariable() => Constraint == null;
		public bool IsConstraint() => Constraint != null;
	}
}
