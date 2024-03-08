namespace NiL.Smtp.PipelineSteps;

public sealed class RcptTo : SmtpPipelineStep
{
    public RcptTo(string address)
        : base("RCPT TO:", "<" + address + ">")
    {
    }
}
