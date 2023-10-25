using System.Reflection;

namespace FileBroker.Business.Helpers;

public static class FlatFileSpecHelper
{
    public static void ExtractRecTypeSingle<T>(ref T recTypeSingle, string flatFileLine,
                                               List<FlatFileSpecificationData> specs, string recType,
                                               int lineNumber, ref string error)
    {
        ExtractAndStoreDataFromLine<T>(flatFileLine, ref recTypeSingle, specs, recType, lineNumber, ref error);
    }

    public static void ExtractRecTypeMultiple<T>(List<T> recTypeMultiple, string flatFileLine,
                                                 List<FlatFileSpecificationData> specs, string recType,
                                                 int lineNumber, ref string error) where T : new()
    {
        var dataForRecType = new T();

        ExtractAndStoreDataFromLine<T>(flatFileLine, ref dataForRecType, specs, recType, lineNumber, ref error);

        recTypeMultiple.Add(dataForRecType);
    }

    private static void ExtractAndStoreDataFromLine<T>(string flatFileLine, ref T newRecType,
                                                       List<FlatFileSpecificationData> specs, string recType,
                                                       int lineNumber, ref string error)
    {
        var sectionSpecs = specs.Where(m => recType == m.Val_FileSect).OrderBy(m => m.Val_SortOrder);
        foreach (var specItem in sectionSpecs)
        {
            // need to use Reflection since the field name is a string from the table and must match
            // exactly with the field in the class/struct we are loading this data into
            string itemName = specItem.Field_Name;
            var objectType = typeof(T);
            var fieldInfo = objectType.GetField(itemName, BindingFlags.Instance | BindingFlags.Public);

            if (fieldInfo is null)
            {
                error = $"[Line: {lineNumber}] Could not find field [{itemName}]: flatFileLine: {flatFileLine}";
                return;
            }

            string valueFromFlatFileLine = "";

            if (flatFileLine.Length >= (specItem.Val_Pos_End))
                valueFromFlatFileLine = flatFileLine.Substring(specItem.Val_Pos_Start - 1, specItem.Val_Pos_End - specItem.Val_Pos_Start + 1);

            try
            {
                // need to user SetValueDirect and __makeref() since we are using struct instead of class
                if ((specItem.PrcsType_Cd.ToLower().Trim() == "date") && (specItem.FormatDate.ToLower().Trim() == "ccyyjjj"))
                    fieldInfo.SetValueDirect(__makeref(newRecType), valueFromFlatFileLine.ConvertJulianDateStringToDateTime(ref error));

                else if (specItem.PrcsType_Cd.ToLower().Trim() == "integer")
                    fieldInfo.SetValueDirect(__makeref(newRecType), valueFromFlatFileLine.ConvertToInteger());

                else
                    fieldInfo.SetValueDirect(__makeref(newRecType), valueFromFlatFileLine);

                if (!string.IsNullOrEmpty(error))
                {
                    if ((specItem.Val_Required == 1) ||
                        ((specItem.Val_Required == 0) && (!string.IsNullOrEmpty(valueFromFlatFileLine?.Trim()))))
                    {
                        Console.WriteLine($"*** ERROR *** {flatFileLine}");
                        error = $"[Line: {lineNumber}][{itemName} ({specItem.PrcsType_Cd.ToLower().Trim()})]: " + error + ": " + flatFileLine;
                    }
                    else
                        error = string.Empty; // only report error if field is mandatory or 
                                              // if it is optional and there is an invalid value
                }
            }
            catch (Exception e)
            {
                error = $"[Line: {lineNumber}] Field [{itemName}] could not be converted to required type [{specItem.PrcsType_Cd.ToLower().Trim()}]: " + e.Message;
                return;
            }
        }
    }
}
