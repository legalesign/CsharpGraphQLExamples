using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace CLILegalesignExamples
{
    public class QLRecipient
    {
        public string? id { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? email { get; set; }
    }

    public class QLDocument
    {
        public string? id { get; set; }
        public List<QLRecipient>? recipients { get; set; }
    }

    public class QLData
    {
        public QLDocument? document { get; set; }      
    }

    public class QLResponse
    {
        public QLData? data { get; set; }      
    }

       public class Data
    {
        public object updateRecipient { get; set; }
    }

    public class Error
    {
        public List<string> path { get; set; }
        public object data { get; set; }
        public string errorType { get; set; }
        public object errorInfo { get; set; }
        public List<Location> locations { get; set; }
        public string message { get; set; }
    }

    public class Location
    {
        public int line { get; set; }
        public int column { get; set; }
        public object sourceName { get; set; }
    }

    public class QLMutation
    {
        public Data data { get; set; }
        public List<Error>? errors { get; set; }
    }
}