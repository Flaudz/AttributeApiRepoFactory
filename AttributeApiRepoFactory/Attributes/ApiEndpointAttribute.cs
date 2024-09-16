using AttributeApiRepoFactory.Enums;

namespace AttributeApiRepoFactory.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class ApiEndpointAttribute : Attribute
{
    public ApiCallType CallType { get; }
    public string Url { get; }

    public ApiEndpointAttribute(ApiCallType callType, string url)
    {
        CallType = callType;
        Url = url;
    }
}