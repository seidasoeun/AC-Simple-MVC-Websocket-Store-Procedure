using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.WebSockets;
using System.Net.WebSockets;
using System.Threading;
using Newtonsoft.Json;
using simpleMvc.Api5.Websocket.Dto;
using System.Text;
using simpleMvc.Api5.Websocket.Service.Impl;

namespace simpleMvc.Api5.Websocket.Controllers
{
    public class UserController : ApiController
    {
        private static readonly ConcurrentDictionary<string, WebSocket> ConnectedSockets =
            new ConcurrentDictionary<string, WebSocket>();

        private readonly UserServiceImpl _userService = new UserServiceImpl();

        [HttpGet]
        [Route("api/user")]
        public HttpResponseMessage Get()
        {
            if (HttpContext.Current.IsWebSocketRequest)
            {
                HttpContext.Current.AcceptWebSocketRequest(ProcessWebSocket);
            }
            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
        }

        private async Task ProcessWebSocket(AspNetWebSocketContext context)
        {
            var webSocket = context.WebSocket;

            var connectionId = Guid.NewGuid().ToString();
            ConnectedSockets.TryAdd(connectionId, webSocket);

            while (webSocket.State == WebSocketState.Open)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);

                WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(
                    buffer, CancellationToken.None);

                if (receiveResult.MessageType == WebSocketMessageType.Text)
                {
                    if (buffer.Array != null)
                    {
                        var receivedData = Encoding.UTF8.GetString(buffer.Array, 0, receiveResult.Count);
                        var user = JsonConvert.DeserializeObject<UserRequest>(receivedData);

                        switch (user.Action)
                        {
                            case "LOGIN":
                                var loginInfo = JsonConvert.DeserializeObject<LoginRequest>(receivedData);
                                await _userService.Login(ConnectedSockets, connectionId, 
                                    loginInfo.Username, loginInfo.Passcode);
                                break;
                            case "REGISTER":
                                var registerInfo = JsonConvert.DeserializeObject<RegisterRequest>(receivedData);
                                await _userService.Register(ConnectedSockets, connectionId, registerInfo);
                                break;
                            case "GET-INFORMATION":
                                await _userService.GetInformationUser(ConnectedSockets, connectionId, user.Token);
                                break;
                            case "GET-TOKEN":
                                var tokenReq = JsonConvert.DeserializeObject<TokenRequest>(receivedData);
                                await _userService.GetToken(ConnectedSockets, connectionId, tokenReq.RefreshToken);
                                break;

                            default:
                                break;
                        }
                    }
                }
                else if (receiveResult.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

                    ConnectedSockets.TryRemove(connectionId, out _);
                }
            }
        }
    }
}