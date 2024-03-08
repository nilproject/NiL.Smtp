using System;
using System.IO;

namespace NiL.Smtp.PipelineSteps;

public sealed class SendMailData : SmtpPipelineStep
{
    public SendMailData(MailData data) : base(string.Empty, data.ToString())
    {
    }

    public override void RunPipelineItem(Func<int> waitData, Stream stream)
    {
        SendString(stream, Argument + "\r\n.");

        if (_then != null)
            ProcessResponse(waitData, stream);
    }
}
