// Read in the Challenge input
//string[] inputData = File.ReadAllLines(@".\ChallengeInput_Test.txt");
string[] inputData = File.ReadAllLines(@".\ChallengeInput.txt");

long runningTotalPart1 = 0;

foreach (string input in inputData)
{
    Console.WriteLine($"{input} => {SnafuToInt(input)}");
    runningTotalPart1 += SnafuToInt(input);
}

Console.WriteLine($"Challenge 1 Answer: {runningTotalPart1} -> {IntToSnafu(runningTotalPart1)}");

long SnafuToInt(string snafu)
{
    if (snafu.Trim().Length > 0)
    {
        return SnafuToInt(snafu[0..^1]) * 5 + ConvertSnafuChar(snafu[^1]);
    }
    else
    {
        return 0;
    }
}

string IntToSnafu(long number)
{
    if (number != 0)
    {
        (long quotient, long remainder) = Math.DivRem(number + 2, 5);
        return $"{IntToSnafu(quotient)}{ConvertIntToChar(remainder)}";
    }
    return "";
}

long ConvertSnafuChar(char c) => c switch
{
    '2' => 2,
    '1' => 1,
    '0' => 0,
    '-' => -1,
    '=' => -2,
    _ => throw new NotImplementedException("A character that is not supported was supplied.")
};

char ConvertIntToChar(long n) => n switch
{
    4 => '2',
    3 => '1',
    2 => '0',
    1 => '-',
    0 => '=',
    _ => throw new NotImplementedException("An integer that is not supported was supplied.")
};
