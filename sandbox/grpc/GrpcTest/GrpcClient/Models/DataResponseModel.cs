using System;
using System.Collections.Generic;

namespace GrpcWebClient.Models;

public class DataResponseModel
{
    public int IntValue { get; set; }
    public long LongValue { get; set; }
    public float FloatValue { get; set; }
    public double DoubleValue { get; set; }
    public string StringValue { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public AddressModel? Address { get; set; }
    public string? NullableString { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateOnly? BirthDate { get; set; }
}

public class AddressModel
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int ZipCode { get; set; }
}

public class DataListResponseModel
{
    public List<DataResponseModel> Items { get; set; } = new();
    public List<int> Numbers { get; set; } = new();
    public List<string> Tags { get; set; } = new();
}