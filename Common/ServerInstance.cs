﻿using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

using HellionExtendedServer.Common.Wrappers.ServerWrappers;
using HellionExtendedServer.Controllers;
using ZeroGravity;

namespace HellionExtendedServer.Common
{
    public class ServerInstance
    {
        private static Thread m_serverThread;

        private Assembly m_assembly;
        private DateTime m_launchedTime;
        private Server m_server;
        private ServerWrapper m_serverWrapper;

        private static ServerInstance m_serverInstance;


        public TimeSpan Uptime { get { return DateTime.Now - m_launchedTime; } }
        public Boolean IsRunning { get {return ServerWrapper.HellionDedi.IsRunning; } }
        public Assembly Assembly { get { return m_assembly; } }
        public Server Server { get { return m_server; } }


        public static ServerInstance Instance { get { return m_serverInstance; } }

        public ServerInstance()
        {
            m_launchedTime = DateTime.MinValue;

            m_serverThread = null;
            m_serverInstance = this;

            m_assembly = Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HELLION_Dedicated.exe"));
            m_serverWrapper = new ServerWrapper(m_assembly);
           
        }

        public void Start()
        {
            String[] serverArgs = new String[]
                {
                    "",
                };

            m_serverThread = ServerWrapper.HellionDedi.StartServer(serverArgs);

            m_serverWrapper.Init();

            Thread.Sleep(5000);

            m_server = ServerWrapper.HellionDedi.Server;

            if (IsRunning)
            {
                Console.WriteLine("Hellion Extended Server: World Initialized!");

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                Thread.Sleep(1);
                stopwatch.Stop();
                long num = (long)(1000.0 / stopwatch.Elapsed.TotalMilliseconds);

                Console.WriteLine(string.Format("==============================================================================\r\n\tServer name: {5}\r\n\tServer ID: {1}\r\n\tStart date: {0}\r\n\tServer ticks: {2}{4}\r\n\tMax server ticks (not precise): {3}\r\n==============================================================================", (object)DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.ffff"), (object)(Server.NetworkController.ServerID <= 0L ? "Not yet assigned" : string.Concat((object)Server.NetworkController.ServerID)), 64, (object)num, (object)(64 > num ? " WARNING: Server ticks is larger than max tick" : ""), (object)Server.ServerName));

            }

            new NetworkController(m_server.NetworkController);
            

            Console.WriteLine("Ready for connections!");
        }

        public void Stop()
        {
            ServerWrapper.HellionDedi.StopServer();
            m_serverThread.Join(60000);
            m_serverThread.Abort();
        }
    }
}