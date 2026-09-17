using CoworkingBooking.Shared.Classes;
using Microsoft.AspNetCore.Mvc;

namespace CoworkingBooking.Api.Classes
{
    public static class ResultsExtension
    {
        public static ActionResult<ApiResponse<T>> ToActionResult<T>(this Result<T> result, Func<ApiResponse<T>, ActionResult> onSuccess)
        {
            if (!result.IsSuccess)
            {
                var response = new ApiResponse<T>(false, default!, result.Errors!.ToList());

                var error = result.Errors!.First();

                return error.Type switch
                {
                    ErrorType.ValidationError => new BadRequestObjectResult(response),

                    ErrorType.NotFound => new NotFoundObjectResult(response),
 
                    ErrorType.Conflict => new ConflictObjectResult(response),

                    ErrorType.Forbidden => new ForbidResult(),

                    _ => new StatusCodeResult(StatusCodes.Status500InternalServerError)
                };
            }

            return onSuccess(new ApiResponse<T>(true, result.Data!, null));
        }
    }
}