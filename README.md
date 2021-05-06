# Tsifter

## Types
* `Problem`: immutable sets of variables and constraints
	* bool IsSatisfiedBy(Assignment a)
* `Assignment`: assignment of values to the variables of a problem. considered partial if not all variables of the problem are assigned.
	* wrapper for an ImmutableDictionary<IVariable, object>
	* object this[IVariable variable]
	* T Read<T>(IVariable<T> variable)
	* Assignment Rebuild(Func<Dict<IVariable, object>, Dict<IVariable, object>> map)
* `Solution`: immutable structure with a `Problem` and an `Assignment` that satisfies its constraints.
	* Problem
	* Assignment
	* constructor fails if assignment is partial or doesnt satisfy the problem.
* `ProblemBuilder`: Configures and assembles a `Problem`.
	* public HashSet<IVariable> Variables
	* public HashSet<IConstraint> Constraints
	* IVariable<T> AddVariable<T>(IEnumerable<T> domain, string? name)
	* IConstraint AddConstraint(ITerm<bool> term)
	* Problem Build()
* `IVariable<T>`: has an IEnumerable<T> as representation of its domain
	* Type  ValueType { get; }
	* IEnumerable<T> Domain { get; }
* `IConstraint`: Computation on variables producing a boolean, implements ITerm<bool>
	* bool IsSatisfiedBy(Problem p, Assignment a)
* `ITerm<T>`: Computation on variables
	* List<IVariable> Scope { get; }
	* T Evaluate(Problem p, Assignment a)
* `AC3Solver`: implementation of the AC3 algorithm supporting only binary constraints (terms with a scope containing 2 variables)
	* IEnumerable<Solution> Solutions()
	* Solution? FirstOrDefault()

## Simple Example

```csharp
using csp;

class Program {
	public static void Main(string[] args) {
		var prob = new ProblemBuilder();
		var a = prob.AddVariable<int>(Enumerable.Range(1,5), "A");
		var b = prob.AddVariable<int>(Enumerable.Range(1,5), "B");

		var constraint = prob.AddConstraint(new EqConstraint<int>(a,b));
		// using linq: prob.AddConstraint(
		//		(from i in a from j in b select i != j)
		//			.SetScope(a,b)
		//  )

		var problem = prob.Build();

		var solver = new AC3Solver();

		foreach (var solution in solver.Solutions())
			Console.WriteLine(solution.Assignment.ToString());
	}
}
```
