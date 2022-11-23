using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace FitnessFaction.Database
{
    public class AzureRDBMS_Connection
    {
        private SqlConnection connection;
        private string username;

        //initialize the connection so it does not need to be repeated
        public AzureRDBMS_Connection(string getUsername)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "fitnessfaction.database.windows.net";
            builder.UserID = "FitnessFactionAdmin";
            builder.Password = "BX0jNkpLw7la1Ys3";
            builder.InitialCatalog = "FitnessFaction";

            connection = new SqlConnection(builder.ConnectionString);
            username = getUsername;
        }

        //adds the user to the relationalDBMS' user table
        public void addUser(string email, string username)
        {
            connection.Open();
            SqlCommand sql = new SqlCommand("INSERT INTO dbo.UserTable (Username, Email) Values (@username, @email);",connection);
            sql.Parameters.AddWithValue("@username", username);
            sql.Parameters.AddWithValue("@email", email);
            sql.ExecuteNonQuery();
            connection.Close();
        }

        //going to be used to retrieve all posts depending on the post type
        public void getGlobalPosts()
        {
            connection.Open();

            SqlCommand sql = new SqlCommand("Select Username, Tags, PostTitle, PostText, PostDate FROM dbo.Posts");

            connection.Close();
        }

    }
}
