using Microsoft.Data.Sqlite;

Logger.Print("Welcome !");
Logger.Print("Enter your username :");
string? User = Console.ReadLine();
Logger.Print("Enter your Password : ");
string? Pass = Console.ReadLine();

Main.CreateTable();
if (Main.Login(User, Pass) == true)
{
    int Perms = Main.GetPermissions(User);
    Logger.Print($"Permission Level : {Perms}");
    if ( Perms == 1)
    {
        while (true){
            Logger.Print("Choose your action : ");
            Logger.Print("0) Exit");
            Logger.Print("1) View all users");
            Logger.Print("2) Manage user permissions");
            string? Input = Console.ReadLine();
            if(Input == "0")
            {
                break;
            }
            else
            {
                Main.AdminAction(Input, User);
            }
        }
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

            Logger.Print("Table 'users' created successfully.");
            connection.Close();
        }
        catch (SqliteException ex)
        {
            Logger.Print(ex.Message);
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
                        Logger.Print($"User {Username} Logged in Successfully");
                        return true;
                    }
                    else
                    {
                        Logger.Print("Incorrect Password !");
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
                Logger.Print($"The user '{Username}' has been registered successfully.");
                return true;
            }
        }
        catch (SqliteException ex)
        {
            Logger.Print(ex.Message);
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
            Logger.Print(ex.Message);
        }
        return 0;
    }

    public static void AdminAction(string? Action, string? ActiveUser)
    {
        if(Action == "1")
        {
            var sql = "SELECT * FROM users";
            using var connection = new SqliteConnection($"Data Source=users.db");
            connection.Open();
            using var command = new SqliteCommand(sql, connection);
            using var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string? Name = Convert.ToString(reader["username"]);
                    string? Perms = Convert.ToString(reader["permissions"]);
                    Logger.Print($"User : {Name}, Permission Level : {Perms}");
                }
            }
        }
        else if (Action == "2")
        {
            try
            {
                Logger.Print("Enter user to modify : ");
                string? User = Console.ReadLine();
                if(User == ActiveUser)
                {
                    return;
                }
                Logger.Print("Enter user's new permission level : ");
                string? Perm = Console.ReadLine();
                var sql = "UPDATE users set permissions = @perm WHERE username = @username";
                using var connection = new SqliteConnection($"Data Source=users.db");
                connection.Open();
                using var command = new SqliteCommand(sql, connection);
                command.Parameters.AddWithValue("@username", User?.ToLower());
                command.Parameters.AddWithValue("@perm", Convert.ToInt32(Perm));
                command.ExecuteNonQuery();
                Logger.Print("Updated user permissions");
            }
            catch(SqliteException ex)
            {
                Logger.Print(ex.Message);
            }
        }
    }
}

class Logger
{
    public static void Print(string? Value)
    {
        Console.WriteLine(Value);
    }
}