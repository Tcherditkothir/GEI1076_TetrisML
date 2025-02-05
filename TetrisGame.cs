/*	Bref

TetrisGame est le cœur de la logique du jeu. Elle orchestre la création et le déplacement des pièces, la mise à jour de la grille, 
l'intégration de l'intelligence artificielle pour optimiser les mouvements, la gestion du score et la détection de la fin de partie. 
C'est ce composant qui encapsule les règles et le déroulement du jeu Tetris.

---
La classe TetrisGame centralise l'ensemble de la logique du jeu Tetris. Voici ses points clés résumés :

Gestion de la Grille et des Pièces :

Grille de jeu : Elle maintient une grille (matrice) qui représente l'état du jeu, c'est-à-dire les cases occupées par des pièces verrouillées.
Pièces Actuelle et Suivante : Elle gère la pièce en cours de jeu (celle qui se déplace) et prépare la prochaine pièce qui viendra remplacer la première une fois verrouillée.
Logique de Jeu :

Gravité et Déplacement : La classe simule la gravité en faisant descendre régulièrement la pièce actuelle.
Mouvements et Rotations : Elle fournit des méthodes pour déplacer la pièce horizontalement, la faire descendre, et la faire pivoter. Chaque mouvement est d'abord vérifié par rapport à la validité de la position sur la grille.
Verrouillage des Pièces : Lorsque la pièce ne peut plus descendre, elle est verrouillée dans la grille, ce qui permet ensuite de vérifier et de supprimer les lignes complètes.
Détection de la Fin de Partie : Elle évalue régulièrement si l'état de la grille ne permet plus d'insérer une nouvelle pièce, ce qui met fin à la partie.
Intégration de l'Intelligence Artificielle (IA) :

La classe intègre un IAController qui, lorsqu'il est activé, calcule et applique automatiquement le meilleur mouvement pour la pièce actuelle.
Elle gère la logique de synchronisation (via un timer) pour permettre à l'IA d'intervenir périodiquement, en parallèle de la gravité normale.
Mécanismes de Timing :

Un Timer est utilisé pour piloter la boucle de jeu (le GameLoop) qui met à jour l'état du jeu à intervalles réguliers (par exemple, pour faire descendre la pièce et gérer les interactions avec la grille).
Gestion du Score et de la Progression :

Chaque ligne complète effacée augmente le score, et la classe met à jour cette information au fur et à mesure du déroulement de la partie.

*/

/*	Explications des principes POO appliqués :

Encapsulation
Les données internes du jeu (grille, score, pièces, etc.) sont stockées dans des champs privés, ce qui protège l'état interne de modifications non contrôlées.

Abstraction
Les méthodes publiques et privées (comme MovePiece, RotatePiece, GameLoop, etc.) offrent une interface simple pour manipuler le jeu sans exposer les détails complexes de la logique de jeu.

Composition
La classe TetrisGame est composée d'autres objets essentiels tels que des instances de Piece, un Timer pour la boucle de jeu, un Random pour la génération de pièces et un IAController pour gérer l'intelligence artificielle.

Polymorphisme et Événements
Bien que la classe n'hérite pas directement d'autres classes (à part l'utilisation de Timer et d'événements), l'abonnement à l'événement Tick du timer et la gestion des actions via GameLoop illustrent le comportement polymorphique en réponse aux événements du système.

Ce code montre comment les principes fondamentaux de la POO permettent de structurer la logique d'un jeu de Tetris de manière modulaire, claire et maintenable.
*/

using System;
using System.Windows.Forms;

