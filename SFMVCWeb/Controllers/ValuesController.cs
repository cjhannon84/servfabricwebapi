using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SFMVCWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly HttpClient client;

        private StatelessServiceContext _ctx { get; set; }
        private readonly string reverseProxyBaseUri;

        public ValuesController(StatelessServiceContext ctx, HttpClient client)
        {
            _ctx = ctx;
            this.client = client;
            this.reverseProxyBaseUri = Environment.GetEnvironmentVariable("ReverseProxyBaseUri");
        }

        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            Uri serviceName = GetBackEndDataServiceName(this._ctx);
            Uri proxyAddress = this.GetProxyAddress(serviceName);
            string proxyUrl = $"{proxyAddress}/api/BEValues";
            
            var resp = await client.GetAsync(proxyUrl);

            return new string[] { "returned" };
        }

        private Uri GetProxyAddress(Uri serviceName)
        {
            return new Uri($"{this.reverseProxyBaseUri}{serviceName.AbsolutePath}");
        }


        internal static Uri GetBackEndDataServiceName(ServiceContext context)
        {
            return new Uri($"{context.CodePackageActivationContext.ApplicationName}/BackEndWbe");
        }

    }
}
