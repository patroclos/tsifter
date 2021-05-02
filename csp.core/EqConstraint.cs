using System.Collections.Generic;
using System.Linq;

namespace csp {
	public class EqConstraint<T> : IConstraint {
		public ITerm<T> A, B;

		public EqConstraint(IVariable<T> a, IVariable<T> b) {
			A = a;
			B = b;
		}

		public IEnumerable<IVariable> Dependencies => new []{A,B}.SelectMany(v=>v.Dependencies);

		public bool IsSatisfiedBy(Problem p, Assignment a) {
			var e = EqualityComparer<T>.Default;
			var x = A.Evaluate(p, a);
			var y = B.Evaluate(p, a);

			return x.Equals(y);
		}

		bool ITerm<bool>.Evaluate(Problem p, Assignment a) => IsSatisfiedBy(p, a);
	}
}

