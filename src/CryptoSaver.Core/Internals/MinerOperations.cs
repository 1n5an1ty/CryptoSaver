using CryptoSaver.Core.Logging;
using CryptoSaver.Core.Miner;
using System;
using System.Diagnostics;
using System.IO;

namespace CryptoSaver.Core.Internals
{
    internal static class MinerOperations
    {
        private static readonly Type This = typeof(MinerOperations);
        private static Process _minerProcess;

        public static void StartMiner(MinerSettings minerSettings)
        {
            This.Log().Info("Miner starting!");
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var minerDir = Path.Combine(baseDir, "Miner");
            var minerExe = Path.Combine(minerDir, "xmrig.exe");
            var gpuSettings = minerSettings.EnableAMD ? "-opencl" : minerSettings.EnableNVIDIA ? "-cuda" : "";

            ProcessStartInfo minerStartArgs = new ProcessStartInfo
            {
                FileName = minerExe,
                Arguments = $"-o {minerSettings.StratumURL} -a rx/0 -u {minerSettings.Username} -p {minerSettings.Password} -B -t {minerSettings.Threads} --coin=monero {gpuSettings} Elevated",
                CreateNoWindow = false,
                UseShellExecute = false,
                Verb = "runas"
            };
            _minerProcess = Process.Start(minerStartArgs);
        }

        public static void StopMiner()
        {
            if (_minerProcess != null)
            {
                This.Log().Info("Miner stopping!");
                _minerProcess.Kill();
                _minerProcess.Dispose();
            }
        }
    }
}
