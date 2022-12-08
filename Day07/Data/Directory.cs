namespace Day07.Data;

public abstract class Directory
{
    public string DirectoryName { get; set; }

    public RootDirectory Root { get; set; }

    public abstract string Path { get; }

    public Directory? ParentDirectory { get; set; }

    public List<SubDirectory> SubDirectories { get; set; } = new();

    public List<(string Name, int Size)> Files { get; set; } = new();

    public abstract bool HasParentDirectory { get; }

    public string ListDirectoryContents()
    {
        string contentString = string.Empty;

        if (Files.Count > 0 || SubDirectories.Count > 0)
        {
            foreach ((string Name, int Size) file in Files)
            {
                contentString += $"{file.Size} - {file.Name}";
                contentString += System.Environment.NewLine;
            }

            foreach (SubDirectory directory in SubDirectories)
            {
                contentString += $"dir {directory.DirectoryName}";
                contentString += System.Environment.NewLine;
            }
        }

        return contentString;
    }

    public int GetDirectorySize()
    {
        int totalSize = 0;
        foreach (SubDirectory subDirectory in SubDirectories)
        {
            totalSize += subDirectory.GetDirectorySize();
        }
        foreach ((string Name, int Size) file in Files)
        {
            totalSize += file.Size;
        }
        if (Root is not null && totalSize > Root.Limit)
        {
            Root.DirectoriesOverLimit.Add(totalSize);
            Console.WriteLine($"{DirectoryName} more than {Root?.Limit} bytes (size: {totalSize} bytes)");
        }
        if (totalSize < 100_000)
        {
            Root?.DirectoriesUnder100K.Add(totalSize);
            Console.WriteLine($"{DirectoryName} less than 100,000 bytes (size: {totalSize} bytes)");
        }

        return totalSize;
    }

    public void PrintContents()
    {
        throw new NotImplementedException();
    }

    public Directory(string directoryName)
    {
        DirectoryName = directoryName;
    }
}

public class RootDirectory : Directory
{
    public new string DirectoryName => "/";
    
    public override string Path => "/";

    public override bool HasParentDirectory => false;

    public new RootDirectory Root { get; init; }

    public new Directory? ParentDirectory { get; init; }

    public int Limit { get; set; }

    public List<int> DirectoriesUnder100K = new();

    public List<int> DirectoriesOverLimit = new();

    public int SolveChallenge1()
    {
        DirectoriesUnder100K.Clear();
        GetDirectorySize();
        return DirectoriesUnder100K.Sum();
    }

    public int SolveChallenge2()
    {
        DirectoriesOverLimit.Clear();
        GetDirectorySize();
        return DirectoriesOverLimit.Count > 0 ? DirectoriesOverLimit.Min() : -1;
    }

    public RootDirectory(string inputPath) : base("/")
    {
        Root = this;
        ParentDirectory = null;

        // Import Data and Build Directory Tree based on input
        string[] inputData = File.ReadAllLines(inputPath);

        Directory currentDirectory = this;

        foreach (string line in inputData)
        {
            if (line.StartsWith("$"))
            {
                string[] trimmedCommand = line[2..].Split(' ');

                switch (trimmedCommand[0])
                {
                    case "ls":
                        continue;
                    case "cd":
                        switch (trimmedCommand[1])
                        {
                            case "/":
                                currentDirectory = this;
                                break;
                            case "..":
                                if (currentDirectory.ParentDirectory is not null)
                                {
                                    currentDirectory = currentDirectory.ParentDirectory;
                                }
                                break;
                            default:
                                SubDirectory? targetDirectory = currentDirectory
                                        .SubDirectories
                                        .SingleOrDefault(d => d.DirectoryName.Equals(trimmedCommand[1], StringComparison.InvariantCultureIgnoreCase));
                                currentDirectory = targetDirectory ?? currentDirectory;
                                break;
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else if (line.StartsWith("dir"))
            {
                currentDirectory.SubDirectories.Add(new SubDirectory(Root, currentDirectory, line.Split(' ')[1]));
            }
            else if (int.TryParse(line.Split(' ')[0], out int fileSize))
            {
                currentDirectory.Files.Add((line.Split(' ')[1], fileSize));
            }
            else
            {
                throw new NotImplementedException(line);
            }
        }

        // Set the Limit
        Limit = 30_000_000 - (70_000_000 - GetDirectorySize());
    }

}

public class SubDirectory : Directory
{
    public override string Path => $"{ParentDirectory?.Path}{DirectoryName}/";

    public override bool HasParentDirectory => true;

    public SubDirectory(RootDirectory rootDirectory, Directory parentDirectory, string Name) : base(Name)
    {
        Root = rootDirectory;
        ParentDirectory = parentDirectory;
    }
}
