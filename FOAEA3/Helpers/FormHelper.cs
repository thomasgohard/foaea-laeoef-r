using FOAEA3.Model;
using Microsoft.AspNetCore.Http;

namespace FOAEA3.Helpers
{
    public static class FormHelper
    {
        public static void SetErrorDisplay(MessageDataList model, string fieldName,
                                           out string errorControlClass,
                                           out string errorLabelClass,
                                           out string errorMessage)
        {
            if (model.ContainsField(fieldName))
            {
                errorControlClass = "is-invalid";
                errorLabelClass = "text-danger";
                errorMessage = model.GetMessageForField(fieldName).Description;
            }
            else
            {
                errorControlClass = string.Empty;
                errorLabelClass = string.Empty;
                errorMessage = string.Empty;
            }
        }

        public static bool GetValueFromCollection(IFormCollection collection, string item, bool defaultValue)
        {
            bool result = default;

            if (collection.Keys.Contains(item))
            {
                bool isValid = bool.TryParse(collection[item], out result);

                if (!isValid)
                    result = defaultValue;

            }

            return result;
        }

        public static int GetValueFromCollection(IFormCollection collection, string item, int defaultValue)
        {
            int result = default;

            if (collection.Keys.Contains(item))
            {
                bool isValid = int.TryParse(collection[item], out result);

                if (!isValid)
                    result = defaultValue;

            }

            return result;
        }

        public static short GetValueFromCollection(IFormCollection collection, string item, short defaultValue)
        {
            short result = default;

            if (collection.Keys.Contains(item))
            {
                bool isValid = short.TryParse(collection[item], out result);

                if (!isValid)
                    result = defaultValue;

            }

            return result;
        }

        public static byte GetValueFromCollection(IFormCollection collection, string item, byte defaultValue)
        {
            byte result = default;

            if (collection.Keys.Contains(item))
            {
                bool isValid = byte.TryParse(collection[item], out result);

                if (!isValid)
                    result = defaultValue;

            }

            return result;
        }

    }
}
