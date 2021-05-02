namespace csp {
	public interface IConstraint {
		bool IsSatisfiedBy(Problem p, Assignment a);
	}
}
