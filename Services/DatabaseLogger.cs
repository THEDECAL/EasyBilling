using EasyBilling.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EasyBilling.Services
{
    public class DatabaseLoggerProvider : ILoggerProvider
    {
        //private IServiceScopeFactory _scopeFactory;
        //public DatabaseLoggerProvider(/*IServiceScopeFactory scopeFactory*/)
        //{
            //_scopeFactory = scopeFactory;
        //}
        public ILogger CreateLogger(string categoryName)
        {
            return new DatabaseLogger(/*_scopeFactory*/);
        }

        public void Dispose()
        { }

        public class DatabaseLogger : ILogger
        {
            //private BillingDbContext _dbContext;

            //public DatabaseLogger(/*IServiceScopeFactory scopeFactory*/)
            //{
                //var scope = scopeFactory.CreateScope();
                //var sp = scope.ServiceProvider;

                //_dbContext = sp.GetRequiredService<BillingDbContext>();
            //}

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                Console.WriteLine(formatter(state, exception));
                File.AppendAllText("log.txt", formatter(state, exception));
            }

            public bool IsEnabled(LogLevel logLevel) => true;

            public IDisposable BeginScope<TState>(TState state) => null;
        }
    }
}
