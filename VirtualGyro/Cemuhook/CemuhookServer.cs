using Force.Crc32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGyro.Cemuhook
{
    internal class CemuhookServer
    {
        private byte[] receivedData = new byte[1024];
        private List<IPEndPoint> clientEPs = new List<IPEndPoint>();
        private Stopwatch stopwatch = new Stopwatch();
        private Socket? serverSocket;
        private uint _serverId;
        public uint ServerId
        {
            get => _serverId;
        }

        private bool _isRunning = false;
        public bool IsRunning
        {
            get => _isRunning;
        }

        public void Start(string ip = "127.0.0.1", int port = 26760)
        {
            if (serverSocket != null)
            {
                serverSocket.Close();
            }
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            serverSocket.Bind(ep);

            byte[] id = new byte[4];
            new Random().NextBytes(id);
            _serverId = BitConverter.ToUInt32(id, 0);

            _isRunning = true;

            stopwatch.Start();

            StartReceive();
        }

        public void Stop()
        {
            if (serverSocket != null)
            {
                serverSocket.Close();
                serverSocket = null;
                clientEPs.Clear();
            }
            _isRunning = false;
        }

        private void StartReceive()
        {
            try
            {
                if (serverSocket == null)
                {
                    return;
                }

                EndPoint ep = new IPEndPoint(IPAddress.Any, 0);
                serverSocket.BeginReceiveFrom(receivedData, 0, receivedData.Length, SocketFlags.None, ref ep, ReceiveCallback, serverSocket);
            }
            catch (Exception ex)
            {
                Stop();
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (serverSocket == null)
                {
                    return;
                }

                byte[]? localMsg = null;
                EndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);

                int localMsgLength = serverSocket.EndReceiveFrom(ar, ref clientEP);
                localMsg = new byte[localMsgLength];
                Array.Copy(receivedData, 0, localMsg, 0, localMsgLength);
                if (!clientEPs.Contains(clientEP))
                {
                    clientEPs.Add((IPEndPoint)clientEP);
                }

                StartReceive();

                HandleMessage(localMsg, (IPEndPoint)clientEP);
            }
            catch (Exception ex)
            {
                Stop();
            }
        }

        public void SendPadData(byte[] padData)
        {
            if (stopwatch.Elapsed.TotalSeconds > 5) // 超过 5 秒没见客户端发送 PadData 请求
            {
                stopwatch.Stop();
                return;
            }

            byte[] packet = Protocol.BuildPacket(padData, Protocol.DataType.PadData, _serverId);
            foreach (IPEndPoint iPEndPoint in clientEPs)
            {
                serverSocket.SendTo(packet, iPEndPoint);
            }
        }

        private void HandleMessage(byte[] localMsg, IPEndPoint clientEP)
        {
            try
            {
                Protocol.Header header = Protocol.AnalysisHeader(localMsg);
                if (header.Magic != Protocol.ClientMagic ||
                    header.ProtocolVersion < 0 || header.ProtocolVersion > 1001 ||
                    header.DataSize != localMsg.Length - 16)
                {
                    return;
                }
                uint localCrc32 = BitConverter.ToUInt32(localMsg, 8);
                Array.Clear(localMsg, 8, 4);
                uint crc32 = Crc32Algorithm.Compute(localMsg, 0, localMsg.Length);
                if (localCrc32 != crc32)
                {
                    return;
                }

                byte[] packet = null;
                switch (header.Type)
                {
                    case (uint)Protocol.DataType.ProtocolVersion:
                        Protocol.ProtocolVersionData protocolVersionData = new Protocol.ProtocolVersionData();
                        protocolVersionData.Version = 1001;
                        byte[] protocolVersionBytes = Protocol.GetProtocolVersionBytes(protocolVersionData);
                        packet = Protocol.BuildPacket(protocolVersionBytes, Protocol.DataType.ProtocolVersion, _serverId);
                        serverSocket.SendTo(packet, clientEP);
                        break;
                    case (uint)Protocol.DataType.PortInfo:
                        byte[] data = Protocol.RemovePacketHeader(localMsg);
                        Protocol.PortInfoClientData portInfoClientData = Protocol.AnalysisPortInfoClientData(data);
                        for (int i = 0; i < portInfoClientData.GamepadCount; i++)
                        {
                            Protocol.PortInfoData portInfoData = new Protocol.PortInfoData();
                            portInfoData.Slot = portInfoClientData.GamepadIds[i];
                            portInfoData.SlotState = i == 0 ? (byte)2 : (byte)0;
                            portInfoData.DeviceModel = 2; // full gyro
                            portInfoData.ConnectionType = 2; // 蓝牙
                            portInfoData.MacAddress[0] = 0x00;
                            portInfoData.MacAddress[1] = 0x01;
                            portInfoData.MacAddress[2] = 0x02;
                            portInfoData.MacAddress[3] = 0x03;
                            portInfoData.MacAddress[4] = 0x04;
                            portInfoData.MacAddress[5] = 0x05;
                            portInfoData.BatteryStatus = 0x05; // 满电
                            byte[] portInfoBytes = Protocol.GetPortInfoBytes(portInfoData);
                            packet = Protocol.BuildPacket(portInfoBytes, Protocol.DataType.PortInfo, _serverId);
                            serverSocket.SendTo(packet, clientEP);
                        }
                        break;
                    case (uint)Protocol.DataType.PadData:
                        stopwatch.Restart();
                        break;
                }
            }
            catch(Exception ex)
            {
                Stop();
            }
        }
    }
}
