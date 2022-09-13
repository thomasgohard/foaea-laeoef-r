using FOAEA3.Common.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace FOAEA3.Common.ModelBinders
{
    public class ApplKeyModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var m = bindingContext.ModelMetadata;
            if (m.BinderType is null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            if (m.ModelType != typeof(ApplKey))
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;
            if (!string.IsNullOrEmpty(value))
            {
                var model = new ApplKey(value);
                bindingContext.Result = ModelBindingResult.Success(model);
                return Task.CompletedTask;
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }
        }
    }
}
