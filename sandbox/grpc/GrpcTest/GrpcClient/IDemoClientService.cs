using System.Collections.Generic;
using System.Threading.Tasks;
using GrpcWebClient.Models;

namespace GrpcWebClient.Services;

public interface IDemoClientService
{
    Task<DataResponseModel> GetDataAsync(int id, string name);
    Task<DataListResponseModel> GetDataListAsync(int id, string name);
    Task<List<DataResponseModel>> GetDataStreamAsync(int id, string name);

    Task BulkUpdate();
    
    IAsyncEnumerable<DataResponseModel> StreamBidirectionalAsync();
}