using System;

namespace HashTool
{
    class Program
    {
        static void Main(string[] args)
        {
            string password = "Admin@123";
            string hash = BCrypt.Net.BCrypt.HashPassword(password);
            Console.WriteLine("REAL_HASH_START|" + hash + "|REAL_HASH_END");
        }
    }
}
