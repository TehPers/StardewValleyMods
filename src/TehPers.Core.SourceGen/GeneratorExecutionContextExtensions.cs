using Microsoft.CodeAnalysis;

namespace TehPers.Core.SourceGen
{
    internal static class GeneratorExecutionContextExtensions
    {
    
        public static void ReportInternalError(
            this GeneratorExecutionContext context,
            LocalizableString message,
            DiagnosticSeverity severity = DiagnosticSeverity.Error,
            Location? location = null
        )
        {
            context.ReportError(
                Constants.InternalErrorId,
                message,
                severity,
                location
            );
        }

        public static void ReportError(
            this GeneratorExecutionContext context,
            string id,
            LocalizableString message,
            DiagnosticSeverity severity = DiagnosticSeverity.Error,
            Location? location = null
        )
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    id,
                    "Compiler",
                    message,
                    severity,
                    severity,
                    true,
                    0,
                    location: location
                )
            );
        }
    }
}