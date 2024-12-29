namespace BattleTechBattleSimulator
{
    public class AnalysisService
    {
        public void RunSimulations(List<Mech> force1, List<Mech> force2, int numberOfSimulations, bool useAlternateDamageRules, MovementMode movementMode)
        {
            int force1Wins = 0;
            int force2Wins = 0;
            int totalRounds = 0;
            int totalForce1Points = 0;
            int totalForce2Points = 0;
            int totalForce1ShortRangeDamage = 0;
            int totalForce1MediumRangeDamage = 0;
            int totalForce1LongRangeDamage = 0;
            int totalForce2ShortRangeDamage = 0;
            int totalForce2MediumRangeDamage = 0;
            int totalForce2LongRangeDamage = 0;

            for (int i = 0; i < numberOfSimulations; i++)
            {
                List<Mech> force1Copy = force1.Select(mech => new Mech
                {
                    Designation = mech.Designation,
                    Name = mech.Name,
                    PointValue = mech.PointValue,
                    Type = mech.Type,
                    Size = mech.Size,
                    TacticalMovementModifier = mech.TacticalMovementModifier,
                    Movement = mech.Movement,
                    MovementType = mech.MovementType,
                    JumpMovement = mech.JumpMovement,
                    Role = mech.Role,
                    Skill = mech.Skill,
                    ShortRangeDamage = mech.ShortRangeDamage,
                    MediumRangeDamage = mech.MediumRangeDamage,
                    LongRangeDamage = mech.LongRangeDamage,
                    OverheatPotential = mech.OverheatPotential,
                    HeatScale = mech.HeatScale,
                    ArmorPoints = mech.ArmorPoints,
                    StructurePoints = mech.StructurePoints,
                    Position = mech.Position
                }).ToList();

                List<Mech> force2Copy = force2.Select(mech => new Mech
                {
                    Designation = mech.Designation,
                    Name = mech.Name,
                    PointValue = mech.PointValue,
                    Type = mech.Type,
                    Size = mech.Size,
                    TacticalMovementModifier = mech.TacticalMovementModifier,
                    Movement = mech.Movement,
                    MovementType = mech.MovementType,
                    JumpMovement = mech.JumpMovement,
                    Role = mech.Role,
                    Skill = mech.Skill,
                    ShortRangeDamage = mech.ShortRangeDamage,
                    MediumRangeDamage = mech.MediumRangeDamage,
                    LongRangeDamage = mech.LongRangeDamage,
                    OverheatPotential = mech.OverheatPotential,
                    HeatScale = mech.HeatScale,
                    ArmorPoints = mech.ArmorPoints,
                    StructurePoints = mech.StructurePoints,
                    Position = mech.Position
                }).ToList();

                bool printOutput = numberOfSimulations == 1;
                Simulation simulation = new Simulation(force1Copy, force2Copy, useAlternateDamageRules, movementMode, printOutput);
                int rounds = simulation.StartSimulation();

                totalRounds += rounds;

                int force1Points = force1Copy.Sum(mech => mech.PointValue);
                int force2Points = force2Copy.Sum(mech => mech.PointValue);

                totalForce1Points += force1Points;
                totalForce2Points += force2Points;

                if (force1Points > force2Points)
                {
                    force1Wins++;
                }
                else if (force2Points > force1Points)
                {
                    force2Wins++;
                }

                totalForce1ShortRangeDamage += simulation.Force1ShortRangeDamage;
                totalForce1MediumRangeDamage += simulation.Force1MediumRangeDamage;
                totalForce1LongRangeDamage += simulation.Force1LongRangeDamage;
                totalForce2ShortRangeDamage += simulation.Force2ShortRangeDamage;
                totalForce2MediumRangeDamage += simulation.Force2MediumRangeDamage;
                totalForce2LongRangeDamage += simulation.Force2LongRangeDamage;
            }

            Console.WriteLine($"\nResults after {numberOfSimulations} simulations:");
            Console.WriteLine($"Force 1 win percentage: {(double)force1Wins / numberOfSimulations * 100}%");
            Console.WriteLine($"Force 2 win percentage: {(double)force2Wins / numberOfSimulations * 100}%");
            Console.WriteLine($"Average rounds per simulation: {(double)totalRounds / numberOfSimulations}");
            Console.WriteLine($"Average points left for Force 1: {(double)totalForce1Points / numberOfSimulations}");
            Console.WriteLine($"Average points left for Force 2: {(double)totalForce2Points / numberOfSimulations}");
            Console.WriteLine($"Force 1 most effective range: {GetMostEffectiveRange(totalForce1ShortRangeDamage, totalForce1MediumRangeDamage, totalForce1LongRangeDamage)}");
            Console.WriteLine($"Force 2 most effective range: {GetMostEffectiveRange(totalForce2ShortRangeDamage, totalForce2MediumRangeDamage, totalForce2LongRangeDamage)}");
        }

        private string GetMostEffectiveRange(int shortRangeDamage, int mediumRangeDamage, int longRangeDamage)
        {
            if (shortRangeDamage >= mediumRangeDamage && shortRangeDamage >= longRangeDamage)
            {
                return "Short Range";
            }
            else if (mediumRangeDamage >= shortRangeDamage && mediumRangeDamage >= longRangeDamage)
            {
                return "Medium Range";
            }
            else
            {
                return "Long Range";
            }
        }
    }
}
