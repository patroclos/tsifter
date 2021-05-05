using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Collections.Immutable;

namespace csp {
	public sealed class AC3Solver {
		private readonly Problem _problem;
		private readonly Queue<Arc> _agenda = new();
		private readonly Stack<Arc> _dequeued = new();

		private readonly Dictionary<IVariable, List<object>> _domains = new();

		public AC3Solver(Problem problem) {
			_problem = problem;
		}

		public Solution? FirstOrDefault() => Solutions().FirstOrDefault();

		public IEnumerable<Solution> Solutions() {
			// instantiate modifiable domains
			foreach (var v in _problem.Variables)
				_domains.Add(v, v.Domain.ToList());

			// assert every constraint has 2 scope entries
			foreach (var c in _problem.Constraints)
				Debug.Assert(c.Scope.Count == 2, $"{c} is not a binary constraint");


			// create 2 arcs per constraint in both directions
			InitializeArcs();

			// ensure arc consistency
			EnsureArcConsistency();


			// there are domains without elements, abort
			if (_domains.Any(kv => kv.Value.Count == 0))
				yield break;

			var vars = _problem.Variables.ToImmutableList();

			// exhaust domains systematically and emit matches along the way
			IEnumerable<IEnumerable<KeyValuePair<IVariable, object>>> combos = new[] { Enumerable.Empty<KeyValuePair<IVariable, object>>() };
			foreach (var inner in vars) {
				var dom = _domains[inner];
				combos = combos.SelectMany(r => dom.Select(v => r.Append(new KeyValuePair<IVariable, object>(inner, v))));
			}

			foreach (var combo in combos) {
				var assign = new Assignment(combo.ToDictionary(kv => kv.Key, kv => kv.Value));
				if (_problem.IsSatisfiedBy(assign))
					yield return new Solution(_problem, assign);
			}
		}

		private void EnsureArcConsistency() {
			while (_agenda.Count != 0) {
				var arc = _agenda.Dequeue();

				var dfrom = _domains[arc.From];
				var dto = _domains[arc.To];
				var changed = false;
				for (var i = dfrom.Count - 1; i >= 0; i--) {
					if (!dto.Any(vOther => {
						var assign = new Assignment(new Dictionary<IVariable, object> { { arc.From, dfrom[i] }, { arc.To, vOther } });
						return arc.Constraint.IsSatisfiedBy(_problem, assign);
					})) {
						dfrom.RemoveAt(i);
						changed = true;
					}
				}
				// enqueue arcs of the form (?, [i]) to agenda
				if (changed) {
					QueueArcsTo(arc.From, arc.Constraint);
				}

				_dequeued.Push(arc);
			}
		}

		private void QueueArcsTo(IVariable v, IConstraint c) {
			foreach (var a in _agenda.Concat(_dequeued).Distinct().Where(x => x.To == v).ToArray()) {
				var newArc = new Arc { From = a.From, To = v, Constraint = c };
				if (!_agenda.Contains(newArc))
					_agenda.Enqueue(newArc);
			}
		}

		private void InitializeArcs() {
			foreach (var con in _problem.Constraints) {
				var a = new Arc {
					From = con.Scope[0],
					To = con.Scope[1],
					Constraint = con
				};
				var b = new Arc {
					From = con.Scope[1],
					To = con.Scope[0],
					Constraint = con
				};

				_agenda.Enqueue(a);
				_agenda.Enqueue(b);
			}
		}

		private struct Arc : IEquatable<Arc> {
			public IVariable From;
			public IVariable To;
			public IConstraint Constraint;

			public bool Equals(Arc other) {
				return From == other.From && To == other.To && Constraint == other.Constraint;
			}

			public override bool Equals(object? obj) {
				if (object.ReferenceEquals(obj, null))
					return false;

				return obj is Arc a ? Equals(a) : false;
			}

			public override int GetHashCode() => HashCode.Combine(From, To, Constraint);

			public override string ToString() {
				return base.ToString();
			}
		}
	}
}
