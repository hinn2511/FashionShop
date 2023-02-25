using System.Collections.Generic;

namespace API.DTOs.Response.DashboardResponse
{
    public class ReportResponse
    {
        public ReportResponse(string name, double value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public double Value { get; set; }
    }

   

    public class ReportMultiSeriesResponse
    {
        public ReportMultiSeriesResponse(

        )
        {

        }

        public ReportMultiSeriesResponse(string name, List<ReportSeriResponse> series)
        {
            Name = name;
            Series = series;
        }

        public string Name { get; set; }
        public List<ReportSeriResponse> Series { get; set; }
    }


     public class ReportSeriResponse
    {
        public ReportSeriResponse(string name, double value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public double Value { get; set; }
    }
}