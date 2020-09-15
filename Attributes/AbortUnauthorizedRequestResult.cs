using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Threading.Tasks;

namespace EasyBilling.Attributes
{
    internal class AbortUnauthorizedRequestResult : StatusCodeResult
    {
        public AbortUnauthorizedRequestResult() : base(401)
        {

        }

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            await base.ExecuteResultAsync(context);
            context.HttpContext.Response.Headers.Add("Context-Length", "0");
            context.HttpContext.Response.Body.Flush();
            context.HttpContext.Abort();
        }
    }
}