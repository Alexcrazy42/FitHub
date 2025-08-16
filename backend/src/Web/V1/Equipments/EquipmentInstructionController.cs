using FitHub.Application.Equipments;
using FitHub.Application.Equipments.Instructions;
using FitHub.Contracts;
using FitHub.Contracts.V1;
using FitHub.Contracts.V1.Equipments.Instructions;
using Microsoft.AspNetCore.Mvc;

namespace FitHub.Web.V1.Equipments;

[ApiController]
public class EquipmentInstructionController : ControllerBase
{
    private readonly IEquipmentInstructionRepository repository;
    private readonly IEquipmentInstructionService service;

    public EquipmentInstructionController(IEquipmentInstructionRepository repository,
        IEquipmentInstructionService service)
    {
        this.repository = repository;
        this.service = service;
    }

    [HttpGet(ApiRoutesV1.EquipmentsInstructions)]
    public async Task<ListResponse<EquipmentInstructionResponse>> GetAllAsync(CancellationToken ct)
    {
        var all = await repository.GetAllAsync(x => true, ct);

        var responses = all.ToResponses();

        return ListResponse<EquipmentInstructionResponse>.Create(responses);
    }
}
