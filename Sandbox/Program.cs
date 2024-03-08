using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using NiL.Dns;
using NiL.Dns.Records;
using NiL.Smtp.Pipelines;
using NiL.Smtp.PipelineSteps;

namespace Sandbox;

internal class Program
{
    static void Main(string[] args)
    {
        asyncMain(args).GetAwaiter().GetResult();
    }

    static async Task asyncMain(string[] args)
    {
        var mxResponse = await DnsClient.Query(getDnsIps(), new DnsQuery("gmail.com", DnsRecordType.MX, DnsQClass.Internet));
        var mxHost = (mxResponse.First().RData as MxRecord)!.Exchange;
        var ipResponse = await DnsClient.Query(getDnsIps(), new DnsQuery(mxHost, DnsRecordType.A, DnsQClass.Internet));
        var ip = (ipResponse.First().RData as ARecord)!.Address;

        var mail = new MailData
        {
            From = new("test@sapier.club"),
            Subject = "test",
            To = new("po.mithril@gmail.com"),
            Views =
            {
                new MailView
                {
                    ContentType = "text/plain;charset=UTF-8",
                    Body = "Hello, world!"
                }
            }
        };

        var client = new TcpClient();
        client.Connect(new IPAddress(ip), 25);
        var timeout = TimeSpan.FromSeconds(1).Ticks;

        var smtpPipeline = new SendMailPipeline("test@sapier.club", "po.mithril@gmail.com", mail, "sapier.club", mxHost);
        smtpPipeline.RunPipelineItem(() =>
        {
            var waitStart = Environment.TickCount;
            while (Environment.TickCount - waitStart < timeout
                && !client.Client.Poll(10000, SelectMode.SelectRead))
            { }

            if (client.Available == 0)
                throw new TimeoutException();

            return client.Available;
        }, client.GetStream());
    }

    private static List<IPAddress> getDnsIps()
    {
        return NetworkInterface.GetAllNetworkInterfaces()
                                    .Where(x => x.OperationalStatus == OperationalStatus.Up
                                            && !x.IsReceiveOnly
                                            && x.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                                    .Reverse()
                                    .SelectMany(x => x.GetIPProperties().DnsAddresses)
                                    .ToList();
    }
}
