using System.Collections.Generic;
using System.Net;

namespace DNS.Server
{
    public class DnsAnswer
    {
        public string Name { get; set; }
        public IPAddress Address { get; set; }

        public DnsAnswer(string name, IPAddress address)
        {
            Name = name;
            Address = address;
        }
    }

    public class DnsResponse
    {
        public ushort Id { get; set; }
        public List<DnsQuestion> Questions { get; set; } = new List<DnsQuestion>();
        public List<DnsAnswer> Answers { get; set; } = new List<DnsAnswer>();

        public static DnsResponse FromRequest(DnsRequest request)
        {
            return new DnsResponse
            {
                Id = request.Id,
                Questions = request.Questions
            };
        }

        public byte[] ToArray()
        {
            List<byte> bytes = new List<byte>
            {
                (byte)(Id >> 8), (byte)(Id & 0xFF),
                0x81, 0x80, // standard response
                (byte)(Questions.Count >> 8), (byte)(Questions.Count & 0xFF),
                (byte)(Answers.Count >> 8), (byte)(Answers.Count & 0xFF),
                0x00, 0x00, // authority count
                0x00, 0x00 // additional count
            };

            foreach (var q in Questions)
                bytes.AddRange(q.ToArray());

            foreach (var answer in Answers)
            {
                bytes.AddRange(DnsQuestion.EncodeName(answer.Name));
                bytes.AddRange(new byte[] { 0x00, 0x01, 0x00, 0x01 }); // A, IN
                bytes.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x3C }); // 60s
                bytes.AddRange(new byte[] { 0x00, 0x04 }); // 4 bytes
                bytes.AddRange(answer.Address.GetAddressBytes());
            }

            return bytes.ToArray();
        }
    }
}