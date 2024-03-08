namespace NiL.Smtp.PipelineSteps;

public sealed class Noop : SmtpPipelineStep
{
    public Noop() : base("NOOP", string.Empty)
    {
    }
}
