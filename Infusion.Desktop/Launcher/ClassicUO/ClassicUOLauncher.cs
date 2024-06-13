﻿using Infusion.LegacyApi.Console;
using Infusion.Proxy;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Infusion.Desktop.Launcher.ClassicUO
{
    public class ClassicUOLauncher : ILauncher
    {
        public Task Launch(IConsole console, InfusionProxy proxy, LauncherOptions options)
        {
            var proxyPort = options.GetDefaultProxyPort();

            var task = proxy.Start(new ProxyStartConfig()
            {
                ServerAddress = options.ServerEndpoint,
                ServerEndPoint = options.ResolveServerEndpoint().Result,
                LocalProxyPort = proxyPort,
                ProtocolVersion = options.ProtocolVersion,
                Encryption = options.ClassicUO.EncryptionSetup,
                LoginEncryptionKey = options.ClassicUO.GetEncryptionKey()
            });

            var ultimaExecutableInfo = new FileInfo(options.ClassicUO.ClientExePath);
            if (!ultimaExecutableInfo.Exists)
            {
                console.Error($"File {ultimaExecutableInfo.FullName} doesn't exist.");
                return task;
            }

            var account = options.UserName;
            var password = options.Password;

            var info = new ProcessStartInfo(ultimaExecutableInfo.FullName);
            info.WorkingDirectory = ultimaExecutableInfo.DirectoryName;

            var insensitiveArguments = $"-ip 127.0.0.1 -port {proxyPort} -username {account}";
            var sensitiveArguments = $" -password {password}";
            info.Arguments = insensitiveArguments + sensitiveArguments;

            var argumentsInfo = insensitiveArguments + " -password <censored>";

            console.Info($"Staring {ultimaExecutableInfo.FullName} {argumentsInfo}");

            var ultimaClientProcess = Process.Start(info);
            if (ultimaClientProcess != null)
            {
                ClientProcessWatcher.Watch(ultimaClientProcess);
                proxy.SetClientWindowHandle(ultimaClientProcess);
            }
            else
            {
                console.Error($"Cannot start {ultimaExecutableInfo.FullName}.");
            }

            return task;
        }
    }
}
