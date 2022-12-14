// Read in the Challenge input
//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

int numberOfPairs = (inputData.Length + 1) / 3;
int runningTotal = 0;

for (int i = 0; i < numberOfPairs; i++)
{
    string leftString  = inputData[i * 3];
    string rightString = inputData[i * 3 + 1];
    bool result = TestPairs(leftString, rightString);

    if (result)
    {
        runningTotal += (i + 1);
    }

    Console.WriteLine($"Comparison #{i + 1}");
    Console.WriteLine($"Left:   {leftString}");
    Console.WriteLine($"Right:  {rightString}");
    Console.WriteLine($"Result: {result}");
    Console.WriteLine("---------------------------------------------");
    Console.WriteLine();
}

Console.WriteLine($"Challenge 1 Answer: {runningTotal}");

List<string> strippedInputs = inputData.Where(x => x != "").ToList();
strippedInputs.Add("[[2]]");
strippedInputs.Add("[[6]]");

for (int i = 0; i < strippedInputs.Count - 1; i++)
{
    for (int j = 0; j < strippedInputs.Count - i - 1; j++)
    {
        if (!TestPairs(strippedInputs[j], strippedInputs[j + 1]))
        {
            string temp = strippedInputs[j];
            strippedInputs[j] = strippedInputs[j + 1];
            strippedInputs[j + 1] = temp;
        }
    }
}

int indexOf2 = 0;
int indexOf6 = 0;

for (int i = 1; i <= strippedInputs.Count; i++)
{
    if (strippedInputs[i - 1] == "[[2]]")
        indexOf2 = i;

    if (strippedInputs[i - 1] == "[[6]]")
        indexOf6 = i;
}

Console.WriteLine($"Challenge 2 Answer: {indexOf2 * indexOf6}");

bool TestPairs(string leftString, string rightString)
{
    int index = 0;
    bool correctOrder = false;

    // Split the left and right strings into lists of string to begin comparisons
    List<string> leftStringArray  = StripOuterBraces(leftString);
    List<string> rightStringArray = StripOuterBraces(rightString);

    while (!correctOrder && (index < leftStringArray.Count || index < rightStringArray.Count))
    {
        // If there's a value on the left side, but not the right, then return true
        if (leftStringArray.Count > 0 && rightStringArray.Count == 0)
        {
            correctOrder = false;
            break;
        }

        // If there's no value on the left side, but there is on the right then return false
        if (leftStringArray.Count == 0 && rightStringArray.Count > 0)
        {
            correctOrder = true;
            break;
        }

        // If they're the same, then move to the next index
        if (leftStringArray[index] == rightStringArray[index])
        {
            if (leftStringArray.Count - 1 == index && leftStringArray.Count < rightStringArray.Count)
            {
                correctOrder = true;
                break;
            }

            if (rightStringArray.Count - 1 == index && leftStringArray.Count > rightStringArray.Count)
            {
                correctOrder = false;
                break;
            }
            index++;
            continue;
        }

        // If the left value is a number and the right is an array, then wrap the left value with square brackets and try again
        if (!leftStringArray[index].StartsWith('[') && rightStringArray[index].StartsWith('['))
        {
            leftStringArray[index] = $"[{leftStringArray[index]}]";
            continue;
        }

        // If the right value is a number and the left is an array, then wrap the right value with square brackets and try again
        if (leftStringArray[index].StartsWith('[') && !rightStringArray[index].StartsWith('['))
        {
            rightStringArray[index] = $"[{rightStringArray[index]}]";
            continue;
        }
        
        // If they're both numbers, then do some simple comparison to see which is larger
        if (!leftStringArray[index].StartsWith('[') && !rightStringArray[index].StartsWith('['))
        {
            int leftValue  = int.Parse(leftStringArray[index]);
            int rightValue = int.Parse(rightStringArray[index]);
            if (leftValue < rightValue)
            {
                correctOrder = true;
                break;
            }
            if (leftValue > rightValue)
            {
                correctOrder = false;
                break;
            }
        }
 
        // If they're both arrays, then recursively test them
        if (leftStringArray[index].StartsWith('[') && rightStringArray[index].StartsWith('['))
        {
            bool result = TestPairs(leftStringArray[index], rightStringArray[index]);
            correctOrder = result;
            break;
        }
    }

    return correctOrder;
}

List<string> StripOuterBraces(string packetString)
{
    int depth = 0;

    List<string> result = new();
    int indexOfCutStart = -1;

    for (int i = 1; i < packetString.Length - 1; i++)
    {
        char c = packetString[i];
        if (packetString[i] == ',')
        {
            continue;
        }
        else if (packetString[i] == '[')
        {
            if (depth == 0) 
                indexOfCutStart = i;
            depth++;
        }
        else if (packetString[i] == ']') 
        {
            depth--;
            if (depth == 0)
                result.Add(packetString.Substring(indexOfCutStart, i - indexOfCutStart + 1));
        }
        else if (depth == 0 && packetString.Substring(i, 2) == "10")  // First check if it's a 10 digit number
        {
            result.Add(packetString.Substring(i, 2));
            i++;  // Skip an extra iteration since we took two characters instead of 1
        }
        else if (depth == 0 && int.TryParse(packetString[i].ToString(), out int _))
        {
            result.Add(packetString[i].ToString());
        }
    }

    return result;
}

