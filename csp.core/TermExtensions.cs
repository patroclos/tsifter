using System;
using System.Collections.Generic;
using System.Linq;

namespace csp {
	public static class TermExtensions {
		public static ITerm<O> Select<A, O>(this ITerm<A> a, Func<A, O> map)
		=> new DelegateTerm<O>(
				Scope(a),
				(_, p, ass) => map(a.Evaluate(p, ass))
			);

		public static ITerm<TRes> SelectMany<TSource, TRes>(this ITerm<TSource> src, Func<TSource, ITerm<TRes>> bind) {
			return new DelegateTerm<TRes>(
					Scope(src),
					(self, p, ass) => {
						var sourceVal = src.Evaluate(p, ass);
						var term = bind(sourceVal);

						foreach (var v in term.Scope)
							self.Scope.Add(v);

						return term.Evaluate(p, ass);
					}
					);
		}

		public static ITerm<TRes> SelectMany<TSource, TColl, TRes>(
			this ITerm<TSource> source,
			Func<TSource, ITerm<TColl>> bind,
			Func<TSource, TColl, TRes> project
		) {
			return new DelegateTerm<TRes>(
			  Scope(source),
			  (self, p, ass) => {
				  var sourceVal = source.Evaluate(p, ass);
				  var term = bind(sourceVal);
				  foreach (var v in term.Scope)
					  self.ScopeSet.Add(v);

				  return project(sourceVal, term.Evaluate(p, ass));
			  }
		  );
		}

		public static List<IVariable> Scope(params ITerm[] terms)
			=> terms.SelectMany(t => t.Scope)
			.Distinct()
			.ToList();

		public static ITerm<bool> Gt<T>(this ITerm<T> a, T val) where T : IComparable<T>
			=> from A in a select A.CompareTo(val) > 0;

		public static ITerm<bool> Eq<T>(this ITerm<T> a, ITerm<T> b) where T : IEquatable<T>
			=> from A in a from B in b select (A as IEquatable<T>).Equals(B);
	}
}

