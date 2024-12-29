namespace BattleTechBattleSimulator
{
    public interface ITag
    {
        string Name { get; }
    }

    public class Tag : ITag
    {
        public string Name { get; }

        public Tag(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Tag name cannot be null or empty.", nameof(name));

            Name = name;
        }
    }
}
