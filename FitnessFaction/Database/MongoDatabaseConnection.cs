using FitnessFaction.Database;
using FitnessFaction.Models;
using MongoDB.Driver;
using System.Linq;
using System.Security.Cryptography;

namespace FitnessFaction
{
    public class MongoDatabaseConnection
    {
        //connection to the database clusters
        private readonly static string cluster_url = "mongodb+srv://FitnessFactionAdmin:BX0jNkpLw7la1Ys3@fitnessfaction.1awhqcy.mongodb.net/?retryWrites=true&w=majority";
        
        //Database handling all the user info is named "User"
        private readonly static string databaseName = "User";

        public MongoDatabaseConnection()
        {

        }

        //add user credentials when signing up
        public static void postUserSignUpCredentials(SignUpViewModel model)
        {
            //create a connection to the MongoDB database
            MongoClient dbClient = new MongoClient(cluster_url);

            string encryptedPassword;

            //insert the credentials the user entered into the database
            dbClient.GetDatabase(databaseName).GetCollection<userSignUpObject>("UserInfo").InsertOne(new userSignUpObject
            {
                Email = model.Email,
                UserName = model.UserName,
                Password = model.Password,
                enrollmentDate = DateTime.Now

            });
            AzureRDBMS_Connection temp = new AzureRDBMS_Connection();
            temp.addUser(model.Email, model.UserName);
            temp.uploadProfilePicture(model.UserName, model.imageUrl);
        }

        //checks to see if the sign up credentials are valid
        public static string userCredentialsValid(string email, string username)
        {
            //connect to the cluster
            MongoClient dbClient = new MongoClient(cluster_url);
            
            //get the database with the specific collection
            var collection = dbClient.GetDatabase(databaseName).GetCollection<userSignUpObject>("UserInfo");

            //query the db to see if the credentials have been taken
            var builder = Builders<userSignUpObject>.Filter;

            var usernameAndEmail = collection.Find(builder.Eq("username", username) &  builder.Eq("email", email)).ToList().Count();
            var usernameOnly = collection.Find(builder.Eq("username", username)).ToList().Count();
            var emailOnly = collection.Find(builder.Eq("email", email)).ToList().Count();


            //if none are found, then the user entered valid credentials
            if (usernameAndEmail != 0)
                return "Email and username taken.";
            else if (usernameOnly != 0)
                return "Username taken";
            else if (emailOnly != 0)
                return "Email taken";
            else
                return "Credentials ok.";
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