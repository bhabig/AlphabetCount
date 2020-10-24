using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace LetterCounter
{
    class Program
    {
        static void Main(string[] args)
        {
            // vars to store helper results
            string userInput = null;
            MatchCollection lettersOnly = null;
            string displayStyle = null;
            Dictionary<string, int> lettersMappedToCount = new Dictionary<string, int>();
            // Greeting
            Console.WriteLine("**Disclaimer** I could have done this with NuGet packages that would save a lot of time and produce a more aesthetically pleasing result, but I opted to manually write as much as possible.");
            Console.WriteLine("Hello and welcome!");
            // 1) get user's input string
            // pupulates userInput var
            userInput = GetInput("initial");
            // 2) filter out any special characters, numbers, spaces, etc...
            // populates lettersOnly var and USES userInput
            lettersOnly = RegexLetters(userInput);
            // check results of LettersOnly & re-prompt user for input if empty
            if (lettersOnly.Count == 0)
            {
                // retry the above flow if user wishes:
                Console.WriteLine("There were no English Alphabet characters in the input. We only analyze letters in the English Alphabet. Press 'y' if you would like to provide new input, otherwise enter any other key to exit the program.");
                string decision = Console.ReadLine();

                // if user says yes, prompt again:
                if (decision != null && decision.ToLower() == "y")
                {
                    // retry until good input:
                    while (lettersOnly.Count == 0)
                    {
                        userInput = GetInput("initial");
                        lettersOnly = RegexLetters(userInput);
                    }
                }
                else
                {
                    Console.WriteLine("Thank you for patroning and see you next time!");
                    Environment.Exit(0);
                }
            }
            // If we made it this far, we have valid input to analyze:
            lettersMappedToCount = mapLetterCounts(lettersOnly);
            // prompt for style of tabulate display:
            displayStyle = GetInput("display");
            
            // create table for display in Console:
            CreateConsoleTable(displayStyle, lettersMappedToCount);
            // That's it!
            ExitMethod();
        }


        /// ////////////////////////////////////////////////////////////////// ///
        // Helper Methods:

        static string GetInput(string typ)
        {
            string text = null;
            if (typ != null && typ != "")
            {
                if (typ == "initial")
                {
                    Console.WriteLine("Please enter text to be analyzed below");
                    text = Console.ReadLine();

                    while (text == null || text.Length == 0)
                    {
                        Console.WriteLine("Please enter text for analysis to continue");
                        text = Console.ReadLine();
                    }
                }
                else if (typ == "display")
                {
                    Console.WriteLine("Please select the style in which you would like to see your results displayed: Enter 1 for a Horizontal table, enter 2 for a Vertical table.");
                    text = Console.ReadLine();

                    while (text == null || (text != "1" && text != "2"))
                    {
                        Console.WriteLine("Please select a display style. Choose 1 or Horizontal or 2 for Vertical");
                        text = Console.ReadLine();
                    }
                }
                else
                {
                    text = "invalid argument value for typ";
                }
            }
            return text;
        }

        static MatchCollection RegexLetters(string input)
        {
            Regex rx = new Regex(@"[a-zA-Z]",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase);

            return rx.Matches(input);
        }

        static Dictionary<string, int> mapLetterCounts(MatchCollection letters)
        {
            Dictionary<string, int> letterCounts = new Dictionary<string, int>();

            foreach (Match match in letters)
            {
                // check if key (letter) is already in dictionary
                if (letterCounts.Keys.Contains(match.Value.ToLower()))
                {
                    // increment this key's value
                    letterCounts[match.Value.ToLower()]++;
                }
                else
                {
                    // letter hasn't been added to dictionary yet
                    letterCounts.Add(match.Value.ToLower(), 1);
                }
            }
            return letterCounts;
        }

        static void CreateConsoleTable(string displayStyle, Dictionary<string, int> letterMaps)
        {
            // "1" for Horizontal, "2" for Vertical
            if (displayStyle != null)
            {
                // find largest letter count to determine cell width:
                int maxChar = letterMaps.Values.OrderByDescending(str => str.ToString().Length).ToList().First().ToString().Length;
                // no else condition because the while loop in Main prevents this var from having alternate values. Technically the null check above is also not necessary, but used for safety nonetheless
                List<string> tableRows = new List<string>();
                string tableTopBottomBorders = "";
                if (displayStyle == "1")
                {
                    // Horizontal display style
                    // 2 rows. top for letters, bottom for numbers
                    // letters:
                    tableRows.Add(
                        "|" + String.Join(
                            "|",
                            letterMaps.Keys.Select(letter =>
                            {
                                return String.Format(" {0, " + maxChar + "} ", letter);
                            }).ToList()
                        ) + "|"
                    );
                    tableRows.Add(
                        "|" + String.Join(
                            "|",
                            letterMaps.Values.Select(number =>
                            {
                               return String.Format(" {0, " + maxChar + "} ", number);
                            }).ToList()
                        ) + "|"
                    );
                    
                    for (int i = 0; i < tableRows[1].Length; i++)
                    {
                        tableTopBottomBorders = tableTopBottomBorders + "-";
                    }
                    Console.WriteLine("Here are the results:");
                    Console.WriteLine(tableTopBottomBorders);
                    Console.WriteLine(tableRows[0]);
                    Console.WriteLine(tableTopBottomBorders);
                    Console.WriteLine(tableRows[1]);
                    Console.WriteLine(tableTopBottomBorders);
                    
                } 
                else if (displayStyle == "2")
                {
                    // Vertical display style
                    // # rows == # Keys in dictionary
                    foreach (KeyValuePair<string, int> letterCount in letterMaps)
                    {
                        tableRows.Add(
                            String.Format("| {0, " + -maxChar + "} | {1, " + maxChar + "} |", letterCount.Key, letterCount.Value)
                        );
                    }
                    // now get the border length from the first row (they are all the same)
                    for (int i = 0; i < tableRows.First().Length; i++)
                    {
                        tableTopBottomBorders = tableTopBottomBorders + "-";
                    }
                    // finally display:
                    Console.WriteLine("Here are the results:");
                    Console.WriteLine(tableTopBottomBorders);
                    foreach (string row in tableRows)
                    {
                        Console.WriteLine(row);
                        Console.WriteLine(tableTopBottomBorders);
                    }
                    
                }
                Console.WriteLine($"\nThe text has been processed. Total unique letters counted: {letterMaps.Keys.Count}. Total letters in input text: {letterMaps.Values.Sum()}\n");
            }
        }

        static void ExitMethod()
        {
            Console.WriteLine("Thank you for patroning and see you next time!");
            Console.ReadKey(true);
        }
    }
}
