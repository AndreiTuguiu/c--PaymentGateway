using PaymentGateway.Data;
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
        private readonly PaymentDbContext _dbContext;
        private readonly IMediator _mediator;
        public CreateProduct(IMediator mediator,PaymentDbContext dbContext)
        {
            _mediator = mediator;
            _dbContext = dbContext;
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

            _dbContext.Products.Add(product);

            _dbContext.SaveChanges();

            ProductCreated productCreated = new(product.Name, product.Value, product.Currency);
            await _mediator.Publish(productCreated,cancellationToken);

            return Unit.Value;
        }

        
    }
}
