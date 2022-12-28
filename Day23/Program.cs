// Read in the Challenge input
//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

HashSet<(int x, int y)> ElfCoordinates = new();

for (int y = 0; y < inputData.Length; y++) 
{ 
    for (int x = 0; x < inputData[0].Length; x++)
    {
        if (inputData[y][x] == '#')
        {
            ElfCoordinates.Add((x, y));
        }
    }
}

DrawMap(ElfCoordinates);

for (int i = 1; i <= 10; i++)
{
    // Create a dictionary that stores the proposed new coordinate to the key, and the list of elfs attempting to move there to the value
    Dictionary<(int x, int y), List<(int x, int y)>> NewElfCoordinates = new();

    HashSet<(int x, int y)> elvesThatWantToMove = new();

    foreach ((int x, int y) c in ElfCoordinates)
    {
        // If no other elves are in their surrounding 8 spaces, then don't do anything
        if (!ElfCoordinates.Contains(NorthWest(c)) && 
            !ElfCoordinates.Contains(North(c)) && 
            !ElfCoordinates.Contains(NorthEast(c)) &&
            !ElfCoordinates.Contains(East(c)) && 
            !ElfCoordinates.Contains(SouthEast(c)) && 
            !ElfCoordinates.Contains(South(c)) &&
            !ElfCoordinates.Contains(SouthWest(c)) &&
            !ElfCoordinates.Contains(West(c)))
        {
            NewElfCoordinates.Add(c, new() { c });
            elvesThatWantToMove.Add(c);
        }

        if (i % 4 == 1)
        {
            CheckForMoveNorth(c, NewElfCoordinates, elvesThatWantToMove);
            CheckForMoveSouth(c, NewElfCoordinates, elvesThatWantToMove);
            CheckForMoveWest(c, NewElfCoordinates, elvesThatWantToMove);
            CheckForMoveEast(c, NewElfCoordinates, elvesThatWantToMove);
        }

        if (i % 4 == 2)
        {
            CheckForMoveSouth(c, NewElfCoordinates, elvesThatWantToMove);
            CheckForMoveWest(c, NewElfCoordinates, elvesThatWantToMove);
            CheckForMoveEast(c, NewElfCoordinates, elvesThatWantToMove);
            CheckForMoveNorth(c, NewElfCoordinates, elvesThatWantToMove);
        }

        if (i % 4 == 3)
        {
            CheckForMoveWest(c, NewElfCoordinates, elvesThatWantToMove);
            CheckForMoveEast(c, NewElfCoordinates, elvesThatWantToMove);
            CheckForMoveNorth(c, NewElfCoordinates, elvesThatWantToMove);
            CheckForMoveSouth(c, NewElfCoordinates, elvesThatWantToMove);
        }

        if (i % 4 == 0)
        {
            CheckForMoveEast(c, NewElfCoordinates, elvesThatWantToMove);
            CheckForMoveNorth(c, NewElfCoordinates, elvesThatWantToMove);
            CheckForMoveSouth(c, NewElfCoordinates, elvesThatWantToMove);
            CheckForMoveWest(c, NewElfCoordinates, elvesThatWantToMove);
        }

        // If no match, then leave them where they are
        if (!elvesThatWantToMove.Contains(c))
        {
            if (!NewElfCoordinates.ContainsKey(c))
            {
                NewElfCoordinates.Add(c, new());
            }
            NewElfCoordinates[c].Add(c);
        }
    }

    // Now set the new positions if there aren't more than one set of coordinates in the list of values
    HashSet<(int x, int y)> NextRoundElfCoordinates = NewElfCoordinates.Where(e => e.Value.Count == 1).Select(e => e.Key).ToHashSet();

    // Now for the ones that do have more than one, they need to be added back where they were originally
    foreach ((int x, int y) coord in NewElfCoordinates.Keys)
    {
        if (NewElfCoordinates[coord].Count > 1) 
        {
            foreach ((int x, int y) elfCoord in NewElfCoordinates[coord])
            {
                NextRoundElfCoordinates.Add(elfCoord);
            }
        }
    }

    ElfCoordinates = new(NextRoundElfCoordinates);
    DrawMap(ElfCoordinates);
}

// Get bounds of the elf coordinates
int minX = ElfCoordinates.Min(c => c.x);
int maxX = ElfCoordinates.Max(c => c.x);
int minY = ElfCoordinates.Min(c => c.y);
int maxY = ElfCoordinates.Max(c => c.y);

