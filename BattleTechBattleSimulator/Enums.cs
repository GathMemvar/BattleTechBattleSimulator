namespace BattleTechBattleSimulator
{
    public enum MovementType
    {
        Ground,
        Hover,
        Wheeled,
        Tracked,
        VTOL
    }

    public enum UnitRole
    {
        Ambusher,
        Brawler,
        Juggernaut,
        MissileBoat,
        Scout,
        Skirmisher,
        Sniper,
        Striker
    }

    public enum VehicleType
    {
        Mech,
        Vehicle,
        VTOL
    }

    public enum CriticalHits
    {
        None,
        Ammo,
        Engine,
        FireControl,
        Weapon,
        Movement,
        Catastrophic
    }

    public enum MovementMode
    {
        SimpleMovement,
        OptimizedMovement
    }
}
