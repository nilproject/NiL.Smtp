using System;
using System.IO;

namespace NiL.Smtp.PipelineSteps;

public sealed class StartTls : SmtpPipelineStep
{
    public StartTls() : base("STARTTLS", string.Empty)
    {
    }

    public override void RunPipelineItem(Func<int> waitData, Stream stream)
    {
        SendString(stream, Command);

        if (_then != null)
            ProcessResponse(waitData, stream);
    }
}
