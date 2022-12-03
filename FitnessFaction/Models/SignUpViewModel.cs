using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FitnessFaction.Models
{
    public class SignUpViewModel
    {
        public string Id { get; set; }

        [StringLength(50), Required(ErrorMessage = "Please enter an email!")]
        public string Email { get; set; }

        [StringLength(40), Required(ErrorMessage = "Please enter a username!")]
        public string UserName { get; set; }

        [StringLength(20), Required(ErrorMessage = "Please enter a password!")]
        public string Password { get; set; }

        [BindProperty]
        public IFormFile ProfilePicture { get; set; }

        public string imageUrl { get; set; }

        public string FollowedAccounts { get; set; }

    }
}