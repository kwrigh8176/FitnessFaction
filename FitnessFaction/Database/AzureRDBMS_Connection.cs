using FitnessFaction.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualBasic.FileIO;
using MongoDB.Driver;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;


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
            string cmd = "INSERT INTO dbo.UserTable (Username, Email, followingCount, followerCount, following) Values (@username, @email, 0, 0, '')";
            SqlCommand sql = new SqlCommand(cmd, connection);
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
            SqlCommand sql = new SqlCommand("Select Username, Tags, PostTitle, PostText, PostDate, pfpURL, ID FROM dbo.Posts WHERE feedType = @feedType", connection);
            sql.Parameters.AddWithValue("@feedType", feedType);
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
                    pfpURL = reader.GetValue(5).ToString().Trim(),
                    ID = Convert.ToInt32(reader.GetValue(6).ToString())
                };
                posts.Add(post);
            }

            posts = posts.OrderByDescending(x => x.PostDate).ToList();


            connection.Close();
            return posts;
        }

        //going to be used to retrieve all posts depending on the post type
        public List<Posts> getFollowingPosts(string feedType, string username)
        {
            connection.Open();

            //first check the accounts that the current user is following
            SqlCommand sql = new SqlCommand("Select following FROM dbo.UserTable WHERE Username = @Username", connection);
            sql.Parameters.AddWithValue("@Username", username);
            SqlDataReader reader = sql.ExecuteReader();

            string following = "";
            while (reader.Read())
            {
                following = reader.GetValue(0).ToString();
            }
            reader.Close();

            //count the amount of accounts that they are following to build the query
            int countAccounts = following.Count(f => f == ';');
            string command = "";
            if (countAccounts == 0)
            {
                return null;
            }
            //if they are following one account the query is simple
            else if (countAccounts == 1)
            {
                command = "Select Username, Tags, PostTitle, PostText, PostDate, pfpURL, ID FROM dbo.Posts WHERE feedType = @feedType AND Username = @Username";
                sql = new SqlCommand(command, connection);
                sql.Parameters.AddWithValue("@Username", following.Substring(0, following.IndexOf(";")));
                sql.Parameters.AddWithValue("@feedType", feedType);

                List<Posts> posts = new List<Posts>();
                reader = sql.ExecuteReader();
                while (reader.Read())
                {
                    Posts post = new Posts
                    {
                        UserName = reader.GetValue(0).ToString().Trim(),
                        Tags = reader.GetValue(1).ToString().Trim(),
                        PostTitle = reader.GetValue(2).ToString().Trim(),
                        PostText = reader.GetValue(3).ToString().Trim(),
                        PostDate = DateTime.Parse(reader.GetValue(4).ToString()),
                        pfpURL = reader.GetValue(5).ToString().Trim(),
                        ID = Convert.ToInt32(reader.GetValue(6).ToString())
                    };
                    posts.Add(post);
                }

                reader.Close();
                posts = posts.OrderByDescending(x => x.PostDate).ToList();
                return posts;

            }
            else
            {
                //following string format would be like: accountName;accountName;
                string[] usernames;
                usernames = following.Split(';');

                List<Posts> posts = new List<Posts>();

                //execute a seperate query to get the post from each user (definitely a better way, but string manipulation was rough)
                foreach (string user in usernames)
                {
                    sql = new SqlCommand("Select Username, Tags, PostTitle, PostText, PostDate, pfpURL, ID FROM dbo.Posts WHERE feedType = @feedType AND Username = @username", connection);
                    sql.Parameters.AddWithValue("@feedType", feedType);
                    sql.Parameters.AddWithValue("@username", user);
                    reader = sql.ExecuteReader();
                    while (reader.Read())
                    {
                        Posts post = new Posts
                        {
                            UserName = reader.GetValue(0).ToString().Trim(),
                            Tags = reader.GetValue(1).ToString().Trim(),
                            PostTitle = reader.GetValue(2).ToString().Trim(),
                            PostText = reader.GetValue(3).ToString().Trim(),
                            PostDate = DateTime.Parse(reader.GetValue(4).ToString()),
                            pfpURL = reader.GetValue(5).ToString().Trim(),
                            ID = Convert.ToInt32(reader.GetValue(6).ToString())
                        };
                        posts.Add(post);
                    }
                    reader.Close();
                }
                //sort post by time
                posts = posts.OrderByDescending(x => x.PostDate).ToList();

                return posts;
            }
            
            



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
            //simply insert the post into the post database
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
            //simply upload the profile picture
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
            sql.Parameters.AddWithValue("@username", username);
            SqlDataReader reader = sql.ExecuteReader();
            string imageUrl = "";
            //read all the posts in
            while (reader.Read())
            {
                
                
               imageUrl = reader.GetValue(1).ToString();
                    
                    
                
            }
            connection.Close();
            return imageUrl;
        }

        public List<Posts> getProfilePosts(string username)
        {
            connection.Open();

            //command to retrieve all the posts in the post database
            SqlCommand sql = new SqlCommand("Select Username, Tags, PostTitle, PostText, PostDate, pfpURL, ID FROM dbo.Posts WHERE Username = @Username", connection);
            sql.Parameters.AddWithValue("@Username", username);
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
                    pfpURL = reader.GetValue(5).ToString().Trim(),
                    ID = Convert.ToInt32(reader.GetValue(6).ToString())
                };
                posts.Add(post);
            }

            posts = posts.OrderByDescending(x => x.PostDate).ToList();


            connection.Close();
            return posts;
        }

         //add follow counts to profiles
        public int[] getFollowCounts(string username)
        {
            connection.Open();

            //just query the two follow count variables from the user table
            SqlCommand sql = new SqlCommand("Select followingCount, followerCount FROM dbo.UserTable WHERE Username = @Username", connection);
            sql.Parameters.AddWithValue("@Username", username);

            SqlDataReader reader = sql.ExecuteReader();

            int[] counts = new int[2];
            //read all the posts in
            while (reader.Read())
            {
                counts[0] = Convert.ToInt32(reader.GetValue(0).ToString());
                counts[1] = Convert.ToInt32(reader.GetValue(1).ToString());
            }

                  

            connection.Close();


            return new int[] { counts[0], counts[1] };

            
        }

        public bool checkFollowing(string currentUser, string visitingUser)
        {
            connection.Open();

            //get the string of followed users from the user table database
            SqlCommand sql = new SqlCommand("Select following FROM dbo.UserTable WHERE Username = @Username", connection);
            sql.Parameters.AddWithValue("@Username", visitingUser);

            SqlDataReader reader = sql.ExecuteReader();

            string followString = "";
            //read all the posts in
            while (reader.Read())
            {
                followString = reader.GetValue(0).ToString();

            }
            connection.Close();

            //check to see if the current user is in the following string of the visiting user
            if (followString.Contains(currentUser))
            {
                return true;
            }
            else
            {
                return false;
            }


          
        }

        public void follow(string currentUser, string visitingUser)
        {
            //multi part process
            connection.Open();

            //first check the following count of the visiting user
            SqlCommand sql = new SqlCommand("Select following, followingCount FROM dbo.UserTable WHERE Username = @Username", connection);
            sql.Parameters.AddWithValue("@Username", visitingUser);

            SqlDataReader reader = sql.ExecuteReader();

            string followString = "";
            int followCount = 0;
            while (reader.Read())
            {
                followString = reader.GetValue(0).ToString();
                followCount = Convert.ToInt32(reader.GetValue(1).ToString());
            }
            reader.Close();

            //add the current user's username to the visting user's following string. Also increment the following count.
            followString = followString + currentUser + ";";
            followCount += 1;

            //update the table to reflect this change
            sql = new SqlCommand("UPDATE UserTable SET [following] = @followString, followingCount = @followingCount WHERE Username = @visitingUser", connection);
            sql.Parameters.AddWithValue("@followString", followString);
            sql.Parameters.AddWithValue("@visitingUser", visitingUser);
            sql.Parameters.AddWithValue("@followingCount", followCount);
            sql.ExecuteNonQuery();


            //get the follower count of the current user
            sql = new SqlCommand("Select followerCount FROM dbo.UserTable WHERE Username = @currentUser", connection);
            sql.Parameters.AddWithValue("@currentUser", currentUser);
            reader = sql.ExecuteReader();
            int followerCount = 0;
            while (reader.Read())
            {
                followerCount = Convert.ToInt32(reader.GetValue(0).ToString());
            }
            reader.Close();

            //update the follower count of the current user
            followerCount += 1;
            sql = new SqlCommand("UPDATE UserTable SET followerCount = @followerCount  WHERE Username = @currentUser", connection);
            sql.Parameters.AddWithValue("@followerCount", followerCount);
            sql.Parameters.AddWithValue("@currentUser", currentUser);
            sql.ExecuteNonQuery();

            connection.Close();
        }

        public void unfollow(string currentUser, string visitingUser)
        {
            connection.Open();

            //first check the following count of the visiting user
            SqlCommand sql = new SqlCommand("Select following, followingCount FROM dbo.UserTable WHERE Username = @Username", connection);
            sql.Parameters.AddWithValue("@Username", visitingUser);

            SqlDataReader reader = sql.ExecuteReader();

            string followString = "";
            int followCount = 0;
            
            while (reader.Read())
            {
                followString = reader.GetValue(0).ToString();
                followCount = Convert.ToInt32(reader.GetValue(1).ToString());
            }
            reader.Close();

            //remove the current user from the following string of the visiting user. Also decrement the followingCount.
            followString = followString.Replace(currentUser + ";", "");
            followCount -= 1;

            //update the visiting user's follow counts
            sql = new SqlCommand("UPDATE UserTable SET [following] = @followString, followingCount = @followingCount WHERE Username = @visitingUser", connection);
            sql.Parameters.AddWithValue("@followString", followString);
            sql.Parameters.AddWithValue("@visitingUser", visitingUser);
            sql.Parameters.AddWithValue("@followingCount", followCount);
            sql.ExecuteNonQuery();


            //get the current page's follower count
            sql = new SqlCommand("Select followerCount FROM dbo.UserTable WHERE Username = @currentUser", connection);
            sql.Parameters.AddWithValue("@currentUser", currentUser);
            reader = sql.ExecuteReader();
            int followerCount = 0;
            while (reader.Read())
            {
                followerCount = Convert.ToInt32(reader.GetValue(0).ToString());
            }
            reader.Close();

            //update the current page's follower count
            followerCount -= 1;
            sql = new SqlCommand("UPDATE UserTable SET followerCount = @followerCount  WHERE Username = @currentUser", connection);
            sql.Parameters.AddWithValue("@followerCount", followerCount);
            sql.Parameters.AddWithValue("@currentUser", currentUser);
            sql.ExecuteNonQuery();

            connection.Close();
        }

        public Posts getSinglePosts(int postNum)
        {
            connection.Open();

            //command to retrieve all the posts in the post database
            SqlCommand sql = new SqlCommand("Select Username, Tags, PostTitle, PostText, PostDate, pfpURL, ID FROM dbo.Posts WHERE ID = @ID", connection);
            sql.Parameters.AddWithValue("@ID", postNum);
            //initialize the reader so we can read in all the posts
            SqlDataReader reader = sql.ExecuteReader();
            Posts post = null;
            //read all the posts in
            while (reader.Read())
            {
                post = new Posts
                {
                    UserName = reader.GetValue(0).ToString().Trim(),
                    Tags = reader.GetValue(1).ToString().Trim(),
                    PostTitle = reader.GetValue(2).ToString().Trim(),
                    PostText = reader.GetValue(3).ToString().Trim(),
                    PostDate = DateTime.Parse(reader.GetValue(4).ToString()),
                    pfpURL = reader.GetValue(5).ToString().Trim(),
                    ID = Convert.ToInt32(reader.GetValue(6).ToString())
                };
                
            }

            connection.Close();
            return post;
        }
    }

    
}
