using System;
using System.Linq;

using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace csp.tests;

public class STR2SolverTests {
	private readonly ITestOutputHelper _testOutputHelper;

	public STR2SolverTests(ITestOutputHelper testOutputHelper) {
		_testOutputHelper = testOutputHelper;
	}

	[Fact]
	public void NAryConstraint() {
		throw new XunitException("broken");
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

	[Fact]
	public void SendMoreMoney() {
		throw new XunitException("broken");
		var prob = new ProblemBuilder();
		var domain = Enumerable.Range(0, 10);

		var vars = "send more money".ToCharArray()
			.Distinct()
			.Where(x => x != ' ')
			.SelectVariableTable(c => prob.AddVariable(domain, $"'{c}'"));
		var carries = Enumerable.Range(0, 4).Select(i => prob.AddVariable<int>(Enumerable.Range(0, 10))).ToList();

		prob.AddConstraint(new AllDiffConstraint<int>(vars.Values.ToArray()).SetScope(vars.Values.ToArray()));
		prob.AddConstraint(
			(from d in vars['d']
				from e in vars['e']
				from y in vars['y']
				from c0 in carries[0]
				select d + e == (10 * c0) + y)
			.SetScope(vars['d'], vars['e'], vars['y'], carries[0])
		);
		prob.AddConstraint(
			(from n in vars['n']
				from r in vars['r']
				from e in vars['e']
				from c0 in carries[0]
				from c1 in carries[1]
				select n + r + c0 == (10 * c1) + e)
			.SetScope(vars['n'], vars['r'], vars['e'], carries[0], carries[1])
		);
		prob.AddConstraint(
			(from e in vars['e']
				from o in vars['o']
				from n in vars['n']
				from c1 in carries[1]
				from c2 in carries[2]
				select e + o + c1 == (10 * c2) + n)
			.SetScope(vars['e'], vars['o'], vars['n'], carries[1], carries[2])
		);
		prob.AddConstraint(
			(from s in vars['s']
				from m in vars['m']
				from o in vars['o']
				from c2 in carries[2]
				from c3 in carries[3]
				select s + m + c2 == (10 * c3) + o)
			.SetScope(vars['s'], vars['m'], vars['o'], carries[2], carries[3])
		);
		prob.AddConstraint(
			(from c3 in carries[3] from m in vars['m'] select c3 == m)
			.SetScope(carries[3], vars['m'])
		);

		var problem = prob.Build();

		var solver = new STR2Solver(problem);

		foreach (var solution in solver.GetSolutions()) {
			Assert.NotNull(solution);
			// TODO: verify correctness
			_testOutputHelper.WriteLine(solution.Assignment.ToString());
		}
	}
}
