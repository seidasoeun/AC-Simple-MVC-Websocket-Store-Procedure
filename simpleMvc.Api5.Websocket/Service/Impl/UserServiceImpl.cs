using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using simpleMvc.Api5.Websocket.Dto;
using simpleMvc.Api5.Websocket.Repository;

namespace simpleMvc.Api5.Websocket.Service.Impl
{
    public class UserServiceImpl : UserService
    {
        private readonly UserRepository _userRepository = new UserRepository();
        private readonly TokenServiceImpl _tokenService = new TokenServiceImpl();
        private readonly WebSocketServiceImpl _websocketService = new WebSocketServiceImpl();

        public async Task Login(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, string username, string password)
        {
            UserResponse userResponse = _userRepository.FindUserByUsernameAndPassword(username, password);
            string refreshToken = _tokenService.GenerateRefreshToken();
            TokenResponse res = new TokenResponse();
            if (userResponse != null && _userRepository.StoreRefreshToken(refreshToken, userResponse.Username))
            {
                res.AccessToken = _tokenService.GenerateAccessToken(userResponse.Username, userResponse.RoleResponse);
                res.RefreshToken = refreshToken;
            }

            var message = JsonConvert.SerializeObject(res);
            await _websocketService.SendMessageToClient(connectedSockets, connectionId, message);
        }

        public async Task Register(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, RegisterRequest userReq)
        {
            UserResponse res = _userRepository.RegisterNewUser(userReq, _tokenService.GenerateRefreshToken());
            var message = JsonConvert.SerializeObject(res);
            await _websocketService.SendMessageToClient(connectedSockets, connectionId, message);
        }

        public async Task GetInformationUser(
            ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, string token)
        {
            string username = _tokenService.GetUsernameWithToken(token);
            UserResponse userRes = new UserResponse();
            if (username != null)
            {
                userRes = _userRepository.GetUser(username);

            }
            var message = JsonConvert.SerializeObject(userRes);
            await _websocketService.SendMessageToClient(connectedSockets, connectionId, message);
        }

        public async Task GetToken(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, string refreshToken)
        {
            TokenResponse tokenRes = new TokenResponse();
            UserResponse user = _userRepository.GetUserWithRefreshToken(refreshToken);
            if (user != null )
            {
                tokenRes.RefreshToken = user.RefreshToken;
                tokenRes.AccessToken = _tokenService.GenerateAccessToken(user.Username, user.RoleResponse);
            }

            var message = JsonConvert.SerializeObject(tokenRes);
            await _websocketService.SendMessageToClient(connectedSockets, connectionId, message);
        }


    }
}