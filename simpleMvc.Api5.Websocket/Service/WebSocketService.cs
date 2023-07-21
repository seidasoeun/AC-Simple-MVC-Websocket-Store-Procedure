using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace simpleMvc.Api5.Websocket.Service
{
    internal interface WebSocketService
    {
        Task SendMessageToClient(
            ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, string message
        );
    }
}
