﻿using System;
using Common.Logging;
using Common.Logging.Simple;
using Noobot.Core;
using Noobot.Core.Configuration;
using Noobot.Core.DependencyResolution;
using Noobot.Core.Logging;

namespace Noobot.Runner
{
    /// <summary>
    /// NoobotHost is required due to TopShelf.
    /// </summary>
    public class NoobotHost
    {
        private readonly IConfigReader _configReader;
        private INoobotCore _noobotCore;
        private readonly Toolbox.Configuration _configuration;
        private ILog _logger;

        public NoobotHost(IConfigReader configReader)
        {
            _configReader = configReader;
            _configuration = new Toolbox.Configuration();
            _logger = LogManager.GetLogger(GetType());
        }

        public void Start()
        {
            IContainerFactory containerFactory = new ContainerFactory(_configuration, _configReader, _logger);
            INoobotContainer container = containerFactory.CreateContainer();
            _noobotCore = container.GetNoobotCore();

            Console.WriteLine("Connecting...");
            _noobotCore
                .Connect()
                .ContinueWith(task =>
                {
                    if (!task.IsCompleted || task.IsFaulted)
                    {
                        Console.WriteLine($"Error connecting to Slack: {task.Exception}");
                    }
                });
        }

        public void Stop()
        {
            Console.WriteLine("Disconnecting...");
            _noobotCore.Disconnect();
        }
    }
}