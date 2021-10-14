using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ExternalService
{
    public class AllEventHandler : INotificationHandler<INotification>
    {
        public Task Handle(INotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine(notification);
            return Task.CompletedTask;
        }

        //public class AccountMadeEventHandler : INotificationHandler<AccountMade>
        //{
        //    public Task Handle(AccountMade notification, CancellationToken cancellationToken)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public class EnrollCustomerCommandHandler : IRequestHandler<EnrollCustomer>
        //{
        //    public Task<Unit> Handle(EnrollCustomer request, CancellationToken cancellationToken)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

    }
}
