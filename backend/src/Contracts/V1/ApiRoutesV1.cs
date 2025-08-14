namespace FitHub.Contracts.V1;

public class ApiRoutesV1
{
    public const string Root = "api/v1";

    public const string Login = Root + "/login";

    public const string Organizations = Root + "/organizations";

    public const string Organization = Organizations + "/{organizationId}";

    public const string Employees = Organization + "/employees";

    public const string Employee = Employees + "/{employeeId}";

    public static string ForPaginationOrganizations(int? limit, int? offset) => Organizations + $"?limit={limit}&offset={offset}";

    public static string ForOrganization(string organizationId) => ReplaceUrlSegment(Organization, "organizationId", organizationId);

    public static string ForEmployees(string organizationId) => ReplaceUrlSegment(Employees, "organizationId", organizationId);

    public static string ForEmployee(string organizationId, string employeeId) => ReplaceUrlSegment(ForOrganization(organizationId), "employeeId", employeeId);

    private static string ReplaceUrlSegment(string urlTemplate, string name, string value)
    {
        var escapedValue = Uri.EscapeDataString(value);
        return urlTemplate.Replace("{" + name + "}", escapedValue);
    }

    private static string ReplaceUrlSegments(string urlTemplate, params (string Name, string Value)[] urlSegments)
    {
        return urlSegments.Aggregate(urlTemplate, (current, urlSegment) => ReplaceUrlSegment(current, urlSegment.Name, urlSegment.Value));
    }
}
