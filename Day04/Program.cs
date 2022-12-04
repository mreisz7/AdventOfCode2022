// Read in the Challenge input
//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

// Part 1 Answer
int numberOfFullyOverlappingAssignments = 0;

// Part 2 Answer
int numberOfOverlappingAssignments = 0;

foreach (string sectionAssignmentPairs in inputData)
{
    // Parse the input and generate assignment ranges
    string[] assignmentPairArray = sectionAssignmentPairs.Split(',');
    string[] elf1SectionAssignment = assignmentPairArray[0].Split("-");
    string[] elf2SectionAssignment = assignmentPairArray[1].Split("-");

    HashSet<int> elf1SectionAssignmentRange = Enumerable.Range(int.Parse(elf1SectionAssignment[0]), int.Parse(elf1SectionAssignment[1]) - int.Parse(elf1SectionAssignment[0]) + 1).ToHashSet<int>();
    HashSet<int> elf2SectionAssignmentRange = Enumerable.Range(int.Parse(elf2SectionAssignment[0]), int.Parse(elf2SectionAssignment[1]) - int.Parse(elf2SectionAssignment[0]) + 1).ToHashSet<int>();

    // Identify if there is any overlap in the two ranges at all
    HashSet<int> overlappingAssignments = new HashSet<int>(elf1SectionAssignmentRange);
    overlappingAssignments.IntersectWith(elf2SectionAssignmentRange);

    if (overlappingAssignments.Count > 0)
    {
        numberOfOverlappingAssignments++;

        // Check if they're fully overlapping
        if ((elf2SectionAssignmentRange.Min() >= elf1SectionAssignmentRange.Min()
          && elf2SectionAssignmentRange.Max() <= elf1SectionAssignmentRange.Max()) ||
            (elf1SectionAssignmentRange.Min() >= elf2SectionAssignmentRange.Min()
          && elf1SectionAssignmentRange.Max() <= elf2SectionAssignmentRange.Max()))
        {
            numberOfFullyOverlappingAssignments++;
        }
    }
}

Console.WriteLine("Challenge 1 Answer: " + numberOfFullyOverlappingAssignments);
Console.WriteLine("Challenge 2 Answer: " + numberOfOverlappingAssignments);