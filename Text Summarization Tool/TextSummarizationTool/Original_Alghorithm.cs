using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace TextSummarizationTool
{
    public class Original_Alghorithm
    {
        // Delimiter character used to know where one word ends and other starts
        char[] delimiterChars = {
                ' ', ',', '"', '.', ':', ';', '!', '?', '(', ')', '[', ']', '\'', '\t', '\r', '\n' };
        private int sumPercent = 0;
        private string text = "";
        private int sumTextCountDuplicate = 0;
        private int sumTextWordCount = 0;

        private string txtOutFileName = "";
        private string txtOutFileNameImproved = "";

        private string fileName = "";
        private string defaultText = "networking.txt";//default text file for analyzing
        private string defaultName = "summarizedText";//default name for text output

        static void Main(string[] args)
        {
            Console.SetWindowSize(100, 60);//set windows size to 100x60

            Original_Alghorithm ua = new Original_Alghorithm();
            ua.askQuestions();
        }

        public void askQuestions()//ask for user input
        {
            Console.WriteLine("Please enter input file name (without extension): ");//Ask question
            fileName = Console.ReadLine(); //read input file name

            Console.WriteLine("Please enter output file name (without extension): ");
            string input = Console.ReadLine(); //read output file name

            txtOutFileNameImproved = input;
            
            sumTextCountDuplicate = 0;//reset to 0 when cycle runs again
            sumTextWordCount = 0;

            try
            {
                Console.Write("Please enter summarization factor (1-100%): ");
                int userInput = Convert.ToInt32(Console.ReadLine());
                validateUserInput(fileName, input, userInput);//pass values for validation
            }
            catch
            {
                Console.Write("\n//////////////////|||\\\\\\\\\\\\\\\\\\");//in case user enters a letter instead of number
                Console.Write("\nSummarization factor should be a number!");
                Console.Write("\n//////////////////|||\\\\\\\\\\\\\\\\\\");
                Console.Write("\n");
                askQuestions();
            }
                        
        }

        private void validateUserInput(string fileNameInput, string fileNameOutput, int userInput)
        {
            sumPercent = userInput;

            if (sumPercent > 100 || sumPercent < 1)//dont allow values outside 1-100 
            {
                Console.WriteLine("\nNumber should be between 1 and 100! Try again...");
                askQuestions();
            }
            else
            {
                if (fileNameInput == "")//if input is blank, analyze default text file
                {
                    fileName = defaultText;
                    Console.WriteLine("\nNo input file received! Using default text file: " + defaultText);
                }
                else
                {
                    fileName = fileNameInput + ".txt";//set input file and add file extension
                }

                if (fileNameOutput == "")//if no output name is specified, default value will be used
                {
                    Console.WriteLine("\nNo output file name specified! Default file name '" + defaultName + "' will be used.");
                    txtOutFileName = defaultName + ".txt";
                    txtOutFileNameImproved = defaultName;
                }
                else
                {
                    txtOutFileName = fileNameOutput + ".txt";
                }

                this.sortAlghorithm();
            }   
                  
        }

        private void sortAlghorithm()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();//start a new timer to measure performance (execution time) of this alghorithm

            try
            {
                var dir = @"Inputs";
                text = File.ReadAllText(Path.Combine(dir, fileName)); //save contents of file to a variable
                text = text.Replace("\"", " \" ");
            }
            catch //if input file name is invalid, display error message and return to start
            {
                Console.WriteLine("\n");
                Console.WriteLine("\n/////////////////////||||||||\\\\\\\\\\\\\\\\\\\\\\\\\\");
                Console.WriteLine("\nOops, something went wrong! File {0} cannot be found or read. Please try again!", fileName);
                Console.WriteLine("\n/////////////////////||||||||\\\\\\\\\\\\\\\\\\\\\\\\\\");
                Console.WriteLine("\n");
                askQuestions();
            }

            Split splitFunctions = new Split();
            
            string sentenceOut = "";

            var summaryText = new List<Tuple<string, int, int>>(); //summary text list
            var compareList = new List<Tuple<string, int, int>>(); //list for comparing sentences
            var allSentencesIndexed = new List<Tuple<string, int>>(); //list for storing all entences with their indexed place in text
            var allSentencesReady = new List<Tuple<string, int>>(); //output list of sentences

            var delimitedWords = splitFunctions.splitText(text); //split text in to words array
            var allWords = splitFunctions.saveWordsToList(delimitedWords); //save split words to list
            var stopWords = splitFunctions.getStopWords(); //extract stop words and save to list
            var allSentences = splitFunctions.extractSentences(text); //extract and save whole sentences
            var sumTextCount = splitFunctions.getRequeiredWordcount(allWords, sumPercent); //get required output lenght
            sumTextCountDuplicate = sumTextCount; 
            var filteredWords = splitFunctions.removeStopWords(allWords); //remove stop words 


            var queryByGroup = filteredWords.GroupBy(i => i); //Group all the filtered words 
            var wordsSorted = queryByGroup.OrderByDescending(i => i.Count()); //Get count of how many times each word appears

            int sentenceIndex = 0;
            foreach (var sentence in allSentences) //get and save sentence and their original index in text
            {
                allSentencesIndexed.Add(new Tuple<string, int>(sentence, sentenceIndex));
                sentenceIndex++;
            }

            foreach (var word in wordsSorted) //loop trough each filtered word
            {
                var wordCount = 0;

                foreach (var sentences in allSentencesIndexed) //loop trough all sentences
                {
                    List<string> wordList = sentences.Item1.Split(' ', ',', '-', '"').ToList(); //split the sentence in to single words

                    wordCount = wordList.Where(r => r.ToLower() == word.Key.ToLower()).Count(); //get the count of key word in sentence

                    string sentence = sentences.Item1.ToString();

                    compareList.Add(new Tuple<string, int, int>(sentences.Item1, wordCount, sentences.Item2)); //save the sentence, key word count and its original index in to temporary compare list
                }

                if (allSentencesIndexed.Count != 0) //if there is still sentences to analyze
                {
                    compareList.Sort((x, y) => y.Item2.CompareTo(x.Item2)); //sort the sentences by the count of key word appearing in text  

                    summaryText.Add(new Tuple<string, int, int>(compareList.First().Item1, compareList.First().Item2, compareList.First().Item3)); //save sentence, its original text index and word count in to temporary list for further analysis

                    allSentencesIndexed.RemoveAll(item => item.Item1 == compareList.First().Item1); //remove sentence from list of sentences that needs to be analyzed

                }
                compareList.Clear(); //clear list and memory for next sentence           
            }

            summaryText.Sort((x, y) => y.Item2.CompareTo(x.Item2));

            foreach (var sentence in summaryText) //loop trough each sentence in list ordered by original index
            {
                string[] delimitedSortedWords = sentence.Item1.Split(delimiterChars);// Split text

                var sentenceWords = new List<string>();

                foreach (var word in delimitedSortedWords) //remove blank spaces
                {
                    if (word != "") //remove blank spaces
                    {
                        sentenceWords.Add(word); //add all words from current sentence to list
                    }
                }

                string currentSentence = sentence.Item1;
                int index = sentence.Item3;

                if (sentence.Item1 != " ")//ignore blank words
                {
                    if (sumTextCount != 0)//if required summary text lenght is not reached yet
                    {
                        if (sentenceWords.Count() <= sumTextCount) //if there are enough space left for one more sentence
                        {
                            allSentencesReady.Add(new Tuple<string, int>(currentSentence, index));//add sentence to output list

                            sumTextCount -= sentenceWords.Count(); //lower the word count thet are still needed to reach the output word lenght
                            sumTextWordCount += sentenceWords.Count(); //keep track of already added words
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (allSentencesReady.Count() == 0)//if user entered summarization factor doesnt produce any results, display error message
            {
                Console.WriteLine("\n");
                Console.WriteLine("\n/////////////////////||||||||\\\\\\\\\\\\\\\\\\\\\\\\\\");
                Console.WriteLine("\nUnfortunately your summarization factor is to low to produce meaningful results! Please try again!");
                Console.WriteLine("\n/////////////////////||||||||\\\\\\\\\\\\\\\\\\\\\\\\\\");
                Console.WriteLine("\n");
                askQuestions();
            }
            else
            {
                foreach (var sentence in allSentencesReady.OrderBy(x => x.Item2)) //order sentences by their original appearance in text
                {
                    var sentenceTrim = "";
                    sentenceTrim = sentence.Item1.Replace("\r\n", string.Empty);
                    sentenceOut += sentenceTrim + '.'; //add back the missing punctuation (not precise as the original punctuation is lost upon sentence extraction. This is improved in other alghorithm)
                }

                watch.Stop();//stop the perfomance timer
                double elapsedMs = watch.ElapsedMilliseconds;

                PrintResults printResults = new PrintResults();

                Console.WriteLine("\n//////////////////////////////////////////");
                Console.WriteLine("\nThis is original alghorithm!");

                //send all data to print method and display in console
                printResults.print(sumPercent, sumTextCountDuplicate, sentenceOut, allWords.Count, allSentences.Count, sumTextWordCount, allSentencesReady.Count, elapsedMs, txtOutFileName);
                

                Improved_Alghorithm ia = new Improved_Alghorithm();
                ia.improvedSortAlghorithm(sumPercent, sumTextCountDuplicate, fileName, txtOutFileNameImproved);//run the improved algorithm
            }
            Console.ReadKey();
        }
        
    }
}
