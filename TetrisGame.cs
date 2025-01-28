using System;
using System.Windows.Forms;

namespace TetrisML
{
    public struct AIMove
    {
        public bool HasValue;
        public IAController.PossibleMove Move;
    }

    public class TetrisGame
    {
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
        private const int AI_MOVE_DELAY = 100;

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

        private void InitializeGame()
        {
            // Effacer la grille
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    grid[y, x] = 0;

            // Créer les premières pièces
            currentPiece = GenerateNewPiece();
            nextPiece = GenerateNewPiece();

            // Configurer le timer
            gameTimer = new Timer
            {
                Interval = 500 // 500ms entre chaque mise à jour
            };
            gameTimer.Tick += GameLoop;
        }

        private Piece GenerateNewPiece()
        {
            return new Piece(random.Next(7));
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (isGameOver) return;

            // Logique IA
            if (isAIEnabled && iaController != null)
            {
                if (!currentAIMove.HasValue)
                {
                    // Calculer un nouveau mouvement seulement si on n'en a pas
                    currentAIMove = new AIMove
                    {
                        HasValue = true,
                        Move = iaController.FindBestMove(grid, currentPiece)
                    };
                    lastAIMove = DateTime.Now;  // Réinitialiser le timer
                }
                else if ((DateTime.Now - lastAIMove).TotalMilliseconds >= AI_MOVE_DELAY)
                {
                    // Appliquer le mouvement existant
                    ApplyAIMove(currentAIMove.Move);
                    lastAIMove = DateTime.Now;
                }
            }

            // Gravité normale
            if (!MovePiece(0, 1))
            {
                LockPiece();
                ClearLines();
                SpawnNewPiece();
                // Forcer un nouveau calcul pour la nouvelle pièce
                currentAIMove = new AIMove { HasValue = false };
            }

            if (CheckGameOver())
            {
                EndGame();
            }
        }

        private bool ApplyAIMove(IAController.PossibleMove move)
        {
            if (currentPiece == null) return false;

            // D'abord, gérer la rotation
            if (currentPiece.Rotation != move.Rotation)
            {
                RotatePiece();
                return true;
            }

            // Ensuite, gérer le mouvement horizontal
            if (currentPiece.X != move.Translation)
            {
                int direction = Math.Sign(move.Translation - currentPiece.X);
                return MovePiece(direction, 0);
            }

            return false;
        }

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

        public IAController GetIAController()
        {
            return iaController;
        }

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

        public void RotatePiece()
        {
            currentPiece.Rotate();
            if (!IsValidPosition(currentPiece.X, currentPiece.Y, currentPiece.GetCurrentRotationBlocks()))
            {
                currentPiece.Rotate(false);
            }
        }

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

        private bool IsLineFull(int y)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[y, x] == 0)
                    return false;
            }
            return true;
        }

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

        private void SpawnNewPiece()
        {
            currentPiece = nextPiece;
            nextPiece = GenerateNewPiece();
        }

        private bool CheckGameOver()
        {
            return !IsValidPosition(currentPiece.X, currentPiece.Y, currentPiece.GetCurrentRotationBlocks());
        }

        private void EndGame()
        {
            isGameOver = true;
            gameTimer.Stop();
        }

        // Méthodes d'accès pour l'interface graphique
        public int[,] GetGrid() => grid;
        public Piece GetCurrentPiece() => currentPiece;
        public Piece GetNextPiece() => nextPiece;
        public int GetScore() => score;
        public bool IsGameOver() => isGameOver;
    }
}