using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using simpleMvc.Api5.Websocket.Dto;

namespace simpleMvc.Api5.Websocket.Service
{
    internal interface BookService
    {
        Task GetAllBook(
            ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> connectedSockets,
            string connectionId);
        Task GetMyBook(
            ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> connectedSockets,
            string connectionId , string token);

        Task GetBook(
            ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> connectedSockets,
            string connectionId, int id);

        Task AddBook(
            ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> connectedSockets,
            string connectionId, BookRequest bookRequest, string token);

        Task UpdateBook(
            ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> connectedSockets,
            string connectionId, BookRequest bookRequest, int id, string token);

        Task DeleteBook(
            ConcurrentDictionary<string, System.Net.WebSockets.WebSocket> connectedSockets,
            string connectionId, int id, string token);
    }
}
