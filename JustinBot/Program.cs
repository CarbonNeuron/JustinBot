using System;
using System.Threading.Tasks;
using NLog;

namespace JustinBot
{
    class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        static async Task Main(string[] args)
        {
            _logger.Debug("I'm alive!");
        }
    }
}