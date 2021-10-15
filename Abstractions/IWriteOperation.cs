//using System;

//namespace Abstractions
//{
//    public interface IWriteOperation<T>
//    {
//        void PerformOperation(T operation);
//    }
//}
namespace Abstractions
{
    public interface ITest<TCommand>
    {
       void PerformOperation(TCommand operation);
    }
}
