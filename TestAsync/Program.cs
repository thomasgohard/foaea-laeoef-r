namespace TestAsync
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting");

            await Func1();
            await Func2();
            await Func3();

            //await Task.Factory.StartNew(async () =>
            //{

            //    await Task.Factory.StartNew(() =>
            //    {
            //        Thread.Sleep(1000);
            //        Console.WriteLine("Completed 1");
            //    });

            //    await Task.Factory.StartNew(() =>
            //    {
            //        Thread.Sleep(1000);
            //        Console.WriteLine("Completed 2");
            //    });

            //    await Task.Factory.StartNew(() =>
            //    {
            //        Thread.Sleep(1000);
            //        Console.WriteLine("Completed 3");
            //    });

            //});

            Console.WriteLine("Completed");
            Console.ReadLine();
        }

        static async Task Func1()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(1000);
                Console.WriteLine("Completed 1");
            });
        }
        static async Task Func2()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(1000);
                Console.WriteLine("Completed 2");
            });
        }
        static async Task Func3()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(1000);
                Console.WriteLine("Completed 3");
            });
        }
    }
}