﻿using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.WriteOperations;
using PaymentGateway.PublishedLanguage.WriteSide;
using PaymentGateway.Application.ReadOperations;
using System.Collections.Generic;

namespace PaymentGateway.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly CreateAccount _createAccountCommandHandler;
        private readonly ListOfAccounts.QueryHandler _queryHandler;
        public AccountController(CreateAccount createAccountCommandHandler,ListOfAccounts.QueryHandler queryHandler)
        {
            _createAccountCommandHandler = createAccountCommandHandler;
            _queryHandler = queryHandler;
        }


        [HttpPost]
        [Route("Create")]
        public string CreateAccount(CreateAccountCommand command)
        {
            _createAccountCommandHandler.PerformOperation(command);
            return "ok";
        }

        [HttpGet]
        [Route("ListOfAccounts")]
        public List<ListOfAccounts.Model> GetListOfAccounts([FromQuery] ListOfAccounts.Query query)
        {
            var result = _queryHandler.PerformOperation(query);
            return result;
        }
    }
}
