using System.Reflection;
using System.Text.Json;
using AttributeApiRepoFactory.Attributes;
using AttributeApiRepoFactory.Enums;
using Castle.DynamicProxy;

namespace AttributeApiRepoFactory;

internal class ApiInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        var methodInfo = invocation.Method;
        var endpointAttribute = methodInfo.GetCustomAttribute<ApiEndpointAttribute>();
        var repositoryAttribute = methodInfo.DeclaringType!.GetCustomAttribute<ApiRepositoryAttribute>();

        if (endpointAttribute != null && repositoryAttribute != null)
        {
            var apiUrl = repositoryAttribute.BaseUrl;
            var endpointUrl = endpointAttribute.Url;
            var callType = endpointAttribute.CallType;

            for (var i = 0; i < invocation.Arguments.Length; i++)
            {
                var paramName = methodInfo.GetParameters()[i].Name;
                var paramValue = invocation.Arguments[i];
                endpointUrl = endpointUrl.Replace($"<{paramName}>", paramValue.ToString());
            }

            var result = callType switch
            {
                ApiCallType.Get => CallApiGetAsync(apiUrl, endpointUrl, invocation.Method.ReturnType),
                ApiCallType.Post => CallApiPostAsync(apiUrl, endpointUrl, invocation.Arguments.FirstOrDefault(a => a is HttpContent) as HttpContent, invocation.Method.ReturnType),
                _ => null
            };

            while (!result!.IsCompleted)
            {
            }

            invocation.ReturnValue = result.Result;
        }
        else
        {
            // Proceed as normal if no attributes found
            invocation.Proceed();
        }
    }

    private async Task<object?> CallApiGetAsync(string baseUrl, string endpoint, Type returnType)
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUrl);
            var response = await client.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            if (returnType == typeof(Stream))
            {
                return await response.Content.ReadAsStreamAsync();
            }

            var stringResult = await response.Content.ReadAsStringAsync();
            return returnType == typeof(string) ? stringResult : JsonSerializer.Deserialize(stringResult, returnType);
        }
    }

    private async Task<object?> CallApiPostAsync(string baseUrl, string endpoint, HttpContent? content, Type returnType)
    {
        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(baseUrl);
            var response = await client.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            if (returnType == typeof(Stream))
            {
                return await response.Content.ReadAsStreamAsync();
            }

            var stringResult = await response.Content.ReadAsStringAsync();
            return returnType == typeof(string) ? stringResult : JsonSerializer.Deserialize(stringResult, returnType);
        }
    }
}