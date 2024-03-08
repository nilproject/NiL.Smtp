using System;
using System.IO;

namespace NiL.Smtp.PipelineSteps;

public sealed class Ehlo : SmtpPipelineStep
{
    public string Host => Argument;

    public Ehlo(string host) : base("EHLO", host)
    {
    }

    public override void RunPipelineItem(Func<int> waitData, Stream stream)
    {
        SendString(stream, Command + " " + Host);

        if (_then != null)
            ProcessResponse(waitData, stream);
    }
}
