# GEI1076_tetris
TetrisML (namespace)
│
├── Program (classe statique)
│     └── Main() : point d'entrée de l'application (lance GameForm)
│
├── IAController (classe)
│     ├── AIParameters (classe imbriquée)  
│     │      - Contient les paramètres ajustables de l'IA (poids de hauteur, trous, lignes complètes, irrégularité)
│     │
│     └── PossibleMove (struct imbriquée)  
│            - Contient le mouvement possible calculé par l'IA (rotation, translation, score)
│
├── ParameterForm (classe partielle, hérite de Form)
│     └── Permet à l'utilisateur d'ajuster dynamiquement les paramètres de l'IA via une interface graphique
│
├── Piece (classe)
│     ├── static TetrominoShapes (champ)  
│     │      - Définit les formes et rotations possibles pour chaque type de pièce
│     ├── static PieceColors (champ)  
│     │      - Associe une couleur à chaque type de pièce
│     ├── GetCurrentRotationBlocks()  
│     ├── Rotate(bool clockwise = true)  
│     ├── Clone()  
│     └── GetColor()
│
├── AIMove (struct)
│     └── Contient deux champs : HasValue (booléen) et Move (du type IAController.PossibleMove)
│         - Sert à encapsuler le résultat d'un calcul de mouvement par l'IA
│
├── TetrisGame (classe)
│     ├── Champs : grid, currentPiece, nextPiece, random, score, isGameOver, isAIEnabled, gameTimer, iaController, etc.
│     ├── Méthodes principales : InitializeGame(), GenerateNewPiece(), GameLoop(), ApplyAIMove(), MovePiece(), RotatePiece(), LockPiece(), ClearLines(), CheckGameOver(), EndGame(), etc.
│     └── Méthodes d'accès : GetGrid(), GetCurrentPiece(), GetNextPiece(), GetScore(), IsGameOver()
│
├── GameForm (classe partielle, hérite de Form)
│     ├── DoubleBufferedPanel (classe interne, dérivée de Panel)  
│     │      - Permet d'activer le double buffering pour un rendu graphique fluide
│     ├── Champs : _blockSize, _game (TetrisGame), _gameRefreshTimer, _gamePanel, _nextPiecePanel, _scoreLabel, _startButton, _aiButton, _parameterForm, etc.
│     ├── Méthodes de configuration : ConfigureInitialState(), ConfigureGameComponents(), ConfigureEventHandlers()
│     ├── Méthodes de rendu graphique : GamePanel_Paint(), NextPiecePanel_Paint(), DrawBlock()
│     ├── Gestionnaires d'événements : StartButton_Click(), AIButton_Click(), GameForm_KeyDown(), RefreshTimer_Tick()
│     └── Surcharge d'OnFormClosing() pour fermer également ParameterForm
│
└── Form1 (classe partielle, hérite de Form)
      └── Généralement générée par l'outil de conception Windows Forms (peut servir pour un autre formulaire ou à des fins de test)
