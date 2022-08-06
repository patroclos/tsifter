namespace csp; 

public class EqConstraint<T> : DelegateTerm<bool>, IConstraint {
	public readonly ITerm<T> A, B;

	public EqConstraint(ITerm<T> a, ITerm<T> b) : base(TermExtensions.Scope(a, b),
		(_, p, ass) => (from x in a from y in b select x.Equals(y)).Evaluate(p, ass)) {
		A = a;
		B = b;
	}

	public bool IsSatisfiedBy(Problem p, Assignment a) => Evaluate(p, a);
}
