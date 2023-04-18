using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class LocationPredictions
    {

    }

    public class MainTextMatchedSubstring
    {
        public int length { get; set; }
        public int offset { get; set; }
    }

    public class MatchedSubstring
    {
        public int length { get; set; }
        public int offset { get; set; }
    }

    public class Prediction
    {
        public string description { get; set; }
        public string place_id { get; set; }

    }

    public class Predictions
    {
        public List<Prediction> predictions { get; set; }
        public string status { get; set; }
    }

    public class StructuredFormatting
    {
        public string main_text { get; set; }
        public List<MainTextMatchedSubstring> main_text_matched_substrings { get; set; }
        public string secondary_text { get; set; }
    }

    public class Term
    {
        public int offset { get; set; }
        public string value { get; set; }
    }

}
