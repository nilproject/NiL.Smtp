using System;
using System.IO;

namespace NiL.Smtp.PipelineSteps;

public interface ISmtpPipelineStep
{
    void RunPipelineItem(Func<int> waitData, Stream stream);

    ISmtpPipelineStep Then(string responsePrefix, ISmtpPipelineStep then);
    ISmtpPipelineStep Then(ISmtpPipelineStep then);
}