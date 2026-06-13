using System.ComponentModel.DataAnnotations;

namespace OptionsExample.Options;

//        "SomeOptions__SomeIntDictionary__one": "123"

public class SomeOptions
{
    public const string Key = "SomeOptions";

    [Required]
    public string SomeString { get; set; } = null!;
    
    [Required]
    public int SomeInt { get; set; }
    
    [Required]
    public bool SomeBool { get; set; }

    [Required]
    public NestedOptions Nested { get; set; } = null!;

    [MinLength(2)]
    public List<int> SomeIntList { get; set; } = [];
    
    [Required]
    public Dictionary<string, int> SomeIntDictionary { get; set; } = null!;
}

public class NestedOptions
{
    [Required]
    public int NestedInt { get; set; }
}