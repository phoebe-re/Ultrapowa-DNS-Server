using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DNS.Server
{
    public class DnsServer
    {
        public MasterFile MasterFile { get; } = new MasterFile();
        public IPEndPoint UpstreamDns { get; }

        public event Action<DnsRequest> Requested;
        public event Action<DnsRequest, DnsResponse> Responded;
        public event Action<Exception> Errored;

        private UdpClient udp;

        public DnsServer(string upstreamIp, int upstreamPort = 53)
        {
            UpstreamDns = new IPEndPoint(IPAddress.Parse(upstreamIp), upstreamPort);
        }

        public void Listen(int port = 53, IPAddress listenAddress = null)
        {
            udp = new UdpClient(new IPEndPoint(listenAddress ?? IPAddress.Any, port));
            _ = ListenLoop();
        }

        private async Task ListenLoop()
        {
            while (true)
            {
                try
                {
                    var result = await udp.ReceiveAsync();
                    _ = HandleRequest(result.Buffer, result.RemoteEndPoint);
                }
                catch (Exception ex)
                {
                    Errored?.Invoke(ex);
                }
            }
        }

        private async Task HandleRequest(byte[] data, IPEndPoint sender)
        {
            try
            {
                DnsRequest request = DnsRequest.FromArray(data);
                Requested?.Invoke(request);

                if (request.Questions.Count > 0 &&
                    MasterFile.TryResolve(request.Questions[0].Name, out IPAddress address))
                {
                    DnsResponse response = DnsResponse.FromRequest(request);
                    response.Answers.Add(new DnsAnswer(request.Questions[0].Name, address));

                    byte[] bytes = response.ToArray();
                    await udp.SendAsync(bytes, bytes.Length, sender);
                    Responded?.Invoke(request, response);
                }
                else
                {
                    using (var forwardClient = new UdpClient())
                    {
                        await forwardClient.SendAsync(data, data.Length, UpstreamDns);
                        var upstreamResult = await forwardClient.ReceiveAsync();
                        await udp.SendAsync(upstreamResult.Buffer, upstreamResult.Buffer.Length, sender);
                    }
                }
            }
            catch (Exception ex)
            {
                Errored?.Invoke(ex);
            }
        }
    }
}