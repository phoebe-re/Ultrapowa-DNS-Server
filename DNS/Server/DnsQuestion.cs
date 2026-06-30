using System;
using System.Collections.Generic;

namespace DNS.Server
{
    public class DnsQuestion
    {
        public string Name { get; set; }
        public ushort Type { get; set; } = 1;   // 1 = A record
        public ushort Class { get; set; } = 1;  // 1 = IN (internet)

        public DnsQuestion() { }

        public DnsQuestion(string name, ushort type = 1, ushort cls = 1)
        {
            Name = name;
            Type = type;
            Class = cls;
        }

        public static DnsQuestion FromArray(byte[] message, ref int offset)
        {
            string name = ReadName(message, ref offset);
            ushort type = (ushort)((message[offset] << 8) | message[offset + 1]);
            ushort cls = (ushort)((message[offset + 2] << 8) | message[offset + 3]);
            offset += 4;

            return new DnsQuestion(name, type, cls);
        }

        public byte[] ToArray()
        {
            List<byte> bytes = new List<byte>();
            bytes.AddRange(EncodeName(Name));
            bytes.Add((byte)(Type >> 8));
            bytes.Add((byte)(Type & 0xFF));
            bytes.Add((byte)(Class >> 8));
            bytes.Add((byte)(Class & 0xFF));
            return bytes.ToArray();
        }

        public static byte[] EncodeName(string name)
        {
            List<byte> bytes = new List<byte>();
            foreach (string label in name.Split('.'))
            {
                bytes.Add((byte)label.Length);
                bytes.AddRange(System.Text.Encoding.ASCII.GetBytes(label));
            }
            bytes.Add(0);
            return bytes.ToArray();
        }

        public static string ReadName(byte[] message, ref int offset)
        {
            List<string> labels = new List<string>();
            while (true)
            {
                byte len = message[offset];
                if (len == 0)
                {
                    offset++;
                    break;
                }

                offset++;
                labels.Add(System.Text.Encoding.ASCII.GetString(message, offset, len));
                offset += len;
            }
            return string.Join(".", labels);
        }

        public override string ToString() => $"{Name} (type {Type}, class {Class})";
    }
}