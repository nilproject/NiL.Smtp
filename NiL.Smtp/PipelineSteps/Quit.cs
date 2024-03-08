namespace NiL.Smtp.PipelineSteps;

public sealed class Quit : SmtpPipelineStep
{
    public Quit() : base("QUIT", string.Empty)
    {
    }
}
