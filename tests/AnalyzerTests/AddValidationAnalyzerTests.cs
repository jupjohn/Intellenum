﻿using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using SmallTests.AnalyzerTests;
using VerifyCS = AnalyzerTests.Verifiers.CSharpCodeFixVerifier<Intellenum.Rules.AddValidationAnalyzer, Intellenum.Rules.AddValidationAnalyzerCodeFixProvider>;

namespace AnalyzerTests
{
    public class AddValidationAnalyzerTests
    {
        //No diagnostics expected to show up
        [Fact]
        public async Task NoDiagnosticsForEmptyCode()
        {
            var test = @"";
            await VerifyCS.VerifyAnalyzerAsync(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [Fact]
        public async Task CodeFixTriggeredForVoWithNoValidateMethod()
        {
            var input = LineEndingsHelper.Normalize(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Vogen;

namespace ConsoleApplication1
{
    [ValueObject(typeof(int))]
    public partial class {|#0:TypeName|}
    {   
    }
}");

            var expectedOutput = LineEndingsHelper.Normalize(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Vogen;

namespace ConsoleApplication1
{
    [ValueObject(typeof(int))]
    public partial class TypeName
    {
        private static Validation Validate(int input)
        {
            bool isValid = true; // todo: your validation
            return isValid ? Validation.Ok : Validation.Invalid(""[todo: describe the validation]"");
        }
    }
}");

            var expectedDiagnostic =
                VerifyCS.Diagnostic("AddValidationMethod").WithSeverity(DiagnosticSeverity.Info).WithLocation(0).WithArguments("TypeName");

            var test = new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { input }
                },

                CompilerDiagnostics = CompilerDiagnostics.Suggestions,
                ReferenceAssemblies = References.Net70AndOurs.Value,
                FixedCode = expectedOutput,
                ExpectedDiagnostics = { expectedDiagnostic },
            };

            test.DisabledDiagnostics.Add("CS1591");

            await test.RunAsync();
        }

        //Diagnostic and CodeFix both triggered and checked for
        [SkippableFact]
        public async Task Generic_CodeFixTriggeredForVoWithNoValidateMethod()
        {
#if !NET7_0_OR_GREATER
            Skip.If(true);
#endif

            var input = LineEndingsHelper.Normalize(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Vogen;

namespace ConsoleApplication1
{
    [ValueObject<int>]
    public partial class {|#0:TypeName|}
    {   
    }
}");

            var expectedOutput = LineEndingsHelper.Normalize(@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Vogen;

namespace ConsoleApplication1
{
    [ValueObject<int>]
    public partial class TypeName
    {
        private static Validation Validate(int input)
        {
            bool isValid = true; // todo: your validation
            return isValid ? Validation.Ok : Validation.Invalid(""[todo: describe the validation]"");
        }
    }
}");

            var expectedDiagnostic =
                VerifyCS.Diagnostic("AddValidationMethod").WithSeverity(DiagnosticSeverity.Info).WithLocation(0).WithArguments("TypeName");

            var test = new VerifyCS.Test
            {
                TestState =
                {
                    Sources = { input },
                    ReferenceAssemblies = References.Net70AndOurs.Value
                },

                CompilerDiagnostics = CompilerDiagnostics.Suggestions,
                ReferenceAssemblies = References.Net70AndOurs.Value,
                FixedCode = expectedOutput,
                ExpectedDiagnostics = { expectedDiagnostic },
            };

            test.DisabledDiagnostics.Add("CS1591");

            await test.RunAsync();
        }
    }
}
