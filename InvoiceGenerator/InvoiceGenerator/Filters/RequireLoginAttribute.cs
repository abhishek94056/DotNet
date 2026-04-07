using InvoiceGenerator.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InvoiceGenerator.Filters
{
    // ── Require Login ──
    public class RequireLoginAttribute : ActionFilterAttribute
    // ActionFilterAttribute -> Base class used to create custom action filters 
    // that execute code before and/or after controller actions
    {
        public override void OnActionExecuting(ActionExecutingContext ctx)
        // ActionExecutingContext -> Provides information about the current request,
        // including HttpContext, Action arguments, Route data before the action runs
        {
            if (!SessionHelper.IsLoggedIn(ctx.HttpContext.Session))
            // HttpContext -> Represents the current HTTP request and response
            // Session -> Used to store user-specific data across requests
            {
                ctx.Result = new RedirectToActionResult("Login", "Auth", null);
                // RedirectToActionResult -> Redirects the user to a specified 
                // controller action (here: Auth/Login)

                return;
            }

            base.OnActionExecuting(ctx);
            // Calls the base implementation (good practice, ensures pipeline continues properly)
        }
    }
}