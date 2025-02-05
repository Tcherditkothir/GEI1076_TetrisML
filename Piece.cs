/*	Bref

Piece est le composant fondamental qui modélise une pièce de Tetris. Elle regroupe à la fois la structure (formes, rotations, couleur) 
et le comportement (rotation, clonage) nécessaires pour intégrer les pièces dans la logique du jeu, que ce soit pour le rendu graphique 
ou pour la manipulation lors des déplacements et des interactions avec la grille.

---
La classe Piece a pour rôle de représenter une pièce de Tetris dans le jeu. Voici ses points clés résumés :

Modélisation des Pièces :
Elle encapsule toutes les caractéristiques d'une pièce, telles que son type (déterminant sa forme et sa couleur), sa position (coordonnées X et Y) sur la grille, ainsi que sa rotation actuelle.

Définition des Formes et des Rotations :
La classe contient un tableau statique (TetrominoShapes) qui définit, pour chaque type de pièce, l'ensemble des configurations possibles (rotations) sous forme de listes de points. Cela permet de connaître la disposition des blocs de la pièce pour chaque rotation.

Gestion de la Couleur :
Un autre tableau statique (PieceColors) associe une couleur à chaque type de pièce, facilitant ainsi l'affichage graphique de la pièce dans le jeu.

Comportements Essentiels :

La méthode GetCurrentRotationBlocks() retourne la liste des points qui définissent la forme actuelle de la pièce en fonction de sa rotation.
La méthode Rotate() permet de faire pivoter la pièce, en ajustant la valeur de Rotation et en assurant que le mouvement respecte la logique des rotations définie.
La méthode Clone() offre la possibilité de créer une copie exacte de la pièce, ce qui est utile notamment lors de la simulation des mouvements sans modifier l'état de la pièce originale.

*/

/*	Explications des principes POO appliqués :

Encapsulation
Les propriétés de la pièce (Type, X, Y, Rotation) contrôlent l'accès à son état interne.
Les champs statiques TetrominoShapes et PieceColors sont privés ou en lecture seule, empêchant leur modification depuis l'extérieur.

Abstraction
Les méthodes telles que GetCurrentRotationBlocks, Rotate, Clone et GetColor offrent une interface claire pour interagir avec une pièce, sans nécessiter de connaître les détails de son implémentation interne.
Le constructeur initialise les valeurs de base, simplifiant ainsi la création d'une pièce.

Composition
La classe Piece utilise des structures complexes (List<List<Point>> pour les formes et Color[] pour les couleurs) pour représenter ses données.
La méthode Clone démontre la composition en créant une nouvelle instance qui reprend les propriétés de l'instance courante.

Ce code montre comment les principes fondamentaux de la POO permettent de concevoir une classe claire, modulable et facile à maintenir pour représenter une pièce dans un jeu de Tetris.
*/

using System;
using System.Collections.Generic;
using System.Drawing;

namespace TetrisML
{
    // La classe Piece représente une pièce de Tetris.
    // Principes POO appliqués :
    // - Encapsulation : Les propriétés et champs masquent l'implémentation interne et permettent un accès contrôlé aux données de la pièce.
    // - Abstraction : Les méthodes (Rotate, GetCurrentRotationBlocks, Clone, GetColor) fournissent une interface simple pour interagir avec l'objet sans exposer sa complexité interne.
    // - Composition : La pièce est définie à partir de structures de données (List<List<Point>> et Color[]) qui représentent ses formes et couleurs.
    public class Piece
    {
        // Propriétés de la pièce.
        // "Type" est en lecture seule depuis l'extérieur (private set) pour garantir l'intégrité de la donnée.
        public int Type { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Rotation { get; set; }

        // -----------------------------------------------------------
        // Définition des formes de pièces avec leurs rotations
        // -----------------------------------------------------------
        // Encapsulation et Abstraction : 
        // La structure des formes (TetrominoShapes) est privée et ne peut être modifiée que par la classe elle-même.
        // Chaque type de pièce possède une liste de rotations, et chaque rotation est représentée par une liste de points.
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

        // -----------------------------------------------------------
        // Couleurs associées à chaque type de pièce
        // -----------------------------------------------------------
        // Encapsulation : Les couleurs sont définies en tant que membre statique readonly, partagé par toutes les instances de Piece.
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

        // -----------------------------------------------------------
        // Constructeur
        // -----------------------------------------------------------
        // Abstraction : Le constructeur initialise la pièce avec un type donné et définit sa position et rotation initiales.
        public Piece(int type)
        {
            Type = type;
            X = 4;     // Position de départ au centre
            Y = 0;     // En haut de la grille
            Rotation = 0;
        }

        // -----------------------------------------------------------
        // Méthode GetCurrentRotationBlocks
        // -----------------------------------------------------------
        // Abstraction : Retourne une nouvelle liste de points représentant la configuration actuelle
        // de la pièce selon sa rotation. L'utilisateur n'a pas besoin de connaître la structure interne.
        public List<Point> GetCurrentRotationBlocks()
        {
            int rotationIndex = Rotation % TetrominoShapes[Type].Count;
            // Retourne une copie de la liste de points pour éviter les modifications externes.
            return new List<Point>(TetrominoShapes[Type][rotationIndex]);
        }

        // -----------------------------------------------------------
        // Méthode Rotate
        // -----------------------------------------------------------
        // Abstraction : Permet de faire pivoter la pièce dans le sens horaire ou antihoraire.
        // Le modulo assure que la rotation reste dans les limites du nombre de rotations disponibles.
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

        // -----------------------------------------------------------
        // Méthode Clone
        // -----------------------------------------------------------
        // Abstraction et Composition : Crée une copie de la pièce actuelle.
        // Cela permet de manipuler une copie sans affecter l'originale (utile pour la simulation).
        public Piece Clone()
        {
            return new Piece(Type)
            {
                X = this.X,
                Y = this.Y,
                Rotation = this.Rotation
            };
        }

        // -----------------------------------------------------------
        // Méthode GetColor
        // -----------------------------------------------------------
        // Abstraction : Retourne la couleur associée à la pièce selon son type.
        public Color GetColor()
        {
            return PieceColors[Type];
        }
    }
}
