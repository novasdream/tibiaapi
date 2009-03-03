﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Tibia.Util;

namespace Tibia.Packets
{
    public class Packet
    {
        public uint PacketId { get; set; }
        public bool Forward { get; set; }
        public PacketDestination Destination { get; set; }
        public Objects.Client Client { get; set; }

        public Packet(Objects.Client c)
        {
            Client = c;
            Forward = true;
        }

        public virtual byte[] ToByteArray() 
        {
            return null;
        }

        public bool Send() 
        {
            if (Client.IO.UsingProxy)
            {
                NetworkMessage msg = new NetworkMessage(Client);
                msg.AddBytes(ToByteArray());
                msg.InsetLogicalPacketHeader();
                msg.PrepareToSend();

                if (Destination == PacketDestination.Client)
                    Client.IO.Proxy.SendToClient(msg);
                else if (Destination == PacketDestination.Server)
                    Client.IO.Proxy.SendToServer(msg);
                else
                    return false;

                return true;

            }
            else if (Destination == PacketDestination.Server)
            {
                byte[] packet = ToByteArray();
                byte[] sendPacket = new byte[packet.Length + 2];
                Array.Copy(packet, 0, sendPacket, 2, packet.Length);
                Array.Copy(BitConverter.GetBytes((ushort)packet.Length), sendPacket, 2);

                return SendPacketByMemory(Client, sendPacket);
            }

            return false;
        }

        #region Sending Packets with Stepler's Method
        // (http://www.tpforums.org/forum/showthread.php?t=2832)
        /// <summary>
        /// Send a packet through the client by writing some code in memory and running it.
        /// The packet must not contain any header(no length nor Adler checksum) and be unencrypted
        /// </summary>
        /// <param name="client"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static bool SendPacketByMemory(Objects.Client client, Byte[] packet)
        {
            if (client.LoggedIn)
            {
                if (!client.IO.IsSendCodeWritten)
                    if (!client.IO.WriteSocketSendCode()) return false;


                NetworkMessage msg = new NetworkMessage(client);
                msg.AddBytes(packet);
                msg.PrepareToSend();

                uint bufferSize = (uint)(4 + msg.Length);
                byte[] readyPacket = new byte[bufferSize];
                Array.Copy(BitConverter.GetBytes(msg.Length), readyPacket, 4);
                Array.Copy(msg.GetBuffer(), 0, readyPacket, 4, msg.Length);

                IntPtr pRemote = Tibia.Util.WinApi.VirtualAllocEx(client.ProcessHandle, IntPtr.Zero, /*bufferSize*/
                                            bufferSize,
                                            Tibia.Util.WinApi.MEM_COMMIT | Tibia.Util.WinApi.MEM_RESERVE,
                                            Tibia.Util.WinApi.PAGE_EXECUTE_READWRITE);

                if (pRemote != IntPtr.Zero)
                {
                    if (client.Memory.WriteBytes(pRemote.ToInt64(), readyPacket, bufferSize))
                    {
                        IntPtr threadHandle = Tibia.Util.WinApi.CreateRemoteThread(client.ProcessHandle, IntPtr.Zero, 0,
                            client.IO.SenderAddress, pRemote, 0, IntPtr.Zero);
                        Tibia.Util.WinApi.WaitForSingleObject(threadHandle, 0xFFFFFFFF);//INFINITE=0xFFFFFFFF
                        Tibia.Util.WinApi.CloseHandle(threadHandle);
                        return true;
                    }
                }
                return false;

            }
            else return false;
        }


        #endregion


        public virtual bool ParseMessage(NetworkMessage msg, PacketDestination destination)
        { 
            return false;
        }
    }
}