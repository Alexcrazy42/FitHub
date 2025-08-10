using System.Reflection;
using FitHub.Common.Entities;
using FitHub.Common.Entities.Identity;
using FitHub.Common.EntityFramework.Conversions.DateTimeOffsetToUtc;
using FitHub.Common.EntityFramework.Conversions.Identifiers;
using FitHub.Common.Utilities.Collections;
using FitHub.Common.Utilities.System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NullabilityInfoContext = System.Reflection.NullabilityInfoContext;

namespace FitHub.Common.EntityFramework;

public static class ModelBuilderExtensions
{
    private const int ShortStringLength = 255;
    private const string EntityIdDefaultPropertyName = nameof(IEntity<IIdentifier>.Id);

    private static readonly Dictionary<Type, Func<ValueConverter>> FactoryNotNullCache = new();
    private static readonly Dictionary<Type, Func<ValueConverter>> FactoryNullableCache = new();

    /// <summary>
    /// Настраивает стандартные соглашения для моделей сущностей в базе данных:
    /// 1) Поведение при удалении - Strict Delete
    /// 2) PK у сущностей IEntity - IEntity.Id
    /// 4) Конвертация Identifier`ов в строку и обратно при работе с БД
    /// 5) Приведение DateTimeOffset к UTC при работе с БД
    /// </summary>
    public static void UseCommonConventions(this ModelBuilder modelBuilder)
    {
        // Прежде чем добавлять новый конвертер, проверьте его существование в базовой библиотеке
        // https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=fluent-api#built-in-converters
        modelBuilder.UseStrictDeleteBehaviorConvention();
        modelBuilder.UsePrimaryKeyConvention();
        modelBuilder.UseEnumConvention();
        modelBuilder.UseDateTimeOffsetToUtcConvention();
    }

    public static void UseStrictDeleteBehaviorConvention(this ModelBuilder modelBuilder)
    {
        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }

    /// <summary>
    /// Использовать в качестве главного ключа для сущностей <see cref="IEntity{TEntityId}"/> их идентификатор
    /// </summary>
    private static void UsePrimaryKeyConvention(this ModelBuilder modelBuilder)
    {
        var entities = modelBuilder.Model
            .GetEntityTypes()
            .Where(entityType =>
                // Проверка на отсутствие базового типа необходима для того, чтобы не конфигурировать сущности,
                // настроенные по типу table-per-hierarchy
                entityType.BaseType is null &&
                entityType.ClrType
                    .GetInterfaces()
                    .Any(interfaceType => interfaceType.IsGenericType &&
                                          interfaceType.GetGenericTypeDefinition() == typeof(IEntity<>)))
            .ToList();

        foreach (var entity in entities)
        {
            var entityBuilder = modelBuilder.Entity(entity.Name);

            entityBuilder.HasKey(EntityIdDefaultPropertyName);

            entityBuilder
                .Property(EntityIdDefaultPropertyName)
                .ValueGeneratedNever();
        }
    }

    /// <summary>
    /// Использовать для работы с БД конвертацию всех <see cref="Enum"/> в строку и обратно
    /// </summary>
    public static void UseEnumConvention(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.ClrType.GetProperties())
            {
                if (!property.PropertyType.IsEnum)
                {
                    continue;
                }

                var propertyBuilder = modelBuilder
                    .Entity(entityType.Name)
                    .Property(property.Name)
                    .HasMaxLength(ShortStringLength);

                var nullability = new NullabilityInfoContext().Create(property);
                switch (nullability.ReadState)
                {
                    case NullabilityState.NotNull:
                        propertyBuilder
                            .HasConversion<string>()
                            .IsRequired();
                        break;

                    case NullabilityState.Nullable:
                        propertyBuilder.HasConversion<string?>();
                        break;

                    case NullabilityState.Unknown:
                        throw new NotSupportedException("Используйте NRT");

                    default:
                        throw new ArgumentOutOfRangeException(nameof(property), nullability.ReadState, $"{nameof(NullabilityInfo.ReadState)}");
                }
            }
        }
    }

    public static void UseDateTimeOffsetToUtcConvention(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.ClrType
                .GetProperties()
                .Where(p =>
                    p.PropertyType == typeof(DateTimeOffset) ||
                    p.PropertyType == typeof(DateTimeOffset?));

            foreach (var property in properties)
            {
                ValueConverter converter = property.PropertyType == typeof(DateTimeOffset)
                    ? new DateTimeOffsetToUtcConverter()
                    : new DateTimeOffsetNullableToUtcConverter();

                modelBuilder
                    .Entity(entityType.Name)
                    .Property(property.Name)
                    .HasConversion(converter);
            }
        }
    }

    /// <summary>
    /// Использовать для работы с БД конвертацию всех идентификаторов сущностей в строку и обратно
    /// </summary>
    public static void UseIdentifierConvention(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.ClrType.GetProperties())
            {
                var converter = ResolveCustomConverter(property);
                if (converter == null)
                {
                    continue;
                }

                modelBuilder
                    .Entity(entityType.Name)
                    .Property(property.Name)
                    .HasConversion(converter)
                    .HasMaxLength(ShortStringLength);
            }
        }

        FactoryNotNullCache.Clear();
        FactoryNullableCache.Clear();
    }

    private static ValueConverter? ResolveCustomConverter(PropertyInfo property)
    {
        if (typeof(IGuidIdentifier).IsAssignableFrom(property.PropertyType))
        {
            return ResolveGuidIdentifierConverter(property);
        }

        return null;
    }

    #region GuidIdentifier

    private static ValueConverter? ResolveGuidIdentifierConverter(PropertyInfo property)
    {
        var nullability = new NullabilityInfoContext().Create(property);

        var factory = nullability.ReadState switch
        {
            NullabilityState.NotNull => FactoryNotNullCache.GetOrAdd(property.PropertyType, BuildGuidIdentifierNotNullFactory),
            NullabilityState.Nullable => FactoryNullableCache.GetOrAdd(property.PropertyType, BuildGuidIdentifierNullableFactory),
            NullabilityState.Unknown => throw new NotSupportedException("Используйте NRT"),
            _ => throw new ArgumentOutOfRangeException(nameof(property), nullability.ReadState, $"{nameof(NullabilityInfo.ReadState)}")
        };

        var converter = factory();
        return converter;
    }

    private static Func<ValueConverter> BuildGuidIdentifierNotNullFactory(Type propertyType)
    {
        var converterType = typeof(GuidIdentifierValueConverter<>).MakeGenericType(propertyType);
        return ExpressionActivator.BuildFactory<ValueConverter>(converterType);
    }

    private static Func<ValueConverter> BuildGuidIdentifierNullableFactory(Type propertyType)
    {
        var converterType = typeof(GuidIdentifierNullableValueConverter<>).MakeGenericType(propertyType);
        return ExpressionActivator.BuildFactory<ValueConverter>(converterType);
    }

    #endregion

}
