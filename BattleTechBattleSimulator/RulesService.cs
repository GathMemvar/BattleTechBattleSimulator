namespace BattleTechBattleSimulator
{
    public class RulesService
    {
        public void ResolveAttack(Mech attacker, Mech defender, int distance, bool useAlternateDamageRules, bool printOutput, Simulation simulation)
        {
            if (printOutput)
            {
                Console.WriteLine($"{attacker.Name} is attacking {defender.Name} at a distance of {distance}!");
            }

            int rangeModifier = distance switch
            {
                <= 6 => 0,
                <= 24 => 2,
                <= 42 => 4,
                _ => throw new ArgumentException("Invalid distance specified")
            };

            int targetNumber = attacker.Skill + rangeModifier + defender.TacticalMovementModifier;

            if (printOutput)
            {
                Console.WriteLine($"Target number to hit: {targetNumber}");
            }

            if (targetNumber < 2)
            {
                if (printOutput)
                {
                    Console.WriteLine("Automatic hit!");
                }
                ApplyDamage(attacker, defender, distance, targetNumber, true, simulation, printOutput);
                return;
            }

            if (targetNumber > 12)
            {
                if (printOutput)
                {
                    Console.WriteLine("Automatic miss!");
                }
                return;
            }

            bool criticalHitOccurred = false;

            if (useAlternateDamageRules)
            {
                int damage = distance switch
                {
                    <= 6 => attacker.ShortRangeDamage,
                    <= 24 => attacker.MediumRangeDamage,
                    <= 42 => attacker.LongRangeDamage,
                    _ => 0
                };

                int totalDamage = 0;
                for (int i = 0; i < damage; i++)
                {
                    List<int> attackRolls = simulation.RollDice(2);
                    int attackRoll = attackRolls.Sum();
                    if (printOutput)
                    {
                        Console.WriteLine($"Attack roll {i + 1}: {attackRoll}");
                    }
                    if (attackRoll >= targetNumber)
                    {
                        totalDamage++;
                    }
                    if (attackRoll == 12 && !criticalHitOccurred)
                    {
                        criticalHitOccurred = true;
                        ApplyCriticalHit(defender, attackRoll, printOutput);
                    }
                }
                ApplyDamage(attacker, defender, distance, targetNumber, false, simulation, printOutput, totalDamage);
            }
            else
            {
                List<int> attackRolls = simulation.RollDice(2);
                int attackRoll = attackRolls.Sum();
                if (printOutput)
                {
                    Console.WriteLine($"Attack roll: {attackRoll}");
                }

                if (attackRoll >= targetNumber)
                {
                    ApplyDamage(attacker, defender, distance, targetNumber, true, simulation, printOutput);
                }
                else
                {
                    if (printOutput)
                    {
                        Console.WriteLine($"{attacker.Name} misses {defender.Name}.");
                    }
                }

                if (attackRoll == 12)
                {
                    criticalHitOccurred = true;
                    ApplyCriticalHit(defender, attackRoll, printOutput);
                }
            }
        }

        private void ApplyDamage(Mech attacker, Mech defender, int distance, int targetNumber, bool isNormalDamage, Simulation simulation, bool printOutput, int alternateDamage = 0)
        {
            int damage = isNormalDamage ? distance switch
            {
                <= 6 => attacker.ShortRangeDamage,
                <= 24 => attacker.MediumRangeDamage,
                <= 42 => attacker.LongRangeDamage,
                _ => 0
            } : alternateDamage;

            if (printOutput)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(attacker.Name);
                Console.ResetColor();
                Console.Write(" hits ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(defender.Name);
                Console.ResetColor();
                Console.Write(" for ");
                Console.ForegroundColor = damage == 0 ? ConsoleColor.Green : ConsoleColor.Yellow;
                Console.Write($"{damage} damage");
                Console.ResetColor();
                Console.WriteLine("!");
            }

            if (damage <= defender.ArmorPoints)
            {
                defender.ArmorPoints -= damage;
            }
            else
            {
                int remainingDamage = damage - defender.ArmorPoints;
                defender.ArmorPoints = 0;
                defender.StructurePoints -= remainingDamage;

                // Check for critical hits due to structure damage
                if (remainingDamage > 0)
                {
                    int criticalRoll = simulation.RollDice(2).Sum();
                    if (printOutput)
                    {
                        Console.WriteLine($"Critical hit roll: {criticalRoll}");
                    }
                    ApplyCriticalHit(defender, criticalRoll, printOutput);
                }
            }

            if (printOutput)
            {
                Console.WriteLine($"{defender.Name} has {defender.ArmorPoints} armor points and {defender.StructurePoints} structure points left.");
            }

            simulation.AddDamage(attacker, distance, damage);
        }

        private void ApplyCriticalHit(Mech defender, int criticalRoll, bool printOutput)
        {
            CriticalHits criticalHit = criticalRoll switch
            {
                2 => CriticalHits.Ammo,
                3 or 11 => CriticalHits.Engine,
                4 or 10 => CriticalHits.FireControl,
                5 or 9 => CriticalHits.None,
                6 or 8 => CriticalHits.Weapon,
                7 => CriticalHits.Movement,
                _ => CriticalHits.None
            };

            if (printOutput)
            {
                Console.WriteLine($"Critical hit: {criticalHit}");
            }

            switch (criticalHit)
            {
                case CriticalHits.Ammo:
                    defender.ArmorPoints = 0;
                    defender.StructurePoints = 0;
                    if (printOutput)
                    {
                        Console.WriteLine($"{defender.Name} is destroyed due to an ammo hit!");
                    }
                    break;
                case CriticalHits.Engine:
                    // defender.HeatScale += 1; // Commented out, not implemented
                    break;
                case CriticalHits.FireControl:
                    defender.Skill += 1;
                    break;
                case CriticalHits.Weapon:
                    defender.ShortRangeDamage = Math.Max(0, defender.ShortRangeDamage - 1);
                    defender.MediumRangeDamage = Math.Max(0, defender.MediumRangeDamage - 1);
                    defender.LongRangeDamage = Math.Max(0, defender.LongRangeDamage - 1);
                    break;
                case CriticalHits.Movement:
                    defender.Movement = Math.Max(0, (int)Math.Ceiling(defender.Movement / 2.0));
                    defender.TacticalMovementModifier = Math.Max(0, (int)Math.Ceiling(defender.TacticalMovementModifier / 2.0));
                    // if (defender.Movement == 0)
                    // {
                    //     defender.AddTag(new Tag("Immobile")); // Commented out, not implemented
                    // }
                    break;
                case CriticalHits.None:
                default:
                    break;
            }
        }

        public bool CanAttack(Mech mech, int distance)
        {
            return distance switch
            {
                <= 6 => mech.ShortRangeDamage > 0,
                <= 24 => mech.MediumRangeDamage > 0,
                <= 42 => mech.LongRangeDamage > 0,
                _ => false
            };
        }
    }
}
