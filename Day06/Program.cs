// Read in the Challenge input
//string inputData = File.ReadAllText(@".\ChallengeInput_Test.txt").Trim();
string inputData = File.ReadAllText(@".\ChallengeInput.txt").Trim();

int startOfPacketMarkerLocation = 0;
int startOfMessageMarkerLocation = 0;

for (int i = 4; i < inputData.Length; i++)
{
    HashSet<char> packetGroup = new(inputData.Substring(i - 4, 4));
    if (startOfPacketMarkerLocation == 0 && packetGroup.Count == 4)
    {
        startOfPacketMarkerLocation = i;
        Console.WriteLine("Challenge 1 Answer: " + startOfPacketMarkerLocation);
    }

    if (i >= 14)
    {
        HashSet<char> messageGroup = new(inputData.Substring(i - 14, 14));
        if (startOfMessageMarkerLocation == 0 && messageGroup.Count == 14)
        {
            startOfMessageMarkerLocation = i;
            Console.WriteLine("Challenge 2 Answer: " + startOfMessageMarkerLocation);
        }
    }
}
