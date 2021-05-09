using System;
using System.Collections.Generic;
using System.Linq;

/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/

internal class Player
{
    private static void Main(string[] args)
    {
        string[] inputs;

        List<Cell> cells = new List<Cell>();
        int numberOfCells = int.Parse(Console.ReadLine()); // 37
        for (int i = 0; i < numberOfCells; i++)
        {
            inputs = Console.ReadLine().Split(' ');
            int index = int.Parse(inputs[0]); // 0 is the center cell, the next cells spiral outwards
            int richness = int.Parse(inputs[1]); // 0 if the cell is unusable, 1-3 for usable cells
            int neigh0 = int.Parse(inputs[2]); // the index of the neighbouring cell for each direction
            int neigh1 = int.Parse(inputs[3]);
            int neigh2 = int.Parse(inputs[4]);
            int neigh3 = int.Parse(inputs[5]);
            int neigh4 = int.Parse(inputs[6]);
            int neigh5 = int.Parse(inputs[7]);

            List<int> neighbours = new List<int>() { neigh0, neigh1, neigh2, neigh3, neigh4, neigh5 };

            Cell cell = new Cell(index, richness, neighbours);
            cells.Add(cell);
        }

        // game loop
        while (true)
        {
            // Day Info
            int day = int.Parse(Console.ReadLine()); // the game lasts 24 days: 0-23
            int nutrients = int.Parse(Console.ReadLine()); // the base score you gain from the next COMPLETE action

            // My Info
            inputs = Console.ReadLine().Split(' ');
            int sun = int.Parse(inputs[0]); // your sun points
            int score = int.Parse(inputs[1]); // your current score

            // Opponent Info
            inputs = Console.ReadLine().Split(' ');
            int oppSun = int.Parse(inputs[0]); // opponent's sun points
            int oppScore = int.Parse(inputs[1]); // opponent's score
            bool oppIsWaiting = inputs[2] != "0"; // whether your opponent is asleep until the next day

            // Tree
            List<Tree> trees = new List<Tree>();
            int numberOfTrees = int.Parse(Console.ReadLine()); // the current amount of trees
            for (int i = 0; i < numberOfTrees; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                int cellIndex = int.Parse(inputs[0]); // location of this tree
                int size = int.Parse(inputs[1]); // size of this tree: 0-3
                bool isMine = inputs[2] != "0"; // 1 if this is your tree
                bool isDormant = inputs[3] != "0"; // 1 if this tree is dormant
                Tree tree = new Tree(cellIndex, size, isMine, isDormant);
                trees.Add(tree);
            }

            // Actions
            int numberOfPossibleActions = int.Parse(Console.ReadLine()); // all legal actions
            for (int i = 0; i < numberOfPossibleActions; i++)
            {
                string possibleAction = Console.ReadLine(); // try printing something from here to start with
                Console.Error.WriteLine(possibleAction);
            }

            // Calculate Grow Cost
            int numberOfSize0Trees = trees.Where(t => t.Size == 0 && t.IsMine == true).Count();
            int numberOfSize1Trees = trees.Where(t => t.Size == 1 && t.IsMine == true).Count();
            int numberOfSize2Trees = trees.Where(t => t.Size == 2 && t.IsMine == true).Count();
            int numberOfSize3Trees = trees.Where(t => t.Size == 3 && t.IsMine == true).Count();

            Console.Error.WriteLine($"Number of size 0 trees: {numberOfSize0Trees}");
            Console.Error.WriteLine($"Number of size 1 trees: {numberOfSize1Trees}");
            Console.Error.WriteLine($"Number of size 2 trees: {numberOfSize2Trees}");
            Console.Error.WriteLine($"Number of size 3 trees: {numberOfSize3Trees}");
            foreach (var tree in trees)
            {
                tree.GrowCost = tree.CalculateGrowCost(numberOfSize1Trees, numberOfSize2Trees, numberOfSize3Trees);
            }

            Console.Error.WriteLine($"Day: {day}");

            // GROW cellIdx | SEED sourceIdx targetIdx | COMPLETE cellIdx | WAIT <message>

            var action = "WAIT";
            int maxItemsLimit = 2;

            foreach (var tree in trees.Where(t => t.IsMine))
            {
                // Get tree position
                var treeCell = cells.Where(c => c.Index == tree.CellIndex).First();
                Console.Error.WriteLine($"Tree position: {treeCell.Index}");
                Console.Error.WriteLine($"Tree size: {tree.Size}");
                Console.Error.WriteLine($"Tree Grow cost: {tree.GrowCost}");
                Console.Error.WriteLine($"Sun: {sun}");

                // Harvest Tree if the remaining rounds are equal to or less than sunpoints/(number
                // of max trees * harvest cost) && tree size is 3
                var remainingRounds = 23 - day;
                var maxItems = Math.Min(maxItemsLimit, remainingRounds);
                Console.Error.WriteLine($"Max items: {maxItems}");
                Console.Error.WriteLine($"Remaining rounds: {remainingRounds}");

                Console.Error.WriteLine($"Checking if I should harvest tree");
                if (sun > 0)
                {
                    if (numberOfSize3Trees > maxItems && tree.Size == 3 && !tree.IsDormant)
                    {
                        // harvest tree
                        action = $"COMPLETE {tree.CellIndex}";
                        break;
                    }
                    else if (numberOfSize3Trees < maxItems)
                    {
                        Console.Error.WriteLine($"Not enough size 3 trees");
                    }
                    else if (tree.Size < 3)
                    {
                        Console.Error.WriteLine($"Tree too small");
                    }
                }
                else
                {
                    Console.Error.WriteLine($"No size 3 trees");
                }

                // Grow Tree if tree size = 0 && you have enough sun points
                Console.Error.WriteLine($"Checking if I should grow a seed");
                if (tree.Size == 0 && sun > tree.GrowCost && numberOfSize1Trees < maxItems)
                {
                    // grow tree
                    action = $"GROW {tree.CellIndex}";
                    break;
                }
                else if (tree.Size != 0)
                {
                    Console.Error.WriteLine("tree not a seed");
                }
                else if (sun < tree.GrowCost)
                {
                    Console.Error.WriteLine("grow cost too high");
                }

                // Seed Tree Check each neighbour
                // TODO: Add extra search range depending on tree size neighbours neighbour etc
                Console.Error.WriteLine($"Checking if I should plant a seed");
                var seedLocations = new List<seedling>();
                if (!tree.IsDormant && tree.IsMine && tree.Size > 0 && numberOfSize0Trees < maxItems)
                {
                    foreach (var neighbour in treeCell.Neighbours)
                    {
                        if (neighbour != -1)
                        {
                            var neighbourCell = cells.Where(c => c.Index == neighbour).First();
                            var cellsWithTrees = trees.Select(t => t.CellIndex).ToList();
                            Console.Error.WriteLine($"Checking cell: {neighbour}");

                            // if neighbour is empty && cell richness > 0 && you have enough sun points
                            if (!cellsWithTrees.Contains(neighbourCell.Index) && neighbourCell.Richness > 0 && sun > numberOfSize0Trees)
                            {
                                // Add cell to possible seed locations
                                Console.Error.WriteLine($"Add cell {neighbourCell.Index} to possible seed locations");
                                var seed = new seedling();
                                seed.SeedTarget = neighbourCell;
                                seed.SeedSource = treeCell;
                                seedLocations.Add(seed);
                            }
                            else if (cellsWithTrees.Contains(neighbourCell.Index))
                            {
                                Console.Error.WriteLine("target cell not empty");
                            }
                            else if (neighbourCell.Richness == 0)
                            {
                                Console.Error.WriteLine("Infertile soil");
                            }
                            else if (sun > numberOfSize0Trees)
                            {
                                Console.Error.WriteLine("growth cost too high");
                            }
                        }
                    }
                }

                // If there are suitable seed locations, plant seed
                if (seedLocations.Count() > 0)
                {
                    var maxRichness = seedLocations.Max(t => t.SeedTarget.Richness);
                    var bestSeedling = seedLocations.First(t => t.SeedTarget.Richness == maxRichness);
                    var target = bestSeedling.SeedTarget.Index;
                    var source = bestSeedling.SeedSource.Index;
                    action = $"SEED {source} {target}";
                    break;
                }

                // if no suitable seed locations available grow largest tree you can afford
                Console.Error.WriteLine($"Checking if i should grow a tree");
                if (tree.Size == 2 && sun >= tree.GrowCost && !tree.IsDormant)
                {
                    action = $"GROW {tree.CellIndex}";
                    break;
                }
                if (tree.Size == 1 && sun >= tree.GrowCost && !tree.IsDormant && numberOfSize2Trees < maxItems)
                {
                    action = $"GROW {tree.CellIndex}";
                    break;
                }
            }
            Console.Error.WriteLine(action);
            Console.WriteLine(action);

        }
    }
}

