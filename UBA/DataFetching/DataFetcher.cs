using System.Threading;

namespace UBA
{
    abstract class DataFetcher
    {
        public int samplingTime { get; set; } = 1000;

        protected Thread fetchingThread;

        // init the fetcher
        protected abstract void InitFetcher();

        // starts the fetching thread
        protected abstract void RunFetcher();

        // should be used to parse the data or atleast add it to the events queue
        protected abstract void ParseData(Event e);

        // starts the fetching thread
        public bool StartFetching()
        {
            try
            {
                InitFetcher();

                fetchingThread = new Thread(new ThreadStart(RunFetcher));
                fetchingThread.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // check if the thread is running
        public bool FetchingRunning()
        {
            if (fetchingThread.ThreadState == System.Threading.ThreadState.Running)
                return true;
            else
                return false;
        }

        // stops the fetching thread
        public virtual bool StopFetching()
        {
            try
            {
                fetchingThread.Abort();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
