using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace AddMyApp.Models
{
    /// <summary> Модель конфигурации </summary>
    public class MyConf
    {
        public string Host { get; set; }
        public int Port { get; set; }
    }

    /// <summary> Задаем метод, который необходимо запустить </summary>
    public interface IMyInterface
    {
        public void PrintConf(MyConf conf);
    }

    /// <summary> Построитель запуска запуска </summary>
    public class MyBuild
    {
        private readonly IMyInterface main_method;
        private readonly MyConf conf;

        public MyBuild(IMyInterface Main_method, MyConf Conf)
        {
            main_method = Main_method;
            conf = Conf;
        }

        /// <summary> Обработка запуска и запуск метода </summary>
        public void Start()
        {
            Console.WriteLine("Start");

            main_method.PrintConf(conf);

            Console.WriteLine("Stop");
        }
    }

    /// <summary> метод расширения, за запуска требуемого метода </summary>
    public static class MyExtensions
    {
        public static void AddMyApp<T>(this IServiceCollection services
            , IConfiguration configuration) where T : class, IMyInterface
        {
            var conf = configuration.GetSection("MyConf").Get<MyConf>();
            services.AddScoped<IMyInterface, T>();
            var service_provider = services.AddScoped<MyBuild>(provider =>
            {
                var obj = provider.GetService<IMyInterface>();
                MyBuild build_model = new MyBuild(obj, conf);
                return build_model;
            }).BuildServiceProvider();
            var obj = service_provider.GetService<MyBuild>();
            obj.Start();
        }
    }

    /// <summary> Пример реализации 1 </summary>
    public class MyHandler : IMyInterface
    {
        private readonly ILogger<MyHandler> logger;

        public MyHandler(ILogger<MyHandler> Logger)
        {
            logger = Logger;
        }
        public void PrintConf(MyConf conf)
        {
            logger.LogInformation($"MyHandler: {conf.Host}:{conf.Port}");
        }
    }

    /// <summary> Пример реализации 2 </summary>
    public class MyHandler2 : IMyInterface
    {
        public void PrintConf(MyConf conf)
        {
            Console.WriteLine($"MyHandler2: {conf.Host}:{conf.Port}");
        }
    }
}
