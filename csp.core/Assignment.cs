using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace csp {
	public readonly struct Assignment {
		public readonly ImmutableDictionary<IVariable, object> Values;

		public object this[IVariable v] => Values[v];

		public Assignment(IDictionary<IVariable, object> values) {
			Values = values.ToImmutableDictionary();
		}

		public bool IsCompleteFor(Problem p) => p.Variables.All(Values.ContainsKey);
	}
}
