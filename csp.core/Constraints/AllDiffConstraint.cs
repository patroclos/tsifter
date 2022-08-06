using System.Linq;

namespace csp; 

public class AllDiffConstraint<T> : TermConstraint {
	public AllDiffConstraint(ITerm<T>[] terms)
		: base(Term.All(terms).Select(list => list.Distinct().Count() != list.Length))
	{
	}
}
