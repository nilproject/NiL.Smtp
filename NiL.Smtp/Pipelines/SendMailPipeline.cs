using NiL.Smtp.PipelineSteps;

namespace NiL.Smtp.Pipelines;

public class SendMailPipeline : SmtpPipeline
{
    public SendMailPipeline(
        string senderAddress, 
        string targetAddress,
        MailData mailData,
        string localHost,
        string remoteMxHostName) 
        : base(new EmptyPipelineStep(), new EmptyPipelineStep())
    {
        var sendPipeline = new Once()
            .Then(new MailFrom(senderAddress))
            .Then("250", new RcptTo(targetAddress))
            .Then("250", new Data())
            .Then("354", new SendMailData(mailData))
            .Then("250", new Quit())
            .Then(FinalNode);

        var pipelineRoot = EntryNode
            .Then("220", new Ehlo(localHost));

        var startTlsPipeline = new StartTls()
                                    .Then("220", new NegotiateTls(remoteMxHostName))
                                    .Then(new Ehlo(localHost))
                                    .Then("250", sendPipeline);

        pipelineRoot.Then("250 STARTTLS", startTlsPipeline);
        pipelineRoot.Then("250-STARTTLS", startTlsPipeline);

        pipelineRoot.Then("250", sendPipeline);
    }
}
