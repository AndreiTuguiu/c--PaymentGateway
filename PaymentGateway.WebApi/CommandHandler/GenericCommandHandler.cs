﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.WebApi.CommandHandler
{
    public class GenericCommandHandler : MediatR.IRequestHandler<MediatR.IRequest>
    {
        public Task<Unit> Handle(IRequest request, CancellationToken cancellationToken)
        {
            // queue send
            Console.WriteLine($"sending to queue {request}");
            return Unit.Task;
        }
    }
}