using System;

namespace NineToFive.Channels {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Hello World, from Channel Server!");
            Server.Initialize();
            
            string input;
            while ((input = Console.ReadLine()) != null) {
                if (input == "exit") {
                    return;
                }
            }
        }
    }
}
