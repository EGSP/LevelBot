namespace LevelBot.Code.Models;

public class Experience
{
    public ulong? Id { get; set; }
    public long Value { get; set; }

    public Experience()
    {
        
    }
    
    public Experience(long value)
    {
        Value = value;
    }
}

public class ExperienceOperation
{
    public ulong Id { get; set; }
    
    public Experience Experience { get; set; }
    public OperationType Operation { get; set; }
    public DateTime Time { get; set; }
    
    public ExperienceOperation(){}

    public ExperienceOperation(Experience experience, OperationType operation, DateTime time)
    {
        Experience = experience;
        Operation = operation;
        Time = time;
    }
}

public enum OperationType
{
    Add,
    Remove
}