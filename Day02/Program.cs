﻿// Read in the Challenge 1 input
//string[] inputData = System.IO.File.ReadAllLines(@"..\..\..\ChallengeInput_Test.txt");
string[] inputData = System.IO.File.ReadAllLines(@"..\..\..\ChallengeInput.txt");

int gameScore = 0;

foreach (string game in inputData)
{
    string[] gameMoves = game.Split(' ');
    string opponentMove = gameMoves[0];
    string yourMove = gameMoves[1];

    gameScore += CalculateScore(opponentMove, yourMove);
}

Console.WriteLine("Challenge 1 Score: " + gameScore);

int CalculateScore(string opponentsMove, string yourMove)
{
    // Scoring: (Your move + outcome)
    // Your Move = "X"   - 1 Point
    // Your Move = "Y"   - 2 Points
    // Your Move = "Z"   - 3 Points
    // Outcome: You lose - 0 Points
    // Outcome: You tie  - 3 Points
    // Outcome: You win  - 6 Points

    int totalScore = 0;

    // Calculate outcome score
    Outcome gameOutcome = GameOutcome(opponentsMove, yourMove);
    switch(gameOutcome)
    {
        case Outcome.Lose:
            break;
        case Outcome.Tie:
            totalScore += 3;
            break;
        case Outcome.Win:
            totalScore += 6;
            break;
    }

    // Calculate your move score
    switch (yourMove)
    {
        case "X": // Rock
            totalScore += 1;
            break;
        case "Y": // Paper
            totalScore += 2;
            break;
        case "Z": // Scissors
            totalScore += 3;
            break;
    }

    return totalScore;
}

Outcome GameOutcome(string opponentsMove, string yourMove)
{
    // Opponent Moves
    // A: Rock
    // B: Paper
    // C: Scissors

    // Your Moves:
    // X: Rock
    // Y: Paper
    // Z: Scissors

    switch (opponentsMove)
    {
        case "A": // Rock
            if (yourMove == "X") // Rock
                return Outcome.Tie;
            if (yourMove == "Y") // Paper
                return Outcome.Win;
            if (yourMove == "Z") // Scissors
                return Outcome.Lose;
            throw new Exception("You played an unexpected move");
        case "B": // Paper
            if (yourMove == "X") // Rock
                return Outcome.Lose;
            if (yourMove == "Y") // Paper
                return Outcome.Tie;
            if (yourMove == "Z") // Scissors
                return Outcome.Win;
            throw new Exception("You played an unexpected move");
        case "C": // Scissors
            if (yourMove == "X") // Rock
                return Outcome.Win;
            if (yourMove == "Y") // Paper
                return Outcome.Lose;
            if (yourMove == "Z") // Scissors
                return Outcome.Tie;
            throw new Exception("You played an unexpected move");
        default:
            throw new Exception("Your opponent played an unexpected move");
    }
}

enum Outcome
{
    Lose,
    Tie,
    Win
}