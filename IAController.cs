/*	Bref
IAController centralise toute la logique de décision de l'IA pour le jeu Tetris, 
en évaluant et en choisissant le meilleur mouvement possible pour optimiser la position de la pièce sur la grille.

---
la classe IAController, sert à gérer l'intelligence artificielle (IA) dans le jeu de Tetris. Voici en résumé sa raison d'être :

Évaluation des Positions :
La classe définit des paramètres ajustables (via la classe interne AIParameters) qui attribuent des poids à différents aspects de la grille (hauteur, trous, lignes complètes, irrégularité). Ces poids servent à calculer un score pour chaque configuration possible de la pièce.

Simulation des Mouvements :
Grâce à la méthode FindBestMove, l'IA explore toutes les rotations possibles et les translations horizontales pour la pièce actuelle. Pour chaque configuration, elle simule la chute de la pièce (via la méthode SimulateDrop) et évalue la position obtenue.

Sélection du Meilleur Mouvement :
En comparant les scores obtenus pour chaque configuration, l'IA détermine le mouvement (rotation et déplacement) qui maximise le score. Ce mouvement est encapsulé dans la structure PossibleMove.

Mise à Jour et Paramétrage :
La classe offre également la possibilité de mettre à jour les paramètres de l'IA via la méthode UpdateParameters, permettant d'ajuster dynamiquement le comportement de l'IA en fonction des préférences ou d'un apprentissage automatique.
	 
*/

/*	Explications des principes POO appliqués :

Encapsulation
Les champs privés (par exemple, currentParameters) et les méthodes privées (par exemple, CalculateAggregateHeight, IsValidPosition) cachent l’implémentation interne.
Les classes et structures (comme IAController, AIParameters, PossibleMove) regroupent les données et les comportements associés.

Abstraction
Les méthodes publiques comme FindBestMove offrent une interface simple pour interagir avec l’IA sans exposer les détails complexes du calcul et de la simulation.
La logique interne (calcul de la hauteur, des trous, etc.) est décomposée en méthodes privées pour simplifier la compréhension.

Composition
La classe IAController utilise une instance de AIParameters pour gérer ses paramètres, illustrant ainsi la composition (un objet est constitué d’autres objets).

Polymorphisme (potentiel)
La méthode Clone() sur l’objet Piece est utilisée sans connaître sa classe concrète, ce qui permettrait, dans un système complet, d’avoir différentes implémentations de pièces avec leur propre comportement (via héritage et substitution de méthodes).
*/

using System;
using System.Collections.Generic;
using System.Drawing;

namespace TetrisML
{
    // La classe IAController illustre plusieurs principes de la Programmation Orientée Objet (POO) :
    // - Encapsulation : Les données (champs) et comportements (méthodes) liés à l'IA sont regroupés dans cette classe.
    // - Abstraction : Les détails complexes (calcul du meilleur mouvement, évaluation de la grille, etc.) sont masqués derrière des méthodes bien définies.
    // - Composition : IAController utilise des objets d'autres classes (comme AIParameters) pour construire son comportement.
    // - Polymorphisme (potentiel) : Par exemple, la méthode Clone() sur l'objet Piece permet, dans une architecture plus complète, de traiter différentes pièces via une interface commune.
    public class IAController
    {
        // Encapsulation et Composition :
        // Le champ privé currentParameters regroupe les paramètres de l'IA et n'est accessible/modifiable que via des méthodes dédiées.
        private AIParameters currentParameters;

        public IAController()
        {
            currentParameters = new AIParameters();
        }

        // ====================================================
        // Classe imbriquée AIParameters
        // ====================================================
        // Encapsulation : Les paramètres de l'IA sont regroupés dans cette classe, qui expose des propriétés
        // pour interagir avec ces données sans exposer leur implémentation interne.
        public class AIParameters
        {
            public double HeightWeight { get; set; } = -0.510066;      // Pénalise la hauteur globale
            public double HolesWeight { get; set; } = -0.35663;        // Pénalise les trous
            public double CompleteLinesWeight { get; set; } = 0.760666; // Récompense les lignes complètes
            public double BumpinessWeight { get; set; } = -0.184483;   // Pénalise les différences de hauteur
        }

