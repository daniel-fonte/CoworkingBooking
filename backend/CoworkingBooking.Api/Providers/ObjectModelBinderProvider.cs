using CoworkingBooking.Api.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;

namespace CoworkingBooking.Api.Providers
{
    public class ObjectModelBinderProvider: IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(BsonObjectId))
            {
                return new ObjectIdBinder();
            }

            return null;
        }
    }
}