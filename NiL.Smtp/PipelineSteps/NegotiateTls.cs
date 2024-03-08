using System;
using System.IO;
using System.Net.Security;

namespace NiL.Smtp.PipelineSteps;

public sealed class NegotiateTls : SmtpPipelineStep
{
    public NegotiateTls(string host) : base(string.Empty, host)
    {
    }

    public override void RunPipelineItem(Func<int> waitData, Stream stream)
    {
        var sslStream = new SslStream(stream);

        var options = new SslClientAuthenticationOptions
        {
            TargetHost = Argument,
        };

        sslStream.AuthenticateAsClient(options);


        RunThen(waitData, sslStream, string.Empty);
    }
}