        // ====================================================
        // Structure PossibleMove
        // ====================================================
        // Abstraction : La structure PossibleMove encapsule les informations d'un mouvement possible
        // (rotation, translation, score) sans exposer les détails de son utilisation dans l'IA.
        public struct PossibleMove
        {
            public int Rotation { get; set; }     // Nombre de rotations à effectuer
            public int Translation { get; set; }   // Déplacement horizontal
            public double Score { get; set; }      // Score évalué pour ce mouvement
        }

        // ====================================================
        // Méthode principale pour trouver le meilleur mouvement
        // ====================================================
        // Abstraction : La méthode FindBestMove offre une interface simple pour déterminer le meilleur mouvement,
        // cachant la complexité de l'exploration des rotations et translations possibles.
        // Polymorphisme (potentiel) : La méthode Clone() utilisée sur currentPiece permet de travailler avec différentes
        // implémentations de Piece (si Piece est une classe de base dans une hiérarchie).
        public PossibleMove FindBestMove(int[,] grid, Piece currentPiece)
        {
            var bestMove = new PossibleMove { Score = double.MinValue };

            // Essayer toutes les rotations possibles
            for (int rotation = 0; rotation < 4; rotation++)
            {
                // Utilisation du polymorphisme : Clone permet de créer une copie de la pièce sans connaître sa classe concrète.
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

        // ====================================================
        // Méthode pour mettre à jour les paramètres de l'IA
        // ====================================================
        // Encapsulation : La modification de l'état interne (currentParameters) se fait via cette méthode,
        // garantissant ainsi un contrôle sur la manière dont les données sont modifiées.
        public void UpdateParameters(AIParameters newParams)
        {
            currentParameters = newParams;
        }

        // ====================================================
        // Méthode d'évaluation d'une position sur la grille
        // ====================================================
        // Abstraction : Les détails du calcul de l'évaluation sont cachés dans des méthodes privées (CalculateAggregateHeight, etc.).
        private double EvaluatePosition(int[,] grid)
        {
            return currentParameters.HeightWeight * CalculateAggregateHeight(grid)
                 + currentParameters.HolesWeight * CalculateHoles(grid)
                 + currentParameters.CompleteLinesWeight * CalculateCompleteLines(grid)
                 + currentParameters.BumpinessWeight * CalculateBumpiness(grid);
        }

        // ====================================================
        // Calcule la hauteur totale de toutes les colonnes
        // ====================================================
        // Abstraction : La méthode encapsule le calcul de la hauteur totale en masquant l'itération sur chaque colonne.
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

        // ====================================================
        // Calcule le nombre de trous (cases vides avec des blocs au-dessus)
        // ====================================================
        // Abstraction : Les détails de la détection des trous dans la grille sont cachés dans cette méthode.
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

        // ====================================================
        // Calcule le nombre de lignes complètes
        // ====================================================
        // Abstraction : La vérification et le comptage des lignes complètes sont encapsulés dans cette méthode.
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

        // ====================================================
        // Calcule l'irrégularité de la surface (bumpiness)
        // ====================================================
        // Abstraction : Le calcul des différences de hauteur entre colonnes est masqué dans cette méthode.
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

        // ====================================================
        // Obtient la hauteur d'une colonne spécifique
        // ====================================================
        // Abstraction : La méthode encapsule le processus de calcul de la hauteur d'une colonne, masquant les détails d'itération.
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

        // ====================================================
        // Simule la chute d'une pièce à une position donnée
        // ====================================================
        // Abstraction : La méthode SimulateDrop masque la complexité de la simulation d'une chute de pièce dans la grille.
        // Encapsulation : Elle travaille sur une copie de la grille afin de ne pas modifier l'originale.
        private int[,] SimulateDrop(int[,] originalGrid, Piece piece, int xPosition)
        {
            // Création d'une copie de la grille (encapsulation des données)
            int[,] simulatedGrid = (int[,])originalGrid.Clone();
            int height = simulatedGrid.GetLength(0);
            // Utilisation du polymorphisme potentiel via la méthode Clone de Piece
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

            // Placement de la pièce à la position finale trouvée
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

        // ====================================================
        // Vérifie si une position est valide pour une pièce
        // ====================================================
        // Abstraction : La méthode IsValidPosition cache les détails de la validation des positions d'une pièce dans la grille.
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
