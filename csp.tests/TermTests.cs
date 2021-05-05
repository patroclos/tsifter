using Xunit;

namespace csp.tests {
	public class TermTests {
		[Fact]
		public void TermHasMutableScope() {
			var term = Term.Return(1);

			Assert.Empty(term.Scope);
			
			term.Scope.Add(null as IVariable);

			Assert.NotEmpty(term.Scope);
		}
	}
}
