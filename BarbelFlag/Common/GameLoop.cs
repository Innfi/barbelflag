using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;


namespace BarbelFlag
{
    public class GameLoop
    {
        public delegate int UpdateDelegate();
        protected UpdateDelegate updateDelegate;
        protected Stopwatch watch;
        public int DeltaTime { get; protected set; }
        public bool IsRunning { get; protected set; }
        protected Task task;


        public GameLoop(UpdateDelegate callback)
        {
            DeltaTime = 0;
            updateDelegate = callback;
            watch = new Stopwatch();
            IsRunning = false;
            task = new Task(MainLoop);
        }

        public void Start()
        {
            IsRunning = true;
            task.Start();
        }

        public void Stop()
        {
            IsRunning = false;
            task.Wait();
        }

        public void MainLoop()
        {
            while (IsRunning)
            {
                if (DeltaTime >= 0) Thread.Sleep(DeltaTime);
                DeltaTime = 0;

                watch.Start();

                for (int i = 0; i < 60; i++) updateDelegate();

                watch.Stop();

                var loopElapsed = watch.ElapsedMilliseconds;
                DeltaTime += (int)(1000 - loopElapsed);
            }
        }
    }
}
