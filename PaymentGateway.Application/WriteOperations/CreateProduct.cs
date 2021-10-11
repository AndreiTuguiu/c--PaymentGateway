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
        public IEventSender eventSender;
        public CreateProduct(IEventSender eventSender)
        {
            this.eventSender = eventSender;
        }
        public void PerformOperation(CreateProductCommand operation)
        {
            Database database = Database.GetInstance();
            Product product = new Product();
            product.ProductId = operation.ProductId;
            product.Name = operation.Name;
            product.Value = operation.Value;
            product.Currency = operation.Currency;
            product.Limit = operation.Limit;

            database.Products.Add(product);

            database.SaveChanges();

            ProductCreated productCreated = new(product.Name, product.Value, product.Currency);
            eventSender.SendEvent(productCreated);
        }
    }
}
