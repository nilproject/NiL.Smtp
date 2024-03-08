namespace NiL.Smtp.PipelineSteps;

public sealed class Data : SmtpPipelineStep
{
    public Data() : base("DATA", string.Empty)
    {
    }
}
