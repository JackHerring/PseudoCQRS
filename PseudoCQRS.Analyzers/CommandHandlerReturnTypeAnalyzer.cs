using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using DiagnosticDescriptor = Microsoft.CodeAnalysis.DiagnosticDescriptor;
using DiagnosticSeverity = Microsoft.CodeAnalysis.DiagnosticSeverity;
using LanguageNames = Microsoft.CodeAnalysis.LanguageNames;
using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

namespace PseudoCQRS.Analyzers
{ 
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class CommandHandlerReturnTypeAnalyzer : DiagnosticAnalyzer
	{

		private const string DiagnosticId = "PC1000";
		private const string Title = "Command handlers must only return the type specified";
		private const string MessageFormat = "Command handlers must only return the type specified";
		private const string Description = "Command handlers must only return the type specified";

		private const string ErrorMessage = "Command handlers must only return the type specified";
		public static readonly DiagnosticDescriptor Descriptor =
			new DiagnosticDescriptor(DiagnosticId,
				Title,
				MessageFormat,
				"Types",
				DiagnosticSeverity.Error,
				true,
				Description);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.ReturnStatement);
		}

		private void AnalyzeNode(SyntaxNodeAnalysisContext context)
		{
			var method = GetParentNode<MethodDeclarationSyntax>( context.Node );
			var symbol = context.SemanticModel.GetDeclaredSymbol( method );
			if (!(symbol.Name == "Handle" || symbol.Name == "HandleAsync"))
				return;
			var @class = GetParentNode<ClassDeclarationSyntax>(method);

			if (!ImplementsOneOf(@class, context, "PseudoCQRS.ICommandHandler", "PseudoCQRS.IAsyncCommandHandler"))
				return;

			var expression = ((ReturnStatementSyntax)context.Node).Expression;


			var expressionType = context.SemanticModel.GetTypeInfo(expression).Type;
			var returnType = context.SemanticModel.GetTypeInfo(method.ReturnType).Type;

			if (returnType.ToString().StartsWith("System.Threading.Tasks.Task<"))
			{
				returnType = ((INamedTypeSymbol)returnType).TypeArguments[0];
				if (method.Modifiers.All(x => x.Text != "async"))
					expressionType = ((INamedTypeSymbol)expressionType).TypeArguments[0];
			}

			if (returnType != expressionType)
			{
				var diagnostic =
					Diagnostic.Create(Descriptor, expression.GetLocation(), ErrorMessage);
				context.ReportDiagnostic(diagnostic);
			}
		}

		private bool ImplementsOneOf(ClassDeclarationSyntax @class, SyntaxNodeAnalysisContext context, params string[] interfaceNames)
		{
			if (@class.BaseList == null)
				return false;

			return @class.BaseList.Types
				.OfType<SimpleBaseTypeSyntax>()
				.Select(t => t.Type).OfType<GenericNameSyntax>()
				.Any(gns => interfaceNames.Any(x => IsOfType(gns, x)));

			bool IsOfType(GenericNameSyntax genericNameSyntax, string @interface)
			{
				var typeInfo = context.SemanticModel.GetTypeInfo(genericNameSyntax);
				var typeName = typeInfo.Type.ToString().Split('<')[0];
				return typeName == @interface;
			}
		}

		private T GetParentNode<T>(SyntaxNode node) where T : SyntaxNode
		{
			if (node is T parent)
				return parent;
			return GetParentNode<T>(node.Parent);
		}
	}
}
