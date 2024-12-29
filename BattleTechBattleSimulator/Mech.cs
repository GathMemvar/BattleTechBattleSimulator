namespace BattleTechBattleSimulator
{
    public class Mech : ITaggable
    {
        public Mech() { }
        public string Designation { get; set; }
        public string Name { get; set; }
        public int PointValue { get; set; }
        public VehicleType Type { get; set; }
        public int Size { get; set; }
        public int TacticalMovementModifier { get; set; }
        public int Movement { get; set; }
        public MovementType MovementType { get; set; }
        public int JumpMovement { get; set; }
        public UnitRole Role { get; set; }
        public int Skill { get; set; }
        public int ShortRangeDamage { get; set; }
        public int MediumRangeDamage { get; set; }
        public int LongRangeDamage { get; set; }
        public int OverheatPotential { get; set; }
        public int HeatScale { get; set; }
        public int ArmorPoints { get; set; }
        public int StructurePoints { get; set; }
        public int Position { get; set; } // New property to track position
        public IList<ITag> Tags { get; }
    }
}
