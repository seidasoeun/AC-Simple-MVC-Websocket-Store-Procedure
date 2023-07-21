using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using simpleMvcFinal.Api.Dto;

namespace simpleMvcFinal.Api.Service
{
    internal interface IRoleService
    {
        Task GetAllRole(
            ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> connectedSockets,
            string connectionId, string token);

        Task GetRole(
            ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> connectedSockets,
            string connectionId, string token, int id);

        Task AddRole(
            ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> connectedSockets,
            string connectionId, RoleRequest req);

        Task UpdateRole(
            ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> connectedSockets,
            string connectionId, RoleRequest req, int id);

        Task DeleteRole(
            ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> connectedSockets,
            string connectionId, string token, int id);

        Task GetUser(
            ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> connectedSockets,
            string connectionId, RoleRequest req);
    }
}
