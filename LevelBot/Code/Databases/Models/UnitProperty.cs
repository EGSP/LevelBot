namespace LevelBot.Code.Databases.Models;

public class StringUnitProperty
{
    public string UnitId { get; set; }
    public string UnitValue { get; set; }
    
    public StringUnitProperty(){}

    public StringUnitProperty(string unitId, string unitValue)
    {
        UnitId = unitId;
        UnitValue = unitValue;
    }
}