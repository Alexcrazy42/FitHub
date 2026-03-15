using FitHub.Common.Entities;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Entities;
using FitHub.Domain.Files;
using FitHub.Shared.Common;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Files;

public class EntityController : ControllerBase
{
    [HttpGet(ApiRoutesV1.Entities)]
    public EntityResponse Get([FromQuery] EntityType? entityType)
    {
        ValidationException.ThrowIfNull(entityType, "entityType is null");
        var entity = EntityExtension.GetAll().First(x => x.EntityType == entityType);

        return entity.ToEntityResponse();
    }
}
