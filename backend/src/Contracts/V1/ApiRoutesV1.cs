namespace FitHub.Contracts.V1;

public class ApiRoutesV1
{
    public const string Root = "api/v1";


    #region Gym

    public const string Gyms = Root + "/gyms";

    public const string GymById = Gyms + "/{id:guid}";

    #endregion

    #region GymZones

    public const string GymZones = Root + "/gym-zones";

    public const string GymZoneById = GymZones + "/{id:guid}";

    #endregion

    #region MuscleGroups

    public const string MuscleGroups = Root + "/muscle-groups";

    public const string MuscleGroupById = MuscleGroups + "/{id:guid}";

    #endregion

    #region Equipment

    public const string Equipments = Root + "/equipments";

    public const string EquipmentById = Equipments + "/{id:guid}";

    #endregion

    #region EquipmentInstructions

    public const string EquipmentsInstructions = Root + "/equipments-instructions";

    public const string EquipmentInstructionById = EquipmentsInstructions + "/{id:guid}";

    #endregion

    #region VideoTrainings

    public const string VideoTrainings = Root + "/video-trainings";

    public const string VideoTrainingsById = VideoTrainings + "/{id:guid}";

    #endregion

    #region BaseGroupTrainings

    public const string BaseGroupTrainings = Root + "/base-group-trainings";

    public const string BaseGroupTrainingsById = BaseGroupTrainings + "/{id:guid}";

    #endregion

    #region TrainingTypes

    public const string TrainingTypes = Root + "/training-types";

    public const string TrainingTypeById = TrainingTypes + "/{id:guid}";

    #endregion



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
