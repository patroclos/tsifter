using System.Collections.Generic;

namespace csp {
	public class EqConstraint<T> : IConstraint {
		public IVariable<T> A, B;

		public EqConstraint(IVariable<T> a, IVariable<T> b) {
			A = a;
			B = b;
		}

		public bool IsSatisfiedBy(Problem p, Assignment a) {
			var e = EqualityComparer<T>.Default;
			var x = a[A];
			var y = a[B];

			return x.Equals(y);
		}
	}
}

