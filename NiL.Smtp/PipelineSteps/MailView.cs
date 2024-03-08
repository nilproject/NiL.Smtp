namespace NiL.Smtp.PipelineSteps;

public sealed class MailView
{
    public string ContentType { get; set; }
    public string Body { get; set; }

    public override string ToString()
    {
        return "Content-Type: " + ContentType + "\r\n\r\n"
            + string.Join("\r\n", Body.Split(new[] { "\r\n", "\n", "\r" }, System.StringSplitOptions.TrimEntries));
    }
}
