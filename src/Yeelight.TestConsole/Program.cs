﻿using System;

namespace Thoemmi.Yeelight.TestConsole {
    internal class Program {
        private static void Main() {
            var listener = new DeviceListener();
            listener.DeviceInformationReceived += (_, args) => {
                Console.WriteLine($"{args.Reason}: device {args.Device.Name} (ID {args.Device.Id})");
            };

            listener.StartListening();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}