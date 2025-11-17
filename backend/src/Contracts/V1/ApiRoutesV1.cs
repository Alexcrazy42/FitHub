namespace FitHub.Contracts.V1;

public class ApiRoutesV1
{
    public const string Root = "api/v1";


    #region Gym

    public const string Gyms = Root + "/gyms";

    public const string GymById = Gyms + "/{id}";

    public const string GymPhoto = Gyms + "/photos";

    public const string GymPhotoById = Gyms + "/{id}/photos";

    #endregion

    #region GymZones

    public const string GymZones = Root + "/gym-zones";

    public const string GymZoneById = GymZones + "/{id}";

    #endregion

    #region MuscleGroups

    public const string MuscleGroups = Root + "/muscle-groups";

    public const string MuscleGroupById = MuscleGroups + "/{id}";

    #endregion

    #region Equipment

    public const string Equipments = Root + "/equipments";

    public const string EquipmentById = Equipments + "/{id}";

    #endregion

    #region EquipmentInstructions

    public const string EquipmentsInstructions = Root + "/equipments-instructions";

    public const string EquipmentInstructionById = EquipmentsInstructions + "/{id}";

    #endregion

    #region VideoTrainings

    public const string VideoTrainings = Root + "/video-trainings";

    public const string VideoTrainingsById = VideoTrainings + "/{id}";

    #endregion

    #region BaseGroupTrainings

    public const string BaseGroupTrainings = Root + "/base-group-trainings";

    public const string BaseGroupTrainingsById = BaseGroupTrainings + "/{id}";

    #endregion

    #region BaseGroupTrainingPhotos

    public const string BaseGroupTrainingPhotos = BaseGroupTrainings + "/photos";

    public const string BaseGroupTrainingPhotoById = BaseGroupTrainingPhotos + "/{id}";

    #endregion

    #region TrainingTypes

    public const string TrainingTypes = Root + "/training-types";

    public const string TrainingTypeById = TrainingTypes + "/{id}";

    #endregion

    #region Files

    public const string Files = Root + "/files";

    public const string FileById = Files + "/{id}";

    public const string FileGetPresignedUrl = Files + "/get-presigned-url";

    public const string FileConfirmUpload = FileById + "/confirm-upload";

    public const string FileMultipleConfirmUpload = Files + "/multiply-confirm-upload";

    public const string FileMakeFilesActive = Files + "/make-files-active";

    #endregion

    #region Entities

    public const string Entities = Root + "/entities";

    #endregion

    #region Brands

    public const string Brands = Root + "/brands";

    public const string BrandsById = Brands + "/{id}";

    #endregion


    #region Auth

    public const string Auth = Root + "/auth";

    public const string Login = Auth + "/login";

    public const string Logout = Auth + "/logout";

    public const string CheckConfirmEmail = Auth + "/check-confirm-email";

    public const string ConfirmEmail = Auth + "/confirm-email";

    public const string SetPassword = Auth + "/set-password";

    public const string InitResetPassword = Auth + "/init-reset-password";

    public const string CheckResetPassword = Auth + "/check-reset-password";

    public const string ResetPassword = Auth + "/reset-password";

    public const string StartRegister = Auth + "/start-register";

    #endregion

    #region Email

    public const string Email = Root + "/emails";

    public const string EmailAvailable = Email + "/available";

    #endregion

    #region Users

    public const string Users = Root + "/users";

    public const string Me = Users + "/me";

    #endregion

    #region CmsAdmins

    public const string CmsAdmins = Root + "/cms-admins";

    public const string CmsAdminById = CmsAdmins + "/{id}";

    public const string CmsAdminSetStatus = CmsAdminById + "/set-status";

    #endregion

    #region GymAdmins

    public const string GymAdmins = Root + "/gym-admins";

    public const string GymAdminById = GymAdmins + "/{id}";

    public const string GymAdminSetStatus = GymAdminById + "/set-status";

    #endregion

    #region Trainers

    public const string Trainers = Root + "/trainers";

    public const string TrainerById = Trainers + "/{id}";

    public const string TrainerSetStatus = TrainerById + "/set-status";

    #endregion

    #region Visitors

    public const string Visitors = Root + "/visitors";

    public const string VisitorById = Visitors + "/{id}";

    public const string VisitorSetStatus = VisitorById + "/set-status";

    #endregion



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
