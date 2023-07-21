using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading.Tasks;
using simpleMvcFinal.Api.Dto;

namespace simpleMvcFinal.Api.Service
{
    internal interface IUserService
    {
        Task Login(
            ConcurrentDictionary<string, WebSocket> connectedSockets,
            string connectionId, LoginRequest req);

        Task GetAllUser(
            ConcurrentDictionary<string, WebSocket> connectedSockets,
            string connectionId, string token);

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
