using md2visio.main;
using md2visio.struc.figure;

namespace md2visio.main
{
    /// <summary>
    /// 控制台应用程序入口点
    /// </summary>
    public static class ConsoleApp
    {
        public static void Main(string[] args)
        {
            try
            {
                if (args.Length > 0 && args[0] == "testmx")
                {
                    TestMxGraph.Test();
                    return;
                }

                var config = new AppConfig();
                if (config.ParseArgs(args))
                {
                    Console.WriteLine("Input file: " + config.InputFile);
                    Console.WriteLine("Output path: " + config.OutputPath);
                    config.Main();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Environment.Exit(1);
            }
        }
    }
} 