using System;
using System.IO;

namespace NiL.Smtp.PipelineSteps;

public class SmtpPipeline : ISmtpPipelineStep
{
    public ISmtpPipelineStep EntryNode { get; protected set; }
    public ISmtpPipelineStep FinalNode { get; protected set; }

    public SmtpPipeline(ISmtpPipelineStep entryNode, ISmtpPipelineStep finalNode)
    {
        EntryNode = (entryNode as SmtpPipeline)?.EntryNode ?? entryNode;
        FinalNode = (finalNode as SmtpPipeline)?.FinalNode ?? finalNode;
    }

    public void RunPipelineItem(Func<int> waitData, Stream stream)
    {
        EntryNode.RunPipelineItem(waitData, stream);
    }

    public ISmtpPipelineStep Then(string responsePrefix, ISmtpPipelineStep then)
    {
        var f = FinalNode.Then(responsePrefix, then);
        return new SmtpPipeline(EntryNode, f);
    }

    public ISmtpPipelineStep Then(ISmtpPipelineStep then)
    {
        var f = FinalNode.Then(then);
        return new SmtpPipeline(EntryNode, f);
    }
}
