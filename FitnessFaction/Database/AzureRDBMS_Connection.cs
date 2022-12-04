using FitnessFaction.Models;
using Microsoft.VisualBasic.FileIO;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace FitnessFaction.Database
{
    public class AzureRDBMS_Connection
    {
        private SqlConnection connection;


        //initialize the connection so it does not need to be repeated
        public AzureRDBMS_Connection()
        {
            SqlConnectionStringBuilder builder = new()
            {
                DataSource = "fitnessfaction.database.windows.net",
                UserID = "FitnessFactionAdmin",
                Password = "BX0jNkpLw7la1Ys3",
                InitialCatalog = "FitnessFaction"
            };

            connection = new SqlConnection(builder.ConnectionString);
        }

        //adds the user to the relationalDBMS' user table
        public void addUser(string email, string username)
        {
            connection.Open();
            SqlCommand sql = new SqlCommand("INSERT INTO dbo.UserTable (Username, Email) Values (@username, @email);", connection);
            sql.Parameters.AddWithValue("@username", username);
            sql.Parameters.AddWithValue("@email", email);
            sql.ExecuteNonQuery();
            connection.Close();
        }

        //going to be used to retrieve all posts depending on the post type
        public List<Posts> getGlobalPosts(string feedType)
        {
            connection.Open();

            //command to retrieve all the posts in the post database
            SqlCommand sql = new SqlCommand("Select Username, Tags, PostTitle, PostText, PostDate, feedType, pfpURL FROM dbo.Posts", connection);

            //initialize the reader so we can read in all the posts
            SqlDataReader reader = sql.ExecuteReader();
            List<Posts> posts = new List<Posts>();

            //read all the posts in
            while (reader.Read())
            {
                Posts post = new Posts
                {
                    UserName = reader.GetValue(0).ToString().Trim(),
                    Tags = reader.GetValue(1).ToString().Trim(),
                    PostTitle = reader.GetValue(2).ToString().Trim(),
                    PostText = reader.GetValue(3).ToString().Trim(),
                    PostDate = DateTime.Parse(reader.GetValue(4).ToString()),
                    feedType = reader.GetValue(5).ToString().Trim(),
                    pfpURL = reader.GetValue(6).ToString().Trim()
                };
                posts.Add(post);
            }

            //filter out based on feed type (diet or fitness)
            posts = posts.Where(o => o.feedType == feedType).ToList();


            connection.Close();
            return posts;
        }
        public List<Tags> pullTags()
        {
            connection.Open();

            //command to retrieve all the posts in the post database
            SqlCommand sql = new SqlCommand("Select TagName, TagType FROM dbo.Tags", connection);

            //initialize the reader so we can read in all the posts
            SqlDataReader reader = sql.ExecuteReader();
            List<Tags> tags = new List<Tags>();

            //read all the posts in
            while (reader.Read())
            {
                Tags tag = new Tags();
                tag.TagName = reader.GetValue(0).ToString().Trim();
                tag.TagType = reader.GetValue(1).ToString().Trim();
                tags.Add(tag);
            }


            connection.Close();
            return tags;
        }
        public void enterPost(string PostTitle, string PostText, string feedType, string Tags, string username, string pfpUrl)
        {
            connection.Open();
            SqlCommand sql = new("INSERT INTO dbo.Posts (Username, Tags, PostTitle, PostText, PostDate, feedType, pfpURL) Values (@username, @tags, @postTitle, @postText, @postDate, @feedType, @pfpURL);", connection);

            DateTime time = DateTime.Now;

            sql.Parameters.AddWithValue("@username", username);
            sql.Parameters.AddWithValue("@tags", Tags);
            sql.Parameters.AddWithValue("@postTitle", PostTitle);
            sql.Parameters.AddWithValue("@postText", PostText);
            sql.Parameters.AddWithValue("@postDate", time);
            sql.Parameters.AddWithValue("@feedType", feedType);
            sql.Parameters.AddWithValue("@pfpURL", pfpUrl);
            sql.ExecuteNonQuery();
            connection.Close();
        }

        public void uploadProfilePicture(string username, string profilePicture)
        {
            connection.Open();
            SqlCommand sql = new SqlCommand("INSERT INTO dbo.ProfilePictures (Username, PFP) Values (@username, @pfp);", connection);
            sql.Parameters.AddWithValue("@username", username);
            sql.Parameters.AddWithValue("@pfp", profilePicture);
            sql.ExecuteNonQuery();
            connection.Close();
        }

        public string getProfilePicture(string username)
        {
            connection.Open();
            SqlCommand sql = new SqlCommand("Select Username, PFP FROM dbo.ProfilePictures WHERE Username = @username", connection);
            //initialize the reader so we can read in all the posts
            SqlDataReader reader = sql.ExecuteReader();
            sql.Parameters.AddWithValue("@username", username);
            string imageUrl = "";
            //read all the posts in
            while (reader.Read())
            {
                
                
               imageUrl = reader.GetValue(1).ToString();
                    
                    
                
            }
            return imageUrl;
            connection.Close();
        }

    }
}
