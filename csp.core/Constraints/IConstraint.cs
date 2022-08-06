namespace csp {
	public interface IConstraint : ITerm<bool> {
		bool IsSatisfiedBy(Problem p, Assignment a);
	}
}
