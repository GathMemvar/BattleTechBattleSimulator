// See https://aka.ms/new-console-template for more information
using BattleTechBattleSimulator;

List<Mech> Database = new List<Mech>();

List<Mech> Force1 = new List<Mech>();
List<Mech> Force2 = new List<Mech>();

string filePath = "MechsDatabase.txt";

Console.WriteLine("Loading database...");

LoadMechDatabase(filePath);

Console.WriteLine("Database loaded.");
Console.WriteLine("Database contains " + Database.Count + " mechs. They are as follows:");

foreach (Mech mech in Database)
{
    Console.WriteLine(mech.Designation + " " + mech.Name);
}

while (true)
{
    Console.WriteLine("\nEnter 'exit' to close the program at any time.");

    Console.WriteLine("\nEnter Force 1 mechs (type 'Done' when finished):");
    Console.WriteLine("Format: [count] [mech designation] skill [skill value]");
    LoadForce(Force1);

    Console.WriteLine("\nForce 1 contains " + Force1.Count + " mechs. They are as follows:");
    PrintForceStatus("Force 1", Force1);

    Console.WriteLine("\nEnter Force 2 mechs (type 'Done' when finished):");
    Console.WriteLine("Format: [count] [mech designation] skill [skill value]");
    LoadForce(Force2);

    Console.WriteLine("\nForce 2 contains " + Force2.Count + " mechs. They are as follows:");
    PrintForceStatus("Force 2", Force2);

    Console.WriteLine("\nWould you like to use alternate damage rules? (yes/no)");
    bool useAlternateDamageRules = Console.ReadLine().Equals("yes", StringComparison.OrdinalIgnoreCase);

    Console.WriteLine("\nSelect movement mode (1 for SimpleMovement, 2 for OptimizedMovement):");
    MovementMode movementMode = Console.ReadLine() switch
    {
        "1" => MovementMode.SimpleMovement,
        "2" => MovementMode.OptimizedMovement,
        _ => MovementMode.SimpleMovement
    };

    Console.WriteLine("\nHow many simulations would you like to run? (1, 10, 100, 1000)");
    int numberOfSimulations = int.Parse(Console.ReadLine());

    RunSimulations(numberOfSimulations, useAlternateDamageRules, movementMode);

    Console.WriteLine("\nWould you like to run another simulation? (yes/no)");
    if (!Console.ReadLine().Equals("yes", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    Console.WriteLine("\nWould you like to use the same forces? (yes/no)");
    if (!Console.ReadLine().Equals("yes", StringComparison.OrdinalIgnoreCase))
    {
        Force1.Clear();
        Force2.Clear();
    }
}

void RunSimulations(int numberOfSimulations, bool useAlternateDamageRules, MovementMode movementMode)
{
    AnalysisService analysisService = new AnalysisService();
    analysisService.RunSimulations(Force1, Force2, numberOfSimulations, useAlternateDamageRules, movementMode);
}

void LoadMechDatabase(string filePath)
{
    if (!File.Exists(filePath))
    {
        Console.WriteLine("File not found.");
        return;
    }

    string[] lines = File.ReadAllLines(filePath)
                            .Where(line => !string.IsNullOrWhiteSpace(line))
                            .ToArray();
    Mech currentMech = null;
    int propertyIndex = 0;

    foreach (string line in lines)
    {
        if (line == "----")
        {
            if (currentMech != null && !string.IsNullOrEmpty(currentMech.Designation))
            {
                Database.Add(currentMech);
            }
            currentMech = new Mech();
            propertyIndex = 0;
        }
        else if (currentMech != null)
        {
            switch (propertyIndex)
            {
                case 0:
                    currentMech.Designation = line;
                    break;
                case 1:
                    currentMech.Name = line;
                    break;
                case 2:
                    currentMech.PointValue = int.Parse(line);
                    break;
                case 3:
                    currentMech.Type = Enum.Parse<VehicleType>(line);
                    break;
                case 4:
                    currentMech.Size = int.Parse(line);
                    break;
                case 5:
                    currentMech.TacticalMovementModifier = int.Parse(line);
                    break;
                case 6:
                    currentMech.Movement = int.Parse(line);
                    break;
                case 7:
                    currentMech.MovementType = Enum.Parse<MovementType>(line);
                    break;
                case 8:
                    currentMech.JumpMovement = int.Parse(line);
                    break;
                case 9:
                    currentMech.Role = Enum.Parse<UnitRole>(line);
                    break;
                case 10:
                    currentMech.Skill = int.Parse(line);
                    break;
                case 11:
                    currentMech.ShortRangeDamage = int.Parse(line);
                    break;
                case 12:
                    currentMech.MediumRangeDamage = int.Parse(line);
                    break;
                case 13:
                    currentMech.LongRangeDamage = int.Parse(line);
                    break;
                case 14:
                    currentMech.OverheatPotential = int.Parse(line);
                    break;
                case 15:
                    currentMech.HeatScale = int.Parse(line);
                    break;
                case 16:
                    currentMech.ArmorPoints = int.Parse(line);
                    break;
                case 17:
                    currentMech.StructurePoints = int.Parse(line);
                    break;
            }
            propertyIndex++;
        }
    }

    if (currentMech != null && !string.IsNullOrEmpty(currentMech.Designation))
    {
        Database.Add(currentMech);
    }
}

void LoadForce(List<Mech> force)
{
    while (true)
    {
        string input = Console.ReadLine();
        if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        {
            Environment.Exit(0);
        }
        if (input.Equals("Done", StringComparison.OrdinalIgnoreCase))
        {
            break;
        }

        int count = 1;
        string mechDesignation = input;
        int skill = -1;

        // Check if the input contains 'skill'
        int skillIndex = input.IndexOf("skill", StringComparison.OrdinalIgnoreCase);
        if (skillIndex > 0)
        {
            string skillPart = input.Substring(skillIndex + 5);
            if (int.TryParse(skillPart, out int parsedSkill))
            {
                skill = parsedSkill;
                mechDesignation = input.Substring(0, skillIndex).Trim();
            }
        }

        // Check if the input starts with a number followed by a space
        int firstSpaceIndex = mechDesignation.IndexOf(' ');
        if (firstSpaceIndex > 0 && int.TryParse(mechDesignation.Substring(0, firstSpaceIndex), out int parsedCount))
        {
            count = parsedCount;
            mechDesignation = mechDesignation.Substring(firstSpaceIndex + 1);
        }

        Mech templateMech = Database.FirstOrDefault(m => m.Designation.Equals(mechDesignation, StringComparison.OrdinalIgnoreCase) || m.Name.Equals(mechDesignation, StringComparison.OrdinalIgnoreCase));
        if (templateMech != null)
        {
            for (int i = 0; i < count; i++)
            {
                Mech newMech = new Mech
                {
                    Designation = templateMech.Designation,
                    Name = templateMech.Name,
                    PointValue = templateMech.PointValue,
                    Type = templateMech.Type,
                    Size = templateMech.Size,
                    TacticalMovementModifier = templateMech.TacticalMovementModifier,
                    Movement = templateMech.Movement,
                    MovementType = templateMech.MovementType,
                    JumpMovement = templateMech.JumpMovement,
                    Role = templateMech.Role,
                    Skill = skill > 0 ? skill : templateMech.Skill,
                    ShortRangeDamage = templateMech.ShortRangeDamage,
                    MediumRangeDamage = templateMech.MediumRangeDamage,
                    LongRangeDamage = templateMech.LongRangeDamage,
                    OverheatPotential = templateMech.OverheatPotential,
                    HeatScale = templateMech.HeatScale,
                    ArmorPoints = templateMech.ArmorPoints,
                    StructurePoints = templateMech.StructurePoints
                };
                force.Add(newMech);
            }
        }
        else
        {
            Console.WriteLine("Mech not found in database.");
        }
    }
}

void PrintForceStatus(string forceName, List<Mech> force)
{
    Console.WriteLine($"\n{forceName} remaining mechs:");
    foreach (Mech mech in force)
    {
        Console.WriteLine($"{mech.Designation} {mech.Name} - Skill: {mech.Skill}, Short Range Damage: {mech.ShortRangeDamage}, Medium Range Damage: {mech.MediumRangeDamage}, Long Range Damage: {mech.LongRangeDamage}, Armor: {mech.ArmorPoints}, Structure: {mech.StructurePoints}, PointValue: {mech.PointValue}");
    }
}