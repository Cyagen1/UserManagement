using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UserManagement.Attributes
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                                   .Where(e => e.Value.Errors.Any())
                                   .Select(e => new
                                   {
                                       Field = e.Key,
                                       Error = e.Value.Errors.First().ErrorMessage
                                   })
                                   .ToList();

                context.Result = new BadRequestObjectResult(errors);
            }
        }
    }
}
