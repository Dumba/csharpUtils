using System;
using System.Threading;
using System.Threading.Tasks;

namespace test_console
{
    public class Threading
    {
        public static void Test()
        {
            var task1 = Task.Run(() => TaskWithLock(1));
            var task2 = Task.Run(() => TaskWithLock(2));

            Task.Delay(1100).GetAwaiter().GetResult();
            var task3 = Task.Run(() => TaskWithLock(3));
            var task4 = Task.Run(() => TaskWithLock(4));

            Task.WaitAll(task1, task2, task3, task4);
        }

        public static void TaskWithLock(int id)
        {
            Console.WriteLine($"START - {id}");
            Threading.RunOrWaitForResult(() => longTask(id), threadLock);
            Console.WriteLine($"END - {id} - {_id}");
        }
        public static void longTask(int id)
        {
            Console.WriteLine($"DOING TASK - {id}");
            Task.Delay(1000).GetAwaiter().GetResult();
            _id = id;
        }

        private static int _id = 0;
        private static object threadLock = new object();

        public static void RunOrWaitForResult(Action action, object lockObject)
        {
            // try lock thread
            Monitor.TryEnter(lockObject);

            // first thread
            if (Monitor.IsEntered(lockObject))
            {
                try
                {
                    // get data
                    action();
                }
                finally
                {
                    // release lock
                    Monitor.Exit(lockObject);
                }
            }
            // other thread is getting data
            else
            {
                // just wait for it
                lock (lockObject)
                {
                }
            }
        }
    }
}