SharpTox
========
Unofficial
--------

This project aims to provide a simple library that wraps all of the functions found in the [Tox library](https://github.com/irungentoo/ProjectTox-Core "ProjectTox GitHub repo").
Tox is a free (as in freedom) Skype replacement.

Feel free to contribute!

## Remarks

I do not really maintain this version of SharpTox! I aim to have a base to play with and be able to use the basic features of Tox in C#!

Feel free to contribute but do not expect a working version!

The current version I program against is 0.2.3 of toxcore.

### Things you'll need

* The libtox library, you should compile that yourself from the [ProjectTox GitHub repo](https://github.com/irungentoo/ProjectTox-Core "Tox Github repo"). Guidelines on how to do this can be found [here](https://github.com/irungentoo/toxcore/blob/master/INSTALL.md "Crosscompile guidelines"). If you don't feel like compiling this yourself, you can find automatic builds for windows here: [x86](https://build.tox.chat/view/libtoxcore/job/libtoxcore-toktok_build_windows_x86_shared_release/ "x86 dll") or [x64](https://build.tox.chat/view/libtoxcore/job/libtoxcore-toktok_build_windows_x86-64_shared_release/ "x64 dll")

Depending on how you compiled the core libraries, the names of those may differ from the 
defaults in SharpTox. Be sure to change the value of the const string **DLL**
in Extern.cs accordingly if needed.

### Compiling and Testing
If you've set up your project like mentioned above you can go ahead and compile SharpTox.

##### .NET Core (Windows/Linux/Mac)
This Repository has been set up to work with **.NET Core**.

Please follow the [How To Setup .NET Core](https://www.microsoft.com/net/core) Guide from
Microsoft to setup your Environment for building and testing SharpTox.
When you've setup everything accordingly to your system go to the project top directory of
SharpTox and execute following in your commandline:

```
dotnet restore
dotnet build
```

#### Run the Tests
In order to run the Tests for SharpTox make sure the **libtox** library is in your
SharpTox.Tests/bin/(Configuration) directory.
Run from your commandline:

```
dotnet test
```

## Attention:
This repository is currently mainly for my own programming uses.
If anything comes up or this changes I will write it down somewhere.
Also I'm developing on Windows. So I'm not certain everything is working on Unix/Mac.

**Looking for precompiled binaries? [Check this](https://jenkins.impy.me/ "SharpTox Binaries").**

### Basic Usage
```csharp
using System;
using SharpTox.Core;

class Program
{
    static Tox tox;

    static void Main(string[] args)
    {
        ToxOptions options = new ToxOptions(true, true);

        tox = new Tox(options);
        tox.OnFriendRequestReceived += tox_OnFriendRequestReceived;
        tox.OnFriendMessageReceived += tox_OnFriendMessageReceived;

        foreach (ToxNode node in Nodes)
            tox.Bootstrap(node);

        tox.Name = "SharpTox";
        tox.StatusMessage = "Testing SharpTox";

        tox.Start();

        string id = tox.Id.ToString();
        Console.WriteLine("ID: {0}", id);

        Console.ReadKey();
        tox.Dispose();
    }

    //check https://wiki.tox.im/Nodes for an up-to-date list of nodes
    static ToxNode[] Nodes = new ToxNode[]
    {
        new ToxNode("192.254.75.98", 33445, new ToxKey(ToxKeyType.Public, "951C88B7E75C867418ACDB5D273821372BB5BD652740BCDF623A4FA293E75D2F")),
        new ToxNode("144.76.60.215", 33445, new ToxKey(ToxKeyType.Public, "04119E835DF3E78BACF0F84235B300546AF8B936F035185E2A8E9E0A67C8924F"))
    };

    static void tox_OnFriendMessageReceived(object sender, ToxEventArgs.FriendMessageEventArgs e)
    {
        //get the name associated with the friendnumber
        string name = tox.GetFriendName(e.FriendNumber);

        //print the message to the console
        Console.WriteLine("<{0}> {1}", name, e.Message);
    }

    static void tox_OnFriendRequestReceived(object sender, ToxEventArgs.FriendRequestEventArgs e)
    {
        //automatically accept every friend request we receive
        tox.AddFriendNoRequest(e.PublicKey);
    }
}

```

Contact
-------
* Join the official IRC channel #tox-dev on freenode
[![Official Tox Dev IRC Channel](https://kiwiirc.com/buttons/irc.freenode.net/tox-dev.png)](https://kiwiirc.com/client/irc.freenode.net/?theme=basic#tox-dev)
