// Read in the Challenge input
//string[] inputData = System.IO.File.ReadAllLines(@".\ChallengeInput_Test.txt");
string[] inputData = System.IO.File.ReadAllLines(@".\ChallengeInput.txt");

List<char> mismatchedCharacters = new();
List<char> groupKeyValues = new();

Dictionary<char, int> characterValue = new Dictionary<char, int>()
{
    { 'a', 1 },
    { 'b', 2 },
    { 'c', 3 },
    { 'd', 4 },
    { 'e', 5 },
    { 'f', 6 },
    { 'g', 7 },
    { 'h', 8 },
    { 'i', 9 },
    { 'j', 10 },
    { 'k', 11 },
    { 'l', 12 },
    { 'm', 13 },
    { 'n', 14 },
    { 'o', 15 },
    { 'p', 16 },
    { 'q', 17 },
    { 'r', 18 },
    { 's', 19 },
    { 't', 20 },
    { 'u', 21 },
    { 'v', 22 },
    { 'w', 23 },
    { 'x', 24 },
    { 'y', 25 },
    { 'z', 26 },
    { 'A', 27 },
    { 'B', 28 },
    { 'C', 29 },
    { 'D', 30 },
    { 'E', 31 },
    { 'F', 32 },
    { 'G', 33 },
    { 'H', 34 },
    { 'I', 35 },
    { 'J', 36 },
    { 'K', 37 },
    { 'L', 38 },
    { 'M', 39 },
    { 'N', 40 },
    { 'O', 41 },
    { 'P', 42 },
    { 'Q', 43 },
    { 'R', 44 },
    { 'S', 45 },
    { 'T', 46 },
    { 'U', 47 },
    { 'V', 48 },
    { 'W', 49 },
    { 'X', 50 },
    { 'Y', 51 },
    { 'Z', 52 },
};

HashSet<char> uniqueCycleItems = new();

for (int i = 0; i < inputData.Length; i++)
{
    if (inputData[i].Length % 2 != 0)
        throw new Exception("Unexpected line length");

    string compartment1Values = inputData[i].Substring(0, inputData[i].Length / 2);
    string compartment2Values = inputData[i].Substring(inputData[i].Length / 2, inputData[i].Length / 2);

    HashSet<char> unduplicatedDuplicates = new();

    int cyclePosition = (i % 3) + 1;
    if (cyclePosition == 1)
    {
        uniqueCycleItems = inputData[i].ToHashSet<char>();
    }
    else
    {
        HashSet<char> uniqueItemValues = inputData[i].ToHashSet<char>();
        uniqueCycleItems.IntersectWith(uniqueItemValues);
    }
    if (cyclePosition == 3)
    {
        if (uniqueCycleItems.Count != 1)
            throw new Exception("More than 1 item left in this group");

        groupKeyValues.Add(uniqueCycleItems.Single<char>());
    }

    foreach (char item in compartment1Values)
    {
        if (compartment2Values.Contains(item))
        {
            unduplicatedDuplicates.Add(item);
        }
    }

    mismatchedCharacters.AddRange(unduplicatedDuplicates);
}

Console.WriteLine("Challenge 1 Answer: " + CalculateValueTotals(mismatchedCharacters));
Console.WriteLine("Challenge 2 Answer: " + CalculateValueTotals(groupKeyValues));

int CalculateValueTotals(List<char> mismatchedCharacters)
{
    int totalValue = 0;
    foreach (char item in mismatchedCharacters)
    {
        totalValue += characterValue[item];
    }

    return totalValue;
}
