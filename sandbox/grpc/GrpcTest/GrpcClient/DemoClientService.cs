using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcDemo;
using GrpcWebClient.Models;
using GrpcWebClient.Services;

namespace GrpcClient;

public class DemoClientService : IDemoClientService
{
    private readonly DemoService.DemoServiceClient _grpcClient;

    public DemoClientService(DemoService.DemoServiceClient grpcClient)
    {
        _grpcClient = grpcClient;
    }

    public async Task<DataResponseModel> GetDataAsync(int id, string name)
    {
        var request = new DataRequest { Id = id, Name = name };
        var response = await _grpcClient.GetDataAsync(request);
        
        return MapToModel(response);
    }

    public async Task<DataListResponseModel> GetDataListAsync(int id, string name)
    {
        var request = new DataRequest { Id = id, Name = name };
        var response = await _grpcClient.GetDataListAsync(request);
        
        return new DataListResponseModel
        {
            Items = response.Items.Select(MapToModel).ToList(),
            Numbers = response.Numbers.ToList(),
            Tags = response.Tags.ToList()
        };
    }

    public async Task<List<DataResponseModel>> GetDataStreamAsync(int id, string name)
    {
        var request = new DataRequest { Id = id, Name = name };
        var streamCall = _grpcClient.GetMultipleData(request);
        
        var results = new List<DataResponseModel>();
        
        await foreach (var item in streamCall.ResponseStream.ReadAllAsync())
        {
            results.Add(MapToModel(item));
        }
        
        return results;
    }

    public async Task BulkUpdate()
    {
        var call = _grpcClient.BulkDataRequest();
        
        try
        {
            // Отправляем несколько запросов
            await call.RequestStream.WriteAsync(new DataRequest 
            { 
                Name = "Alice",
                Id = 10
            });
            
            await call.RequestStream.WriteAsync(new DataRequest 
            { 
                Name = "Bob",
                Id = 20
            });
            
            await call.RequestStream.WriteAsync(new DataRequest 
            { 
                Name = "Charlie",
                Id = 30
            });
            
            // Сообщаем серверу что больше данных не будет
            await call.RequestStream.CompleteAsync();
            
            // Получаем агрегированный результат
            var response = await call;
            
            Console.WriteLine($"Server processed {response.IntValue} requests");
            Console.WriteLine($"Total sum: {response.LongValue}");
            Console.WriteLine($"Message: {response.StringValue}");
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"gRPC error: {ex.Status}");
        }

    }

    private DataResponseModel MapToModel(DataResponse response)
    {
        return new DataResponseModel
        {
            IntValue = response.IntValue,
            LongValue = response.LongValue,
            FloatValue = response.FloatValue,
            DoubleValue = response.DoubleValue,
            StringValue = response.StringValue,
            Status = response.Status.ToString(),
            Address = response.Address != null ? new AddressModel
            {
                Street = response.Address.Street,
                City = response.Address.City,
                ZipCode = response.Address.ZipCode
            } : null,
            NullableString = response.NullableString,
            CreatedAt = response.CreatedAt?.ToDateTime(),
            UpdatedAt = response.UpdatedAt?.ToDateTimeOffset(),
            BirthDate = response.BirthDate != null 
                ? new DateOnly(response.BirthDate.Year, response.BirthDate.Month, response.BirthDate.Day)
                : null
        };
    }
}
