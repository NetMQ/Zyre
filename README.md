NetMQ.Zyre
==========

Zyre does local area discovery and clustering. A Zyre node broadcasts UDP beacons, and connects to peers that it finds. This class wraps a Zyre node with a message-based API.

NetMQ.Zyre is the C\# implementation of the ZeroMQ RealMQ Realtime Exchange Protocol found at <http://rfc.zeromq.org/spec:36>. Other implementations exist for C, Java, Python and more. This is version 2, **not** the first version (from <http://rfc.zeromq.org/spec:20>).

This C\# implementation is based primarily on the C zeromq/zyre implementation at <https://github.com/zeromq/zyre> ...but there is no implementation using the zgossip capability in czmq.

This C\# implementation offers an optional Event-based subscription to messages from peers. See Zyre.cs.

Installation
------------

Coming soon. ~~You can download NetMQ via~~ [~~NuGet~~](https://nuget.org/packages/NetMQ/)~~.~~

Dependencies
------------

The only dependency is NetMQ. You can download NetMQ via [NuGet](https://nuget.org/packages/NetMQ/).

This project targets .Net 4, but it uses C\# 6.0.

Using / Documentation
---------------------

This project assumes you have some familiarity with NetMQ.

Before using NetMQ, make sure to read the [ZeroMQ Guide](http://zguide.zeromq.org/page:all).

The NetMQ documentation can be found at [netmq.readthedocs.org](http://netmq.readthedocs.org/en/latest/).

The SamplePeer project under the Samples directory demonstrates the features supported by Zyre. To run several instances at once, run a batch file like Run3SamplePeers.bat in the build directory.

### Getting Started

\_zyre = new Zyre(“Node1”, true);

\_name = \_zyre.Name();

\_uuid = \_zyre.Uuid();

\_zyre.EnterEvent += ZyreEnterEvent;

\_zyre.StopEvent += ZyreStopEvent;

\_zyre.ExitEvent += ZyreExitEvent;

\_zyre.EvasiveEvent += ZyreEvasiveEvent;

\_zyre.JoinEvent += ZyreJoinEvent;

\_zyre.LeaveEvent += ZyreLeaveEvent;

\_zyre.WhisperEvent += ZyreWhisperEvent;

\_zyre.ShoutEvent += ZyreShoutEvent;

Then handle the events and make calls to the Zyre API. For example:

\_zyre.Join(“MyGroup”);

var peers = \_zyre.Peers();

Who owns NetMQ.Zyre?
====================

NetMQ.Zyre is owned by all its authors and contributors. This is an open source project licensed under the MPL 2.0. To contribute to NetMQ please read the [C4.1 process](http://rfc.zeromq.org/spec:22), it's what we use. Contributors are welcome!