namespace TetrisML
{
    // -----------------------------------------------------------
    // Structure AIMove
    // -----------------------------------------------------------
    // Abstraction : La structure AIMove encapsule la notion d'un mouvement
    // calculé par l'IA, en indiquant s'il a une valeur et en stockant le mouvement.
	// note : AIMove est déclarée directement dans le namespace TetrisML, ce qui la rend accessible à tous les composants qui utilisent ce namespace. Cela contribue à organiser les types relatifs au jeu sans les forcer à être imbriqués dans d'autres classes.
	/* Pourquoi utiliser une Struct pour AIMove ?

		Simplicité et Performance :
		La struct AIMove contient simplement deux champs : un booléen (HasValue) pour indiquer si un mouvement a été calculé, et un champ (Move) qui stocke les informations détaillées du mouvement (rotation, translation et score).
		Ce sont des données légères, et en tant que type valeur, AIMove est rapide à créer, copier et manipuler, sans avoir besoin de l'overhead associé à l'allocation sur le tas d'une classe.
		Utilisation comme Conteneur de Données :
		La struct est idéale pour encapsuler des informations simples ou des "paquets" de données. Ici, AIMove joue le rôle d'un conteneur de résultat pour la meilleure action calculée par l'IA.
		On n'a pas besoin d'héritage ou de polymorphisme pour ce type de donnée, ce qui rend une struct parfaitement appropriée.
	
		La struct AIMove dans ce projet est utilisée pour encapsuler de manière efficace et légère le résultat d'une évaluation de mouvement par l'IA. Elle est déclarée directement dans le namespace parce qu'en C#, ce type de déclaration est courant pour regrouper des types de données liés au domaine fonctionnel (ici, le Tetris). Le choix d'une struct est judicieux car elle permet de manipuler rapidement des données simples sans la surcharge d'une classe, tout en s'inscrivant parfaitement dans l'organisation globale du code.
	*/
    public struct AIMove
    {
        public bool HasValue;
        public IAController.PossibleMove Move;
    }

    // -----------------------------------------------------------
    // Classe TetrisGame
    // -----------------------------------------------------------
    // La classe TetrisGame gère la logique globale du jeu Tetris.
    // Principes POO appliqués :
    // - Encapsulation : Les données internes (grille, pièces, score, etc.) sont stockées dans des champs privés.
    // - Abstraction : Les méthodes publiques et privées offrent une interface simple pour contrôler le jeu sans exposer les détails d'implémentation.
    // - Composition : TetrisGame est composé d'autres objets (Piece, Timer, IAController, Random) pour construire le comportement du jeu.
    public class TetrisGame
    {
        // -----------------------------------------------------------
        // Champs privés et constantes
        // -----------------------------------------------------------
        // Encapsulation : Les attributs privés contrôlent l'état interne du jeu.
        private readonly int[,] grid;
        private readonly int width = 10;
        private readonly int height = 20;
        private Piece currentPiece;
        private Piece nextPiece;
        private readonly Random random;
        private int score;
        private bool isGameOver;
        private bool isAIEnabled;
        private Timer gameTimer;
        private IAController iaController;
        private AIMove currentAIMove;
        private DateTime lastAIMove;
        private const int AI_MOVE_DELAY = 100; // Délai en millisecondes entre les mouvements IA

        // -----------------------------------------------------------
        // Constructeur
        // -----------------------------------------------------------
        // Abstraction et Composition : Le constructeur initialise les composants essentiels du jeu,
        // en créant la grille, le générateur aléatoire, l'IA, etc.
        public TetrisGame()
        {
            grid = new int[height, width];
            random = new Random();
            score = 0;
            isGameOver = false;
            isAIEnabled = false;
            iaController = new IAController();
            lastAIMove = DateTime.Now;
            currentAIMove = new AIMove { HasValue = false };
            InitializeGame();
        }

        // -----------------------------------------------------------
        // Méthode InitializeGame
        // -----------------------------------------------------------
        // Abstraction : Cette méthode encapsule l'initialisation de la grille, la création des premières pièces
        // et la configuration du timer de jeu.
        private void InitializeGame()
        {
            // Effacer la grille
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    grid[y, x] = 0;

            // Créer les premières pièces
            currentPiece = GenerateNewPiece();
            nextPiece = GenerateNewPiece();

            // Configurer le timer pour gérer la boucle de jeu
            gameTimer = new Timer
            {
                Interval = 500 // 500ms entre chaque mise à jour
            };
            gameTimer.Tick += GameLoop;
        }

        // -----------------------------------------------------------
        // Méthode GenerateNewPiece
        // -----------------------------------------------------------
        // Abstraction : Génère une nouvelle pièce aléatoirement à partir des 7 types possibles.
        private Piece GenerateNewPiece()
        {
            return new Piece(random.Next(7));
        }

