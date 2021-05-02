using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace csp.tests {
	public class UsageTests {

		[Fact]
		public void UsecaseAB() {
			var prob = new ProblemBuilder();
			var a = prob.AddVariable<int>(Enumerable.Range(1, 5), "a");
			var b = prob.AddVariable<int>(Enumerable.Range(-5, 10).Where(x => x % 2 == 0), "b");

			var aEqBConstraint = prob.AddConstraint(a.Eq(b));

			var problem = prob.Build();

			Assert.Equal(2, problem.Variables.Count);
			Assert.Equal(1, problem.Constraints.Count);

			var assignment = new Assignment(new Dictionary<IVariable, object> {
					{a, 1},
					{b, -5}
			});

			Assert.True(assignment.IsCompleteFor(problem));
			Assert.False(problem.IsSatisfiedBy(assignment));

			var ab = from x in a from y in b select x + y;
			Assert.Equal(-4, ab.Evaluate(problem, assignment));

			var gt = ab.Gt(-10);
			Assert.True(gt.Evaluate(problem, assignment));
		}

		[Fact]
		public void MixedTypes() {
			var prob = new ProblemBuilder();
			var x = prob.AddVariable<int>(Enumerable.Range(0, 10), "x");
			var name = prob.AddVariable<string>(new[] { "Hans", "Erik", "Jan" }, "name");

			var nameLenEqX = prob.AddConstraint(
				from _x in x
				from _name in name
				select _name.Length == _x
			);

			var problem = prob.Build();

			var ass = new Assignment(new Dictionary<IVariable, object>{
				{x, 4},
				{name, "Hans"}
			});

			Assert.True(problem.IsSatisfiedBy(ass));
		}

		[Fact]
		public void SolverBasic() {
			var prob = new ProblemBuilder();
			var a = prob.AddVariable<int>(Enumerable.Range(1, 4), "a");
			var b = prob.AddVariable<int>(Enumerable.Range(-5, 1000).Where(x => x % 2 == 0), "b");

			var aEqBConstraint = prob.AddConstraint(new EqConstraint<int>(a, b));

			var problem = prob.Build();

			var solver = new AC3Solver(problem);
			var solutions = solver.Solutions().Select(s => s.Assignment).ToArray();

			Assert.NotNull(solutions);
			Assert.NotEmpty(solutions);
			Assert.Equal(2, solutions.Length);
		}
	}
}
