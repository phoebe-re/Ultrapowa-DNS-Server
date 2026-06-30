using System.Collections.Generic;

namespace DNS.Server
{
    public class DnsRequest
    {
        public ushort Id { get; set; }
        public List<DnsQuestion> Questions { get; set; } = new List<DnsQuestion>();

        public static DnsRequest FromArray(byte[] message)
        {
            var request = new DnsRequest();
            int offset = 0;

            request.Id = (ushort)((message[0] << 8) | message[1]);
            ushort questionCount = (ushort)((message[4] << 8) | message[5]);

            offset = 12; // fixed 12-byte header

            for (int i = 0; i < questionCount; i++)
                request.Questions.Add(DnsQuestion.FromArray(message, ref offset));

            return request;
        }

        public byte[] ToArray()
        {
            List<byte> bytes = new List<byte>
            {
                (byte)(Id >> 8), (byte)(Id & 0xFF),
                0x01, 0x00, // standard query
                (byte)(Questions.Count >> 8), (byte)(Questions.Count & 0xFF),
                0x00, 0x00, // answer count
                0x00, 0x00, // authority count
                0x00, 0x00 // additional count
            };

            foreach (var q in Questions)
                bytes.AddRange(q.ToArray());

            return bytes.ToArray();
        }

        public override string ToString() =>
            Questions.Count > 0 ? Questions[0].ToString() : "(empty request)";
    }
}