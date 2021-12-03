using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace lab_3
{
    internal class Program
    {
        private static void Main()
        {
            int size;
            try
            {
                size = ReadUserInput();
            }
            catch
            {
                return;
            }

            var (vectorA, vectorB) = GenerateData(size);
            PrintData(vectorA, nameof(vectorA));
            PrintData(vectorB, nameof(vectorB));

            // Single Thread
            Console.WriteLine("Single Thread results.");
            var watch = Stopwatch.StartNew();
            var result = CalculateFunctionSingleThread(vectorA, vectorB);
            watch.Stop();
            PrintData(result, nameof(result));
            Console.WriteLine($"Single thread, time of execution {watch.ElapsedMilliseconds} ms");
            Console.WriteLine();

            // Multiple Thread
            Console.WriteLine("Multiple Thread results.");
            watch = Stopwatch.StartNew();
            result = CalculateFunctionMultipleThread(vectorA, vectorB);
            watch.Stop();
            PrintData(result, nameof(result));
            Console.WriteLine($"Multiple thread, time of execution {watch.ElapsedMilliseconds} ms");
            Console.WriteLine();

            // Multiple Thread with Delays
            Console.WriteLine("Multiple Thread with Delays results.");
            watch = Stopwatch.StartNew();
            result = CalculateFunctionMultipleThreadWithDelays(vectorA, vectorB);
            watch.Stop();
            PrintData(result, nameof(result));
            Console.WriteLine($"Multiple thread with Delays, time of execution {watch.ElapsedMilliseconds} ms");
            Console.WriteLine();

            // Single Thread Smart
            Console.WriteLine("Single Thread Smart results.");
            watch = Stopwatch.StartNew();
            result = CalculateFunctionSingleThreadSmart(vectorA, vectorB);
            watch.Stop();
            PrintData(result, nameof(result));
            Console.WriteLine($"Single thread Smart, time of execution {watch.ElapsedMilliseconds} ms");
            Console.WriteLine();

            // Multiple Thread Smart
            Console.WriteLine("Multiple Thread Smart results.");
            watch = Stopwatch.StartNew();
            result = CalculateFunctionMultipleThreadSmart(vectorA, vectorB);
            watch.Stop();
            PrintData(result, nameof(result));
            Console.WriteLine($"Multiple thread Smart, time of execution {watch.ElapsedMilliseconds} ms");
            Console.WriteLine();
        }

        #region Investigated functions

        private static List<int> CalculateFunctionSingleThread(List<int> vectorA, List<int> vectorB)
        {
            var multiplyResult = Multiply(vectorB, 7);
            var sumResult = Sum(vectorA, multiplyResult);
            var finalResult = Multiply(sumResult, 3);
            return finalResult;
        }

        private static List<int> CalculateFunctionMultipleThread(List<int> vectorA, List<int> vectorB)
        {
            var multiplyResult = Task.Run(() =>
            {
                var result = Multiply(vectorB, 7);
                PrintData(result, "Thread 1, results");
                return result;
            }).Result;

            var sumResult = Task.Run(() =>
            {
                var result = Sum(vectorA, multiplyResult);
                PrintData(result, "Thread 2, results");
                return result;
            }).Result;

            var finalResult = Task.Run(() =>
            {
                var result = Multiply(sumResult, 3);
                PrintData(result, "Thread 3, results");
                return result;
            }).Result;

            return finalResult;
        }

        private static List<int> CalculateFunctionMultipleThreadWithDelays(List<int> vectorA, List<int> vectorB)
        {
            var multiplyResult = Task.Run(() =>
            {
                var result = MultiplySlow(vectorB, 7);
                PrintData(result, "Thread 1, results");
                return result;
            }).Result;

            var sumResult = Task.Run(() =>
            {
                var result = SumSlow(vectorA, multiplyResult);
                PrintData(result, "Thread 2, results");
                return result;
            }).Result;

            var finalResult = Task.Run(() =>
            {
                var result = MultiplySlow(sumResult, 3);
                PrintData(result, "Thread 3, results");
                return result;
            }).Result;

            return finalResult;
        }

        private static List<int> CalculateFunctionSingleThreadSmart(List<int> vectorA, List<int> vectorB)
        {
            var firstMultiplyResult = MultiplySlow(vectorB, 21);
            var secondMultiplyResult = MultiplySlow(vectorA, 3);
            var finalResult = SumSlow(firstMultiplyResult, secondMultiplyResult);
            return finalResult;
        }

        private static List<int> CalculateFunctionMultipleThreadSmart(List<int> vectorA, List<int> vectorB)
        {
            var firstMultiplyTask = Task.Run(() => MultiplySlow(vectorB, 21));
            var secondMultiplyTask = Task.Run(() => MultiplySlow(vectorA, 3));

            Task.WaitAll(firstMultiplyTask, secondMultiplyTask);

            var firstMultiplyResult = firstMultiplyTask.Result;
            var secondMultiplyResult = secondMultiplyTask.Result;
            var finalResult = Task.Run(() => SumSlow(firstMultiplyResult, secondMultiplyResult)).Result;
            return finalResult;
        }

        #endregion

        #region Print

        private static void PrintData(IEnumerable<int> enumerable, string header)
        {
            Console.Write($"{header}: ");
            foreach (var item in enumerable) Console.Write($"{item} ");
            Console.WriteLine();
        }

        #endregion

        #region MathOperationsFast

        private static List<int> Sum(List<int> vector, int k)
        {
            return vector.Select(n => n + k).ToList();
        }

        private static List<int> Sum(List<int> first, List<int> second)
        {
            if (first.Count != second.Count)
                throw new ArgumentException("Pass lists with the same size!");

            var zippedLists = first.Zip(second, (f, s) => new {First = f, Second = s});

            return zippedLists.Select(item => item.First + item.Second).ToList();
        }

        private static List<int> Multiply(List<int> vector, int k)
        {
            return vector.Select(n => n * k).ToList();
        }

        private static List<int> Multiply(List<int> first, List<int> second)
        {
            if (first.Count != second.Count)
                throw new ArgumentException("Pass lists with the same size!");

            var zippedLists = first.Zip(second, (f, s) => new {First = f, Second = s});

            return zippedLists.Select(item => item.First * item.Second).ToList();
        }

        #endregion

        #region MathOperationsSlow

        private static List<int> SumSlow(List<int> vector, int k)
        {
            Thread.Sleep(300);
            return vector.Select(n => n + k).ToList();
        }

        private static List<int> SumSlow(List<int> first, List<int> second)
        {
            Thread.Sleep(300);
            if (first.Count != second.Count)
                throw new ArgumentException("Pass lists with the same size!");

            var zippedLists = first.Zip(second, (f, s) => new {First = f, Second = s});

            return zippedLists.Select(item => item.First + item.Second).ToList();
        }

        private static List<int> MultiplySlow(List<int> vector, int k)
        {
            Thread.Sleep(300);
            return vector.Select(n => n * k).ToList();
        }

        private static List<int> MultiplySlow(List<int> first, List<int> second)
        {
            Thread.Sleep(300);
            if (first.Count != second.Count)
                throw new ArgumentException("Pass lists with the same size!");

            var zippedLists = first.Zip(second, (f, s) => new {First = f, Second = s});

            return zippedLists.Select(item => item.First * item.Second).ToList();
        }

        #endregion

        #region Initialization

        private static (List<int>, List<int>) GenerateData(int size)
        {
            var rnd = new Random();
            var vectorA = Enumerable.Range(0, size).Select(_ => rnd.Next(1, 20)).ToList();
            var vectorB = Enumerable.Range(0, size).Select(_ => rnd.Next(1, 20)).ToList();

            return (vectorA, vectorB);
        }

        private static int ReadUserInput()
        {
            while (true)
            {
                Console.WriteLine("Enter q to exit.");
                Console.Write("Enter size of vectors: ");
                var input = Console.ReadLine();
                if (int.TryParse(input, out var size))
                {
                    if (size <= 0)
                    {
                        Console.WriteLine("Size must be positive integer.");
                        continue;
                    }

                    return size;
                }

                if (input == "q")
                    throw new Exception();
            }
        }

        #endregion
    }
}