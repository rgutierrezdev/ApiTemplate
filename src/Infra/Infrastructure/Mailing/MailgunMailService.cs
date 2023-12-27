using Microsoft.Extensions.Options;
using ApiTemplate.Application.Common.Extensions;
using ApiTemplate.Application.Common.Mailing;
using ApiTemplate.Application.Common.Interfaces;
using ApiTemplate.Infrastructure.Common;
using RestSharp;
using RestSharp.Authenticators;

namespace ApiTemplate.Infrastructure.Mailing;

public class MailgunMailService : IMailService
{
    private readonly MailSettings _mailSettings;
    private readonly AppSettings _appSettings;
    private readonly ISerializerService _serializer;
    private readonly RestClient _client;

    public MailgunMailService(
        IOptions<MailSettings> mailSettings,
        IOptions<AppSettings> appSettings,
        ISerializerService serializer
    )
    {
        _mailSettings = mailSettings.Value;
        _appSettings = appSettings.Value;
        _serializer = serializer;

        _client = new RestClient(
            _mailSettings.BaseUrl,
            options => { options.Authenticator = new HttpBasicAuthenticator("api", _mailSettings.Apikey); }
        );
    }

    public async Task<bool> SendAsync(SimpleMailRequest mailRequest, CancellationToken cancellationToken = default)
    {
        var request = GenerateRequest(mailRequest);

        foreach (var email in mailRequest.To)
        {
            request.AddParameter("to", email);
        }

        if (mailRequest.Cc != null)
        {
            foreach (var email in mailRequest.Cc)
            {
                request.AddParameter("cc", email);
            }
        }

        if (mailRequest.Bcc != null)
        {
            foreach (var email in mailRequest.Bcc)
            {
                request.AddParameter("bcc", email);
            }
        }

        if (mailRequest.Vars != null)
        {
            request.AddParameter("t:variables", _serializer.Serialize(mailRequest.Vars));
        }

        var res = await _client.ExecuteAsync(request, cancellationToken);

        if (res.StatusCode == System.Net.HttpStatusCode.OK)
            return true;

        throw new Exception(res.ErrorMessage, res.ErrorException);
    }

    public async Task<bool> SendBatchAsync(BatchMailRequest mailRequest, CancellationToken cancellationToken = default)
    {
        var request = GenerateRequest(mailRequest);

        request.AddParameter("recipient-variables", _serializer.Serialize(mailRequest.To));

        foreach (var recipient in mailRequest.To)
        {
            request.AddParameter("to", recipient.Key);
        }

        var vars = mailRequest.To.First().Value;

        foreach (var variable in vars)
            request.AddParameter($"v:{variable.Key}", $"%recipient.{variable.Key}%");

        var res = await _client.ExecuteAsync(request, cancellationToken);

        if (res.StatusCode == System.Net.HttpStatusCode.OK)
            return true;

        throw new Exception(res.ErrorMessage, res.ErrorException);
    }

    private RestRequest GenerateRequest(MailRequest mailRequest)
    {
        var request = new RestRequest($"{_mailSettings.Domain}/messages", Method.Post);

        request.AddParameter("template", mailRequest.Template);
        request.AddParameter("subject", mailRequest.Subject);

        var fromAddress = _mailSettings.FromAddress;
        var fromName = _mailSettings.FromName;

        if (!string.IsNullOrWhiteSpace(mailRequest.FromAddress) && !string.IsNullOrWhiteSpace(mailRequest.FromName))
        {
            fromAddress = mailRequest.FromAddress;
            fromName = mailRequest.FromName;
        }

        request.AddParameter("from", $"{fromName} <{fromAddress}>");

        if (!string.IsNullOrWhiteSpace(mailRequest.ReplyToAddress))
        {
            request.AddParameter("h:Reply-To", mailRequest.ReplyToAddress);
        }

        if (mailRequest.Tags != null)
        {
            foreach (var tag in mailRequest.Tags)
                request.AddParameter("o:tag", tag);
        }

        if (_mailSettings.TestMode)
        {
            request.AddParameter("o:testmode", "true");
        }

        if (mailRequest.Attachments != null && mailRequest.Attachments?.Any() == true)
        {
            foreach (var attachment in mailRequest.Attachments)
            {
                if (attachment.Type == AttachmentType.Base64)
                {
                    request.AddFile("attachment", Convert.FromBase64String(attachment.Content), attachment.Name);
                }
                else
                {
                    request.AddFile("attachment", attachment.Content, attachment.Name);
                }
            }
        }

        return request;
    }

    public Task<bool> SendTemplatedEmail<T>(
        IEnumerable<string> to,
        IEmailTemplate<T> template,
        CancellationToken cancellationToken = default
    )
    {
        var mailRequest = new SimpleMailRequest()
        {
            To = to,
            Template = template.Name,
            Tags = template.Tags,
            Subject = template.Subject,
            Vars = VarsToDictionary(template.Vars!),
        };

        return SendAsync(mailRequest, cancellationToken);
    }

    private IDictionary<string, object> VarsToDictionary(object obj)
    {
        var dictionary = new Dictionary<string, object>();

        foreach (var property in obj.GetType().GetProperties())
        {
            if (!property.CanRead) continue;

            var name = property.Name.ToCamelCase();
            var value = property.GetValue(obj)!;

            if (property.PropertyType == typeof(AppLink))
            {
                var link = (AppLink)value;

                value = _appSettings.FrontEndBaseUrl + link.Route;
            }

            dictionary[name] = value;
        }

        return dictionary;
    }
}
