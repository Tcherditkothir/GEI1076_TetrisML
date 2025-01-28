using System;
using System.Collections.Generic;
using System.Drawing;

namespace TetrisML
{
    public class Piece
    {
        public int Type { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Rotation { get; set; }

        // Définition des formes de pièces avec leurs rotations
        private static readonly List<List<Point>>[] TetrominoShapes = new List<List<Point>>[]
        {
            // I-Piece (cyan)
            new List<List<Point>>
            {
                new List<Point> { new Point(0,0), new Point(0,1), new Point(0,2), new Point(0,3) },
                new List<Point> { new Point(0,0), new Point(1,0), new Point(2,0), new Point(3,0) }
            },
            
            // O-Piece (jaune)
            new List<List<Point>>
            {
                new List<Point> { new Point(0,0), new Point(1,0), new Point(0,1), new Point(1,1) }
            },
            
            // T-Piece (violet)
            new List<List<Point>>
            {
                new List<Point> { new Point(1,0), new Point(0,1), new Point(1,1), new Point(2,1) },
                new List<Point> { new Point(1,0), new Point(1,1), new Point(2,1), new Point(1,2) },
                new List<Point> { new Point(0,1), new Point(1,1), new Point(2,1), new Point(1,2) },
                new List<Point> { new Point(1,0), new Point(0,1), new Point(1,1), new Point(1,2) }
            },

            // L-Piece (orange)
            new List<List<Point>>
            {
                new List<Point> { new Point(2,0), new Point(0,1), new Point(1,1), new Point(2,1) },
                new List<Point> { new Point(1,0), new Point(1,1), new Point(1,2), new Point(2,2) },
                new List<Point> { new Point(0,1), new Point(1,1), new Point(2,1), new Point(0,2) },
                new List<Point> { new Point(0,0), new Point(1,0), new Point(1,1), new Point(1,2) }
            },

            // J-Piece (bleu)
            new List<List<Point>>
            {
                new List<Point> { new Point(0,0), new Point(0,1), new Point(1,1), new Point(2,1) },
                new List<Point> { new Point(1,0), new Point(2,0), new Point(1,1), new Point(1,2) },
                new List<Point> { new Point(0,1), new Point(1,1), new Point(2,1), new Point(2,2) },
                new List<Point> { new Point(1,0), new Point(1,1), new Point(0,2), new Point(1,2) }
            },

            // S-Piece (vert)
            new List<List<Point>>
            {
                new List<Point> { new Point(1,0), new Point(2,0), new Point(0,1), new Point(1,1) },
                new List<Point> { new Point(1,0), new Point(1,1), new Point(2,1), new Point(2,2) }
            },

            // Z-Piece (rouge)
            new List<List<Point>>
            {
                new List<Point> { new Point(0,0), new Point(1,0), new Point(1,1), new Point(2,1) },
                new List<Point> { new Point(2,0), new Point(1,1), new Point(2,1), new Point(1,2) }
            }
        };

        // Couleurs associées à chaque type de pièce
        public static readonly Color[] PieceColors = new Color[]
        {
            Color.Cyan,     // I
            Color.Yellow,   // O
            Color.Purple,   // T
            Color.Orange,   // L
            Color.Blue,     // J
            Color.Green,    // S
            Color.Red       // Z
        };

        public Piece(int type)
        {
            Type = type;
            X = 4;     // Position de départ au centre
            Y = 0;     // En haut de la grille
            Rotation = 0;
        }

        public List<Point> GetCurrentRotationBlocks()
        {
            int rotationIndex = Rotation % TetrominoShapes[Type].Count;
            return new List<Point>(TetrominoShapes[Type][rotationIndex]);
        }

        public void Rotate(bool clockwise = true)
        {
            if (clockwise)
            {
                Rotation = (Rotation + 1) % TetrominoShapes[Type].Count;
            }
            else
            {
                Rotation = (Rotation - 1 + TetrominoShapes[Type].Count) % TetrominoShapes[Type].Count;
            }
        }

        public Piece Clone()
        {
            return new Piece(Type)
            {
                X = this.X,
                Y = this.Y,
                Rotation = this.Rotation
            };
        }

        public Color GetColor()
        {
            return PieceColors[Type];
        }
    }
}