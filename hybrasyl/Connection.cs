﻿/*
 * This file is part of Project Hybrasyl.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the Affero General Public License as published by
 * the Free Software Foundation, version 3.
 *
 * This program is distributed in the hope that it will be useful, but
 * without ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
 * or FITNESS FOR A PARTICULAR PURPOSE. See the Affero General Public License
 * for more details.
 *
 * You should have received a copy of the Affero General Public License along
 * with this program. If not, see <http://www.gnu.org/licenses/>.
 *
 * (C) 2020 ERISCO, LLC 
 *
 * For contributors and individual authors please refer to CONTRIBUTORS.MD.
 * 
 */

using Hybrasyl.Enums;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Hybrasyl;

public static class GlobalConnectionManifest
{
    private static long _connectionId;
    public static long GetNewConnectionId()
    {
        Interlocked.Increment(ref _connectionId);
        return _connectionId;
    }

    public static ConcurrentDictionary<long, Client> ConnectedClients = new ConcurrentDictionary<long, Client>();
    public static ConcurrentDictionary<long, Client> WorldClients = new ConcurrentDictionary<long, Client>();
    public static ConcurrentDictionary<long, Redirect> Redirects = new ConcurrentDictionary<long, Redirect>();

    public static void RegisterRedirect(Client client, Redirect redirect)
    {
        Redirects[client.ConnectionId] = redirect;
    }

    public static bool TryGetRedirect(long cid, out Redirect redirect)
    {
        return Redirects.TryGetValue(cid, out redirect);
    }

    public static void RegisterClient(Client client)
    {
        ConnectedClients[client.ConnectionId] = client;
        if (client.ServerType == ServerTypes.World)
            WorldClients[client.ConnectionId] = client;
    }

    public static void DeregisterClient(Client client)
    {
        ConnectedClients.TryRemove(client.ConnectionId, out Client _);
        GameLog.InfoFormat("Deregistering {0}", client.ConnectionId);
        // Send a control message to clean up after World users; Lobby and Login handle themselves
        if (client.ServerType == ServerTypes.World)
        {
            if (!WorldClients.TryRemove(client.ConnectionId, out Client _))
                GameLog.Error("Couldn't deregister cid {id}", client.ConnectionId);
            try
            {
                if (!World.ControlMessageQueue.IsCompleted)
                    World.ControlMessageQueue.Add(new HybrasylControlMessage(ControlOpcodes.CleanupUser, 
                        CleanupType.ByConnectionId, client.ConnectionId));
            }
            catch (InvalidOperationException e)
            {
                Game.ReportException(e);
                if (!World.ControlMessageQueue.IsCompleted)
                    GameLog.ErrorFormat("Connection {id}: DeregisterClient failed", client.ConnectionId);
            }
        }
    }

    public static byte[] RequestEncryptionKey(string endpoint, IPAddress remoteAddress)
    {
        byte[] key;

        try
        {
            var seed = new Seed() { Ip = remoteAddress.ToString() };

            var webReq = WebRequest.Create(new Uri(endpoint));
            webReq.ContentType = "application/json";
            webReq.Method = "POST";

            var json = JsonSerializer.Serialize(seed);

            using (var sw = new StreamWriter(webReq.GetRequestStream()))
            {
                sw.Write(json);
            }

            var response = webReq.GetResponse();
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                key = (byte[])JsonSerializer.Deserialize(sr.ReadToEnd(), typeof(byte[]));
            }
        }
        catch (Exception e)
        {
            Game.ReportException(e);
            GameLog.Error("RequestEncryptionKey failure: {e}", e);
            key = Encoding.ASCII.GetBytes("NOTVALID!");
        }
        return key;
    }

    public static bool ValidateEncryptionKey(string endpoint, ServerToken token)
    {
        bool valid;

        try
        {
            var webReq = WebRequest.Create(new Uri(endpoint));
            webReq.ContentType = "application/json";
            webReq.Method = "POST";

            var json = JsonSerializer.Serialize(token);

            using (var sw = new StreamWriter(webReq.GetRequestStream()))
            {
                sw.Write(json);
            }

            var response = webReq.GetResponse();
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                valid = (bool)JsonSerializer.Deserialize(sr.ReadToEnd(), typeof(bool));
            }
        }
        catch (Exception e)
        {
            Game.ReportException(e);
            GameLog.Error("ValidateEncryptionKey failure: {e}", e);
            return false;
        }
        return valid;
    }
}

public class HybrasylMessage
{
    public Int64 Ticks { get; private set; }
    // Maybe this can be like, idk, function name or something? Thread context? Whatever?
    public string Sender { get; private set; }
    public object[] Arguments { get; private set; }

    public HybrasylMessage(string sender = "HybrasylMessage", params object[] parameters)
    {
        Ticks = DateTime.Now.Ticks;
        Sender = sender;
        Arguments = parameters;
    }
}

public class HybrasylClientMessage : HybrasylMessage
{
    public ClientPacket Packet { get; private set; }
    public long ConnectionId { get; private set; }

    public HybrasylClientMessage(ClientPacket packet, long connectionId, params object[] arguments) : 
        base("HybrasylClientMessage", arguments)
    {
        Packet = packet;
        ConnectionId = connectionId;
    }
}

public class HybrasylControlMessage : HybrasylMessage
{
    public int Opcode;
 
    public HybrasylControlMessage(int opcode, params object[] parameters) 
        : base("HybrasylControlMessage", parameters)
    {
        Opcode = opcode;
    }
}

[Serializable]
public class ServerToken
{
    public byte[] Seed { get; set; }
    public string Ip { get; set; }
}

[Serializable]
public class Seed
{
    public string Ip { get; set; }
    public string Key { get; set; }
}