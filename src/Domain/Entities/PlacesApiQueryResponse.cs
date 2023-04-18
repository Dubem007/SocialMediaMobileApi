using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{

    //public class LocationDetails
    //{
    //    public float latitude { get; set; }
    //    public float longitude { get; set; }
    //}

    //public class Geometry
    //{
    //    public LocationDetails location { get; set; }
    //}

    ////public class OpeningHours
    ////{
    ////    public bool open_now { get; set; }
    ////    public List<object> weekday_text { get; set; }
    ////}

    ////public class Photo
    ////{
    ////    public int height { get; set; }
    ////    public List<string> html_attributions { get; set; }
    ////    public string photo_reference { get; set; }
    ////    public int width { get; set; }
    ////}

    //public class Result
    //{
    //    public Geometry geometry { get; set; }
       
    //}

    public class PlacesApiQueryResponse
    {
        public List<object> html_attributions { get; set; }
        public List<Result> results { get; set; }
        public string status { get; set; }
    }

}
