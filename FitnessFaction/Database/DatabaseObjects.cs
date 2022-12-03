﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlTypes;

namespace FitnessFaction
{

    public class DatabaseObjects
    {


    }

    //handles user information fields (some determined on sign up)
    public class userSignUpObject
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("username")]
        public string UserName { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("enrollmentDate")]
        public DateTime enrollmentDate { get; set; }

        [BsonElement("followedAccounts")]
        public string followedAccounts {get; set;}

    }

    public class Posts
    {
        public string UserName { get; set; }

        public string Tags { get; set;}

        public string PostTitle { get; set;}

        public string PostText { get; set;}

        public DateTime PostDate { get; set;}

        public string feedType { get; set;}

        public string pfpURL { get; set; }
    }

    public class Tags
    {
        public string TagName { get; set; }

        public string TagType { get; set; }

    }

    public class profilePicture
    {
        public string Username { get; set; }

        public string ProfilePicture { get; set; }
    }

}