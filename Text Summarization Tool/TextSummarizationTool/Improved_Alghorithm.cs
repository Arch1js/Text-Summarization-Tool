using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace TextSummarizationTool
{
    //SID 1432223/1
    class Improved_Alghorithm
    {
        // Delimiter characters used to know where one word ends and other starts
        char[] delimiterChars = { 
                ' ', ',', '"', '.', ':', ';', '!', '?', '(', ')', '[', ']', '\'', '\t', '\r', '\n' };

        private string text = "";
        private int sumTextCountDuplicate = 0;
        private int sumTextWordCount = 0;

        private string textOutFile = "";
        private string fileName = ""; //static file name for testing   

        public void improvedSortAlghorithm(int sumPercent, int sumTextCountPass, string fileNameInput, string fileNameOutput)//improved alghorithm
        {
            Console.SetWindowSize(100, 60);//set windows size to 100x60

            fileName = fileNameInput;
            textOutFile = fileNameOutput + "-Improved.txt";//file name of improved algorithm
            var watch = System.Diagnostics.Stopwatch.StartNew();//start a new timer for measuring perfomance of the algorithm

            sumTextCountDuplicate = sumTextCountPass;
            int sumTextCount = sumTextCountPass;

            var dir = @"Inputs";//local directory for inputs
            text = File.ReadAllText(Path.Combine(dir, fileName)); //save contents of file to a variable
            text = text.Replace("\"", " \" ");

            Split splitFunctions = new Split();

            string sentenceOut = "";

            //tuple lists and lists for storing information as text is analyzed
            var summaryText = new List<Tuple<string,string, int, int>>();
            var compareList = new List<Tuple<string, string, int, int>>();
            var allSentencesIndexed = new List<Tuple<string, int>>();
            var allSentencesReady = new List<Tuple<string, int>>();
            var stemWordList = new List<string>();

            var delimitedWords = splitFunctions.splitText(text); //split text in to words array
            var allWords = splitFunctions.saveWordsToList(delimitedWords); //save split words to list
            var stopWords = splitFunctions.getStopWords(); //extract stop words and save to list
            var allSentences = splitFunctions.extractSentencesImproved(text); //extract and save whole sentences

            var filteredWords = splitFunctions.removeStopWords(allWords); //remove stop words 

            foreach (var word in filteredWords)//stem all filtered words
            {
                Stemmer ps = new Stemmer();
                string stemWord = ps.StemWord(word);//get stem of a current word
                stemWordList.Add(stemWord);
            }

            var queryByGroup = filteredWords.GroupBy(i => i); //Group all the filtered words 
            var wordsSorted = queryByGroup.OrderByDescending(i => i.Count()); //Get count of how many times each word appears

            int sentenceIndex = 0;
            foreach (var sentence in allSentences) //get and save sentence and their original index in text
            {
                allSentencesIndexed.Add(new Tuple<string, int>(sentence, sentenceIndex));
                sentenceIndex++;
            }

            foreach (var word in wordsSorted) //loop trough each sorted word
            {
                var wordCount = 0;

                foreach (var sentences in allSentencesIndexed) //loop trough all sentences
                {
                    wordCount = 0;
                    List<string> wordList = sentences.Item1.Split(' ', ',', '-', '"', '.').ToList(); //split the sentence in to single words
                    var stemList = new List<string>();

                    Stemmer ps = new Stemmer();

                    foreach (var splitWord in wordList)//convert each word to its stem
                    {
                        string stemWord = ps.StemWord(splitWord);
                        stemList.Add(stemWord); //add stem to stem word list
                    }

                    foreach (var stemWord in stemList)
                    {
                        if (stemWord == word.Key)//count the occurences of word in sentence
                        {
                            wordCount++;
                        }
                    }

                    compareList.Add(new Tuple<string, string, int, int>(sentences.Item1, word.Key, wordCount, sentences.Item2)); //save the sentence, key word, uccurence count and its original index in to temporary compare list
                }

                if (allSentencesIndexed.Count != 0) //if there is still sentences to analyze
                {
                    compareList.Sort((x, y) => y.Item3.CompareTo(x.Item3)); //sort the sentences by the count of key word appearing in text  

                    summaryText.Add(new Tuple<string, string, int, int>(compareList.First().Item1,compareList.First().Item2, compareList.First().Item3, compareList.First().Item4)); //save sentence, its original text index and word count in to temporary list for further analysis

                    allSentencesIndexed.RemoveAll(item => item.Item1 == compareList.First().Item1); //remove sentence from list of sentences that needs to be analyzed

                }
                compareList.Clear(); //clear list and memory for next sentence           
            }

            summaryText.Sort((x, y) => y.Item3.CompareTo(x.Item3));//sort the list by the common word occurence

            foreach (var sentence in summaryText) //loop trough each sentence in list ordered by original index
            {
                string[] delimitedSortedWords = sentence.Item1.Split(delimiterChars);// split text

                var sentenceWords = new List<string>();

                foreach (var word in delimitedSortedWords)//remove blank spaces from delimited word list and to list
                {
                    if (word != "")
                    {
                        sentenceWords.Add(word);//add word to sentence word list
                    }
                }
                string currentSentence = sentence.Item1;
                int index = sentence.Item4;

                if (sentence.Item1 != " ")
                {
                    if (sumTextCount != 0)//if summarized text has some words left
                    {
                        if (sentenceWords.Count() <= sumTextCount)//if current sentence words are less or equal of words left in summarization text output
                        {
                            allSentencesReady.Add(new Tuple<string, int>(currentSentence, index));//add sentence to output list

                            sumTextCount -= sentenceWords.Count();//lower the count of SF word count 
                            sumTextWordCount += sentenceWords.Count();//add used word count to total used word statistics
                        }
                    }
                    else
                    {
                        break;
                    }                                    
                }                
            }
            if (allSentencesReady.Count() == 0)//if no analyzed sentences are long enough to be included in summarization
            {
                Console.WriteLine("\nUnfortunately your summarization factor is to low to produce meaningful results! Please try again!");
                Console.WriteLine("\n");

                Original_Alghorithm ua = new Original_Alghorithm();
                ua.askQuestions();//return to start
            }
            else
            {
                foreach (var sentence in allSentencesReady.OrderBy(x => x.Item2))//order sentences by their original index and remove any line breaks
                {
                    var sentenceTrim = "";
                                        
                    sentenceTrim = sentence.Item1.Replace("\r\n", string.Empty);//trim out the line breaks
                    sentenceOut += sentenceTrim;
                }

                watch.Stop();//stop the timer
                double elapsedMs = watch.ElapsedMilliseconds;//save elapsed time in milliseconds

                PrintResults printResults = new PrintResults();

                Console.WriteLine("\n//////////////////////////////////////////");
                Console.WriteLine("\nThis is improved alghorithm!");

                //pass parameters for printing
                printResults.print(sumPercent, sumTextCountDuplicate, sentenceOut, allWords.Count, allSentences.Count, sumTextWordCount, allSentencesReady.Count, elapsedMs, textOutFile);

                Original_Alghorithm ua = new Original_Alghorithm();
                ua.askQuestions();
            }          
            Console.ReadKey();
        }
    }
}
