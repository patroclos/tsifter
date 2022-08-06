using System;

using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace csp.tests; 

public class UsageTests {

	/*
	[Fact]
	public void BinarizationExists() {
		var prob = new ProblemBuilder();
		var problem = prob.Build();
		var bin = problem.ToBinaryProblem();
	}
	*/

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
		Assert.Equal(2, solutions[0].Values[a]);
		Assert.Equal(2, solutions[0].Values[b]);
		Assert.Equal(4, solutions[1].Values[a]);
		Assert.Equal(4, solutions[1].Values[b]);
	}

	[Fact]
	public void SendMoreMoney() {
		var prob = new ProblemBuilder();
		var domain = Enumerable.Range(0, 10);

		var vars = "send more money".ToCharArray()
			.Distinct()
			.Where(x => x != ' ')
			.SelectVariableTable(c => prob.AddVariable<int>(domain, $"'{c}'"));
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
			Console.WriteLine(solution.Assignment);
		}
	}

	public enum Color {
		Red,
		Green,
		Blue
	}

	[Fact]
	public void Australia() {
		var prob = new ProblemBuilder();
		var west = prob.AddVariable(Enum.GetValues<Color>(), "Western Australia");
		var north = prob.AddVariable(Enum.GetValues<Color>(), "Northern Territory");
		var south = prob.AddVariable(Enum.GetValues<Color>(), "South Australia");
		var queensland = prob.AddVariable(Enum.GetValues<Color>(), "Queensland");
		var newSouth = prob.AddVariable(Enum.GetValues<Color>(), "New South Wales");
		var victoria = prob.AddVariable(Enum.GetValues<Color>(), "Victoria");
		var tansania = prob.AddVariable(Enum.GetValues<Color>(), "Tansania");

		var neighbours = new[]{
			(west, north),
			(west, south),
			(north, south),
			(north, queensland),
			(south, queensland),
			(south, newSouth),
			(south, victoria),
			(queensland, newSouth),
			(newSouth, victoria)
		};

		foreach (var pair in neighbours) {
			// prob.AddConstraint((from a in pair.Item1 from b in pair.Item2 select a != b).Scope);
			var x = from a in pair.Item1 from b in pair.Item2 select a != b;
			x.Scope.Clear();
			x.Scope.Add(pair.Item1);
			x.Scope.Add(pair.Item2);
			prob.AddConstraint(x);
		}

		var problem = prob.Build();

		var solver = new AC3Solver(problem);
		var solution = solver.Solutions().ToArray();

		Console.WriteLine(string.Join("\n", solution.Select(x => x.Assignment.ToString())));
	}

	[Fact]
	public void CompareToCspNet() {
		var prob = new ProblemBuilder();
		var vars = Enumerable.Range(1, 8).Select(i => prob.AddVariable<int>(Enumerable.Range(1, 8), $"Var {i}")).ToArray();
		prob.AddConstraint(new AllDiffConstraint<int>(vars));
	}
}