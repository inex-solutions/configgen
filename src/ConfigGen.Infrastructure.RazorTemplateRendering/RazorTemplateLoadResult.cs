namespace ConfigGen.Infrastructure.RazorTemplateRendering
{
    public sealed class RazorTemplateLoadResult
    {
        public RazorTemplateLoadResult(LoadResultStatus status, string[] errors = null)
        {
            Status = status;
            Errors = errors ?? new string[0];
        }

        public LoadResultStatus Status { get; }

        public string[] Errors { get; }

        public enum LoadResultStatus
        {
            Unknown,
            Success,
            CodeGenerationFailed,
            CodeCompilationFailed
        }
    }
}