using System.Net.Http;
using Reloop.Models;
using Reloop.Validation;
using static Reloop.Models.MailModels;

namespace Reloop.Services;

/** Sends transactional email. */
public class MailService
{
    private readonly ReloopClient _client;

    internal MailService(ReloopClient client)
    {
        _client = client;
    }

    public Task<SendMailResponse?> SendAsync(SendMailParams? parameters)
    {
        parameters ??= new SendMailParams();

        var from = Validators.RequireMailString(parameters.From, "from");
        var to = Validators.RequireRecipient(parameters.To, "to");
        var subject = Validators.RequireMailString(parameters.Subject, "subject");

        var body = new SendMailParams
        {
            From = from,
            To = to,
            Subject = subject,
            Cc = parameters.Cc,
            Bcc = parameters.Bcc,
            Text = parameters.Text,
            Html = parameters.Html,
            ReplyTo = parameters.ReplyTo,
            ScheduledAt = parameters.ScheduledAt,
            Headers = parameters.Headers,
            ChannelId = parameters.ChannelId,
            Attachments = parameters.Attachments,
            Tags = parameters.Tags,
            Template = parameters.Template,
            ThreadId = parameters.ThreadId,
        };

        return _client.FetchAsync<SendMailResponse>(HttpMethod.Post, "/api/mail/v1/send", body);
    }
}
