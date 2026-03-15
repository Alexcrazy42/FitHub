using System.Reflection;
using AutoFixture.Kernel;

namespace FitHub.TestsCommon;

public sealed class NonPublicConstructorBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not Type type)
        {
            return new NoSpecimen();
        }

        if (type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Any())
        {
            return new NoSpecimen();
        }

        var nonPublicCtors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
        if (nonPublicCtors.Length == 0)
        {
            return new NoSpecimen();
        }

        var ctor = nonPublicCtors
            .OrderByDescending(c => c.GetParameters().Length)
            .First();

        var parameters = ctor.GetParameters()
            .Select(p => context.Resolve(p.ParameterType))
            .ToArray();

        return ctor.Invoke(parameters);
    }
}
