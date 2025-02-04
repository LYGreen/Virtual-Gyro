using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using Force.Crc32;
using static VirtualGyro.Cemuhook.Protocol;
using System.Collections;

namespace VirtualGyro.Cemuhook
{
    internal class Protocol
    {
        public const UInt32 ServerMagic = 0x53555344; // "DSUS"
        public const UInt32 ClientMagic = 0x43555344; // "DSUC"

        public enum DataType
        {
            ProtocolVersion = 0x00100000,
            PortInfo = 0x00100001,
            PadData = 0x00100002,
        }

        public struct Header
        {
            public Header() { }

            public UInt32 Magic;
            public UInt16 ProtocolVersion;
            public UInt16 DataSize; // packet size without header
            public UInt32 Crc32;
            public UInt32 Id;

            public UInt32 Type; // 不是 Header 里的数据但是为了方便放在了 Header 里面
        }

        public struct ProtocolVersionData
        {
            public ProtocolVersionData() { }

            public UInt16 Version;
            public UInt16 End = 0x0000; // 结束符，不要修改
        }

        public struct PortInfoData
        {
            public PortInfoData() { }

            public Byte Slot;
            public Byte SlotState;
            public Byte DeviceModel;
            public Byte ConnectionType;
            public Byte[] MacAddress = new Byte[6];
            public Byte BatteryStatus;

            public Byte End = 0x00; // 结束符，不要修改
        }

        public struct PortInfoClientData
        {
            public PortInfoClientData() { }

            public Int32 GamepadCount;
            public Byte[] GamepadIds = new Byte[4];
        }

        public struct PadData
        {
            public PadData() { }

            public Byte Slot;
            public Byte SlotState;
            public Byte DeviceModel;
            public Byte ConnectionType;
            public Byte[] MacAddress = new Byte[6];
            public Byte BatteryStatus;

            public Byte Connected;
            public UInt32 PacketNumber; // 可以为 0

            public Boolean DPadLeft;
            public Boolean DPadDown;
            public Boolean DPadRight;
            public Boolean DPadUp;
            public Boolean Options;
            public Boolean R3;
            public Boolean L3;
            public Boolean Share;

            public Boolean Y;
            public Boolean B;
            public Boolean A;
            public Boolean X;
            public Boolean R1;
            public Boolean L1;
            public Boolean R2;
            public Boolean L2;

            public Byte Home;
            public Byte Touch;

            public Byte LeftStickX;
            public Byte LeftStickY;
            public Byte RightStickX;
            public Byte RightStickY;

            public Byte AnalogDPadLeft;
            public Byte AnalogDPadDown;
            public Byte AnalogDPadRight;
            public Byte AnalogDPadUp;

            public Byte AnalogY;
            public Byte AnalogB;
            public Byte AnalogA;
            public Byte AnalogX;
            public Byte AnalogR1;
            public Byte AnalogL1;
            public Byte AnalogR2;
            public Byte AnalogL2;

            public struct touch
            {
                public Byte IsTouchActive;
                public Byte TouchId;
                public UInt16 TouchX;
                public UInt16 TouchY;
            };
            public touch FirstTouch;
            public touch SecondTouch;

            public UInt64 MotionDataTimestamp; // 单位: 微秒

            public UInt32 AccelX;
            public UInt32 AccelY;
            public UInt32 AccelZ;

            public UInt32 GyroPitch;
            public UInt32 GyroYaw;
            public UInt32 GyroRoll;
        }

        public static Header AnalysisHeader(byte[] packet)
        {
            int curIdx = 0;

            Header header = new Header();
            header.Magic = BitConverter.ToUInt32(packet, curIdx);
            curIdx += 4;

            header.ProtocolVersion = BitConverter.ToUInt16(packet, curIdx);
            curIdx += 2;

            header.DataSize = BitConverter.ToUInt16(packet, curIdx);
            curIdx += 2;

            header.Crc32 = BitConverter.ToUInt32(packet, curIdx);
            curIdx += 4;

            header.Id = BitConverter.ToUInt32(packet, curIdx);
            curIdx += 4;

            header.Type = BitConverter.ToUInt32(packet, curIdx);
            curIdx += 4;

            return header;
        }

