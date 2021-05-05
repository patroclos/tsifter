
namespace csp {
	public class EqConstraint<T> : DelegateTerm<bool>, IConstraint {
		public ITerm<T> A, B;

		public EqConstraint(IVariable<T> a, IVariable<T> b): base(TermExtensions.Scope(a,b), (self, p, ass) => (from x in a from y in b select x.Equals(y)).Evaluate(p,ass)) {
			A = a;
			B = b;
		}

		public bool IsSatisfiedBy(Problem p, Assignment a) => Evaluate(p, a);
	}
}

