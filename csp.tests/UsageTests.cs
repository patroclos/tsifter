using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace csp.tests {
	public class UsageTests {
		[Fact]
		public void UsecaseAB() {
			var prob = Problem.Build();
			var a = prob.AddVariable<int>(Enumerable.Range(1, 5), "a");
			var b = prob.AddVariable<int>(Enumerable.Range(-5, 10).Where(x => x % 2 == 0), "b");

			prob.Constraints.Add(new EqConstraint<int>(a, b));

			var problem = prob.Build();

			Assert.Equal(2, problem.Variables.Count);
			Assert.Equal(1, problem.Constraints.Count);

			var assignment = new Assignment(new Dictionary<IVariable, object> {
					{a, 1},
					{b, -5}
			});

			Assert.True(assignment.IsCompleteFor(problem));
			Assert.False(problem.IsSatisfiedBy(assignment));
		}
	}
}
