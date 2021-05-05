using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace csp {
	public class DelegateTerm<T> : ITerm<T> {
		internal List<IVariable> ScopeSet;
		private readonly Func<DelegateTerm<T>, Problem, Assignment, T> _reader;

		public DelegateTerm(List<IVariable> deps, Func<DelegateTerm<T>, Problem, Assignment, T> reader) {
			ScopeSet = deps;
			_reader = reader;
		}

		public List<IVariable> Scope => ScopeSet;

		public T Evaluate(Problem p, Assignment a) {
			return _reader(this, p, a);
		}
	}

	public static class Term {
		public static ITerm<T> Return<T>(T val) {
			return new DelegateTerm<T>(
				new List<IVariable>(0),
				(t, p, a) => val
			);
		}

		public static ITerm<T[]> All<T>(params ITerm<T>[] terms) {
			if (terms.Length == 0)
				return Term.Return(Array.Empty<T>());

			var first = from x in terms[0] select new List<T> { x };

			static List<T> AddTo<T>(List<T> list, T elem) {
				list.Add(elem);
				return list;
			}

			for (var i = 1; i < terms.Length; i++)
				first = from agg in first
						from tail in terms[i]
						select AddTo(agg, tail);

			return from lst in first select lst.ToArray();
		}
	}
}
