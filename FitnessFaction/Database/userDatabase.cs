﻿using MongoDB.Driver;

namespace FitnessFaction
{
    public class userDatabase
    {
        //connection to the database clusters
        public static string cluster_url = "mongodb+srv://FitnessFactionAdmin:BX0jNkpLw7la1Ys3@fitnessfaction.1awhqcy.mongodb.net/?retryWrites=true&w=majority";

        //Database handling all the user info is named "User"
        public static string databaseName = "User";
        public userDatabase()
        {

        }

        //inserts all the user credentials into the UserInfo database if valid
        public static void postUserSignUpCredentials(string email, string username, string password)
        {
            //create a connection to the MongoDB database
            MongoClient dbClient = new MongoClient(cluster_url);
            
            //insert the credentials the user entered into the database
            dbClient.GetDatabase(databaseName).GetCollection<userSignUpObject>("UserInfo").InsertOne(new userSignUpObject
            {
                Email = email,
                UserName = username,
                Password = password,
                enrollmentDate = DateTime.Now

                
            });

        }

        //checks to see if the sign up credentials are valid
        public static bool userCredentialsValid(string email, string username)
        {
            //connect to the cluster
            MongoClient dbClient = new MongoClient(cluster_url);
            
            //get the database with the specific collection
            var collection = dbClient.GetDatabase(databaseName).GetCollection<userSignUpObject>("UserInfo");

            //query the db to see if the credentials have been taken
            var builder = Builders<userSignUpObject>.Filter;
            var filter = builder.Eq("username", username) &  builder.Eq("email", email);

            //get a count of all the elements that match
            var condition = collection.Find(filter).ToList().Count();

            //if none are found, then the user entered valid credentials
            if (condition == 0)
                return true;
            else
                return false;
        }

        //checks the user's credentials on login
        public static bool userLoginCredentialsValid(string username, string password)
        {
            //connect to the cluster
            MongoClient dbClient = new MongoClient(cluster_url);

            //get the database with the specific collection
            var collection = dbClient.GetDatabase(databaseName).GetCollection<userSignUpObject>("UserInfo");

            //query the db to see if the credentials have been taken
            var builder = Builders<userSignUpObject>.Filter;
            var filter = builder.Eq("username", username) & builder.Eq("password", password);

            //get a count of all the elements that match
            var condition = collection.Find(filter).ToList().Count();

            //if none are found, then the user entered valid credentials
            if (condition != 0)
                return true;
            else
                return false;
        }

      

        
    }
}