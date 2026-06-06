// ════════════════════════════════════════════
// REQUIRE INVOICE ACCESS
// Admin / MD / CEO / HOD / User / Supervisor
// Blocks: Operator
// ════════════════════════════════════════════
using InvoiceGenerator.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class RequireInvoiceAccessAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext ctx)
    {
        if (!SessionHelper.IsLoggedIn(ctx.HttpContext.Session))
        {
            ctx.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        if (!SessionHelper.HasInvoiceAccess(ctx.HttpContext.Session))
        {
            ctx.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
            return;
        }

        base.OnActionExecuting(ctx);
    }
}
