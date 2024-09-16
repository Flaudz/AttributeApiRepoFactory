using Castle.DynamicProxy;

namespace AttributeApiRepoFactory;

internal class ApiInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        throw new NotImplementedException();
    }
}