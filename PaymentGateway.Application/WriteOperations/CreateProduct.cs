using Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Event;
using PaymentGateway.PublishedLanguage.Commands;
using MediatR;
using System.Threading.Tasks;
using System.Threading;

namespace PaymentGateway.Application.WriteOperations
{
    public class CreateProduct : IRequestHandler<CreateProductCommand>
    {
        private readonly Database _database;
        private readonly IEventSender _eventSender;
        public CreateProduct(IEventSender eventSender,Database database)
        {
            _eventSender = eventSender;
            _database = database;
        }

        public Task<Unit> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            Product product = new Product();
            product.ProductId = request.ProductId;
            product.Name = request.Name;
            product.Value = request.Value;
            product.Currency = request.Currency;
            product.Limit = request.Limit;

            _database.Products.Add(product);

            _database.SaveChanges();

            ProductCreated productCreated = new(product.Name, product.Value, product.Currency);
            _eventSender.SendEvent(productCreated);

            return Unit.Task;
        }

        
    }
}
