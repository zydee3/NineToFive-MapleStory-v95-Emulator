using System;

namespace NineToFive.Login {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Hello World, from Login Server!");
            _ = new LoginServer(8484);
            string input;
            while ((input = Console.ReadLine()) != null) {
                if (input == "exit") {
                    return;
                }
            }
        }
    }
}