int width = Math.Abs(maxX - minX) + 1;
int height = Math.Abs(maxY - minY) + 1;

int AnswerChallenge1 = (width * height) - ElfCoordinates.Count;


Console.WriteLine($"Challenge 1 Answer: {AnswerChallenge1}");

(int x, int y) NorthWest((int x, int y) coord) => (coord.x - 1, coord.y - 1);
(int x, int y) North((int x, int y) coord) =>     (coord.x, coord.y - 1);
(int x, int y) NorthEast((int x, int y) coord) => (coord.x + 1, coord.y - 1);
(int x, int y) East((int x, int y) coord) =>      (coord.x + 1, coord.y);
(int x, int y) SouthEast((int x, int y) coord) => (coord.x + 1, coord.y + 1);
(int x, int y) South((int x, int y) coord) =>     (coord.x, coord.y + 1);
(int x, int y) SouthWest((int x, int y) coord) => (coord.x - 1, coord.y + 1);
(int x, int y) West((int x, int y) coord) =>      (coord.x - 1, coord.y);

void CheckForMoveNorth((int x, int y) c, Dictionary<(int x, int y), List<(int x, int y)>> elfDict, HashSet<(int x, int y)> elvesThatWillMove)
{
    if (!elvesThatWillMove.Contains(c) &&
        !ElfCoordinates.Contains(NorthWest(c)) &&
        !ElfCoordinates.Contains(North(c)) &&
        !ElfCoordinates.Contains(NorthEast(c)))
    {
        (int, int) newCoord = North(c);
        if (!elfDict.ContainsKey(newCoord))
        {
            elfDict.Add(newCoord, new());
        }
        elfDict[newCoord].Add(c);
        elvesThatWillMove.Add(c);
    }
}

void CheckForMoveSouth((int x, int y) c, Dictionary<(int x, int y), List<(int x, int y)>> elfDict, HashSet<(int x, int y)> elvesThatWillMove)
{
    if (!elvesThatWillMove.Contains(c) &&
        !ElfCoordinates.Contains(SouthEast(c)) &&
        !ElfCoordinates.Contains(South(c)) &&
        !ElfCoordinates.Contains(SouthWest(c)))
    {
        (int, int) newCoord = South(c);
        if (!elfDict.ContainsKey(newCoord))
        {
            elfDict.Add(newCoord, new());
        }
        elfDict[newCoord].Add(c);
        elvesThatWillMove.Add(c);
    }
}

void CheckForMoveEast((int x, int y) c, Dictionary<(int x, int y), List<(int x, int y)>> elfDict, HashSet<(int x, int y)> elvesThatWillMove)
{
    if (!elvesThatWillMove.Contains(c) &&
        !ElfCoordinates.Contains(SouthEast(c)) &&
        !ElfCoordinates.Contains(East(c)) &&
        !ElfCoordinates.Contains(NorthEast(c)))
    {
        (int, int) newCoord = East(c);
        if (!elfDict.ContainsKey(newCoord))
        {
            elfDict.Add(newCoord, new());
        }
        elfDict[newCoord].Add(c);
        elvesThatWillMove.Add(c);
    }
}

void CheckForMoveWest((int x, int y) c, Dictionary<(int x, int y), List<(int x, int y)>> elfDict, HashSet<(int x, int y)> elvesThatWillMove)
{
    if (!elvesThatWillMove.Contains(c) &&
        !ElfCoordinates.Contains(SouthWest(c)) &&
        !ElfCoordinates.Contains(West(c)) &&
        !ElfCoordinates.Contains(NorthWest(c)))
    {
        (int, int) newCoord = West(c);
        if (!elfDict.ContainsKey(newCoord))
        {
            elfDict.Add(newCoord, new());
        }
        elfDict[newCoord].Add(c);
        elvesThatWillMove.Add(c);
    }
}

void DrawMap(HashSet<(int x, int y)> coordinates)
{
    int minX = coordinates.Min(c => c.x);
    int maxX = coordinates.Max(c => c.x);
    int minY = coordinates.Min(c => c.y);
    int maxY = coordinates.Max(c => c.y);

    for (int y = minY; y <= maxY; y++)
    {
        Console.Write($"{y.ToString().PadLeft(3)} || ");
        for (int x = minX; x <= maxX; x++)
        {
            if (coordinates.Contains((x, y)))
            {
                Console.Write('#');
            } else
            {
                Console.Write('.');
            }
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}