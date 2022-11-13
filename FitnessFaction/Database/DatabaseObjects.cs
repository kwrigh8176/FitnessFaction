using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }

    //going to hold specific user attributes
    public class UserSettings
    {


    }
}