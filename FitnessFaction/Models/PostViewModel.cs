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

        [StringLength(40), Required(ErrorMessage = "A post title is requried.")]
        public string PostTitle { get; set; }

        [StringLength(1000), Required(ErrorMessage = "A post text is required.")]
        public string PostText { get; set; }

        public bool dietPost { get; set; }
        public bool fitnessPost { get; set; }

        public string Tags { get; set; } = "";
        public string pfpURL { get; set; }
    }
}