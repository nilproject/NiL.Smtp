namespace NiL.Smtp.PipelineSteps;

public sealed class MailFrom : SmtpPipelineStep
{
    public MailFrom(string address)
        : base("MAIL FROM:", "<" + address + ">")
    {
    }
}
