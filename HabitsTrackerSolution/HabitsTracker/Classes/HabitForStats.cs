namespace HabitsTracker.Classes
{
    public class HabitForStats : Habit
    {
        public Dictionary<string, bool> Stats { get; set; } = new Dictionary<string, bool>();
    }
}
