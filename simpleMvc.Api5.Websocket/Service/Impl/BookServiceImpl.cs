using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using simpleMvc.Api5.Websocket.Dto;
using simpleMvc.Api5.Websocket.Repository;

namespace simpleMvc.Api5.Websocket.Service.Impl
{
    public class BookServiceImpl : BookService
    {
        private readonly BookRepository _bookRepository = new BookRepository();
        private readonly TokenServiceImpl _tokenService = new TokenServiceImpl();
        private readonly WebSocketServiceImpl _websocketService = new WebSocketServiceImpl();

        public async Task GetAllBook(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId)
        {
            var books = _bookRepository.GetAllBook();
            var message = JsonConvert.SerializeObject(books);
            await _websocketService.SendMessageToClient(connectedSockets, connectionId, message);
        }

        public async Task GetMyBook(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, string token)
        {
            string username = _tokenService.GetUsernameWithToken(token);
            var books = _bookRepository.GetBookWithUsername(username);

            var message = JsonConvert.SerializeObject(books);
            await _websocketService.SendMessageToClient(connectedSockets, connectionId, message);
        }

        public async Task GetBook(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, int id)
        {
            var book =_bookRepository.GetBook(id);
            var message = JsonConvert.SerializeObject(book);
            await _websocketService.SendMessageToClient(connectedSockets, connectionId, message);
        }

        public async Task AddBook(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, BookRequest bookRequest, string token)
        {
            string username = _tokenService.GetUsernameWithToken(token);
            var book = _bookRepository.AddBook(bookRequest, username);
            var message = JsonConvert.SerializeObject(book);
            await _websocketService.SendMessageToClient(connectedSockets, connectionId, message);
        }

        public async Task UpdateBook(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, BookRequest bookRequest, int id,
            string token)
        {
            string username = _tokenService.GetUsernameWithToken(token);
            var book = _bookRepository.UpdateBook(bookRequest, username, id);
            var message = JsonConvert.SerializeObject(book);
            await _websocketService.SendMessageToClient(connectedSockets, connectionId, message);
        }

        public async Task DeleteBook(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, int id, string token)
        {
            string username = _tokenService.GetUsernameWithToken(token);
            var book = _bookRepository.DeleteBook( username, id);
            var message = JsonConvert.SerializeObject(book);
            await _websocketService.SendMessageToClient(connectedSockets, connectionId, message);
        }
    }
}