using System;
using System.Collections.Generic;
using System.Drawing;

namespace TetrisML
{
    public class IAController
    {
        private AIParameters currentParameters;

        public IAController()
        {
            currentParameters = new AIParameters();
        }

        // Paramètres ajustables de l'IA
        public class AIParameters
        {
            public double HeightWeight { get; set; } = -0.510066;      // Pénalise la hauteur globale
            public double HolesWeight { get; set; } = -0.35663;        // Pénalise les trous
            public double CompleteLinesWeight { get; set; } = 0.760666; // Récompense les lignes complètes
            public double BumpinessWeight { get; set; } = -0.184483;   // Pénalise les différences de hauteur
        }

        // Structure pour stocker un mouvement possible
        public struct PossibleMove
        {
            public int Rotation { get; set; }     // Nombre de rotations à effectuer
            public int Translation { get; set; }   // Déplacement horizontal
            public double Score { get; set; }      // Score évalué pour ce mouvement
        }

        // Méthode principale pour trouver le meilleur mouvement
        public PossibleMove FindBestMove(int[,] grid, Piece currentPiece)
        {
            var bestMove = new PossibleMove { Score = double.MinValue };

            // Essayer toutes les rotations possibles
            for (int rotation = 0; rotation < 4; rotation++)
            {
                var testPiece = currentPiece.Clone();
                for (int i = 0; i < rotation; i++)
                    testPiece.Rotate();

                // Pour chaque position horizontale possible
                for (int x = -2; x < grid.GetLength(1); x++)
                {
                    var simulatedGrid = SimulateDrop(grid, testPiece, x);
                    if (simulatedGrid != null)
                    {
                        double score = EvaluatePosition(simulatedGrid);
                        if (score > bestMove.Score)
                        {
                            bestMove = new PossibleMove
                            {
                                Rotation = rotation,
                                Translation = x,
                                Score = score
                            };
                        }
                    }
                }
            }

            return bestMove;
        }

        // Mise à jour des paramètres depuis l'interface utilisateur
        public void UpdateParameters(AIParameters newParams)
        {
            currentParameters = newParams;
        }

        // Évaluation d'une position
        private double EvaluatePosition(int[,] grid)
        {
            return currentParameters.HeightWeight * CalculateAggregateHeight(grid)
                 + currentParameters.HolesWeight * CalculateHoles(grid)
                 + currentParameters.CompleteLinesWeight * CalculateCompleteLines(grid)
                 + currentParameters.BumpinessWeight * CalculateBumpiness(grid);
        }

        // Calcule la hauteur totale de toutes les colonnes
        private int CalculateAggregateHeight(int[,] grid)
        {
            int totalHeight = 0;
            int width = grid.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                totalHeight += GetColumnHeight(grid, x);
            }

            return totalHeight;
        }

        // Calcule le nombre de trous (cases vides avec des blocs au-dessus)
        private int CalculateHoles(int[,] grid)
        {
            int holes = 0;
            int width = grid.GetLength(1);
            int height = grid.GetLength(0);

            for (int x = 0; x < width; x++)
            {
                bool blockFound = false;
                for (int y = 0; y < height; y++)
                {
                    if (grid[y, x] != 0)
                        blockFound = true;
                    else if (blockFound)
                        holes++;
                }
            }

            return holes;
        }

        // Calcule le nombre de lignes complètes
        private int CalculateCompleteLines(int[,] grid)
        {
            int completeLines = 0;
            int width = grid.GetLength(1);
            int height = grid.GetLength(0);

            for (int y = 0; y < height; y++)
            {
                bool isComplete = true;
                for (int x = 0; x < width; x++)
                {
                    if (grid[y, x] == 0)
                    {
                        isComplete = false;
                        break;
                    }
                }
                if (isComplete)
                    completeLines++;
            }

            return completeLines;
        }

        // Calcule l'irrégularité de la surface (différences de hauteur entre colonnes adjacentes)
        private int CalculateBumpiness(int[,] grid)
        {
            int bumpiness = 0;
            int width = grid.GetLength(1);
            int[] heights = new int[width];

            for (int x = 0; x < width; x++)
            {
                heights[x] = GetColumnHeight(grid, x);
            }

            for (int x = 0; x < width - 1; x++)
            {
                bumpiness += Math.Abs(heights[x] - heights[x + 1]);
            }

            return bumpiness;
        }

        // Obtient la hauteur d'une colonne spécifique
        private int GetColumnHeight(int[,] grid, int x)
        {
            int height = grid.GetLength(0);

            for (int y = 0; y < height; y++)
            {
                if (grid[y, x] != 0)
                    return height - y;
            }

            return 0;
        }

        // Simule la chute d'une pièce à une position donnée
        private int[,] SimulateDrop(int[,] originalGrid, Piece piece, int xPosition)
        {
            int[,] simulatedGrid = (int[,])originalGrid.Clone();
            int height = simulatedGrid.GetLength(0);
            piece = piece.Clone();
            piece.X = xPosition;

            // Trouver la position Y finale
            int finalY = 0;
            for (int y = 0; y < height; y++)
            {
                piece.Y = y;
                if (!IsValidPosition(simulatedGrid, piece))
                {
                    finalY = y - 1;
                    break;
                }
                finalY = y;
            }

            // Si la position finale est valide, placer la pièce
            piece.Y = finalY;
            if (IsValidPosition(simulatedGrid, piece))
            {
                foreach (var block in piece.GetCurrentRotationBlocks())
                {
                    int blockX = piece.X + block.X;
                    int blockY = piece.Y + block.Y;
                    if (blockX >= 0 && blockX < simulatedGrid.GetLength(1) &&
                        blockY >= 0 && blockY < simulatedGrid.GetLength(0))
                    {
                        simulatedGrid[blockY, blockX] = piece.Type + 1;
                    }
                }
                return simulatedGrid;
            }

            return null;
        }

        // Vérifie si une position est valide
        private bool IsValidPosition(int[,] grid, Piece piece)
        {
            foreach (var block in piece.GetCurrentRotationBlocks())
            {
                int x = piece.X + block.X;
                int y = piece.Y + block.Y;

                if (x < 0 || x >= grid.GetLength(1) || y >= grid.GetLength(0))
                    return false;

                if (y >= 0 && grid[y, x] != 0)
                    return false;
            }
            return true;
        }
    }
}