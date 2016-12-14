using System;
using System.IO;

namespace TextSummarizationTool
{
    class PrintResults
    {
        public void print(int sumarizationPercent, int sumTextCount, string sentenceOut, int wordCount, int sentenceCount, int sumTextWordCount, int finalSentenceCount, double elapsedTime, string textOutFile)
        {
            double achievedSummaryPercent = Math.Round(100 * (double)sumTextWordCount / sumTextCount, 2);//calculate the achieved summarization percentage

            var dir = @"Outputs";
            File.WriteAllText(Path.Combine(dir, textOutFile), sentenceOut); //write result to output file
             
            Console.WriteLine("\nOutput text is saved in: {0}", textOutFile);
            Console.WriteLine("\nText Summary: ");
            Console.WriteLine("\n" + sentenceOut + "\n");
      
            Console.WriteLine("\nStatistics: ");
            Console.WriteLine("Summarization factor: {0}%", sumarizationPercent);
            Console.WriteLine("Achieved summarization factor: {0}%", achievedSummaryPercent);
            Console.WriteLine("Required word count: {0}", sumTextCount);
            Console.WriteLine("\nOriginal text word count: {0}", wordCount);
            Console.WriteLine("Orginal text sentence count: {0}", sentenceCount);
            Console.WriteLine("\nSummarized text word count: {0}", sumTextWordCount);
            Console.WriteLine("Summarized text sentence count: {0}", finalSentenceCount);
            Console.WriteLine("Execution time: {0} milliseconds", elapsedTime);

            Console.WriteLine("\n");
        }
    }
}
