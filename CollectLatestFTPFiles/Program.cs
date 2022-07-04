// See https://aka.ms/new-console-template for more information
string sourceRoot = @"\\otvfoaea5\FOAEAftpbackup\";
string destinationRoot = @"C:\FOAEA\FTProot\";
var cyclesFound = new Dictionary<string, List<string>>();

var subfolders = Directory.GetDirectories(sourceRoot);
foreach(var subfolder in subfolders)
{
    var subfolderName = Path.GetFileName(subfolder);
    if (Directory.Exists(destinationRoot + subfolderName))
        CopyTodaysFiles(subfolder, destinationRoot + subfolderName, cyclesFound);
}

Console.WriteLine("");
Console.WriteLine("INSERT INTO @cycles");
Console.WriteLine("VALUES");

string lastKey = cyclesFound.Keys.Last();
foreach (var provinceCycles in cyclesFound)
{
    var cycles = provinceCycles.Value;
    var lastCycle = cycles.Last();
    foreach (var cycle in cycles)
    {
        string comma = ((provinceCycles.Key == lastKey) && (lastCycle == cycle)) ? "" : ", ";
        Console.WriteLine($"  ('{provinceCycles.Key}', '{cycle}'){comma}");
    }
}

static void CopyTodaysFiles(string sourceFolder, string destinationFolder, Dictionary<string, List<string>> cyclesFound)
{
    var allFiles = Directory.GetFiles(sourceFolder, "*II.??????.xml", searchOption: SearchOption.TopDirectoryOnly);
    foreach(string filePath in allFiles)
    {
        var fileCreationDateTime = File.GetCreationTime(filePath);
        if (fileCreationDateTime.Date == DateTime.Now.Date)
        {
            string fileName = Path.GetFileName(filePath);
            string cycleNumber = GetCycleStringFromFilename(fileName);
            string provinceCode = fileName[0..2];
            Console.WriteLine($"{cycleNumber}: Copying {fileName} from {sourceFolder} to {destinationFolder}");
            File.Copy(sourceFolder + @"\" + fileName, destinationFolder + @"\" + fileName, overwrite: true);
            if (!cyclesFound.ContainsKey(provinceCode))
                cyclesFound.Add(provinceCode, new List<string> { cycleNumber });
            else
                cyclesFound[provinceCode].Add(cycleNumber);
        }
    }
}

static string GetCycleStringFromFilename(string fileName)
{
    if (fileName.ToUpper().EndsWith(".XML"))
        fileName = Path.GetFileNameWithoutExtension(fileName);

    int lastPeriod = fileName.LastIndexOf('.');
    if (lastPeriod > 0)
        return fileName[(lastPeriod + 1)..];
    else
        return "";
}
