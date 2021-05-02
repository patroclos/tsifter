using System;
using System.Collections.Generic;
using System.Linq;

namespace csp {
	public class DelegateTerm<T> : ITerm<T> {
		private readonly IReadOnlyCollection<IVariable> _deps;
		private readonly Func<Problem, Assignment, T> _reader;

		public DelegateTerm(IReadOnlyCollection<IVariable> deps, Func<Problem, Assignment, T> reader) {
			_deps = deps;
			_reader = reader;
		}

		public IEnumerable<IVariable> Dependencies => _deps;

		public T Evaluate(Problem p, Assignment a) {
			return _reader(p, a);
		}
	}

	public static class TermExtensions {
		public static ITerm<O> Select<A, O>(this ITerm<A> a, Func<A, O> map)
		=> new DelegateTerm<O>(
				Dependencies(a),
				(p, ass) => map(a.Evaluate(p, ass))
			);

		public static ITerm<TRes> SelectMany<TSource, TColl, TRes>(
			this ITerm<TSource> source,
			Func<TSource, ITerm<TColl>> bind,
			Func<TSource, TColl, TRes> project
		)
		=> new DelegateTerm<TRes>(
			Dependencies(source),
			(p, ass) => {
				var val = source.Evaluate(p, ass);
				return project(val, bind(val).Evaluate(p, ass));
			}
		);

		public static IVariable[] Dependencies(params ITerm[] terms)
			=> terms.SelectMany(t => t.Dependencies)
			.Distinct()
			.ToArray();

		public static ITerm<bool> Gt<T>(this ITerm<T> a, T val) where T : IComparable<T>
			=> from A in a select A.CompareTo(val) > 0;

		public static ITerm<bool> Eq<T>(this ITerm<T> a, ITerm<T> b) where T : IEquatable<T>
			=> from A in a from B in b select (A as IEquatable<T>).Equals(B);
	}
}
