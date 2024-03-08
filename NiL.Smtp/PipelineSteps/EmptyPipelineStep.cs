using System;
using System.IO;

namespace NiL.Smtp.PipelineSteps;

public sealed class EmptyPipelineStep : SmtpPipelineStep
{
    public EmptyPipelineStep()
        : base(string.Empty, string.Empty)
    {
    }

    public override void RunPipelineItem(Func<int> waitData, Stream stream)
    {
        ProcessResponse(waitData, stream);
    }
}
