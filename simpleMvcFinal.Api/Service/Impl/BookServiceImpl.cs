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
    public class BookServiceImpl : IBookService
    {
        private readonly StoreProcedureServiceImpl _procedureService = new StoreProcedureServiceImpl();
        private readonly WebSocketServiceImpl _websocketService = new WebSocketServiceImpl();
        private readonly TokenServiceImpl _tokenService = new TokenServiceImpl();
        public async Task GetAllBook(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId)
        {
            var res = _procedureService.ProcessProcedureWithDataTable(new ProcedureModel
            {
                Action = "SELECT-ALL-BOOK"
            }, out _);
            string message = String.Empty;
            if (res != null)
                message = JsonConvert.SerializeObject(GetBookListFromDataTable(res));
            
            await _websocketService.SendMessageToClient(connectedSockets, connectionId, message);
        }

        public async Task GetMyBook(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, string token)
        {
            string username = _tokenService.GetUsernameWithToken(token);
            if (username == null)
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, "Invalid Token!");
                return;
            }
            DataTable data = _procedureService.ProcessProcedureWithDataTable(new ProcedureModel
            {
                Action = "SELECT-MY-BOOK-WITH-USERNAME",
                Username = username
            }, out _);

            var message = JsonConvert.SerializeObject(GetBookListFromDataTable(data));
            await _websocketService.SendMessageToClient(connectedSockets, connectionId, message);
        }

        public async Task GetBook(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, int id)
        {
            var res = _procedureService.ProcessProcedureWithDataTable(new ProcedureModel
            {
                Action = "SELECT-BOOK",
                BookId = id
            }, out _);
            string message = String.Empty;
            if (res != null)
                message = JsonConvert.SerializeObject(GetBookFromDataTable(res));
            await _websocketService.SendMessageToClient(connectedSockets, connectionId, message);
        }

        public async Task AddBook(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, BookRequest bookRequest, string token)
        {
            string username = _tokenService.GetUsernameWithToken(token);
            if (username == null)
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, "Invalid Token!");
                return;
            }

            string insertBook = _procedureService.ProcessProcedureWithMessage(new ProcedureModel
            {
                Action = "INSERT-BOOK",
                BookTitle = bookRequest.BookTitle,
                Author = bookRequest.Author,
                Username = username
            }, out _);

            if (insertBook != "SUCCESS")
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, "Failed to Add!");
                return;
            }

            await GetMyBook(connectedSockets, connectionId, token);
        }

        public async Task UpdateBook(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, BookRequest bookRequest, int id,
            string token)
        {
            string username = _tokenService.GetUsernameWithToken(token);
            if (username == null)
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, "Invalid Token!");
                return;
            }

            string updateBook = _procedureService.ProcessProcedureWithMessage(new ProcedureModel
            {
                Action = "UPDATE-BOOK",
                BookTitle = bookRequest.BookTitle,
                Author = bookRequest.Author,
                BookId = id,
                Username = username
            }, out _);

            if (updateBook != "SUCCESS")
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, "Failed to Update!");
                return;
            }

            await GetMyBook(connectedSockets, connectionId, token);
        }

        public async Task DeleteBook(ConcurrentDictionary<string, WebSocket> connectedSockets, string connectionId, int id, string token)
        {
            string username = _tokenService.GetUsernameWithToken(token);
            if (username == null)
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, "Invalid Token!");
                return;
            }

            string updateBook = _procedureService.ProcessProcedureWithMessage(new ProcedureModel
            {
                Action = "DELETE-BOOK",
                BookId = id,
                Username = username
            }, out _);

            if (updateBook != "SUCCESS")
            {
                await _websocketService.SendMessageToClient(connectedSockets, connectionId, "Failed to DELETE!");
                return;
            }

            await GetMyBook(connectedSockets, connectionId, token);
        }

        private List<BookResponse> GetBookListFromDataTable(DataTable data)
        {
            List<BookResponse> books = new List<BookResponse>();

            foreach (DataRow row in data.Rows)
            {
                books.Add(new BookResponse
                {
                    Author = row["author"].ToString(),
                    BookTitle = row["bookTitle"].ToString()
                });  
            }
            return books;
        }

        private BookResponse GetBookFromDataTable(DataTable data)
        {
            foreach (DataRow row in data.Rows)
            {
                if (!string.IsNullOrEmpty(row["bookId"].ToString()))
                {
                    return new BookResponse()
                    {
                        Author = row["author"].ToString(),
                        BookTitle = row["bookTitle"].ToString()
                    };
                }
            }

            return null;
        }
    }
}