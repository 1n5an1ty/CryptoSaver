using CryptoSaver.Core.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoSaver.Core.Internals
{
    internal static class MinerOperations
    {
        private static readonly Type This = typeof(MinerOperations);
        private static Process _minerProcess;

        public static void StartMiner(string stratumURL, string username, string password, int threads)
        {
            This.Log().Info("Miner starting!");
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var minerDir = Path.Combine(baseDir, "Miner");
            var minerExe = Path.Combine(minerDir, "xmrig.exe");

            ProcessStartInfo minerStartArgs = new ProcessStartInfo
            {
                FileName = minerExe,
                Arguments = $"-o {stratumURL} -a rx/0 -u {username} -p {password} -B -t {threads} --coin=monero Elevated",
                CreateNoWindow = true,
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
