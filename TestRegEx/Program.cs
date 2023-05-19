// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;

Console.WriteLine("Test Regex");

string fullPath = @"C:\FOAEA\FTProot\QC3M01\QC3M01II.003861.XML";
string xmlData = File.ReadAllText(fullPath);
xmlData = RemoveXMLartifacts(xmlData);
Console.WriteLine("--------------");
Console.WriteLine(xmlData);
Console.WriteLine("--------------");

string RemoveXMLartifacts(string xmlData)
{
    string result = xmlData;

    string replacement = string.Empty;
    string pattern = @"<\?xml.*\?>";
    result = Regex.Replace(result, pattern, replacement, RegexOptions.IgnoreCase);

    replacement = "";
    pattern = @"[\t\s]+xmlns[^>]+";
    result = Regex.Replace(result, pattern, replacement, RegexOptions.IgnoreCase);

    replacement = "";
    pattern = @"[\t\s]+xsi[^>]+";
    result = Regex.Replace(result, pattern, replacement, RegexOptions.IgnoreCase);

    return result;

}