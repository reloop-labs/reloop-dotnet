using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Reloop.Models;

namespace Reloop.Services
{
    public class MailService
    {
        private readonly ReloopClient _client;

        internal MailService(ReloopClient client)
        {
            _client = client;
        }

        public Task<SendMailResponse?> SendAsync(Dictionary<string, object?> parameters)
        {
            return _client.FetchAsync<SendMailResponse>(HttpMethod.Post, "/api/mail/v1/send", parameters);
        }
    }
}
