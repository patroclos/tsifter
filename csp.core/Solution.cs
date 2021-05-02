namespace csp {
	public class Solution {
		public readonly Problem Problem;
		public readonly Assignment Assignment;

		public Solution(Problem problem, Assignment assignment) {
			if(!assignment.IsCompleteFor(problem))
				throw new System.InvalidOperationException("Cannot construct Solution from partial Assignment");

			if(!problem.IsSatisfiedBy(assignment))
				throw new System.InvalidOperationException("Cannot construct Solution from non-satisfactory Assignment");

			this.Problem = problem;
			this.Assignment = assignment;
		}
	}
}
