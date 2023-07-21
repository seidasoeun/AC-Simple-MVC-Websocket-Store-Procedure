using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using simpleMvc.Api5.StoreProcedure.Dto;
using simpleMvc.Api5.StoreProcedure.Service.Impl;
using StoreProcedure.Api1.Models;

namespace simpleMvc.Api5.StoreProcedure.Controllers
{
    public class BookController : ApiController
    {
        private readonly ProcedureServiceImpl _procedureService = new ProcedureServiceImpl();
        private readonly TokenServiceImpl _tokenService = new TokenServiceImpl();

        [HttpGet]
        [Route("api/book/get-all")]
        public DataTable GetAllBook()
        {
            return _procedureService.ProcessProcedureWithDataTable(new ProcedureModel
            {
                Action = "SELECT-ALL-BOOK"
            }, out _);
        }

        [HttpGet]
        [Route("api/book/get")]
        public DataTable GetBook(int id)
        {
            return _procedureService.ProcessProcedureWithDataTable(new ProcedureModel
            {
                Action = "SELECT-BOOK",
                BookId = id
            }, out _);
        }

        [HttpPost]
        [Route("api/book/add")]
        public string AddBook(BookRequest req)
        {
            if (req == null) return new ArgumentNullException(nameof(req)).ToString();
            string username = _tokenService.GetUsernameWithToken(req.Token);
            return _procedureService.ProcessProcedureWithMessage(new ProcedureModel
            {
                Username = username,
                Author = req.Author,
                BookTitle = req.BookTitle,
                Action = "INSERT-BOOK"
            }, out _);
        }

        [HttpPatch]
        [Route("api/book/update")]
        public string UpdateBook(int id, [FromBody]BookRequest req)
        {
            if (req == null) return new ArgumentNullException(nameof(req)).ToString();
            string username = _tokenService.GetUsernameWithToken(req.Token);
            return _procedureService.ProcessProcedureWithMessage(new ProcedureModel
            {
                BookId = id,
                Username = username,
                Author = req.Author,
                BookTitle = req.BookTitle,
                Action = "UPDATE-BOOK"
            }, out _);
        }

        [HttpDelete]
        [Route("api/book/update")]
        public string DeleteBook(int id, [FromBody] UserRequest req)
        {
            string username = _tokenService.GetUsernameWithToken(req.Token);
            return _procedureService.ProcessProcedureWithMessage(new ProcedureModel
            {
                BookId = id,
                Username = username,
                Action = "DELETE-BOOK"
            }, out _);
        }

        

    }
}
