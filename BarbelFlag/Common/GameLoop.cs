using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System;


namespace BarbelFlag
{
    public class GameLoop
    {
        public delegate int UpdateDelegate();
        protected UpdateDelegate updateDelegate;
        protected Stopwatch watch;
        public int DeltaTime { get; protected set; }
        public bool IsRunning { get; protected set; }
        protected long msPerFrame = (long)TimeSpan.FromSeconds(1 / 60).TotalMilliseconds;

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
            watch.Start();
            var previous = watch.ElapsedMilliseconds;
            var lag = 0.0;

            while (IsRunning)
            {                
                var current = watch.ElapsedMilliseconds;
                var elapsed = current - previous;

                previous = current;
                lag += elapsed;

                while (lag * 100 >= 16)
                {
                    updateDelegate();
                    lag -= 16;
                }
            }

            watch.Stop();
        }
    }
}
