using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NiL.Smtp.PipelineSteps;

public abstract class SmtpPipelineStep : ISmtpPipelineStep
{
    protected List<KeyValuePair<string, ISmtpPipelineStep>>? _then;

    public string Command { get; }
    public string Argument { get; }

    protected SmtpPipelineStep(string command, string argument)
    {
        Command = command;
        Argument = argument;
    }

    public virtual ISmtpPipelineStep Then(string responsePrefix, ISmtpPipelineStep then)
    {
        if (then == this)
            throw new InvalidOperationException("Recursion");

        _then ??= new List<KeyValuePair<string, ISmtpPipelineStep>>();
        _then.Add(new KeyValuePair<string, ISmtpPipelineStep>(responsePrefix, then));
        return new SmtpPipeline(this, then);
    }

    public ISmtpPipelineStep Then(ISmtpPipelineStep then) => Then(string.Empty, then);

    public virtual void RunPipelineItem(Func<int> waitData, Stream stream)
    {
        if (!string.IsNullOrWhiteSpace(Argument))
            SendString(stream, Command + " " + Argument);
        else
            SendString(stream, Command);

        if (_then != null)
            ProcessResponse(waitData, stream);
    }

    protected void ProcessResponse(Func<int> waitData, Stream stream)
    {
        if (_then == null)
            return;

        string response = GetResponse(waitData, stream);

        RunThen(waitData, stream, response);
    }

    protected void RunThen(Func<int> waitData, Stream stream, string response)
    {
        if (_then is null)
            return;

        var lines = response.Split('\n');
        var thenToStart = new List<ISmtpPipelineStep>();
        foreach (var then in _then)
        {
            foreach (var line in lines)
            {
                if (line.StartsWith(then.Key, StringComparison.OrdinalIgnoreCase))
                {
                    if (!thenToStart.Contains(then.Value))
                        thenToStart.Add(then.Value);
                }
            }
        }

        foreach (var then in thenToStart)
        {
            then.RunPipelineItem(waitData, stream);
        }
    }

    protected static unsafe string GetResponse(Func<int> waitData, Stream stream)
    {
        var dataSize = waitData();

        var bufSize = Math.Min(dataSize, 1024);

        var response = string.Empty;

        if (bufSize < dataSize)
        {
            var buf = new byte[dataSize];
            var responseBuf = new StringBuilder(bufSize);

            stream.Read(buf);
            response = Encoding.UTF8.GetString(buf);
        }
        else
        {
            byte* buf = stackalloc byte[bufSize];
            stream.Read(new Span<byte>(buf, dataSize));
            response = Encoding.UTF8.GetString(buf, dataSize);
        }

#if DEBUG
        Console.WriteLine("<");
        Console.WriteLine(response);
#endif

        return response;
    }

    protected static unsafe void SendString(Stream stream, string s)
    {
        var bufSize = s.Length * 2 + 2;
        byte* buf = stackalloc byte[bufSize];
        fixed (char* chars = s)
        {
            var bytes = Encoding.UTF8.GetBytes(chars, s.Length, buf, bufSize);
            buf[bytes++] = (byte)'\r';
            buf[bytes++] = (byte)'\n';
            stream.Write(new ReadOnlySpan<byte>(buf, bytes));
        }

#if DEBUG
        Console.WriteLine(">");
        Console.WriteLine(s);
#endif
    }
}
