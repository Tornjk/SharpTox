using System;
using SharpTox.Core;
using SharpTox.Core.Interfaces;

namespace SharpTox.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            using (IToxOptions options = ToxOptions.Default())
            using (ITox tox = options.Create())
            {
                tox.OnFriendRequestReceived += OnFriendRequestReceived;
                tox.OnFriendMessageReceived += OnFriendMessageReceived;
                tox.OnConnectionStatusChanged += Tox_OnConnectionStatusChanged;

                foreach (ToxNode node in Nodes)
                {
                    tox.Bootstrap(node, out _);
                }

                tox.Name = "SharpTox";
                tox.StatusMessage = "Testing SharpTox";

                using (ToxLoop.Start(tox))
                {
                    Console.WriteLine($"ID: {tox.Id}");
                    Console.ReadKey();
                }

                void OnFriendMessageReceived(object sender, ToxEventArgs.FriendMessageEventArgs e)
                {
                    //get the name associated with the friendnumber
                    string name = tox.GetFriendName(e.FriendNumber, out _);

                    //print the message to the console
                    Console.WriteLine("<{0}> {1}", name, e.Message);
                }

                void OnFriendRequestReceived(object sender, ToxEventArgs.FriendRequestEventArgs e)
                {
                    //automatically accept every friend request we receive
                    tox.AddFriendNoRequest(e.PublicKey, out _);
                }
            }
        }

        private static void Tox_OnConnectionStatusChanged(object sender, ToxEventArgs.ConnectionStatusEventArgs e)
        {
            Console.WriteLine(e.Status);
        }

        //check https://wiki.tox.im/Nodes for an up-to-date list of nodes
        private static readonly ToxNode[] Nodes = new ToxNode[]
        {
            new ToxNode("node.tox.biribiri.org", 33445, new ToxKey(ToxKeyType.Public, "8E7D0B859922EF569298B4D261A8CCB5FEA14FB91ED412A7603A585A25698832")),
            new ToxNode("tox.verdict.gg", 33445, new ToxKey(ToxKeyType.Public, "1C5293AEF2114717547B39DA8EA6F1E331E5E358B35F9B6B5F19317911C5F976")),
        };
    }
}
