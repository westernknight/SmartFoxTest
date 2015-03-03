using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Data;
using Sfs2X.Logging;
using Sfs2X.Requests;
using Sfs2X.Requests.MMO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartFoxTest
{
    class Program
    {
        static void Main(string[] args)
        {

            string zone = "BasicExamples";
            string username = "1212";
            string UNITY_MMO_ROOM = "UnityMMODemo";

            LoginRequest req = new LoginRequest(username, "", zone);
            SmartFox smartFox = new SmartFox(true);
            Console.WriteLine("API Version: " + smartFox.Version);

            smartFox.AddEventListener(SFSEvent.CONNECTION, (evt) =>
            {
                bool success = (bool)evt.Params["success"];
                string error = (string)evt.Params["errorMessage"];

                Console.WriteLine("On Connection callback got: " + success + " (error : <" + error + ">)");

                if (success)
                {
                    Console.WriteLine("Connection succesful!");
                    smartFox.Send(new LoginRequest(username, "", zone));
                }
                else
                {
                    Console.WriteLine("Can't connect to server!");
                }

            });
            smartFox.AddEventListener(SFSEvent.LOGIN, (evt) =>
            {
                Console.WriteLine("Logged in successfully");
                if (smartFox.GetRoomByName(UNITY_MMO_ROOM) == null)
                {
                    var settings = new MMORoomSettings(UNITY_MMO_ROOM);
                    settings.DefaultAOI = new Vec3D(25f, 1f, 25f);
                    settings.MapLimits = new MapLimits(new Vec3D(-100f, 1f, -100f), new Vec3D(100f, 1f, 100f));
                    settings.MaxUsers = 100;
                    settings.Extension = new RoomExtension("pyTest", "MMORoomDemo.py");

                    //settings.ProximityListUpdateMillis = 50;
                    smartFox.Send(new CreateRoomRequest(settings, true, null));
                }
                else
                {
                    // We either create the Game Room or join it if it exists already
                    smartFox.Send(new JoinRoomRequest(UNITY_MMO_ROOM));
                }
            });
            smartFox.AddEventListener(SFSEvent.ROOM_JOIN, (evt) =>
            {
                Console.WriteLine("Joined room successfully");
                //smartFox.RemoveAllEventListeners();
            });

            smartFox.AddLogListener(LogLevel.DEBUG, (evt) =>
            {
                string message = (string)evt.Params["message"];
                Console.WriteLine("[SFS DEBUG] " + message);
            });

            //
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (smartFox != null)
                    {
                        smartFox.ProcessEvents();
                    }
                    Thread.Sleep(10);
                }
            });

            Console.Write("write user name:");
            username = Console.ReadLine();
            smartFox.Connect("192.168.0.113", 9933);




            while (true)
            {

                string sendstring = Console.ReadLine();
                Console.WriteLine("send: " + sendstring);
            }


          
        }
    }
}
