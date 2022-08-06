using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;
using Xunit.Abstractions;

namespace csp.tests;

public class UsageTests {
	private readonly ITestOutputHelper _testOutputHelper;

	public UsageTests(ITestOutputHelper testOutputHelper) {
		_testOutputHelper = testOutputHelper;
	}

	[Fact]
	public void UsecaseAB() {
		var prob = new ProblemBuilder();
		var a = prob.AddVariable(Enumerable.Range(1, 5), "a");
		var b = prob.AddVariable(Enumerable.Range(-5, 10).Where(x => x % 2 == 0), "b");

		var aEqBConstraint = prob.AddConstraint(a.Eq(b));

		var problem = prob.Build();

		Assert.Equal(2, problem.Variables.Count);
		Assert.Single(problem.Constraints);

		var assignment = new Assignment(new Dictionary<IVariable, object> {
			{ a, 1 },
			{ b, -5 }
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
		var x = prob.AddVariable(Enumerable.Range(0, 10), "x");
		var name = prob.AddVariable(new[] { "Hans", "Erik", "Jan" }, "name");

		var nameLenEqX = prob.AddConstraint(
			from _x in x
			from _name in name
			select _name.Length == _x
		);

		var problem = prob.Build();

		var ass = new Assignment(new Dictionary<IVariable, object> {
			{ x, 4 },
			{ name, "Hans" }
		});

		Assert.True(problem.IsSatisfiedBy(ass));
	}

	[Fact]
	public void SolverBasic() {
		var prob = new ProblemBuilder();
		var a = prob.AddVariable(Enumerable.Range(1, 4), "a");
		var b = prob.AddVariable(Enumerable.Range(-5, 1000).Where(x => x % 2 == 0), "b");

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


	private enum Color {
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
		var _ = prob.AddVariable(Enum.GetValues<Color>(), "Tansania");

		var neighbours = new[] {
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
			var x = from a in pair.Item1 from b in pair.Item2 select a != b;
			x.Scope.Clear();
			x.Scope.Add(pair.Item1);
			x.Scope.Add(pair.Item2);
			prob.AddConstraint(x);
		}

		var problem = prob.Build();

		var solver = new AC3Solver(problem);
		var solution = solver.Solutions().ToArray();

		_testOutputHelper.WriteLine(string.Join("\n", solution.Select(x => x.Assignment.ToString())));
	}
}
