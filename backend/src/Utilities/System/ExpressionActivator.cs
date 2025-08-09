using System.Linq.Expressions;
using System.Reflection;

namespace FitHub.Utilities.System;

/// <summary>
/// Активатор на скомпилированных <see cref="Expression"/>, быстрее обычного <see cref="Activator"/>
/// </summary>
public static class ExpressionActivator
{
    /// <summary>
    /// Сбилдить фабрику по созданию типов
    /// </summary>
    /// <typeparam name="T">Тип возвращаемого экземпляра</typeparam>
    /// <param name="instanceType">Тип создаваемого экземпляра</param>
    /// <returns>Фабрика, создающая экземпляры типов</returns>
    public static Func<T> BuildFactory<T>(Type instanceType)
    {
        var constructorExpression = Expression.New(instanceType);
        var lambdaExpression = Expression.Lambda<Func<T>>(constructorExpression);
        return lambdaExpression.Compile();
    }

    /// <summary>
    /// Сбилдить фабрику по созданию типов
    /// </summary>
    /// <typeparam name="TArg">Аргумент конструктора типа <see cref="TType"/></typeparam>
    /// <typeparam name="TType">Тип, для которого, необходимо создать фабрику</typeparam>
    public static Func<TArg, TType> BuildFactory<TArg, TType>()
    {
        var constructor = typeof(TType).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
            null,
            new[] { typeof(TArg) },
            null) ?? throw new InvalidOperationException($"No suitable constructor found for {typeof(TType)}");

        var param = Expression.Parameter(typeof(TArg));
        var newExpr = Expression.New(constructor, param);
        return Expression.Lambda<Func<TArg, TType>>(newExpr, param).Compile();
    }
}
