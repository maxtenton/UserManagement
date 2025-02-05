using Microsoft.Data.Sqlite;

Console.WriteLine("Welcome !");
Console.WriteLine("Enter your username :");
string? User = Console.ReadLine();
Console.WriteLine("Enter your Password : ");
string? Pass = Console.ReadLine();

Main.CreateTable();
if (Main.Login(User, Pass) == true)
{
    int Perms = Main.GetPermissions(User);
    Logger.Print($"Permission Level : {Perms}");
    if ( Perms == 1)
    {

    }
}


class Main
{
    public static void CreateTable()
    {
        try
        {
            var sql = @"CREATE TABLE users(username TEXT NOT NULL, password TEXT NOT NULL, permissions INTEGER NOT NULL)"; // 0 - user, 1 - admin
            using var connection = new SqliteConnection($"Data Source=users.db");
            connection.Open();

            using var command = new SqliteCommand(sql, connection);
            command.ExecuteNonQuery();

            Console.WriteLine("Table 'users' created successfully.");
            connection.Close();
        }
        catch (SqliteException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    public static bool Login(string? Username, string? Password)
    {
        try
        {
            var sql = "SELECT * FROM users WHERE username = @username";
            using var connection = new SqliteConnection($"Data Source=users.db");
            connection.Open();
            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@username", Username?.ToLower());

            using var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    if (Password == reader.GetString(1))
                    {
                        Console.WriteLine($"User {Username} Logged in Successfully");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("Incorrect Password !");
                        return false;
                    }
                }
            }
            else
            {
                var sqlinsert = @"INSERT INTO users(username, password, permissions)" + "VALUES (@username, @password, @permissions)";
                connection.Open();
                using var comm = new SqliteCommand(sqlinsert, connection);
                comm.Parameters.AddWithValue("@username", Username?.ToLower());
                comm.Parameters.AddWithValue("@password", Password);
                if(Username == "Admin")
                {
                    comm.Parameters.AddWithValue("@permissions", 1);
                }
                else
                {
                    comm.Parameters.AddWithValue("@permissions", 0);
                }
                var rowInserted = comm.ExecuteNonQuery();
                Console.WriteLine($"The user '{Username}' has been registered successfully.");
                return true;
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine(ex.Message);
        }
    return false;
    }

    public static int GetPermissions(string? Username)
    {
        try
        {
            var sql = "SELECT * FROM users WHERE username = @username";
            using var connection = new SqliteConnection($"Data Source=users.db");
            connection.Open();
            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@username", Username?.ToLower());

            using var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    return Convert.ToInt32(reader["permissions"]);
                }
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine(ex.Message);
        }
        return 0;
    }
}

class Logger
{
    public static void Print(string Value)
    {
        Console.WriteLine(Value);
    }
}