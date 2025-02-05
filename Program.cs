/*	Bref
Ce code représente le point d'entrée dans une application Windows Forms en C#
*/

/*	Explications des principes POO appliqués :

Abstraction
La classe Program fournit un point d'entrée simple pour démarrer l'application, sans exposer les détails internes de l'initialisation des composants Windows Forms.

Encapsulation
La méthode Main centralise l'initialisation et le lancement de l'application, garantissant que les étapes nécessaires (activation des styles visuels, configuration du rendu de texte, et lancement du formulaire principal) sont correctement exécutées et isolées du reste de l'application.

Composition
L'appel à Application.Run(new GameForm()) illustre la composition, où le formulaire principal (GameForm) est composé de multiples composants et contrôles qui ensemble forment l'interface utilisateur du jeu.

*/

using System;
using System.Windows.Forms;

namespace TetrisML
{
    // La classe Program est déclarée en tant que classe statique.
    // Principes POO appliqués :
    // - Abstraction : Cette classe encapsule le point d'entrée de l'application et masque la complexité de l'initialisation de l'environnement Windows Forms.
    // - Encapsulation : La méthode Main regroupe les appels nécessaires pour démarrer l'application, protégeant ainsi la logique d'initialisation.
    static class Program
    {
        // L'attribut [STAThread] indique que le modèle de threading utilisé est à appartement unique (Single Thread Apartment),
        // ce qui est requis pour le bon fonctionnement des applications Windows Forms.
        [STAThread]
        static void Main()
        {
            // Active les styles visuels pour l'application,
            // ce qui permet aux contrôles Windows Forms d'adopter l'apparence moderne du système d'exploitation.
            Application.EnableVisualStyles();

            // Configure le rendu du texte pour l'application,
            // assurant ainsi la compatibilité avec la façon dont les contrôles affichent le texte.
            Application.SetCompatibleTextRenderingDefault(false);

            // Démarre l'application en créant et affichant le formulaire principal (GameForm).
            // Composition : Le GameForm est constitué de divers composants (contrôles, logiques de jeu, etc.) qui
            // gèrent l'interface utilisateur et la logique du jeu.
            Application.Run(new GameForm());
        }
    }
}
