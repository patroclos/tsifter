using System.Collections.Generic;

namespace csp {
	public interface ITerm {
		List<IVariable> Scope { get; }

		object Evaluate(Problem p, Assignment a);
	}
}
namespace csp {
	public interface ITerm<T> : ITerm {
		new T Evaluate(Problem p, Assignment a);

		object ITerm.Evaluate(Problem p, Assignment a) => (object)Evaluate(p, a);
	}
}

