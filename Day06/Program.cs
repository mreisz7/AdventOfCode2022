// Read in the Challenge input
//string inputData = File.ReadAllText(@".\ChallengeInput_Test.txt").Trim();
string inputData = File.ReadAllText(@".\ChallengeInput.txt").Trim();

for (int i = 4; i < inputData.Length; i++)
{
    HashSet<char> charGroup = new(inputData.Substring(i - 4, 4));
    if (charGroup.Count == 4)
    {
        Console.WriteLine("Challenge 1 Answer: " + i);
        break;
    } 
}