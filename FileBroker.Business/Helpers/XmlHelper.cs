namespace FileBroker.Business.Helpers;

public class XmlHelper
{
    public static string GenerateXMLTagWithValue(string tagName, string value)
    {
        string trimmedValue = value?.Trim();
        if (string.IsNullOrEmpty(trimmedValue))
            return $"   <{tagName} />";
        else
            return $"   <{tagName}>{trimmedValue}</{tagName}>";
    }
}
