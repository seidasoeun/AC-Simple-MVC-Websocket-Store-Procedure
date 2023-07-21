using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace simpleMvcFinal.Api.Service
{
    internal interface IWebSocketService
    {
        Task SendMessageToClient(
            ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, string message
        );
    }
}
