using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.WebSockets;
using System.Net;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Web.WebSockets;
using System;
using simpleMvcFinal.Api.Dto;
using simpleMvcFinal.Api.Service.Impl;

namespace simpleMvcFinal.Api.Controllers
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
                        switch (book.Action){
                            case "GET-ALL":
                                await _bookService.GetAllBook(ConnectedSockets, connectionId);
                                break;
                            case "GET-MY-BOOK":

                                await _bookService.GetMyBook(ConnectedSockets, connectionId, book.Token);
                                break;
                            case "GET-BOOK":
                                await _bookService.GetBook(ConnectedSockets, connectionId, int.Parse(context.QueryString["id"]));
                                break;
                            case "ADD":
                                await _bookService.AddBook(ConnectedSockets, connectionId, book, book.Token);
                                break;
                            case "UPDATE":
                            {
                                await _bookService.UpdateBook(ConnectedSockets, connectionId, book, int.Parse(context.QueryString["id"]),
                                    book.Token);
                                break;
                            }
                            case "DELETE":
                                await _bookService.DeleteBook(ConnectedSockets, connectionId, int.Parse(context.QueryString["id"]), book.Token);
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

