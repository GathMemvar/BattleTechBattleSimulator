namespace BattleTechBattleSimulator
{
    public interface ITaggable
    {
        IList<ITag> Tags { get; }

        void AddTag(ITag tag)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));

            if (!Tags.Contains(tag))
            {
                Tags.Add(tag);
            }
        }

        void RemoveTag(ITag tag)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));

            Tags.Remove(tag);
        }

        bool HasTag(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName)) throw new ArgumentException("Tag name cannot be null or empty.", nameof(tagName));

            return Tags.Any(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
