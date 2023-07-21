using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace simpleMvcFinal.Api.Service.Impl
{
    public class WebSocketServiceImpl : IWebSocketService
    {
        public async Task SendMessageToClient(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);

            if (connectedSockets.TryGetValue(connectionId, out var clientWebSocket))
            {
                await clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}