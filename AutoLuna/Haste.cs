using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace AutoLuna
{
    public class Paste
    {
        public string Key = "";
        public string URL = "";
    }

    public class Hastebin
    {
        public string Domain;

        public Hastebin(string domain)
        {
            Domain = domain;
        }

        public async Task<Paste> CreatePaste(string contents)
        {
            var paste = new Paste();
            var req = await Domain.AppendPathSegment("documents")
                .SendStringAsync(HttpMethod.Post, contents);
            var json = await req.GetJsonAsync();
            paste.Key = json.key;
            paste.URL = Domain.AppendPathSegment((string)json.key);
            return paste;
        }
    }
}