        public static PortInfoClientData AnalysisPortInfoClientData(byte[] data)
        {
            int curIdx = 0;

            PortInfoClientData picd = new PortInfoClientData();
            
            picd.GamepadCount = BitConverter.ToInt32(data, curIdx);
            curIdx += 4;

            for (int i = 0; i < picd.GamepadCount; i++)
            {
                picd.GamepadIds[i] = data[curIdx++];
            }

            return picd;
        }

        public static byte[] RemovePacketHeader(byte[] packet)
        {
            int dataLength = packet.Length - 20; // 20 字节包含了 DataType
            byte[] data = new byte[dataLength];
            Array.Copy(packet, 20, data, 0, dataLength);

            return data;
        }

        public static byte[] GetHeaderBytes(Header header)
        {
            byte[] bytes = new byte[20];
            int curIdx = 0;

            Array.Copy(BitConverter.GetBytes(header.Magic), 0, bytes, curIdx, 4);
            curIdx += 4;

            Array.Copy(BitConverter.GetBytes(header.ProtocolVersion), 0, bytes, curIdx, 2);
            curIdx += 2;

            Array.Copy(BitConverter.GetBytes(header.DataSize), 0, bytes, curIdx, 2);
            curIdx += 2;

            Array.Copy(BitConverter.GetBytes(header.Crc32), 0, bytes, curIdx, 4);
            curIdx += 4;

            Array.Copy(BitConverter.GetBytes(header.Id), 0, bytes, curIdx, 4);
            curIdx += 4;

            Array.Copy(BitConverter.GetBytes(header.Type), 0, bytes, curIdx, 4);
            curIdx += 4;

            return bytes;
        }

        public static byte[] GetProtocolVersionBytes(ProtocolVersionData protocolVersionData)
        {
            byte[] bytes = new byte[4];
            int curIdx = 0;

            Array.Copy(BitConverter.GetBytes(protocolVersionData.Version), 0, bytes, curIdx, 2);
            curIdx += 2;

            Array.Copy(BitConverter.GetBytes(protocolVersionData.End), 0, bytes, curIdx, 2);
            curIdx += 2;

            return bytes;
        }

        public static byte[] GetPortInfoBytes(PortInfoData portInfoData)
        {
            byte[] bytes = new byte[12];
            int curIdx = 0;

            bytes[curIdx++] = portInfoData.Slot;
            bytes[curIdx++] = portInfoData.SlotState;
            bytes[curIdx++] = portInfoData.DeviceModel;
            bytes[curIdx++] = portInfoData.ConnectionType;

            Array.Copy(portInfoData.MacAddress, 0, bytes, curIdx, 6);
            curIdx += 6;

            bytes[curIdx++] = portInfoData.BatteryStatus;
            bytes[curIdx++] = 0;

            return bytes;
        }

