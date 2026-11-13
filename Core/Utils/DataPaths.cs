namespace BookFair.Core.Utils;

public static class DataPaths
{
    private const string DataFolderName = "Data";
    private const string SolutionFileName = "BookFair.slnx";

    public static string DataDirectory { get; } = FindDataDirectory();

    public static string GetFilePath(string fileName)
    {
        return DataDirectory + Path.DirectorySeparatorChar + fileName;
    }

    private static string FindDataDirectory()
    {
        DirectoryInfo? directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            if (File.Exists(directory.FullName + Path.DirectorySeparatorChar + SolutionFileName))
            {
                return directory.FullName + Path.DirectorySeparatorChar + DataFolderName;
            }

            directory = directory.Parent;
        }

        return Path.Combine(AppContext.BaseDirectory, DataFolderName);
    }
}
