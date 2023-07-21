using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.WebSockets;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.WebSockets;
using Newtonsoft.Json;
using System.Text;
using System.Threading;
using simpleMvc.Api5.Websocket.Dto;
using simpleMvc.Api5.Websocket.Service.Impl;

namespace simpleMvc.Api5.Websocket.Controllers
{
    public class BookController : ApiController
    {
        private static readonly ConcurrentDictionary<string, WebSocket> ConnectedSockets =
            new ConcurrentDictionary<string, WebSocket>();
        private readonly BookServiceImpl _bookService = new BookServiceImpl();

        [HttpGet]
        [Route("api/book")]
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
                        var book = JsonConvert.DeserializeObject<BookRequest>(receivedData);
                        var token = JsonConvert.DeserializeObject<UserRequest>(receivedData).Token;

                        switch (book.Action)
                        {
                            case "GET-ALL":
                                await _bookService.GetAllBook(ConnectedSockets, connectionId);
                                break;
                            case "GET-MY-BOOK":
                                
                                await _bookService.GetMyBook(ConnectedSockets, connectionId, token);
                                break;
                            case "GET-BOOK":
                                int idGet = Int32.Parse(context.QueryString["id"]);
                                await _bookService.GetBook(ConnectedSockets, connectionId, idGet);
                                break;
                            case "ADD":
                                await _bookService.AddBook(ConnectedSockets, connectionId, book, token);
                                break;
                            case "UPDATE":
                                int idUpdate = Int32.Parse(context.QueryString["id"]);
                                await _bookService.UpdateBook(ConnectedSockets, connectionId, book, idUpdate, token);
                                break;
                            case "DELETE":
                                int idDelete = Int32.Parse(context.QueryString["id"]);
                                await _bookService.DeleteBook(ConnectedSockets, connectionId, idDelete, token);
                                break;
                            default:
                                await _bookService.GetAllBook(ConnectedSockets, connectionId);
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