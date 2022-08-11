using System;
using System.Linq;

using Xunit;

namespace csp.tests;

public class GacTests {
	[Fact]
	public void SimpleUsecase() {
		var pb = new ProblemBuilder();
		var a = pb.AddVariable(Enumerable.Range(1, 3), "a");
		var b = pb.AddVariable(Enumerable.Range(1, 3), "b");
		var c = pb.AddVariable(Enumerable.Range(1, 3), "c");

		pb.AddConstraint(new EqConstraint<int>(a, b));
		pb.AddConstraint((from x in a from y in c select x < y).SetScope(a, c));
		pb.AddConstraint((from x in b from y in c select x <= y).SetScope(b, c));

		var problem = pb.Build();
		var solver = new GacSolver(problem);
		var solutions = solver.Solutions();
		Assert.Collection(solutions, sol => Assert.True(problem.IsSatisfiedBy(sol.Assignment)));
	}

	[Fact]
	public void NAryConstraint() {
		var pb = new ProblemBuilder();
		var a = pb.AddVariable(Enumerable.Range(1, 3), "a");
		var b = pb.AddVariable(Enumerable.Range(1, 3), "b");
		var c = pb.AddVariable(Enumerable.Range(1, 3), "c");

		pb.AddConstraint(
			(from A in a
				from B in b
				from C in c
				select A != B && A != C && B != C)
			.SetScope(a, b, c)
		);

		var problem = pb.Build();
		var solver = new GacSolver(problem);
		var solutions = solver.Solutions().ToArray();
		Assert.NotEmpty(solutions);
	}

	[Fact]
	public void SendMoreMoney() {
		var pb = new ProblemBuilder();
		var domain = Enumerable.Range(0, 10);

		var vars = "send more money".ToCharArray()
			.Distinct()
			.Where(x => x != ' ')
			.SelectVariableTable(c => pb.AddVariable(domain, $"{c.ToString().ToUpper()}"));
		var carries = Enumerable.Range(0, 4).Select(i => pb.AddVariable(Enumerable.Range(0, 10), $"carry{i}"))
			.ToArray();

		pb.AddConstraint(new DistinctConstraint<int>(vars.Values.ToArray()));
		pb.AddConstraint(
			(from d in vars['d']
				from e in vars['e']
				from y in vars['y']
				from c0 in carries[0]
				select d + e == (10 * c0) + y)
			.SetScope(vars['d'], vars['e'], vars['y'], carries[0]));
		
		pb.AddConstraint(
			(from n in vars['n']
				from r in vars['r']
				from e in vars['e']
				from c0 in carries[0]
				from c1 in carries[1]
				select n + r + c0 == (10 * c1) + e)
			.SetScope(vars['n'], vars['r'], vars['e'], carries[0], carries[1])
		);
		pb.AddConstraint(
			(from e in vars['e']
				from o in vars['o']
				from n in vars['n']
				from c1 in carries[1]
				from c2 in carries[2]
				select e + o + c1 == (10 * c2) + n)
			.SetScope(vars['e'], vars['o'], vars['n'], carries[1], carries[2])
		);
		pb.AddConstraint(
			(from s in vars['s']
				from m in vars['m']
				from o in vars['o']
				from c2 in carries[2]
				from c3 in carries[3]
				select s + m + c2 == (10 * c3) + o)
			.SetScope(vars['s'], vars['m'], vars['o'], carries[2], carries[3])
		);
		pb.AddConstraint(
			(from c3 in carries[3] from m in vars['m'] select c3 == m)
			.SetScope(carries[3], vars['m'])
		);

		var problem = pb.Build();
		var solver = new GacSolver(problem);
		var solutions = solver.Solutions().ToArray();
		
		Assert.NotEmpty(solutions);

		foreach (var solution in solver.Solutions()) {
			Assert.True(problem.IsSatisfiedBy(solution.Assignment));
			Console.WriteLine(solution.Assignment.ToString());
		}
	}
}
