using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FitnessFaction.Models
{
    public class SecurityModel
    {
        public string currentUsername { get; set; }

        public string loggedIn { get; set; }

    }
}