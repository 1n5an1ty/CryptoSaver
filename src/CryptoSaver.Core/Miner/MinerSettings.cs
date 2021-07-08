namespace CryptoSaver.Core.Miner
{
    public class MinerSettings
    {
        public string StratumURL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Threads { get; set; }
        public bool EnableAMD { get; set; }
        public bool EnableNVIDIA { get; set; }
    }
}
