namespace AttributeApiRepoFactory.Attributes;

[AttributeUsage(AttributeTargets.Interface)]
public class ApiRepositoryAttribute : Attribute
{
    public string BaseUrl { get; }
    public ApiRepositoryAttribute(string baseUrl)
    {
        BaseUrl = baseUrl;
    }
}