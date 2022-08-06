using System;
using System.Linq;

using Xunit;

namespace csp.tests; 

public class STR2SolverTests {
	[Fact]
	public void NAryConstraint() {
		var prob = new ProblemBuilder();
		var A = prob.AddVariable(Enumerable.Range(1, 10), "A");
		var B = prob.AddVariable(Enumerable.Range(1, 10), "B");
		var C = prob.AddVariable(Enumerable.Range(1, 10), "C");

		var con = prob.AddConstraint(
			(from a in A from b in B from c in C select a - b == c && c == b)
			.SetScope(A, B, C)
		);

		var solver = new STR2Solver(prob.Build());

		foreach (var solution in solver.GetSolutions()) {
			Console.WriteLine(solution.Assignment);
		}
	}
}