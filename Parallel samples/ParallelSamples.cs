using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Samples
{
    public enum OperationResult
    {
        OK,
        Error
    }  

    public interface IOperation
    {
        OperationResult DoWork();
    }


    /// <summary>
    /// Mockup operation class
    /// </summary>
    public class Operation : IOperation
    {
        public OperationResult DoWork()
        {
            for(int i = 0; i < 10000000; i++)
            {
                long ii = i * i;
            }

            return OperationResult.OK;
        }
    }

    public static class ParallelSamples
    {
        /// <summary>
        /// Example of itarrating over an array in parallel,
        /// the number of threads is limited by a number of logical processors
        /// This is a useful way to perform parallel calculation on Large sets of data 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="factor"></param>
        public static void IncreaseByFactor(double[] array, double factor)
        {
            // get the number of logical processors
            int degreeOfParallelism = Environment.ProcessorCount;

            // calculate the length of each chunk that will run in parallel
            int chunkLength = array.Length / degreeOfParallelism;

            Parallel.For(0, degreeOfParallelism, workerId =>
            {
                var chunkMaxIndex = chunkLength * (workerId + 1);
                for (int i = chunkLength * workerId ; i < chunkMaxIndex; i++)
                    array[i] = array[i] * factor;
            });
        }

        ///<summary>
        /// Create task for each operation
        ///</summary>
        ///<param name="operations">Operations to complete</param>
        public static IEnumerable<OperationResult> TaskBasedParallelCalculation(IEnumerable<IOperation> operations)
        {
            // we can create custom task scheduler that will suit our needs
            //TaskFactory factory = new TaskFactory(TaskScheduler.Current);

            List<Task> runningTasks = new List<Task>();
            OperationResult[] resultList = new OperationResult[operations.Count()];

            for(int i = 0; i < operations.Count(); i++ )
            {
                IOperation operation = operations.ElementAt(i);

                // if we use "i" it wil use the last index
                int index = i;

                Task runningTask = Task.Factory.StartNew(op =>
               {
                   // we can set the priority of current thread
                   //Thread.CurrentThread.Priority = ThreadPriority.Highest;

                   // do some calculations
                   OperationResult operationResult = operation.DoWork();
                   resultList[index] = operationResult;

               }, operation); //, TaskCreationOptions.LongRunning); // we can specify TaskCreationOptions in task creatio
                runningTasks.Add(runningTask);
            }
            Task.WaitAll(runningTasks.ToArray());

            return resultList;
        }

    }
}
