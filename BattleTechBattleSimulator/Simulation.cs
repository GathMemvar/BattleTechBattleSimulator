namespace BattleTechBattleSimulator
{
    public class Simulation
    {
        private static readonly Random random = new Random();
        private readonly bool _useAlternateDamageRules;
        private readonly bool _printOutput;
        private readonly RulesService _rulesService;
        private readonly MovementMode _movementMode;

        List<Mech> _force1;
        List<Mech> _force2;

        public int Force1ShortRangeDamage { get; private set; }
        public int Force1MediumRangeDamage { get; private set; }
        public int Force1LongRangeDamage { get; private set; }
        public int Force2ShortRangeDamage { get; private set; }
        public int Force2MediumRangeDamage { get; private set; }
        public int Force2LongRangeDamage { get; private set; }

        public Simulation(List<Mech> force1, List<Mech> force2, bool useAlternateDamageRules, MovementMode movementMode, bool printOutput = true)
        {
            _force1 = force1;
            _force2 = force2;
            _useAlternateDamageRules = useAlternateDamageRules;
            _printOutput = printOutput;
            _rulesService = new RulesService();
            _movementMode = movementMode;

            // Initialize positions
            foreach (var mech in _force1)
            {
                mech.Position = 0;
            }
            foreach (var mech in _force2)
            {
                mech.Position = 42;
            }
        }

        public int StartSimulation()
        {
            int turn = 1;

            while (turn <= 12 && (_force1.Count > 0 && _force2.Count > 0))
            {
                if (_printOutput)
                {
                    Console.WriteLine($"\nTurn {turn}:");
                }

                // Move mechs based on the selected movement mode
                if (_movementMode == MovementMode.SimpleMovement)
                {
                    MoveMechsSimple();
                }
                else if (_movementMode == MovementMode.OptimizedMovement)
                {
                    MoveMechsOptimized();
                }

                List<Mech> force1Attacks = new List<Mech>(_force1);
                List<Mech> force2Attacks = new List<Mech>(_force2);

                while (force1Attacks.Count > 0 && force2Attacks.Count > 0)
                {
                    Mech attacker1 = force1Attacks[random.Next(force1Attacks.Count)];
                    int range1 = Math.Abs(attacker1.Position - _force2.Min(mech => mech.Position));
                    if (_rulesService.CanAttack(attacker1, range1))
                    {
                        Mech defender1 = _force2[random.Next(_force2.Count)];
                        _rulesService.ResolveAttack(attacker1, defender1, range1, _useAlternateDamageRules, _printOutput, this);
                    }
                    force1Attacks.Remove(attacker1);

                    if (force2Attacks.Count > 0)
                    {
                        Mech attacker2 = force2Attacks[random.Next(force2Attacks.Count)];
                        int range2 = Math.Abs(attacker2.Position - _force1.Max(mech => mech.Position));
                        if (_rulesService.CanAttack(attacker2, range2))
                        {
                            Mech defender2 = _force1[random.Next(_force1.Count)];
                            _rulesService.ResolveAttack(attacker2, defender2, range2, _useAlternateDamageRules, _printOutput, this);
                        }
                        force2Attacks.Remove(attacker2);
                    }
                }

                // Remove destroyed mechs at the end of the round
                _force1.RemoveAll(mech => mech.StructurePoints <= 0);
                _force2.RemoveAll(mech => mech.StructurePoints <= 0);

                // Increment turn
                turn++;
            }

            return turn - 1;
        }

        private void MoveMechsSimple()
        {
            foreach (var mech in _force1)
            {
                int rangeToClosestEnemy = Math.Abs(mech.Position - _force2.Min(m => m.Position));
                if (rangeToClosestEnemy > 6)
                {
                    mech.Position += Math.Min(mech.Movement, rangeToClosestEnemy - 1);
                }
            }

            foreach (var mech in _force2)
            {
                int rangeToClosestEnemy = Math.Abs(mech.Position - _force1.Max(m => m.Position));
                if (rangeToClosestEnemy > 6)
                {
                    mech.Position -= Math.Min(mech.Movement, rangeToClosestEnemy - 1);
                }
            }
        }

        private void MoveMechsOptimized()
        {
            foreach (var mech in _force1)
            {
                int optimalRange = GetOptimalRange(mech);
                int rangeToClosestEnemy = Math.Abs(mech.Position - _force2.Min(m => m.Position));
                if (rangeToClosestEnemy != optimalRange)
                {
                    if (rangeToClosestEnemy > optimalRange)
                    {
                        mech.Position += Math.Min(mech.Movement, rangeToClosestEnemy - optimalRange);
                    }
                    else
                    {
                        mech.Position -= Math.Min(mech.Movement, optimalRange - rangeToClosestEnemy);
                    }
                }
            }

            foreach (var mech in _force2)
            {
                int optimalRange = GetOptimalRange(mech);
                int rangeToClosestEnemy = Math.Abs(mech.Position - _force1.Max(m => m.Position));
                if (rangeToClosestEnemy != optimalRange)
                {
                    if (rangeToClosestEnemy > optimalRange)
                    {
                        mech.Position -= Math.Min(mech.Movement, rangeToClosestEnemy - optimalRange);
                    }
                    else
                    {
                        mech.Position += Math.Min(mech.Movement, optimalRange - rangeToClosestEnemy);
                    }
                }
            }
        }

        private int GetOptimalRange(Mech mech)
        {
            if (mech.ShortRangeDamage > 0)
            {
                return 6;
            }
            else if (mech.MediumRangeDamage > 0)
            {
                return 24;
            }
            else if (mech.LongRangeDamage > 0)
            {
                return 42;
            }
            return 42; // Default to long range if no other range is specified
        }

        public List<int> RollDice(int numberOfDice)
        {
            List<int> individualRolls = new List<int>();

            for (int i = 0; i < numberOfDice; i++)
            {
                int roll = random.Next(1, 7); // 6-sided die roll (1-6)
                individualRolls.Add(roll);
            }

            return individualRolls;
        }

        public void AddDamage(Mech attacker, int distance, int damage)
        {
            if (attacker == _force1.FirstOrDefault(m => m.Name == attacker.Name))
            {
                if (distance <= 6)
                {
                    Force1ShortRangeDamage += damage;
                }
                else if (distance <= 24)
                {
                    Force1MediumRangeDamage += damage;
                }
                else if (distance <= 42)
                {
                    Force1LongRangeDamage += damage;
                }
            }
            else if (attacker == _force2.FirstOrDefault(m => m.Name == attacker.Name))
            {
                if (distance <= 6)
                {
                    Force2ShortRangeDamage += damage;
                }
                else if (distance <= 24)
                {
                    Force2MediumRangeDamage += damage;
                }
                else if (distance <= 42)
                {
                    Force2LongRangeDamage += damage;
                }
            }
        }
    }
}