using System;
using System.Threading;

namespace philosophers_os
{
    class Fork
    {
        private Mutex m = new Mutex();

        public void take()
        {
            m.WaitOne();
        }

        public void put()
        {
            m.ReleaseMutex();
        }
    };

    class Philosopher
    {
        int id;
        Fork forkMinIndex;
        Fork forkMaxIndex;
        uint eat_count;
        double wait_time;
        DateTime wait_start;
        bool stop_flag;
        bool debug_flag;
        Random random;

        void think()
        {
            if (this.debug_flag)
                Console.WriteLine(this.id + " thinking");

            Thread.Sleep(this.random.Next(0, 100));
            if (this.debug_flag)
                Console.WriteLine(this.id + " hungry");

            this.wait_start = DateTime.Now;
        }

        void eat()
        {
            this.wait_time += DateTime.Now.Subtract(this.wait_start).TotalMilliseconds;
            if (this.debug_flag)
                Console.WriteLine(this.id + " eating");

            Thread.Sleep(this.random.Next(0, 100));
            eat_count++;
        }

        public Philosopher(int number, Fork first, Fork second, bool dbg)
        {
            this.id = number;
            this.forkMinIndex = first;
            this.forkMaxIndex = second;
            this.eat_count = 0;
            this.wait_time = 0;
            this.debug_flag = dbg;
            this.stop_flag = false;
            this.random = new Random();
        }

        public void run()
        {
            while (!stop_flag)
            {
                think();

                this.forkMinIndex.take();
                if (this.debug_flag)
                    Console.WriteLine(this.id + " took first fork");

                this.forkMaxIndex.take();
                if (this.debug_flag)
                    Console.WriteLine(this.id + " took second fork");

                eat();

                this.forkMaxIndex.put();
                if (this.debug_flag)
                    Console.WriteLine(this.id + " put right fork");

                this.forkMinIndex.put();
                if (this.debug_flag)
                    Console.WriteLine(this.id + " put left fork");
            }
        }

        public void stop()
        {
            stop_flag = true;
        }

        public void printStats()
        {
            Console.WriteLine(this.id + " " + this.eat_count + " " + Convert.ToInt32(this.wait_time));
        }
    };

    class Program
    {
        static void Main(string[] args)
        {
            int N = 1000;
            bool dbg = false;
            int duration = 60000;

            Fork[] forks = new Fork[N];
            for (int i = 0; i < N; i++)
                forks[i] = new Fork();

            Philosopher[] phils = new Philosopher[N];
            for (int i = 0; i < N; i++)
            {
                var min_index_fork = Math.Min((i + 1) % N, i);
                var max_index_fork = Math.Max((i + 1) % N, i);
                phils[i] = new Philosopher(i + 1, forks[min_index_fork], forks[max_index_fork], dbg);
            }

            Thread[] runners = new Thread[N];
            for (int i = 0; i < N; i++)
                runners[i] = new Thread(phils[i].run);
            for (int i = 0; i < N; i++)
                runners[i].Start();

            Thread.Sleep(duration);

            for (int i = 0; i < N; i++)
                phils[i].stop();

            for (int i = 0; i < N; i++)
                runners[i].Join();

            for (int i = 0; i < N; i++)
                phils[i].printStats();
        }
    }
}
