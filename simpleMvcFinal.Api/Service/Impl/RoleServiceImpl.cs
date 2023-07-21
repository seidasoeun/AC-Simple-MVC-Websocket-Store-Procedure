using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Newtonsoft.Json;
using simpleMvcFinal.Api.Dto;
using simpleMvcFinal.Api.Models;

namespace simpleMvcFinal.Api.Service.Impl
{
    public class RoleServiceImpl : IRoleService
    {
        private readonly TokenServiceImpl _tokenService = new TokenServiceImpl();
        private readonly WebSocketServiceImpl _websocketService = new WebSocketServiceImpl();
        private readonly StoreProcedureServiceImpl _storeProcedureService = new StoreProcedureServiceImpl();
        public async Task GetAllRole(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, string token)
        {
            if (!_tokenService.IsAdmin(token))
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, "Unauthorized!");
                return;
            }

            DataTable data = _storeProcedureService.ProcessProcedureWithDataTable(new ProcedureModel
            {
                Action = "SELECT-ALL-ROLE"
            }, out _);

            var message = JsonConvert.SerializeObject(GetRoleListFromDataTable(data));
            await _websocketService.SendMessageToClient(connectedSockets, connectionId, message);
        }

        public async Task GetRole(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, string token, int id)
        {
            if (!_tokenService.IsAdmin(token))
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, "Unauthorized!");
                return;
            }

            DataTable data = _storeProcedureService.ProcessProcedureWithDataTable(new ProcedureModel
            {
                Action = "SELECT-ROLE",
                RoleId = id
            }, out _);

            if (data == null)
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, null);
                return;
            }

            var message = JsonConvert.SerializeObject(GetRoleListFromDataTable(data));
            await _websocketService.SendMessageToClient(connectedSockets, connectionId, message);
        }

        public async Task AddRole(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, RoleRequest req)
        {
            if (!_tokenService.IsAdmin(req.Token))
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, "Unauthorized!");
                return;
            }

            string addRole = _storeProcedureService.ProcessProcedureWithMessage(new ProcedureModel
            {
                Action = "ADD-ROLE",
                RoleName = req.RoleName
            }, out _);

            if (addRole != "SUCCESS")
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, "Add Role Fail!");
                return;
            }

            await GetAllRole(connectedSockets, connectionId, req.Token);
        }

        public async Task UpdateRole(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, RoleRequest req, int id)
        {
            if (!_tokenService.IsAdmin(req.Token))
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, "Unauthorized!");
                return;
            }

            string addRole = _storeProcedureService.ProcessProcedureWithMessage(new ProcedureModel
            {
                Action = "UPDATE-ROLE",
                RoleName = req.RoleName,
                RoleId = id
            }, out _);

            if (addRole != "SUCCESS")
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, "Update Role Fail!");
                return;
            }

            await GetAllRole(connectedSockets, connectionId, req.Token);
        }

        public async Task DeleteRole(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, string token, int id)
        {
            if (!_tokenService.IsAdmin(token))
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, "Unauthorized!");
                return;
            }

            string addRole = _storeProcedureService.ProcessProcedureWithMessage(new ProcedureModel
            {
                Action = "DELETE-ROLE",
                RoleId = id
            }, out _);

            if (addRole != "SUCCESS")
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, "Update Role Fail!");
                return;
            }

            await GetAllRole(connectedSockets, connectionId, token);
        }

        public async Task GetUser(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, RoleRequest req)
        {
            if (!_tokenService.IsAdmin(req.Token))
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, "Unauthorized!");
                return;
            }

            DataTable data = _storeProcedureService.ProcessProcedureWithDataTable(new ProcedureModel
            {
                Action = "SELECT-ALL-USER-WITH-ROLE",
                RoleName = req.RoleName,
            }, out _);

            if (data == null)
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, null);
                return;
            }

            var message = JsonConvert.SerializeObject(_storeProcedureService.GetUserListFromDataTable(data));
            await _websocketService.SendMessageToClient(connectedSockets, connectionId, message);
        }


        private List<RoleResponse> GetRoleListFromDataTable(DataTable data)
        {
            List<RoleResponse> res = new List<RoleResponse>();
            foreach (DataRow row in data.Rows)
            {
                res.Add(new RoleResponse
                {
                    RoleName = row["roleName"].ToString()
                });
            }

            return res;
        }


    }
}