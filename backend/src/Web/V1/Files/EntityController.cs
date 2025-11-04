using FitHub.Common.Entities;
using FitHub.Contracts.Common;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Entities;
using FitHub.Domain.Files;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Files;

public class EntityController : ControllerBase
{
    [HttpGet(ApiRoutesV1.Entities)]
    public EntityResponse Get([FromQuery] EntityTypeDto? entityTypeDto)
    {
        var entityType = entityTypeDto?.FromDto() ?? throw new ValidationException("Не удается получить EntityTypeDto");
        var entity = EntityExtension.GetAll().First(x => x.EntityType == entityType);

        return entity.ToEntityResponse();
    }
}
