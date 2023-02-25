using System;

namespace API.DTOs.Params
{
    public class ChartParams
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Limit { get; set; }
        public string Metric { get; set; }
    }
}