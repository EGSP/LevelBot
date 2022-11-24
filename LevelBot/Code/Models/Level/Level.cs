namespace LevelBot.Code.Models;

public class Level
{
    public ulong Value { get; set; }

    public Level(){}
    public Level(ulong value) => Value = value;

    public static Level Calculate(Experience experience, ulong step)
    {
        if (experience.Value == 0)
            return new Level(0);

        if (step == 0)
            throw new InvalidOperationException();

        var result = MathF.Sqrt((1F/step) * experience.Value);
        return new Level(Convert.ToUInt64(MathF.Floor(result)));
    }
}