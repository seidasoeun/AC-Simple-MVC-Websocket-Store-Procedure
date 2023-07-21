using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.WebSockets;
using simpleMvcFinal.Api.Dto;
using simpleMvcFinal.Api.Service.Impl;

namespace simpleMvcFinal.Api.Controllers
{
    public class RoleController : ApiController
    {
        private static readonly ConcurrentDictionary<string, WebSocket> ConnectedSockets =
            new ConcurrentDictionary<string, WebSocket>();
        private readonly RoleServiceImpl _roleService = new RoleServiceImpl();

        [HttpGet]
        [Route("api/role")]
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
                        var role = JsonConvert.DeserializeObject<RoleRequest>(receivedData);

                        switch (role.Action)
                        {
                            case "GET-ALL":
                                await _roleService.GetAllRole(ConnectedSockets, connectionId, role.Token);
                                break;
                            case "GET":
                                await _roleService.GetRole(ConnectedSockets, connectionId, role.Token, Int32.Parse(context.QueryString["id"]));
                                break;
                            case "GET-ALL-USER":
                                await _roleService.GetUser(ConnectedSockets, connectionId, role);
                                break;
                            case "ADD":
                                await _roleService.AddRole(ConnectedSockets, connectionId, role);
                                break;
                            case "UPDATE":
                                await _roleService.UpdateRole(ConnectedSockets, connectionId, role, Int32.Parse(context.QueryString["id"]));
                                break;
                            case "DELETE":
                                await _roleService.DeleteRole(ConnectedSockets, connectionId, role.Token, Int32.Parse(context.QueryString["id"]));
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