        // -----------------------------------------------------------
        // Méthode GameLoop
        // -----------------------------------------------------------
        // Abstraction : La boucle de jeu principale qui se déclenche à chaque tick du timer.
        // Elle gère la logique de l'IA, la gravité, le verrouillage des pièces, la suppression des lignes et le contrôle de la fin du jeu.
        private void GameLoop(object sender, EventArgs e)
        {
            if (isGameOver) return;

            // -----------------------------------------------------------
            // Gestion de la logique de l'IA
            // -----------------------------------------------------------
            if (isAIEnabled && iaController != null)
            {
                if (!currentAIMove.HasValue)
                {
                    // Calculer un nouveau mouvement seulement si aucun mouvement n'est en cours.
                    currentAIMove = new AIMove
                    {
                        HasValue = true,
                        Move = iaController.FindBestMove(grid, currentPiece)
                    };
                    lastAIMove = DateTime.Now;  // Réinitialiser le timer IA
                }
                else if ((DateTime.Now - lastAIMove).TotalMilliseconds >= AI_MOVE_DELAY)
                {
                    // Appliquer le mouvement calculé par l'IA après un délai défini
                    ApplyAIMove(currentAIMove.Move);
                    lastAIMove = DateTime.Now;
                }
            }

            // -----------------------------------------------------------
            // Gravité normale : déplacer la pièce vers le bas
            // -----------------------------------------------------------
            if (!MovePiece(0, 1))
            {
                // Si la pièce ne peut plus descendre, verrouiller la pièce, supprimer les lignes complètes et générer une nouvelle pièce
                LockPiece();
                ClearLines();
                SpawnNewPiece();
                // Réinitialiser le mouvement IA pour la nouvelle pièce
                currentAIMove = new AIMove { HasValue = false };
            }

            // Vérifier si la partie est terminée
            if (CheckGameOver())
            {
                EndGame();
            }
        }

        // -----------------------------------------------------------
        // Méthode ApplyAIMove
        // -----------------------------------------------------------
        // Abstraction : Applique le mouvement calculé par l'IA en gérant d'abord la rotation,
        // puis le déplacement horizontal de la pièce.
        private bool ApplyAIMove(IAController.PossibleMove move)
        {
            if (currentPiece == null) return false;

            // Gérer la rotation : si la rotation actuelle diffère de celle désirée, effectuer une rotation
            if (currentPiece.Rotation != move.Rotation)
            {
                RotatePiece();
                return true;
            }

            // Gérer le déplacement horizontal : ajuster la position X de la pièce pour atteindre la translation souhaitée
            if (currentPiece.X != move.Translation)
            {
                int direction = Math.Sign(move.Translation - currentPiece.X);
                return MovePiece(direction, 0);
            }

            return false;
        }

        // -----------------------------------------------------------
        // Méthodes de contrôle du jeu
        // -----------------------------------------------------------
        // Abstraction : Ces méthodes permettent de démarrer, mettre en pause et basculer le mode IA du jeu.
        public void StartGame()
        {
            if (!isGameOver)
                gameTimer.Start();
        }

        public void PauseGame()
        {
            gameTimer.Stop();
        }

        public void ToggleAI()
        {
            isAIEnabled = !isAIEnabled;
            currentAIMove = new AIMove { HasValue = false };
            lastAIMove = DateTime.Now;
        }

        // Méthode d'accès à l'IA pour d'autres composants (par exemple, pour mettre à jour les paramètres)
        public IAController GetIAController()
        {
            return iaController;
        }

        // -----------------------------------------------------------
        // Méthode MovePiece
        // -----------------------------------------------------------
        // Abstraction : Déplace la pièce actuelle de (deltaX, deltaY) si la nouvelle position est valide.
        // Encapsulation : La vérification de la validité de la position se fait via IsValidPosition.
        public bool MovePiece(int deltaX, int deltaY)
        {
            int newX = currentPiece.X + deltaX;
            int newY = currentPiece.Y + deltaY;

            if (IsValidPosition(newX, newY, currentPiece.GetCurrentRotationBlocks()))
            {
                currentPiece.X = newX;
                currentPiece.Y = newY;
                return true;
            }
            return false;
        }

        // -----------------------------------------------------------
        // Méthode RotatePiece
        // -----------------------------------------------------------
        // Abstraction : Fait pivoter la pièce actuelle et annule la rotation si la nouvelle position n'est pas valide.
        public void RotatePiece()
        {
            currentPiece.Rotate();
            if (!IsValidPosition(currentPiece.X, currentPiece.Y, currentPiece.GetCurrentRotationBlocks()))
            {
                // Si la rotation entraîne une position invalide, faire pivoter dans l'autre sens pour revenir à la configuration précédente.
                currentPiece.Rotate(false);
            }
        }

