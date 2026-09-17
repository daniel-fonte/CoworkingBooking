using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;

namespace CoworkingBooking.Api.Binders
{
    public class ObjectIdBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask; // Falls back to defaults
            }

            // Set the value in ModelState for tracking/validation validation
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            var rawValue = valueProviderResult.FirstValue;
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return Task.CompletedTask;
            }

            if(!ObjectId.TryParse(rawValue, out var objectId))
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid ObjectId format.");
                return Task.CompletedTask;
            }

            bindingContext.Result = ModelBindingResult.Success(objectId);
            return Task.CompletedTask;
        }
    }
}