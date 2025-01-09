using System;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace BMICalculator
{
    class Program
    {
        static string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the BMI Calculator!");
            while (true)
            {
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");
                Console.Write("Choose an option: ");
                int choice = int.Parse(Console.ReadLine());

                if (choice == 1)
                {
                    RegisterUser();
                }
                else if (choice == 2)
                {
                    int userId = LogIn();
                    if (userId != -1)
                    {
                        Console.WriteLine("Login successful.");
                        BMI_Calculation(userId);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        static void RegisterUser()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            Console.Write("Enter age: ");
            int age = int.Parse(Console.ReadLine());

            Console.Write("Enter gender (Male/Female): ");
            string gender = Console.ReadLine();

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "INSERT INTO users (username, password, age, gender) VALUES (@username, @password, @age, @gender)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.Parameters.AddWithValue("@age", age);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.ExecuteNonQuery();

                Console.WriteLine("Registration successful.");
            }
        }

        static int LogIn()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT id FROM users WHERE username=@username AND password=@password";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);  // User ID
                    }
                    else
                    {
                        Console.WriteLine("Invalid username or password.");
                        return -1;
                    }
                }
            }
        }

        static void BMI_Calculation(int userId)
        {
            Console.Write("Enter your height (in meters): ");
            decimal height = decimal.Parse(Console.ReadLine());

            Console.Write("Enter your weight (in kilograms): ");
            decimal weight = decimal.Parse(Console.ReadLine());

            decimal bmi = weight / (height * height);
            Console.WriteLine($"Your BMI is: {bmi:F2}");

            // Save BMI record to the database
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "INSERT INTO bmi_history (user_id, height, weight, bmi) VALUES (@userId, @height, @weight, @bmi)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@height", height);
                cmd.Parameters.AddWithValue("@weight", weight);
                cmd.Parameters.AddWithValue("@bmi", bmi);
                cmd.ExecuteNonQuery();

                Console.WriteLine("BMI saved to your profile.");
            }

            // Display BMI History
            DisplayBMIHistory(userId);
        }

        static void DisplayBMIHistory(int userId)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT height, weight, bmi, date FROM bmi_history WHERE user_id=@userId ORDER BY date DESC";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\n--- BMI History ---");
                    while (reader.Read())
                    {
                        decimal height = reader.GetDecimal(0);
                        decimal weight = reader.GetDecimal(1);
                        decimal bmi = reader.GetDecimal(2);
                        DateTime date = reader.GetDateTime(3);

                        Console.WriteLine($"Date: {date}, Height: {height}m, Weight: {weight}kg, BMI: {bmi:F2}");
                    }
                }
            }
        }
    }
}

