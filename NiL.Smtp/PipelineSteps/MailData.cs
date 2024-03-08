using System.Collections.Generic;
using System.Net.Mail;

namespace NiL.Smtp.PipelineSteps;

public sealed class MailData
{
    public MailAddress From { get; set; }
    public MailAddress To { get; set; }
    public string Subject { get; set; }
    public IList<MailView> Views { get; set; } = new List<MailView>();

    public override string ToString()
    {
        var result = "From: " + From + "\r\n"
            + "To: " + To + "\r\n"
            + "Subject: " + Subject + "\r\n";

        var views = new string[Views.Count];
        for (var i = 0; i < views.Length; i++)
        {
            views[i] = Views[i].ToString();
        }

        if (Views.Count > 1)
        {
            var boundary = "simple-boundary";
            bool found;
            do
            {
                boundary = boundary.GetHashCode().ToString();

                found = false;

                for (var i = 0; !found && i < views.Length; i++)
                    found = views[i].Contains(boundary);
            }
            while (found);

            result += "Content-Type: multipart/alternative; boundary=" + boundary;

            for (var i = 0; i < views.Length; i++)
            {
                result += "\r\n--" + boundary + "\r\n";
                result += views[i];
            }
        }
        else if (views.Length == 1)
        {
            result += views[0] + "\r\n";
        }

        return result;
    }
}