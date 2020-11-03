using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace ThreadApp
{
    public class Program
    {
        public static Dictionary<string, int> wordDictionary = new Dictionary<string, int>();
        public static List<string> sentenceList = new List<string>();

        public static void Main(string[] args)
        {
            //Console.WriteLine("Main Thread Started");
            DoMainThread();

            CallSubThreads();

            ShowResult();
            
            //Console.WriteLine("Main Thread Ended");
            Console.ReadKey();

        }

        public static void CallSubThreads()
        {
            int threadCount = 5;
            Thread[] threads = new Thread[threadCount];
            for (int n = 0; n < threads.Length; n++)
            {
                if (sentenceList.Count > 0)
                {
                    threads[n] = new Thread(DoSubThread);
                    threads[n].Start();
                    threads[n].Join();
                }
            }
        }

        public static void DoSubThread()
        {
            if(sentenceList != null && sentenceList.Count> 0)
            {
                var selectedSentence = sentenceList.FirstOrDefault();
                GetWordCountOnSentence(selectedSentence);
                CalculateEachWordCountinAllText(selectedSentence);
                sentenceList.Remove(selectedSentence);
            }
        }

        public static void DoMainThread()
        {
            string textFile;
            string text = string.Empty;

            Console.Write("Please enter to path of the text file : ");

            textFile = Console.ReadLine();

            try
            {
                 text = File.ReadAllText(textFile);
            }
            catch (Exception e)
            {

                Console.WriteLine("File read error  {0}", e.Message);
                Console.WriteLine("Please stop the app and try again");
                Console.Read();
            }

            int sentencesCount = 0;
            int avgWordCount = 0;
            int totalWordCount = 0;


            string[] sentences = Regex.Split(text, @"(?<=[\.!\?])\s+");
            sentencesCount = sentences.Length;

            List<string> _senteces = new List<string>();

            foreach (string sentence in sentences)
            {
                var afterRemoveLastChar = sentence.Remove(sentence.Length - 1, 1);
                _senteces.Add(afterRemoveLastChar);
                sentenceList.Add(afterRemoveLastChar);
            }

            foreach (var sentence in _senteces)
            {
                totalWordCount += GetWordCountOnSentence(sentence);
            }

            avgWordCount = totalWordCount / sentencesCount;
            Console.WriteLine("Sentece Count: {0}", sentencesCount);
            Console.WriteLine("Avg. Word Count:{0} ", avgWordCount);
        }

        public static int GetWordCountOnSentence(string sentence)
        {

            string[] words = sentence.Split(' ');

            return words.Length; 
        }

        public static void CalculateEachWordCountinAllText(string sentence)
        {
            string[] words = sentence.Split(' ');

            foreach (var word in words)
            {
                if (wordDictionary.TryGetValue(word, out int currentCount))
                {
                    wordDictionary[word] = currentCount + 1;
                }
                else
                {
                    wordDictionary.Add(word, 1);
                }

            }
        }

        public static void ShowResult()
        {
            foreach (var item in wordDictionary.OrderByDescending(key => key.Value))
            {
                Console.WriteLine("{0} , {1}", item.Key, item.Value);
            }
        }
    }
}
