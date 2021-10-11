using System;

namespace Abstractions
{
    public interface IWriteOperation<T>
    {
        void PerformOperation(T operation);
    }
}
