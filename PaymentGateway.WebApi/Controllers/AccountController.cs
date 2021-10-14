using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.WriteOperations;
using PaymentGateway.PublishedLanguage.Commands;
using PaymentGateway.Application.ReadOperations;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

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
        public async Task<string> CreateAccount(CreateAccountCommand command, CancellationToken cancellationToken)
        {
            await _createAccountCommandHandler.Handle(command, cancellationToken);
            return "ok";
        }

        [HttpGet]
        [Route("ListOfAccounts")]
        public async Task<List<ListOfAccounts.Model>> GetListOfAccounts([FromQuery] ListOfAccounts.Query query, CancellationToken cancellationToken)
        {
            var result = await _queryHandler.Handle(query,cancellationToken);
            return result;
        }
    }
}
