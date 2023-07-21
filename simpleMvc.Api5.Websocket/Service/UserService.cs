using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using simpleMvc.Api5.Websocket.Dto;

namespace simpleMvc.Api5.Websocket.Service
{
    internal interface UserService
    {
        Task Login(
            ConcurrentDictionary<string, WebSocket> connectedSockets,
            string connectionId, string username, string password);

        Task Register(
            ConcurrentDictionary<string, WebSocket> connectedSockets,
            string connectionId, RegisterRequest userReq);

        Task GetInformationUser(
            ConcurrentDictionary<string, WebSocket> connectedSockets,
            string connectionId, string token);

        Task GetToken(
            ConcurrentDictionary<string, WebSocket> connectedSockets,
            string connectionId, string refreshToken);
    }
}
