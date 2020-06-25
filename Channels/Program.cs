using System;

namespace NineToFive.Channels {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Hello World, from Channel Server!");
            _ = new ChannelServer(7575);
            string input;
            while ((input = Console.ReadLine()) != null) {
                if (input == "exit") {
                    return;
                }
            }
        }
    }
}
