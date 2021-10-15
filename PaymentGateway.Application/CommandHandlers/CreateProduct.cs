﻿using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Event;
using PaymentGateway.PublishedLanguage.Commands;
using MediatR;
using System.Threading.Tasks;
using System.Threading;

namespace PaymentGateway.Application.CommandHandlers
{
    public class CreateProduct : IRequestHandler<CreateProductCommand>
    {
        private readonly Database _database;
        private readonly IMediator _mediator;
        public CreateProduct(IMediator mediator,Database database)
        {
            _mediator = mediator;
            _database = database;
        }

        public async Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            Product product = new Product
            {
                ProductId = request.ProductId,
                Name = request.Name,
                Value = request.Value,
                Currency = request.Currency,
                Limit = request.Limit
            };

            _database.Products.Add(product);

            _database.SaveChanges();

            ProductCreated productCreated = new(product.Name, product.Value, product.Currency);
            await _mediator.Publish(productCreated,cancellationToken);

            return Unit.Value;
        }

        
    }
}