        public static byte[] GetPadDataBytes(PadData padData)
        {
            byte[] bytes = new byte[12];
            int curIdx = 0;

            bytes[curIdx++] = padData.Slot;
            bytes[curIdx++] = padData.SlotState;
            bytes[curIdx++] = padData.DeviceModel;
            bytes[curIdx++] = padData.ConnectionType;

            Array.Copy(padData.MacAddress, 0, bytes, curIdx, 6);
            curIdx += 6;

            bytes[curIdx++] = padData.BatteryStatus;

            bytes[curIdx++] = padData.Connected;

            BitArray ba1 = new BitArray(new bool[8]
            {
                padData.DPadLeft, padData.DPadDown, padData.DPadRight, padData.DPadUp,
                padData.Options, padData.R3, padData.L3, padData.Share
            });
            ba1.CopyTo(bytes, curIdx);
            curIdx++;

            BitArray ba2 = new BitArray(new bool[8]
            {
                padData.Y, padData.B, padData.A, padData.X,
                padData.R1, padData.L1, padData.R2, padData.L2
            });
            ba2.CopyTo(bytes, curIdx);
            curIdx++;

            bytes[curIdx++] = padData.Home;
            bytes[curIdx++] = padData.Touch;
            bytes[curIdx++] = padData.LeftStickX;
            bytes[curIdx++] = padData.LeftStickY;
            bytes[curIdx++] = padData.RightStickX;
            bytes[curIdx++] = padData.RightStickY;
            bytes[curIdx++] = padData.AnalogDPadLeft;
            bytes[curIdx++] = padData.AnalogDPadDown;
            bytes[curIdx++] = padData.AnalogDPadRight;
            bytes[curIdx++] = padData.AnalogDPadUp;
            bytes[curIdx++] = padData.AnalogY;
            bytes[curIdx++] = padData.AnalogB;
            bytes[curIdx++] = padData.AnalogA;
            bytes[curIdx++] = padData.AnalogX;
            bytes[curIdx++] = padData.AnalogR1;
            bytes[curIdx++] = padData.AnalogL1;
            bytes[curIdx++] = padData.AnalogR2;
            bytes[curIdx++] = padData.AnalogL2;

            bytes[curIdx++] = padData.FirstTouch.IsTouchActive;
            bytes[curIdx++] = padData.FirstTouch.TouchId;
            Array.Copy(BitConverter.GetBytes(padData.FirstTouch.TouchX), 0, bytes, curIdx, 2);
            curIdx += 2;
            Array.Copy(BitConverter.GetBytes(padData.FirstTouch.TouchY), 0, bytes, curIdx, 2);
            curIdx += 2;

            bytes[curIdx++] = padData.SecondTouch.IsTouchActive;
            bytes[curIdx++] = padData.SecondTouch.TouchId;
            Array.Copy(BitConverter.GetBytes(padData.SecondTouch.TouchX), 0, bytes, curIdx, 2);
            curIdx += 2;
            Array.Copy(BitConverter.GetBytes(padData.SecondTouch.TouchY), 0, bytes, curIdx, 2);
            curIdx += 2;

            Array.Copy(BitConverter.GetBytes(padData.MotionDataTimestamp), 0, bytes, curIdx, 8);
            curIdx += 8;

            Array.Copy(BitConverter.GetBytes(padData.AccelX), 0, bytes, curIdx, 4);
            curIdx += 4;
            Array.Copy(BitConverter.GetBytes(padData.AccelY), 0, bytes, curIdx, 4);
            curIdx += 4;
            Array.Copy(BitConverter.GetBytes(padData.AccelZ), 0, bytes, curIdx, 4);
            curIdx += 4;

            Array.Copy(BitConverter.GetBytes(padData.GyroPitch), 0, bytes, curIdx, 4);
            curIdx += 4;
            Array.Copy(BitConverter.GetBytes(padData.GyroYaw), 0, bytes, curIdx, 4);
            curIdx += 4;
            Array.Copy(BitConverter.GetBytes(padData.GyroRoll), 0, bytes, curIdx, 4);
            curIdx += 4;

            return bytes;
        }

        public static byte[] BuildPacket(byte[] data, DataType dataType, uint id)
        {
            byte[] packet = BuildHeader(data, dataType, id);
            BuildCrc32(packet);

            return packet;
        }

        private static byte[] BuildHeader(byte[] data, DataType dataType, uint id)
        {
            byte[] packet = new byte[data.Length + 20]; // 20 字节包含了 DataType
            int curIdx = 0;

            // 字符串 "DSUS"
            packet[curIdx++] = (byte)'D';
            packet[curIdx++] = (byte)'S';
            packet[curIdx++] = (byte)'U';
            packet[curIdx++] = (byte)'S';

            // 协议版本
            ushort protocolVersion = 1001;
            Array.Copy(BitConverter.GetBytes(protocolVersion), 0, packet, curIdx, 2);
            curIdx += 2;

            // 数据大小
            ushort dataSize = (ushort)(data.Length + 4); // 包含 DataType
            Array.Copy(BitConverter.GetBytes(dataSize), 0, packet, curIdx, 2);
            curIdx += 2;

            // Crc32，暂时清零
            Array.Clear(packet, curIdx, 4);
            curIdx += 4;

            // Id
            Array.Copy(BitConverter.GetBytes(id), 0, packet, curIdx, 4);
            curIdx += 4;

            // 数据类型
            uint type = (uint)dataType;
            Array.Copy(BitConverter.GetBytes(type), 0, packet, curIdx, 4);
            curIdx += 4;

            // 数据
            Array.Copy(data, 0, packet, curIdx, data.Length);
            curIdx += data.Length;

            return packet;
        }

        private static void BuildCrc32(byte[] packet)
        {
            uint crc32 = Crc32Algorithm.Compute(packet, 0, packet.Length);
            Array.Copy(BitConverter.GetBytes(crc32), 0, packet, 8, 4);
        }
    }
}
