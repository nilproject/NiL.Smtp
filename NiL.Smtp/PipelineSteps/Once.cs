using System;
using System.IO;

namespace NiL.Smtp.PipelineSteps;

public sealed class Once : SmtpPipelineStep
{
    public bool Runned { get; set; }

    public Once() : base(string.Empty, string.Empty)
    {
    }

    public override void RunPipelineItem(Func<int> waitData, Stream stream)
    {
        if (Runned)
            return;

        Runned = true;

        if (_then != null)
            RunThen(waitData, stream, string.Empty);
    }
}
