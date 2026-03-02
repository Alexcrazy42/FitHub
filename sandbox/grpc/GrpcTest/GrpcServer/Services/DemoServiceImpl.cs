using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcDemo;
using Status = GrpcDemo.Status;

namespace GrpcServer.Services;

public class DemoServiceImpl : DemoService.DemoServiceBase
{
    public override Task<DataResponse> GetData(DataRequest request, ServerCallContext context)
    {
        var response = new DataResponse
        {
            // Числа
            IntValue = 42,
            LongValue = 9876543210L,
            FloatValue = 3.14f,
            DoubleValue = 2.718281828,
            
            // Строка
            StringValue = $"Hello, {request.Name}!",
            
            // Enum
            Status = Status.Active,
            
            // Вложенный объект (нуллябельный)
            Address = new Address
            {
                Street = "123 Main St",
                City = "Moscow",
                ZipCode = 101000
            },
            
            // Нуллябельная строка
            NullableString = "This is nullable",
            
            // DateTime/DateTimeOffset
            CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow),
            UpdatedAt = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow),
            
            // DateOnly (кастомный тип)
            BirthDate = new DateOnlyMessage
            {
                Year = 1990,
                Month = 5,
                Day = 15
            }
        };
        
        return Task.FromResult(response);
    }
    
    public override async Task GetMultipleData(DataRequest request, 
        IServerStreamWriter<DataResponse> responseStream, ServerCallContext context)
    {
        for (int i = 1; i <= 5; i++)
        {
            await responseStream.WriteAsync(new DataResponse
            {
                IntValue = i,
                StringValue = $"Item {i}",
                Status = (Status)(i % 3),
                CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
            });
            
            await Task.Delay(500);
        }
    }
    
    public override Task<DataListResponse> GetDataList(DataRequest request, ServerCallContext context)
    {
        var response = new DataListResponse();
        
        // Массив объектов
        response.Items.Add(new DataResponse 
        { 
            IntValue = 1, 
            StringValue = "First",
            CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
        });
        response.Items.Add(new DataResponse 
        { 
            IntValue = 2, 
            StringValue = "Second",
            CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
        });
        
        // Массив чисел
        response.Numbers.Add(10);
        response.Numbers.Add(20);
        response.Numbers.Add(30);
        
        // Массив строк
        response.Tags.Add("tag1");
        response.Tags.Add("tag2");
        
        return Task.FromResult(response);
    }
    
    /// <summary>
    /// Client Streaming: клиент отправляет поток запросов, сервер возвращает один агрегированный ответ
    /// </summary>
    public override async Task<DataResponse> BulkDataRequest(
        IAsyncStreamReader<DataRequest> requestStream, 
        ServerCallContext context)
    {
        
        var processedCount = 0;
        var allNames = new List<string>();
        var totalIntValue = 0;
        
        // Читаем все запросы из стрима
        await foreach (var request in requestStream.ReadAllAsync(context.CancellationToken))
        {
            processedCount++;
            allNames.Add(request.Name ?? "Unknown");
            totalIntValue += request.Id;
            
            // Можно прервать обработку если клиент отменил запрос
            if (context.CancellationToken.IsCancellationRequested)
            {
                break;
            }
        }
        
        
        // Возвращаем агрегированный результат
        var response = new DataResponse
        {
            IntValue = processedCount,
            LongValue = totalIntValue,
            StringValue = $"Processed {processedCount} requests from: {string.Join(", ", allNames)}",
            Status = Status.Active,
            CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
        };
        
        return response;
    }

    /// <summary>
    /// Bidirectional Streaming с параллельной обработкой
    /// Читаем запросы и отправляем ответы независимо друг от друга
    /// </summary>
    public override async Task StreamRequest(
        IAsyncStreamReader<DataRequest> requestStream,
        IServerStreamWriter<DataResponse> responseStream,
        ServerCallContext context)
    {
        // Channel для связи между чтением и отправкой
        var channel = System.Threading.Channels.Channel.CreateUnbounded<DataResponse>();
        
        // Task 1: Читаем входящие запросы и обрабатываем их
        var readTask = Task.Run(async () =>
        {
            try
            {
                await foreach (var request in requestStream.ReadAllAsync(context.CancellationToken))
                {
                    Console.WriteLine($"[Server] Received: Id={request.Id}, Name={request.Name}");
                    
                    // Обрабатываем запрос
                    var response = new DataResponse
                    {
                        IntValue = request.Id * 2,
                        StringValue = $"Processed: {request.Name}",
                        Status = request.Id % 2 == 0 ? Status.Active : Status.Inactive,
                        CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
                    };
                    
                    // Отправляем в канал для отправки клиенту
                    await channel.Writer.WriteAsync(response, context.CancellationToken);
                    
                    // Имитация обработки
                    await Task.Delay(100, context.CancellationToken);
                }
                
                Console.WriteLine("[Server] Client finished sending requests");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Server] Read error: {ex.Message}");
            }
            finally
            {
                // Сигнализируем что больше не будет данных
                channel.Writer.Complete();
            }
        }, context.CancellationToken);
        
        // Task 2: Читаем из канала и отправляем клиенту
        try
        {
            await foreach (var response in channel.Reader.ReadAllAsync(context.CancellationToken))
            {
                await responseStream.WriteAsync(response, context.CancellationToken);
                Console.WriteLine($"[Server] Sent: IntValue={response.IntValue}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Server] Write error: {ex.Message}");
        }
        
        // Ждем завершения чтения
        await readTask;
        
        Console.WriteLine("[Server] StreamRequest completed");
    }
}