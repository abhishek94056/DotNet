using InvoiceGenerator.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InvoiceGenerator.Filters
{
    public class RequireAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            if (!SessionHelper.IsLoggedIn(ctx.HttpContext.Session))
            {
                ctx.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }
            if (!SessionHelper.IsAdmin(ctx.HttpContext.Session))
            {
                ctx.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
                return;
            }
            base.OnActionExecuting(ctx);
        }
    }
}