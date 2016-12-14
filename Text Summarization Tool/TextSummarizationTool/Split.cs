using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace TextSummarizationTool
{
    class Split
    {
        char[] delimiterChars = {' ', ',', '"', '.', ':', ';', '!', '?', '(', ')', '[', ']', '\'', '\t', '\r', '\n',};
        string stopWords = "";
        int sumTextCountDuplicate = 0;

        public string[] splitText(string text)
        {
            string[] delimitedWords = text.Split(delimiterChars);// Split text

            return delimitedWords;
        }

        public List<string> saveWordsToList(string[] delimitedWords)//extract and save words from text
        {
            var allWords = new List<string>();

            foreach (var word in delimitedWords)
            {
                if (word != "")
                {
                    allWords.Add(word);
                }
            }

            return allWords;
        }

        public List<string> getStopWords()//read stop words from file
        {
            stopWords = File.ReadAllText("stopwords.txt");//read stopword file
            string[] delimitedStopWords = stopWords.Split(delimiterChars);//split stopwords

            var allStopWords = new List<string>();
            foreach (var stop in delimitedStopWords)
            {
                if (stop != "")
                {
                    allStopWords.Add(stop);
                }
            }
            return allStopWords;
        }

        public List<string> extractSentences(string text)//extract whole sentences
        {
            char[] sentenceDelimiter = { '.', '!', '?' };
            string[] delimitedSentences = text.Split(sentenceDelimiter);// Split text

            var allSentences = new List<string>();
            foreach (var sentence in delimitedSentences)
            {
                if (sentence != "")
                {                    
                        allSentences.Add(sentence);
                }
            }
            return allSentences;
        }
        public List<string> extractSentencesImproved(string text)//extract whole sentences
        {

            string[] sentences = Regex.Split(text, @"(?<=[\.!\?])\s+"); //regex works more precise then LINQ split method. Doesn't split abbreviations (i.e, e.g) and keeps punctuation
            var allSentences = new List<string>();

            foreach (var sentence in sentences)
            {
                if (sentence != "")
                {
                    //for texts that contain paragraphs with titles. These usualy dont have any punctuation at the end, so they mix with other sentences
                    if (sentence.Contains("\r\n\r\n"))
                    {
                        var newSentence = sentence.Replace("\r\n\r\n", ". ");
                        string[] thisSentence = Regex.Split(newSentence, @"(?<=[\.!\?])\s+");//split sentence using regex

                        foreach (var s in thisSentence)
                        {
                            allSentences.Add(s);//add sentence to sentence list
                        }
                    }
                    else
                        allSentences.Add(sentence);
                }
            }
            return allSentences;
        }

        public int getRequeiredWordcount(List<string> allWords, int sumPercent)//get required word count
        {
            int wordLenght = allWords.Count();
            double factor = 100;
            double desiredWordCount = (wordLenght * (sumPercent / factor)); //Get output sentence count

            var sumTextCount = Convert.ToInt32(Math.Ceiling(desiredWordCount)); //Round the number and save as count
            sumTextCountDuplicate = sumTextCount;

            return sumTextCount;
        }

        public List<string> removeStopWords(List<string> allWords)//remove stop words from all extraced words
        {
            List<string> filteredWords = new List<string>();
            foreach (string s in allWords)
            {
                if (!stopWords.Contains(s))//if stop words doesn't contain current word
                {
                    filteredWords.Add(s);//add to filtered word list
                }
            }
            return filteredWords;
        }
    }
}
