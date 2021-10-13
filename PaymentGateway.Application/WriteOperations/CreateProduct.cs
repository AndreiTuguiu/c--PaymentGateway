using Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using PaymentGateway.PublishedLanguage.Event;
using PaymentGateway.PublishedLanguage.WriteSide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway.Application.WriteOperations
{
    public class CreateProduct : IWriteOperation<CreateProductCommand>
    {
        private readonly Database _database;
        private readonly IEventSender _eventSender;
        public CreateProduct(IEventSender eventSender,Database database)
        {
            _eventSender = eventSender;
            _database = database;
        }
        public void PerformOperation(CreateProductCommand operation)
        {
            
            Product product = new Product();
            product.ProductId = operation.ProductId;
            product.Name = operation.Name;
            product.Value = operation.Value;
            product.Currency = operation.Currency;
            product.Limit = operation.Limit;

            _database.Products.Add(product);

            _database.SaveChanges();

            ProductCreated productCreated = new(product.Name, product.Value, product.Currency);
            eventSender.SendEvent(productCreated);
        }
    }
}
