using System.Collections.Concurrent;
using System.Data;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Newtonsoft.Json;
using simpleMvcFinal.Api.Dto;
using simpleMvcFinal.Api.Models;

namespace simpleMvcFinal.Api.Service.Impl
{
    public class UserServiceImpl : IUserService
    {
        private readonly StoreProcedureServiceImpl _procedureService = new StoreProcedureServiceImpl();
        private readonly WebSocketServiceImpl _websocketService = new WebSocketServiceImpl();
        private readonly TokenServiceImpl _tokenService = new TokenServiceImpl();

        public async Task Login(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, LoginRequest req)
        {
            DataTable res = _procedureService.ProcessProcedureWithDataTable(new ProcedureModel
            {
                Action = "SELECT-USER-WITH-USERNAME-AND-PASSWORD",
                Username = req.Username,
                Password = req.Password,
            }, out _);

            if (res != null)
            {
                string refreshToken = _tokenService.GenerateRefreshToken();
                _procedureService.ProcessProcedureWithMessage(new ProcedureModel
                {
                    Action = "ADD-REFRESH-TOKEN-WITH-USERNAME",
                    Username = req.Username,
                    RefreshToken = refreshToken,
                }, out _);

                var message = JsonConvert.SerializeObject(new TokenResponse
                {
                    RefreshToken = refreshToken,
                    AccessToken = _tokenService.GenerateAccessToken(req.Username, _procedureService.GetRoleFromDataTable(res))
                });
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, message);
                return;
            }
            await _websocketService.SendMessageToClient(connectedSockets, connectionId, null);
        }

        public async Task GetAllUser(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, string token)
        {
            if (!_tokenService.IsAdmin(token))
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, "Unauthorized!");
                return;
            }
            DataTable res = _procedureService.ProcessProcedureWithDataTable(new ProcedureModel
            {
                Action = "SELECT-ALL-USER"
            }, out _);

            if (res == null)
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, null);
                return;
            }

            var message = JsonConvert.SerializeObject(_procedureService.GetUserListFromDataTable(res));
            await _websocketService.SendMessageToClient(connectedSockets, connectionId, message);
        }

        public async Task Register(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, RegisterRequest userReq)
        {
            string refreshToken = _tokenService.GenerateRefreshToken();
            string insertUser = _procedureService.ProcessProcedureWithMessage(new ProcedureModel
            {
                Action = "INSERT-USER",
                Username = userReq.Username,
                Email = userReq.Email,
                Password = userReq.Passcode,
                RefreshToken = refreshToken
            }, out _);
            if (insertUser != "SUCCESS")
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, null);
                return;
            }

            foreach (RoleResponse role in userReq.RoleResponse)
            {
                _procedureService.ProcessProcedureWithMessage(new ProcedureModel
                {
                    Action = "INSERT-ROLE-WITH-REFRESH-TOKEN",
                    RefreshToken = refreshToken,
                    RoleName = role.RoleName
                }, out _);
            }

            await Login(connectedSockets, connectionId, new LoginRequest
            {
                Password = userReq.Passcode,
                Username = userReq.Username
            });
        }

        public async Task GetInformationUser(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, string token)
        {
            string username = _tokenService.GetUsernameWithToken(token);
            DataTable res = _procedureService.ProcessProcedureWithDataTable(new ProcedureModel
            {
                Action = "SELECT-USER-WITH-USERNAME",
                Username = username
            }, out _);

            if (res != null)
            {
                var message = JsonConvert.SerializeObject(_procedureService.GetUserFromDataTable(res));
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, message);
                return;
            }
            await _websocketService.SendMessageToClient(connectedSockets, connectionId, null);
        }

        public async Task GetToken(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, string refreshToken)
        {
            DataTable dataTable = _procedureService.ProcessProcedureWithDataTable(new ProcedureModel
            {
                Action = "SELECT-USERNAME-WITH-REFRESH-TOKEN",
                RefreshToken = refreshToken,
            }, out _);
            if (dataTable == null)
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, null);
                return;
            }
                

            string username = string.Empty;
            foreach (DataRow row in dataTable.Rows)
            {
                username = row["username"].ToString();
            }

            DataTable res = _procedureService.ProcessProcedureWithDataTable(new ProcedureModel
            {
                Action = "SELECT-USER-WITH-USERNAME",
                Username = username,
            }, out _);
            if (res == null)
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, null);
                return;
            }

            var message = JsonConvert.SerializeObject(new TokenResponse
            {
                RefreshToken = refreshToken,
                AccessToken = _tokenService.GenerateAccessToken(username, _procedureService.GetRoleFromDataTable(res))
            });
            await _websocketService.SendMessageToClient(connectedSockets, connectionId, message);
        }
    }
}