using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CateringManagement.CustomControllers
{

    /// <summary>
    /// Makes the controller "self aware" knowing it's own name
    /// and what Action was called.
    /// </summary>
    public class CognizantController : Controller
    {
        internal string ControllerName()
        {
            return ControllerContext.RouteData.Values["controller"].ToString();
        }
        internal string ActionName()
        {
            return ControllerContext.RouteData.Values["action"].ToString();
        }

        /// <summary>
        /// This code is executed before any Action method is called
        /// and gives us a chance to add to the ViewData dictionary.
        /// Great way to make these values available to all Views.
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //Add the Controller and Action names to ViewData
            ViewData["ControllerName"] = ControllerName();
            ViewData["ActionName"] = ActionName();
            ViewData["Title"] = ControllerName() + " " + ActionName();
            if (!ViewData.ContainsKey("returnURL"))
            {
                ViewData["returnURL"] = "/" + ControllerName();
            }
            base.OnActionExecuting(context);
        }

        /// <summary>
        /// Same as above but for async Actions
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public override Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            //Add the Controller and Action names to ViewData
            ViewData["ControllerName"] = ControllerName();
            ViewData["ActionName"] = ActionName();
            ViewData["Title"] = ControllerName() + " " + ActionName();
            if (!ViewData.ContainsKey("returnURL"))
            {
                ViewData["returnURL"] = "/" + ControllerName();
            }
            return base.OnActionExecutionAsync(context, next);
        }
    }
}
