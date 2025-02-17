namespace CodeHub.Data.Models
{
    public class PistonResponse
    {
        public string Language { get; set; }
        public string Version { get; set; }
        public RunResult Run { get; set; }
    }

    public class RunResult
    {
        public string Stdout { get; set; }
        public string Stderr { get; set; }
        public int Code { get; set; }
        public string Signal { get; set; }
        public string Output { get; set; }
    }
}