internal class seedling
{
    public Cell SeedSource { get; set; }

    public Cell SeedTarget { get; set; }

    public int CompareTo(seedling comparePart)
    {
        // A null value means that this object is greater.
        if (comparePart == null)
            return 1;
        else
            return this.SeedTarget.Richness.CompareTo(comparePart.SeedTarget.Richness);
    }
}

internal class Cell
{
    public int Index { get; set; }

    public int Richness { get; set; }

    public List<int> Neighbours { get; set; }

    public Cell(int index, int richness, List<int> neighbours)
    {
        Index = index;
        Richness = richness;
        Neighbours = neighbours;
    }
}

internal class Tree
{
    public int CellIndex { get; set; }

    public int Size { get; set; }

    public bool IsMine { get; set; }

    public bool IsDormant { get; set; }

    public int GrowCost { get; set; }

    public Tree(int cellIndex, int size, bool isMine, bool isDormant)
    {
        CellIndex = cellIndex;
        Size = size;
        IsMine = isMine;
        IsDormant = isDormant;
    }

    public int CalculateGrowCost(int numberOfSize1Trees, int numberOfSize2Trees, int numberOfSize3Trees)
    {
        // Growing a seed into a size 1 tree costs 1 sun point +the number of size 1 trees you
        // already own. Growing a size 1 tree into a size 2 tree costs 3 sun points +the number of
        // size 2 trees you already own. Growing a size 2 tree into a size 3 tree costs 7 sun points
        // +the number of size 3 trees you already own.

        if (Size == 0)
        {
            GrowCost = 1 + numberOfSize1Trees;
        }
        else if (Size == 1)
        {
            GrowCost = 3 + numberOfSize2Trees;
        }
        else if (Size == 2)
        {
            GrowCost = 7 + numberOfSize3Trees;
        }
        return GrowCost;
    }
}