        // -----------------------------------------------------------
        // Méthode IsValidPosition
        // -----------------------------------------------------------
        // Abstraction : Vérifie que pour chaque bloc de la pièce, la position calculée est valide (dans la grille et non occupée).
        private bool IsValidPosition(int x, int y, System.Collections.Generic.List<System.Drawing.Point> blocks)
        {
            foreach (var block in blocks)
            {
                int blockX = x + block.X;
                int blockY = y + block.Y;

                if (blockX < 0 || blockX >= width || blockY >= height)
                    return false;

                if (blockY >= 0 && grid[blockY, blockX] != 0)
                    return false;
            }
            return true;
        }

        // -----------------------------------------------------------
        // Méthode LockPiece
        // -----------------------------------------------------------
        // Abstraction : Verrouille la pièce actuelle en marquant ses blocs dans la grille,
        // ce qui empêche la pièce de bouger et la rend permanente.
        private void LockPiece()
        {
            foreach (var block in currentPiece.GetCurrentRotationBlocks())
            {
                int blockX = currentPiece.X + block.X;
                int blockY = currentPiece.Y + block.Y;

                if (blockY >= 0 && blockY < height && blockX >= 0 && blockX < width)
                {
                    grid[blockY, blockX] = currentPiece.Type + 1;
                }
            }
        }

        // -----------------------------------------------------------
        // Méthode ClearLines
        // -----------------------------------------------------------
        // Abstraction : Parcourt la grille pour détecter et effacer les lignes complètes,
        // en mettant à jour le score en conséquence.
        private void ClearLines()
        {
            for (int y = height - 1; y >= 0; y--)
            {
                if (IsLineFull(y))
                {
                    ClearLine(y);
                    score += 100;
                }
            }
        }

        // -----------------------------------------------------------
        // Méthode IsLineFull
        // -----------------------------------------------------------
        // Abstraction : Vérifie si une ligne est complète (aucune case vide).
        private bool IsLineFull(int y)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[y, x] == 0)
                    return false;
            }
            return true;
        }

        // -----------------------------------------------------------
        // Méthode ClearLine
        // -----------------------------------------------------------
        // Abstraction : Efface une ligne donnée en décalant toutes les lignes situées au-dessus.
        private void ClearLine(int y)
        {
            for (int yy = y; yy > 0; yy--)
            {
                for (int x = 0; x < width; x++)
                {
                    grid[yy, x] = grid[yy - 1, x];
                }
            }
        }

        // -----------------------------------------------------------
        // Méthode SpawnNewPiece
        // -----------------------------------------------------------
        // Abstraction : Met à jour la pièce actuelle en utilisant la pièce suivante,
        // puis génère une nouvelle pièce pour venir remplacer celle qui vient d'être utilisée.
        private void SpawnNewPiece()
        {
            currentPiece = nextPiece;
            nextPiece = GenerateNewPiece();
        }

        // -----------------------------------------------------------
        // Méthode CheckGameOver
        // -----------------------------------------------------------
        // Abstraction : Vérifie si la position actuelle de la pièce est invalide,
        // ce qui signifie que le jeu est terminé.
        private bool CheckGameOver()
        {
            return !IsValidPosition(currentPiece.X, currentPiece.Y, currentPiece.GetCurrentRotationBlocks());
        }

        // -----------------------------------------------------------
        // Méthode EndGame
        // -----------------------------------------------------------
        // Abstraction : Termine le jeu en arrêtant le timer et en marquant l'état de fin de partie.
        private void EndGame()
        {
            isGameOver = true;
            gameTimer.Stop();
        }

        // -----------------------------------------------------------
        // Méthodes d'accès pour l'interface graphique
        // -----------------------------------------------------------
        // Abstraction : Ces méthodes fournissent une interface simple pour accéder à l'état du jeu (grille, pièces, score, etc.)
        public int[,] GetGrid() => grid;
        public Piece GetCurrentPiece() => currentPiece;
        public Piece GetNextPiece() => nextPiece;
        public int GetScore() => score;
        public bool IsGameOver() => isGameOver;
    }
}
