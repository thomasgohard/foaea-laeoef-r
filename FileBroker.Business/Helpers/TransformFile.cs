namespace FileBroker.Business.Helpers
{
    internal static class TransformFile
    {
        public static async Task<string> Process(FileTableData fileTableData, string originalPath)
        {
            string baseName = fileTableData.Name.ToUpper();
            string newPath = originalPath.Replace("Original", "Transform");
            var newFile = File.CreateText(newPath);
            using (var reader = File.OpenText(originalPath))
            {
                string thisLine = await reader.ReadLineAsync();

                while (!string.IsNullOrEmpty(thisLine) && (thisLine.Trim().Length >= 2))
                {
                    string recordType = thisLine[..2].Trim();
                    string transformedLine;

                    switch (baseName)
                    {
                        case "TR3SISOB":
                            switch (recordType)
                            {
                                case "01":
                                    transformedLine = "UI" + string.Format("{0:dd-MMM-yyyy}", ConvertToDate(thisLine.Substring(8, 7))).ToUpper();
                                    transformedLine += string.Concat(thisLine.AsSpan(2, 6), "-".PadLeft(61, ' '));
                                    await newFile.WriteLineAsync(transformedLine);
                                    break;

                                case "02":
                                    thisLine = thisLine.Trim();
                                    transformedLine = thisLine.Substring(10, 9).PadRight(12, ' ');
                                    transformedLine += string.Format("{0:dd-MMM-yyyy}", ConvertToDate(thisLine.Substring(19, 7))).ToUpper() + " ";
                                    transformedLine += string.Format("{0:dd-MMM-yyyy}", ConvertToDate(thisLine.Substring(26, 7))).ToUpper() + "  ";
                                    transformedLine += string.Concat(thisLine.AsSpan(3, 7), "-".PadLeft(36, ' '));
                                    await newFile.WriteLineAsync(transformedLine);
                                    break;

                                case "99":
                                    thisLine = thisLine.Trim();
                                    transformedLine = "".PadLeft(35, ' ');
                                    transformedLine += (int.Parse(thisLine.Substring(2, 8)) + 2).ToString().PadLeft(13, '0');
                                    transformedLine += "-".PadLeft(32, ' ');
                                    await newFile.WriteLineAsync(transformedLine);
                                    break;
                            }
                            break;
                    }

                    thisLine = await reader.ReadLineAsync();
                }

                newFile.Close();
            }

            return newPath;
        }

        private static DateTime ConvertToDate(string value)
        {
            if (value.Trim().Length == 7)
            {
                var result = new DateTime();
                result = result.AddYears(int.Parse(value.Substring(0, 4)) - 1);
                result = result.AddDays(int.Parse(value.Substring(4, 3)) - 1);
                return result;
            }
            else
                return new DateTime();
        }

    }
}
