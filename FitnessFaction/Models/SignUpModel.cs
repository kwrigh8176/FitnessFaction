using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FitnessFaction.Models
{
    public class SignUpModel
    {
        public string Id { get; set; }

        [StringLength(50)]
        public string Email { get; set; }

        [StringLength(40)]
        public string UserName { get; set; }

        [StringLength(20)]
        public string Password { get; set; }

        public string FollowedAccounts { get; set; }

    }
}