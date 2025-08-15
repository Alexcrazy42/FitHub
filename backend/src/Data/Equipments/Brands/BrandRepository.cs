using FitHub.Application.Equipments.Brands;
using FitHub.Common.EntityFramework;
using FitHub.Domain.Equipments;

namespace FitHub.Data.Equipments.Brands;

public class BrandRepository : DefaultPendingRepository<Brand, BrandId, DataContext>, IBrandRepository
{
    public BrandRepository(DataContext context) : base(context)
    {
    }
}
