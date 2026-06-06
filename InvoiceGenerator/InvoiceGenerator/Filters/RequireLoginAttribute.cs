// Filters/SessionAuthFilter.cs
using InvoiceGenerator.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InvoiceGenerator.Filters
{
    // ════════════════════════════════════════════
    // REQUIRE LOGIN — any authenticated user
    // ════════════════════════════════════════════
    public class RequireLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            if (!SessionHelper.IsLoggedIn(ctx.HttpContext.Session))
            {
                ctx.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            // Operator has NO access to anything
            if (SessionHelper.IsOperator(ctx.HttpContext.Session))
            {
                ctx.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
                return;
            }

            base.OnActionExecuting(ctx);
        }
    }
}