using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;

namespace PseudoCQRS.Analyzers.Tests
{
	public class AnalyzerTests : BaseDiagnosticVerifier
	{
		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() => new CommandHandlerReturnTypeAnalyzer();

		private static string GetTypeName(Type type) => type.Name.Split( '`' )[ 0 ];

		private readonly string PseudoCQRSTypes = $@"namespace PseudoCQRS
		{{
			public interface {GetTypeName(typeof(ICommandHandler<>))}<T>
			{{
				T Handle();
			}}

			public interface {GetTypeName(typeof(IAsyncCommandHandler<>))}<T>
			{{
				Task<T> HandleAsync();
			}}
		}}";

		[Fact]
		public void ForNonAsyncMethodReturningSameTypeAsDeclaredReturnType_DoesNotCauseError()
		{
			var source = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName : ICommandHandler<DateTime>
        {   
			public DateTime MyMethod(){
				return DateTime.Now;
			}
        }
    }";
			VerifyCSharpDiagnostic( source );
		}

		[Fact]
		public void ForAsyncMethodReturningSameTypeAsDeclaredReturnType_DoesNotCauseError()
		{
			var source = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName : ICommandHandlerAsync<DateTime>
        {   
			public async Task<DateTime> HandleAsync(){
				return await Task.FromResult( DateTime.Now );
			}
        }
    }";
			VerifyCSharpDiagnostic( source );
		}

		[Fact]
		public void ForAsyncMethodReturningSameTypeAsDeclaredReturnType_ButReturnsATask_DoesNotCauseError()
		{
			var source = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName : ICommandHandlerAsync<DateTime>
        {   
			public Task<DateTime> HandleAsync(){
				return Task.FromResult( DateTime.Now );
			}
        }
    }";
			VerifyCSharpDiagnostic( source );
		}


		[Fact]
		public void ForNonAsyncMethodReturningSameTypeAsDeclaredReturnType_ButReturnsObjectAssignedFromAnotherVariable_DoesNotCauseError()
		{
			var source = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName : ICommandHandler<DateTime>
        {   
			public DateTime Handle(){
				var today = DateTime.Now;
				return today;
			}
        }
    }";
			VerifyCSharpDiagnostic( source );
		}

		[Fact]
		public void ForAsyncMethodReturningSameTypeAsDeclaredReturnType_ButReturnsObjectAssignedFromAnotherVariable_DoesNotCauseError()
		{
			var source = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class SomeType : ICommandHandlerAsync<DateTime>
        {   
			public async Task<DateTime> HandleAsync()
			{
				var today =  await Task.FromResult(DateTime.Now);
				return today;
			}
        }
    }";
			VerifyCSharpDiagnostic( source );
		}

		[Fact]
		public void ForClassThatImplementsICommandHandlerAndReturnTypeDoesNotMatch_GivesAnError()
		{
			var source = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
	using PseudoCQRS;
	

    namespace ConsoleApplication1
    {
		public class SomeType : ICommandHandler<object>
        {   
			public object Handle(){
				return """";
			}
        }
    }";
			VerifyCSharpDiagnostic( new[] { source, PseudoCQRSTypes }, CreateDiagnosticResult( 16, 12, CommandHandlerReturnTypeAnalyzer.Descriptor, DiagnosticSeverity.Error ) );
		}

		[Fact]
		public void ForClassThatImplementsIAsyncCommandHandlerAndReturnTypeDoesNotMatch_GivesAnError()
		{
			var source = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
	using PseudoCQRS;
	

    namespace ConsoleApplication1
    {
		public class SomeType : IAsyncCommandHandler<object>
        {   
			public Task<object> Handle(){
				return Task.FromResult("""");
			}
        }
    }";
			VerifyCSharpDiagnostic( new[] { source, PseudoCQRSTypes }, CreateDiagnosticResult( 16, 12, CommandHandlerReturnTypeAnalyzer.Descriptor, DiagnosticSeverity.Error ) );
		}

		[Fact]
		public void ForClassWhichImplementsInterfaceButHasOtherPublicAndNonPublicMethods_DoesNotApplyRulesToThoseMethods()
		{
			var source = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;
	using PseudoCQRS;
	

    namespace ConsoleApplication1
    {
		public class SomeType : IAsyncCommandHandler<object>
        {   
			public Task<object> Handle(){
				return Task.FromResult(new object());
			}

			private object GetObjectPrivately()
			{
				return """";
			}

			public object GetObjectPublicly()
			{
				return """";
			}
        }
    }";
			VerifyCSharpDiagnostic( new[] { source, PseudoCQRSTypes } );

		}
	}
}