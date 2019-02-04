﻿using Infusion.Proxy;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Infusion.Desktop.Launcher
{
    public static class CrossUOLauncher
    {
        public static void Launch(LauncherOptions options, ushort proxyPort)
        {
            var ultimaExecutablePath = options.Cross.ClientExePath;
            if (!File.Exists(ultimaExecutablePath))
            {
                Program.Console.Error($"File {ultimaExecutablePath} doesn't exist.");

                return;
            }

            Program.Console.Info($"Staring {ultimaExecutablePath}");

            var account = BitConverter.ToString(System.Text.Encoding.UTF8.GetBytes(options.UserName)).Replace("-", "");
            var password = BitConverter.ToString(System.Text.Encoding.UTF8.GetBytes(options.Password)).Replace("-", "");

            var info = new ProcessStartInfo(ultimaExecutablePath);
            info.WorkingDirectory = Path.GetDirectoryName(ultimaExecutablePath);
            info.Arguments = $"-autologin:0 -savepassword:0 \"-login 127.0.0.1,{proxyPort}\" -account:{account},{password}";

            var ultimaClientProcess = Process.Start(info);
            if (ultimaClientProcess == null)
            {
                Program.Console.Error($"Cannot start {ultimaExecutablePath}.");
                return;
            }

            Program.SetClientWindowHandle(ultimaClientProcess);
        }
    }
}
