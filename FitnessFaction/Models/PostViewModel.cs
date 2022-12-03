using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FitnessFaction.Models
{
    public class PostViewModel
    {

        [StringLength(40)]
        public string PostTitle { get; set; }

        [StringLength(700)]
        public string PostText { get; set; }

        public bool dietPost { get; set; }
        public bool fitnessPost { get; set; }

        public string Tags { get; set; } = "";
        public string pfpURL { get; set; }
    }
}