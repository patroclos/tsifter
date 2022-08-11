using System;
using System.Linq;

namespace csp; 

public class DistinctConstraint<T> : TermConstraint {
	public DistinctConstraint(params ITerm<T>[] terms)
		: base(Term.All(terms).Select(list => list.Distinct().Count() == list.Length))
	{
	}
}
