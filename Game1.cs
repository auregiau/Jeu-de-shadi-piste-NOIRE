using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace JeuDeShadi
{
    public class Game1 : Game
    {
        // IMPORTANT POUR LA PISTE BLEUE :
        // Vous n'avez pas besoin de comprendre la classe Game1 ni la programmation orientée objet.
        // Le cadre MonoGame est fourni. Concentrez-vous uniquement sur la zone élève.

        // Valeurs utilisées dans le tableau plateauDeJeu.
        // Ces constantes sont fournies.
        const int VIDE = 0;
        const int BLANC = 1;
        const int NOIR = 2;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // --- Configuration visuelle ---
        private const int TailleGrille = 10;
        private const int TailleCase = 64;
        private const int MargeFenetre = 40;
        private const int LargeurPanneauAide = 340;

        // CODE FOURNI - PISTE NOIRE : profondeur de recherche de Minimax.
        // Une profondeur de 4 donne déjà un adversaire raisonnable sans ralentir excessivement le jeu.
        private const int ProfondeurMinimax = 4;

        private Texture2D _pixel;      // texture 1x1 réutilisée pour tout dessiner (rectangles, cercles)
        private Texture2D _texturePion; // cercle plein généré procéduralement
        private Texture2D _textureAnneau; // anneau pour les surlignages

        // --- Données principales du jeu ---
        // Ces variables sont fournies. Vous allez les UTILISER, pas les recréer.
        // plateauDeJeu[x, y] : x = colonne, y = ligne. (0,0) = en haut à gauche.
        int[,] plateauDeJeu = new int[TailleGrille, TailleGrille];

        int joueurCourant = BLANC;

        // Variable interne fournie : elle mémorise les coups possibles.
        // Vous n'avez pas à la manipuler directement dans la piste bleue.
        List<Coup> coupsLegaux = new List<Coup>();

        // Sélection en cours (pion cliqué en attente d'une destination)
        private bool _unPionEstSelectionne = false;
        private int _pionSelectionneX = -1;
        private int _pionSelectionneY = -1;
        private List<Coup> _destinationsPossiblesPourSelection = new List<Coup>();

        private int _finDePartie = 0; // 0 = en cours, 1 = blanc gagne, 2 = noir gagne, -1 = match nul / blocage

        private MouseState _etatSourisPrecedent;
        private KeyboardState _etatClavierPrecedent;

        // Outils de vérification fournis pour la piste bleue.
        // Ils permettent de tester visuellement les étapes sans connaître MonoGame.
        private bool _afficherTestEtape4 = false;
        private List<Point> _casesTrouveesPourTest = new List<Point>();
        private string[] _resultatTest = { "AUCUN TEST LANCE" };

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            int largeur = MargeFenetre * 2 + TailleGrille * TailleCase + LargeurPanneauAide;
            int hauteur = MargeFenetre * 2 + TailleGrille * TailleCase;
            _graphics.PreferredBackBufferWidth = largeur;
            _graphics.PreferredBackBufferHeight = hauteur;
            _graphics.ApplyChanges();

            InitialiserPlateau();
            RecalculerCoupsLegaux();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Texture 1x1 blanche, sert de base à tous les dessins (rectangles colorés via tint)
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            _texturePion = CreerTextureCercle(TailleCase - 16, plein: true);
            _textureAnneau = CreerTextureCercle(TailleCase - 8, plein: false);

        }

        /// <summary>
        /// Génère procéduralement une texture circulaire (cercle plein ou anneau)
        /// pour ne dépendre d'aucune image externe.
        /// </summary>
        private Texture2D CreerTextureCercle(int diametre, bool plein)
        {
            var texture = new Texture2D(GraphicsDevice, diametre, diametre);
            var donnees = new Color[diametre * diametre];
            float rayon = diametre / 2f;
            Vector2 centre = new Vector2(rayon, rayon);
            float epaisseurAnneau = 4f;

            for (int y = 0; y < diametre; y++)
            {
                for (int x = 0; x < diametre; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), centre);
                    bool dansLeCercle = distance <= rayon;
                    bool dansLAnneau = dansLeCercle && distance >= rayon - epaisseurAnneau;

                    bool visible = plein ? dansLeCercle : dansLAnneau;
                    donnees[y * diametre + x] = visible ? Color.White : Color.Transparent;
                }
            }

            texture.SetData(donnees);
            return texture;
        }

        // =====================================================================
        // ZONE ÉLÈVE — PISTE NOIRE
        // =====================================================================
        // La piste noire reprend le niveau d'autonomie de la piste rouge :
        // les méthodes à réaliser sont décrites, mais aucun pseudo-code n'est fourni.
        //
        // Vous devez utiliser principalement les notions déjà étudiées :
        // variables, conditions, boucles, tableaux et méthodes/fonctions.
        //
        // En plus du jeu complet, vous devez réaliser la fonction d'évaluation
        // utilisée par l'intelligence artificielle des Noirs.
        // Le moteur Minimax est fourni plus bas et ne doit pas être modifié.
        // =====================================================================

        /// <summary>
        /// Initialise le plateau de jeu.
        ///
        /// Règles à respecter :
        /// - le plateau contient 10 lignes et 10 colonnes ;
        /// - seules les cases où (x + y) est impair sont jouables ;
        /// - les Noirs occupent les lignes 0 à 3 ;
        /// - les Blancs occupent les lignes 6 à 9 ;
        /// - toutes les autres cases sont vides.
        /// </summary>
        void InitialiserPlateau()
        {
            plateauDeJeu = new int[TailleGrille, TailleGrille];

            // TODO ÉLÈVE : initialiser le plateau et placer les pions.

            //loop dans les colonnes du tableau
            for (int i = 0; i < TailleGrille; i++)
            {
                //puis loop dans les 4 premières lignes du tableau
                for (int j = 0; j < 4; j++)
                {
                    //vérification si l'addition des coordonées est impair
                    if ((i + j) % 2 == 0)
                    {
                        //si oui, le pion sur cette case sera noir
                        plateauDeJeu[i, j] = NOIR;
                    }
                }

                //ensuite loop dans les 4 dernières lignes du tableau
                for (int j = TailleGrille - 4; j < TailleGrille; j++)
                {
                    //vérification comme plus haut que l'addition des coordonées est impaire
                    if ((i + j) % 2 == 0)
                    {
                        //si tel est le cas, le pion sera blanc
                        plateauDeJeu[i, j] = BLANC;
                    }
                }
            }

            // CODE FOURNI : remet l'état général de la partie à zéro.
            ReinitialiserEtatPartie();
        }

        /// <summary>
        /// Retourne la direction verticale dans laquelle un pion avance normalement.
        ///
        /// Un pion blanc avance vers le haut du tableau.
        /// Un pion noir avance vers le bas du tableau.
        /// </summary>
        int DirectionAvant(int couleur)
        {
            // TODO ÉLÈVE

            //si la case contient un pion noir, la direction est 1
            if (couleur == NOIR)
            {
                return 1;
            }
            //si la case contient un pion blanc, la direction est -1
            else if (couleur == BLANC)
            {
                return -1;
            }
            //si la case est vide, la direction est 0
            return 0;
        }

        /// <summary>
        /// Retourne true si les coordonnées (x, y) correspondent à une case
        /// existante du plateau, sinon false.
        /// </summary>
        bool DansLesLimites(int x, int y)
        {
            // TODO ÉLÈVE

            //vérification que X et y ne sont pas plus petit que 0, et plus petits que la taille de la grille
            if (x >= 0 && x < TailleGrille && y >= 0 && y < TailleGrille)
            {
                return true;
            }
            //si tel n'est pas le cas, le coup ne se trouve pas dans les limites
            return false;
        }

        /// <summary>
        /// Recherche tous les coups légaux du joueur courant.
        ///
        /// Vous devez parcourir le plateau et appeler GenererCoupsPourPion(x, y)
        /// pour chaque pion appartenant au joueur courant.
        ///
        /// CommencerRechercheCoups() et TerminerRechercheCoups() sont fournies
        /// et doivent rester au début et à la fin de cette méthode.
        ///
        /// MarquerPionTrouvePourTest(x, y) peut être appelée lorsqu'un pion du
        /// joueur courant est trouvé afin de conserver le test visuel F4.
        /// </summary>
        void RecalculerCoupsLegaux()
        {
            CommencerRechercheCoups();

            // TODO ÉLÈVE : rechercher les pions du joueur courant et générer leurs coups.

            //premier loop dans les lignes du tableau
            for (int i = 0; i < TailleGrille; i++)
            {
                //puis deuxième loop dans les colonnes, donc pour regarder chaque case
                for (int j = 0; j < TailleGrille; j++)
                {
                    //si le pion trouvé sur cette case == joueur courant
                    if (plateauDeJeu[i, j] == joueurCourant)
                    {
                        //alors on appelle les deux fonctions en mettant comme paramètres les coordonées de cette case
                        MarquerPionTrouvePourTest(i, j);
                        GenererCoupsPourPion(i, j);
                    }
                }
            }

            TerminerRechercheCoups();
        }

        /// <summary>
        /// Recherche tous les coups possibles pour le pion situé en (x, y).
        ///
        /// Règles à respecter :
        /// - un déplacement simple se fait d'une case en diagonale vers l'avant ;
        /// - la case d'arrivée doit se trouver dans le plateau et être vide ;
        /// - une prise consiste à sauter en diagonale par-dessus un pion adverse ;
        /// - une prise peut être effectuée vers l'avant ou vers l'arrière ;
        /// - la case située derrière le pion adverse doit être libre.
        ///
        /// Lorsqu'un déplacement est trouvé, utilisez :
        /// AjouterDeplacement(xDepart, yDepart, xArrivee, yArrivee);
        ///
        /// Lorsqu'une prise est trouvée, utilisez :
        /// AjouterPrise(xDepart, yDepart, xArrivee, yArrivee);
        ///
        /// La règle de prise obligatoire est ensuite gérée par le code fourni.
        /// </summary>
        void GenererCoupsPourPion(int x, int y)
        {
            // TODO ÉLÈVE : rechercher les déplacements simples et les prises.

            int couleurPion = plateauDeJeu[x, y];
            int avant = DirectionAvant(couleurPion);

            int[] decalagesX = { -1, 1 };

            //boucle de 0 à 1, afin de ne pas répéter la fonction avec une seule différence
            for (int i = 0; i <= 1; i++)
            {
                //vérification que le coup proposé se trouve dans les limites
                if (DansLesLimites(x + decalagesX[i], y + avant))
                {
                    //puis vérification que la case d'arrivée du coup proposé est vide
                    if (plateauDeJeu[x + decalagesX[i], y + avant] == 0)
                    {
                        //si tel est le cas, le coup est ajouté aux déplacements possibles
                        AjouterDeplacement(x, y, x + decalagesX[i], y + avant);
                    }
                }
            }

            // Pour une prise, le pion peut sauter vers l'avant ou vers l'arrière.
            int[] directionsY = { avant, -avant };

            //appel de la fonction de vérification de prises avec toutes les directions de déplacement possibles
            for (int i = 1; i <= 4; i++)
            {
                VerificationPrise(x, y, decalagesX[i % 3 % 2], directionsY[i % 2], couleurPion);
            }
        }

        //méthode permettant de vérifier le cas d'une prise
        //paramètres:
        //positionX = position x actuelle
        //positionY = position Y actuelle
        //decalageX = direction du décalage X
        //decalageY = direction du décalage Y
        //couleurPion = couleur du pion actuel
        //retours : -
        void VerificationPrise(int positionX, int positionY, int decalageX, int directionY, int couleurPion)
        {
            //si le coup dans 2x le déplacement est dans les limites
            if (DansLesLimites(positionX + (decalageX * 2), positionY + (directionY * 2)))
            {
                //et que la case adjacente à la case de départ n'est pas vide, et que le pion sur cette case est un adversiare 
                if (plateauDeJeu[positionX + decalageX, positionY + directionY] != 0 && EstUnAdversaire(positionX + decalageX, positionY + directionY, couleurPion) && plateauDeJeu[positionX + (decalageX * 2), positionY + (directionY * 2)] == 0)
                {
                    //alors on ajoute une prise sur la case dans deux fois le déplacement usuel de 1 case
                    AjouterPrise(positionX, positionY, positionX + (decalageX * 2), positionY + (directionY * 2));
                }
            }
        }

        /// <summary>
        /// Applique au plateau le coup choisi par le joueur ou par l'IA.
        ///
        /// La méthode reçoit :
        /// - la position de départ ;
        /// - la position d'arrivée ;
        /// - un booléen indiquant si le coup est une prise.
        ///
        /// Vous devez :
        /// - déplacer le pion ;
        /// - supprimer le pion capturé lorsqu'il s'agit d'une prise ;
        /// - détecter une éventuelle victoire ;
        /// - sinon terminer le tour.
        ///
        /// Méthodes fournies :
        /// DeclarerVictoire(couleurPion);
        /// TerminerTour(couleurPion);
        /// </summary>
        void AppliquerCoup(
            int xDepart,
            int yDepart,
            int xArrivee,
            int yArrivee,
            bool estUnePrise)
        {
            // TODO ÉLÈVE

            //le pion sur la case d'arrivée est le même que celui de la case de départ
            plateauDeJeu[xArrivee, yArrivee] = plateauDeJeu[xDepart, yDepart];

            //la case de départ est ensuite vidée
            plateauDeJeu[xDepart, yDepart] = VIDE;

            //si le coup est une prise
            if (estUnePrise)
            {
                //la case entre la case de départ et celle d'arrivée est vidée
                plateauDeJeu[(xDepart + xArrivee) / 2, (yDepart + yArrivee) / 2] = VIDE;
            }

            //si le pion se trouve sur un des deux extremités haut et bas du plateau, alors le joueur de ce pion est vainqueur de la partie
            if (yArrivee == 0 || yArrivee == TailleGrille - 1)
            {
                //déclaration d'une victoire avec le pion gagnant
                DeclarerVictoire(plateauDeJeu[xArrivee, yArrivee]);
            }
            else
            {
                //sinon le tour est terminé, et le joueur du prochain tour sera l'opposé du pion venant de jouer
                TerminerTour(plateauDeJeu[xArrivee, yArrivee]);
            }
        }

        /// <summary>
        /// PISTE NOIRE - Évalue une position pour l'intelligence artificielle.
        ///
        /// Le moteur Minimax est entièrement fourni. Vous n'avez pas à programmer
        /// la récursivité ni l'exploration des coups futurs.
        ///
        /// Cette méthode doit retourner un nombre entier représentant la qualité
        /// de la position du point de vue de l'IA noire :
        ///
        ///     score positif    = avantage aux NOIRS ;
        ///     score négatif    = avantage aux BLANCS ;
        ///     score proche de 0 = position équilibrée.
        ///
        /// Vous choisissez vous-même les critères utilisés pour calculer le score.
        /// Vous devez pouvoir expliquer et justifier ces choix.
        ///
        /// Exemples de données que vous pouvez observer dans positionDuJeu :
        /// - le nombre de pions de chaque couleur ;
        /// - la position et l'avancement des pions.
        ///
        /// Pour aller plus loin, deux méthodes d'aide sont fournies :
        /// CompterCoupsPossibles(positionDuJeu, couleur)
        /// CompterPrisesPossibles(positionDuJeu, couleur)
        ///
        /// Les situations de victoire et de défaite sont déjà gérées par Minimax.
        /// </summary>
        int EvaluerPosition(int[,] positionDuJeu)
        {
            int score = 0;
            // TODO ÉLÈVE - PISTE NOIRE : calculer et retourner le score de la position.

            return score;
        }

        // =====================================================================
        // FIN DE LA ZONE ÉLÈVE PRINCIPALE
        // =====================================================================


        // =====================================================================
        // MÉTHODES D'AIDE FOURNIES — NE PAS MODIFIER
        // =====================================================================
        // Ces méthodes cachent les notions qui ne font pas partie des objectifs
        // de la piste bleue : listes, struct, construction d'objets et détails
        // internes du moteur graphique.
        // =====================================================================

        private List<Coup> _deplacementsTrouves = new List<Coup>();
        private List<Coup> _prisesTrouvees = new List<Coup>();

        private int CouleurAdverse(int couleur)
        {
            if (couleur == BLANC)
            {
                return NOIR;
            }

            return BLANC;
        }

        private void ReinitialiserEtatPartie()
        {
            joueurCourant = BLANC;
            _finDePartie = 0;
            _unPionEstSelectionne = false;
            _destinationsPossiblesPourSelection.Clear();
        }

        private void CommencerRechercheCoups()
        {
            _deplacementsTrouves.Clear();
            _prisesTrouvees.Clear();
            _casesTrouveesPourTest.Clear();
        }

        // À appeler dans l'étape 4 lorsque vous avez trouvé un pion du joueur courant.
        // Cette méthode ne change pas les règles du jeu. Elle sert uniquement à afficher
        // un surlignage lorsque le mode de test de l'étape 4 est activé avec F4.
        private void MarquerPionTrouvePourTest(int x, int y)
        {
            if (_afficherTestEtape4)
            {
                _casesTrouveesPourTest.Add(new Point(x, y));
            }
        }

        // Test automatique fourni pour l'étape 2.
        private void TesterDirectionAvant()
        {
            bool blancCorrect = DirectionAvant(BLANC) == -1;
            bool noirCorrect = DirectionAvant(NOIR) == 1;

            _resultatTest = new[]
            {
                "TEST ETAPE 2",
                "BLANC : " + DirectionAvant(BLANC) + "  ATTENDU : -1",
                "NOIR  : " + DirectionAvant(NOIR) + "  ATTENDU : +1",
                blancCorrect && noirCorrect ? "RESULTAT : OK" : "RESULTAT : A CORRIGER"
            };
        }

        // Test automatique fourni pour l'étape 3.
        private void TesterDansLesLimites()
        {
            bool test1 = DansLesLimites(0, 0);
            bool test2 = DansLesLimites(9, 9);
            bool test3 = !DansLesLimites(-1, 3);
            bool test4 = !DansLesLimites(10, 4);

            _resultatTest = new[]
            {
                "TEST ETAPE 3",
                "(0,0)   ATTENDU VRAI : " + (test1 ? "OK" : "ERREUR"),
                "(9,9)   ATTENDU VRAI : " + (test2 ? "OK" : "ERREUR"),
                "(-1,3)  ATTENDU FAUX : " + (test3 ? "OK" : "ERREUR"),
                "(10,4)  ATTENDU FAUX : " + (test4 ? "OK" : "ERREUR"),
                test1 && test2 && test3 && test4 ? "RESULTAT : OK" : "RESULTAT : A CORRIGER"
            };
        }

        private void AjouterDeplacement(int xDepart, int yDepart, int xArrivee, int yArrivee)
        {
            _deplacementsTrouves.Add(
                new Coup(xDepart, yDepart, xArrivee, yArrivee, false));
        }

        private void AjouterPrise(int xDepart, int yDepart, int xArrivee, int yArrivee)
        {
            _prisesTrouvees.Add(
                new Coup(xDepart, yDepart, xArrivee, yArrivee, true));
        }

        private void TerminerRechercheCoups()
        {
            if (_prisesTrouvees.Count > 0)
            {
                coupsLegaux = new List<Coup>(_prisesTrouvees);
            }
            else
            {
                coupsLegaux = new List<Coup>(_deplacementsTrouves);
            }
        }

        private bool EstUnAdversaire(int x, int y, int couleur)
        {
            if (!DansLesLimites(x, y))
            {
                return false;
            }

            if (plateauDeJeu[x, y] == VIDE)
            {
                return false;
            }

            return plateauDeJeu[x, y] != couleur;
        }

        private void DeclarerVictoire(int couleur)
        {
            _finDePartie = couleur;
        }

        private void TerminerTour(int couleurQuiVientDeJouer)
        {
            joueurCourant = CouleurAdverse(couleurQuiVientDeJouer);
            RecalculerCoupsLegaux();

            if (coupsLegaux.Count == 0)
            {
                _finDePartie = CouleurAdverse(joueurCourant);
            }
        }

        // Situation fournie pour tester les étapes 5B et 6.
        // F5 place un pion blanc pouvant capturer un pion noir.
        private void ChargerSituationTestPrise()
        {
            plateauDeJeu = new int[TailleGrille, TailleGrille];
            plateauDeJeu[3, 6] = BLANC;
            plateauDeJeu[4, 5] = NOIR;
            joueurCourant = BLANC;
            _finDePartie = 0;
            _unPionEstSelectionne = false;
            _destinationsPossiblesPourSelection.Clear();
            _afficherTestEtape4 = false;
            RecalculerCoupsLegaux();
            _resultatTest = new[]
            {
                "TEST PRISE",
                "CLIQUEZ LE PION BLANC",
                "EN (3,6)",
                "LA PRISE DOIT ALLER",
                "VERS (5,4)"
            };
        }

        // Adaptateur entre la partie graphique (qui utilise Coup) et la zone élève
        // (qui ne travaille qu'avec des int et un bool).
        private void JouerCoup(Coup coup)
        {
            AppliquerCoup(
                coup.XDepart,
                coup.YDepart,
                coup.XArrivee,
                coup.YArrivee,
                coup.EstUnePrise);
        }

        // =====================================================================
        // CODE FOURNI - PISTE NOIRE : MOTEUR MINIMAX
        // =====================================================================
        // Cette partie est volontairement fournie.
        // Elle utilise notamment la récursivité, les listes et la simulation
        // de positions. Ces notions ne font pas partie du travail demandé.
        //
        // Le moteur appelle votre méthode EvaluerPosition(...) lorsqu'il atteint
        // la profondeur maximale de recherche.
        // =====================================================================

        private void JouerTourIA()
        {
            if (_finDePartie != 0 || joueurCourant != NOIR)
            {
                return;
            }

            List<Coup> coupsPossibles = GenererCoupsPourPosition(plateauDeJeu, NOIR);

            if (coupsPossibles.Count == 0)
            {
                _finDePartie = BLANC;
                return;
            }

            Coup meilleurCoup = coupsPossibles[0];
            int meilleurScore = int.MinValue;

            foreach (Coup coup in coupsPossibles)
            {
                int[,] positionSimulee = CopierPlateau(plateauDeJeu);
                AppliquerCoupSurPosition(positionSimulee, coup);

                int score;

                if (coup.YArrivee == TailleGrille - 1)
                {
                    // Le coup fait immédiatement gagner les Noirs.
                    score = 100000;
                }
                else
                {
                    // Après le coup noir, on suppose que les Blancs jouent au mieux.
                    score = Minimax(positionSimulee, BLANC, ProfondeurMinimax - 1);
                }

                if (score > meilleurScore)
                {
                    meilleurScore = score;
                    meilleurCoup = coup;
                }
            }

            // On utilise la même méthode que pour un coup humain afin de conserver
            // une seule logique pour modifier le vrai plateau.
            JouerCoup(meilleurCoup);

            _resultatTest = new[]
            {
                "IA NOIRE : COUP JOUE",
                "SCORE CHOISI : " + meilleurScore,
                "A VOUS DE JOUER"
            };
        }

        private int Minimax(int[,] positionDuJeu, int couleurQuiJoue, int profondeur)
        {
            List<Coup> coupsPossibles = GenererCoupsPourPosition(positionDuJeu, couleurQuiJoue);

            // Un joueur sans coup possible perd la partie.
            if (coupsPossibles.Count == 0)
            {
                if (couleurQuiJoue == NOIR)
                {
                    return -100000 - profondeur;
                }

                return 100000 + profondeur;
            }

            // À la profondeur maximale, Minimax demande à l'élève d'évaluer
            // la position actuelle.
            if (profondeur == 0)
            {
                return EvaluerPosition(positionDuJeu);
            }

            if (couleurQuiJoue == NOIR)
            {
                // Les Noirs sont l'IA : ils cherchent le score le plus grand.
                int meilleurScore = int.MinValue;

                foreach (Coup coup in coupsPossibles)
                {
                    int[,] nouvellePosition = CopierPlateau(positionDuJeu);
                    AppliquerCoupSurPosition(nouvellePosition, coup);

                    if (coup.YArrivee == TailleGrille - 1)
                    {
                        return 100000 + profondeur;
                    }

                    int score = Minimax(nouvellePosition, BLANC, profondeur - 1);

                    if (score > meilleurScore)
                    {
                        meilleurScore = score;
                    }
                }

                return meilleurScore;
            }
            else
            {
                // Les Blancs sont l'humain : Minimax suppose qu'ils jouent
                // eux aussi le meilleur coup possible pour eux.
                int meilleurScore = int.MaxValue;

                foreach (Coup coup in coupsPossibles)
                {
                    int[,] nouvellePosition = CopierPlateau(positionDuJeu);
                    AppliquerCoupSurPosition(nouvellePosition, coup);

                    if (coup.YArrivee == 0)
                    {
                        return -100000 - profondeur;
                    }

                    int score = Minimax(nouvellePosition, NOIR, profondeur - 1);

                    if (score < meilleurScore)
                    {
                        meilleurScore = score;
                    }
                }

                return meilleurScore;
            }
        }

        private int[,] CopierPlateau(int[,] plateauOriginal)
        {
            int[,] copie = new int[TailleGrille, TailleGrille];

            for (int x = 0; x < TailleGrille; x++)
            {
                for (int y = 0; y < TailleGrille; y++)
                {
                    copie[x, y] = plateauOriginal[x, y];
                }
            }

            return copie;
        }

        private void AppliquerCoupSurPosition(int[,] positionDuJeu, Coup coup)
        {
            int couleurPion = positionDuJeu[coup.XDepart, coup.YDepart];

            positionDuJeu[coup.XDepart, coup.YDepart] = VIDE;
            positionDuJeu[coup.XArrivee, coup.YArrivee] = couleurPion;

            if (coup.EstUnePrise)
            {
                int xMilieu = (coup.XDepart + coup.XArrivee) / 2;
                int yMilieu = (coup.YDepart + coup.YArrivee) / 2;
                positionDuJeu[xMilieu, yMilieu] = VIDE;
            }
        }

        private List<Coup> GenererCoupsPourPosition(int[,] positionDuJeu, int couleur)
        {
            List<Coup> deplacements = new List<Coup>();
            List<Coup> prises = new List<Coup>();

            int avant = couleur == BLANC ? -1 : 1;
            int adversaire = couleur == BLANC ? NOIR : BLANC;
            int[] decalagesX = { -1, 1 };
            int[] directionsPriseY = { -1, 1 };

            for (int x = 0; x < TailleGrille; x++)
            {
                for (int y = 0; y < TailleGrille; y++)
                {
                    if (positionDuJeu[x, y] != couleur)
                    {
                        continue;
                    }

                    // Déplacements simples : uniquement vers l'avant.
                    foreach (int decalageX in decalagesX)
                    {
                        int xArrivee = x + decalageX;
                        int yArrivee = y + avant;

                        if (DansLesLimitesSimulation(xArrivee, yArrivee)
                            && positionDuJeu[xArrivee, yArrivee] == VIDE)
                        {
                            deplacements.Add(new Coup(x, y, xArrivee, yArrivee, false));
                        }
                    }

                    // Prises : autorisées vers l'avant et vers l'arrière.
                    foreach (int decalageX in decalagesX)
                    {
                        foreach (int directionY in directionsPriseY)
                        {
                            int xMilieu = x + decalageX;
                            int yMilieu = y + directionY;
                            int xArrivee = x + 2 * decalageX;
                            int yArrivee = y + 2 * directionY;

                            if (DansLesLimitesSimulation(xMilieu, yMilieu)
                                && DansLesLimitesSimulation(xArrivee, yArrivee)
                                && positionDuJeu[xMilieu, yMilieu] == adversaire
                                && positionDuJeu[xArrivee, yArrivee] == VIDE)
                            {
                                prises.Add(new Coup(x, y, xArrivee, yArrivee, true));
                            }
                        }
                    }
                }
            }

            // La prise est obligatoire.
            if (prises.Count > 0)
            {
                return prises;
            }

            return deplacements;
        }

        private bool DansLesLimitesSimulation(int x, int y)
        {
            return x >= 0 && x < TailleGrille && y >= 0 && y < TailleGrille;
        }

        // Méthodes d'aide facultatives pour la fonction d'évaluation.
        int CompterCoupsPossibles(int[,] positionDuJeu, int couleur)
        {
            return GenererCoupsPourPosition(positionDuJeu, couleur).Count;
        }

        int CompterPrisesPossibles(int[,] positionDuJeu, int couleur)
        {
            List<Coup> coups = GenererCoupsPourPosition(positionDuJeu, couleur);
            int nombrePrises = 0;

            foreach (Coup coup in coups)
            {
                if (coup.EstUnePrise)
                {
                    nombrePrises++;
                }
            }

            return nombrePrises;
        }

        // =====================================================================
        // CODE FOURNI : AFFICHAGE ET INTERACTIONS
        // Ne modifiez pas cette partie sauf indication de l'enseignant.
        // =====================================================================

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var etatSouris = Mouse.GetState();
            var etatClavier = Keyboard.GetState();
            bool clicGaucheRelache = _etatSourisPrecedent.LeftButton == ButtonState.Pressed
                                      && etatSouris.LeftButton == ButtonState.Released;

            bool toucheF2 = etatClavier.IsKeyDown(Keys.F2) && !_etatClavierPrecedent.IsKeyDown(Keys.F2);
            bool toucheF3 = etatClavier.IsKeyDown(Keys.F3) && !_etatClavierPrecedent.IsKeyDown(Keys.F3);
            bool toucheF4 = etatClavier.IsKeyDown(Keys.F4) && !_etatClavierPrecedent.IsKeyDown(Keys.F4);
            bool toucheF5 = etatClavier.IsKeyDown(Keys.F5) && !_etatClavierPrecedent.IsKeyDown(Keys.F5);

            if (toucheF2)
            {
                TesterDirectionAvant();
            }

            if (toucheF3)
            {
                TesterDansLesLimites();
            }

            if (toucheF4)
            {
                _afficherTestEtape4 = !_afficherTestEtape4;
                RecalculerCoupsLegaux();
                if (_afficherTestEtape4)
                {
                    _resultatTest = new[]
                    {
                        "TEST ETAPE 4",
                        "PIONS TROUVES : " + _casesTrouveesPourTest.Count,
                        "ATTENDU AU DEPART : 20",
                        "LES PIONS BLANCS DOIVENT",
                        "ETRE SURLIGNES EN BLEU"
                    };
                }
                else
                {
                    _resultatTest = new[] { "TEST ETAPE 4 MASQUE" };
                }
            }

            if (toucheF5)
            {
                ChargerSituationTestPrise();
            }

            // Touche R pour relancer une partie rapidement pendant les tests.
            if (etatClavier.IsKeyDown(Keys.R))
            {
                InitialiserPlateau();
                RecalculerCoupsLegaux();
                _resultatTest = new[] { "POSITION INITIALE RECHARGEE" };
            }

            // PISTE NOIRE : les Blancs sont humains et les Noirs sont contrôlés par l'IA.
            if (_finDePartie == 0 && joueurCourant == NOIR)
            {
                JouerTourIA();
            }

            if (_finDePartie == 0 && joueurCourant == BLANC && clicGaucheRelache)
            {
                GererClic(etatSouris.X, etatSouris.Y);
            }

            _etatSourisPrecedent = etatSouris;
            _etatClavierPrecedent = etatClavier;

            base.Update(gameTime);
        }

        /// <summary>
        /// Convertit une position écran en coordonnées de case, puis gère la sélection
        /// d'un pion ou l'exécution d'un coup si une destination valide est cliquée.
        /// </summary>
        private void GererClic(int ecranX, int ecranY)
        {
            int caseX = (ecranX - MargeFenetre) / TailleCase;
            int caseY = (ecranY - MargeFenetre) / TailleCase;

            if (!DansLesLimites(caseX, caseY)) return;

            if (_unPionEstSelectionne)
            {
                // Le joueur clique sur une destination déjà proposée -> on joue le coup.
                foreach (var coup in _destinationsPossiblesPourSelection)
                {
                    if (coup.XArrivee == caseX && coup.YArrivee == caseY)
                    {
                        JouerCoup(coup);
                        _unPionEstSelectionne = false;
                        _destinationsPossiblesPourSelection.Clear();
                        return;
                    }
                }

                // Clic ailleurs : si c'est un autre pion jouable, on change la sélection ; sinon on désélectionne.
                if (plateauDeJeu[caseX, caseY] == joueurCourant)
                {
                    SelectionnerPion(caseX, caseY);
                }
                else
                {
                    _unPionEstSelectionne = false;
                    _destinationsPossiblesPourSelection.Clear();
                }
            }
            else
            {
                if (plateauDeJeu[caseX, caseY] == joueurCourant)
                {
                    SelectionnerPion(caseX, caseY);
                }
            }
        }

        /// <summary>
        /// Sélectionne un pion et filtre, parmi les coups légaux globaux, ceux qui partent de ce pion.
        /// Si la prise est obligatoire, seuls les pions ayant une prise possible deviennent sélectionnables
        /// avec des destinations (les autres pions n'auront aucune destination affichée).
        /// </summary>
        private void SelectionnerPion(int x, int y)
        {
            var destinations = new List<Coup>();
            foreach (var coup in coupsLegaux)
            {
                if (coup.XDepart == x && coup.YDepart == y)
                    destinations.Add(coup);
            }

            if (destinations.Count == 0)
            {
                // Ce pion n'a aucun coup légal actuellement (probablement parce qu'une prise est
                // obligatoire ailleurs) : on ne sélectionne rien.
                _unPionEstSelectionne = false;
                _destinationsPossiblesPourSelection.Clear();
                return;
            }

            _unPionEstSelectionne = true;
            _pionSelectionneX = x;
            _pionSelectionneY = y;
            _destinationsPossiblesPourSelection = destinations;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(30, 30, 30));

            _spriteBatch.Begin();

            DessinerDamier();
            DessinerSurlignages();
            DessinerPions();
            DessinerPanneauAide();

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DessinerRectangle(int x, int y, int largeur, int hauteur, Color couleur)
        {
            _spriteBatch.Draw(_pixel, new Rectangle(x, y, largeur, hauteur), couleur);
        }

        private void DessinerDamier()
        {
            Color caseClaire = new Color(230, 220, 200);
            Color caseSombre = new Color(120, 90, 60);

            for (int y = 0; y < TailleGrille; y++)
            {
                for (int x = 0; x < TailleGrille; x++)
                {
                    bool sombre = (x + y) % 2 == 1;
                    Color couleurCase = sombre ? caseSombre : caseClaire;

                    DessinerRectangle(
                        MargeFenetre + x * TailleCase,
                        MargeFenetre + y * TailleCase,
                        TailleCase,
                        TailleCase,
                        couleurCase);
                }
            }

            // Bordure globale du plateau.
            int tailleTotale = TailleGrille * TailleCase;
            DessinerContour(MargeFenetre, MargeFenetre, tailleTotale, tailleTotale, Color.Black, 2);
        }

        private void DessinerContour(int x, int y, int largeur, int hauteur, Color couleur, int epaisseur)
        {
            DessinerRectangle(x, y, largeur, epaisseur, couleur);
            DessinerRectangle(x, y + hauteur - epaisseur, largeur, epaisseur, couleur);
            DessinerRectangle(x, y, epaisseur, hauteur, couleur);
            DessinerRectangle(x + largeur - epaisseur, y, epaisseur, hauteur, couleur);
        }

        private void DessinerSurlignages()
        {
            // TEST ETAPE 4 : cases des pions trouvés par le code de l'élève.
            if (_afficherTestEtape4)
            {
                Color couleurTest = new Color(0, 180, 255, 110);
                foreach (Point caseTrouvee in _casesTrouveesPourTest)
                {
                    DessinerRectangle(
                        MargeFenetre + caseTrouvee.X * TailleCase,
                        MargeFenetre + caseTrouvee.Y * TailleCase,
                        TailleCase, TailleCase, couleurTest);
                }
            }

            if (!_unPionEstSelectionne) return;

            // Surlignage du pion sélectionné.
            Color surlignageSelection = new Color(255, 220, 0, 120);
            DessinerRectangle(
                MargeFenetre + _pionSelectionneX * TailleCase,
                MargeFenetre + _pionSelectionneY * TailleCase,
                TailleCase, TailleCase, surlignageSelection);

            // Anneaux verts sur les destinations possibles.
            foreach (var coup in _destinationsPossiblesPourSelection)
            {
                Color couleurAnneau = coup.EstUnePrise ? Color.OrangeRed : Color.LimeGreen;
                int cx = MargeFenetre + coup.XArrivee * TailleCase + TailleCase / 2 - _textureAnneau.Width / 2;
                int cy = MargeFenetre + coup.YArrivee * TailleCase + TailleCase / 2 - _textureAnneau.Height / 2;
                _spriteBatch.Draw(_textureAnneau, new Vector2(cx, cy), couleurAnneau);
            }
        }

        private void DessinerPions()
        {
            for (int y = 0; y < TailleGrille; y++)
            {
                for (int x = 0; x < TailleGrille; x++)
                {
                    int couleurPion = plateauDeJeu[x, y];
                    if (couleurPion == VIDE) continue;

                    Color couleurDessin = couleurPion == BLANC ? Color.White : new Color(20, 20, 20);
                    Color couleurContourPion = couleurPion == BLANC ? Color.Black : Color.White;

                    int px = MargeFenetre + x * TailleCase + TailleCase / 2 - _texturePion.Width / 2;
                    int py = MargeFenetre + y * TailleCase + TailleCase / 2 - _texturePion.Height / 2;

                    // Léger contour : on dessine d'abord un cercle légèrement plus grand de la couleur de contour.
                    _spriteBatch.Draw(_texturePion, new Vector2(px - 1, py - 1),
                        null, couleurContourPion, 0f, Vector2.Zero, 1.06f, SpriteEffects.None, 0f);

                    _spriteBatch.Draw(_texturePion, new Vector2(px, py), couleurDessin);
                }
            }
        }

        // ---------------------------------------------------------------------
        // CODE FOURNI : panneau d'aide et petite police bitmap.
        // Aucun fichier de police MonoGame n'est nécessaire.
        // Vous n'avez pas à comprendre ni modifier cette partie.
        // ---------------------------------------------------------------------
        private void DessinerPanneauAide()
        {
            int xPanneau = MargeFenetre + TailleGrille * TailleCase + 25;
            int yPanneau = MargeFenetre;
            int largeur = LargeurPanneauAide - 45;
            int hauteur = TailleGrille * TailleCase;

            DessinerRectangle(xPanneau - 12, yPanneau - 12, largeur + 24, hauteur + 24, new Color(45, 45, 45));
            DessinerContour(xPanneau - 12, yPanneau - 12, largeur + 24, hauteur + 24, new Color(110, 110, 110), 2);

            DessinerTexteBitmap("JEU DE SHADI", xPanneau, yPanneau, 2, Color.White);

            string joueur = joueurCourant == BLANC ? "BLANCS (HUMAIN)" : "NOIRS (IA)";
            DessinerTexteBitmap("JOUEUR : " + joueur, xPanneau, yPanneau + 34, 2, Color.White);

            if (_finDePartie == BLANC)
                DessinerTexteBitmap("VICTOIRE DES BLANCS", xPanneau, yPanneau + 64, 2, Color.White);
            else if (_finDePartie == NOIR)
                DessinerTexteBitmap("VICTOIRE DES NOIRS", xPanneau, yPanneau + 64, 2, Color.White);
            else if (PriseObligatoireEnCours())
                DessinerTexteBitmap("PRISE OBLIGATOIRE", xPanneau, yPanneau + 64, 2, Color.OrangeRed);

            int yTouches = yPanneau + 110;
            DessinerTexteBitmap("TOUCHES", xPanneau, yTouches, 2, Color.White);
            DessinerTexteBitmap("F2 : DIRECTION", xPanneau, yTouches + 28, 2, Color.LightGray);
            DessinerTexteBitmap("F3 : LIMITES", xPanneau, yTouches + 52, 2, Color.LightGray);
            DessinerTexteBitmap("F4 : PIONS TROUVES", xPanneau, yTouches + 76, 2, Color.LightGray);
            DessinerTexteBitmap("F5 : TEST PRISE", xPanneau, yTouches + 100, 2, Color.LightGray);
            DessinerTexteBitmap("R  : REINITIALISER", xPanneau, yTouches + 124, 2, Color.LightGray);

            int yResultat = yPanneau + 285;
            DessinerTexteBitmap("RESULTAT DU TEST", xPanneau, yResultat, 2, Color.White);

            for (int i = 0; i < _resultatTest.Length; i++)
            {
                DessinerTexteBitmap(_resultatTest[i], xPanneau, yResultat + 30 + i * 25, 2, Color.LightGreen);
            }
        }

        private void DessinerTexteBitmap(string texte, int x, int y, int echelle, Color couleur)
        {
            int xDepart = x;
            texte = texte.ToUpperInvariant();

            foreach (char caractere in texte)
            {
                if (caractere == '\n')
                {
                    x = xDepart;
                    y += 8 * echelle;
                    continue;
                }

                string[] motif = ObtenirMotifCaractere(caractere);
                if (motif != null)
                {
                    for (int ligne = 0; ligne < motif.Length; ligne++)
                    {
                        for (int colonne = 0; colonne < motif[ligne].Length; colonne++)
                        {
                            if (motif[ligne][colonne] == '1')
                            {
                                DessinerRectangle(
                                    x + colonne * echelle,
                                    y + ligne * echelle,
                                    echelle,
                                    echelle,
                                    couleur);
                            }
                        }
                    }
                }

                x += 6 * echelle;
            }
        }

        private string[] ObtenirMotifCaractere(char c)
        {
            switch (c)
            {
                case 'A': return new[] { "01110", "10001", "10001", "11111", "10001", "10001", "10001" };
                case 'B': return new[] { "11110", "10001", "10001", "11110", "10001", "10001", "11110" };
                case 'C': return new[] { "01111", "10000", "10000", "10000", "10000", "10000", "01111" };
                case 'D': return new[] { "11110", "10001", "10001", "10001", "10001", "10001", "11110" };
                case 'E': return new[] { "11111", "10000", "10000", "11110", "10000", "10000", "11111" };
                case 'F': return new[] { "11111", "10000", "10000", "11110", "10000", "10000", "10000" };
                case 'G': return new[] { "01111", "10000", "10000", "10111", "10001", "10001", "01111" };
                case 'H': return new[] { "10001", "10001", "10001", "11111", "10001", "10001", "10001" };
                case 'I': return new[] { "11111", "00100", "00100", "00100", "00100", "00100", "11111" };
                case 'J': return new[] { "00111", "00010", "00010", "00010", "10010", "10010", "01100" };
                case 'K': return new[] { "10001", "10010", "10100", "11000", "10100", "10010", "10001" };
                case 'L': return new[] { "10000", "10000", "10000", "10000", "10000", "10000", "11111" };
                case 'M': return new[] { "10001", "11011", "10101", "10101", "10001", "10001", "10001" };
                case 'N': return new[] { "10001", "11001", "10101", "10011", "10001", "10001", "10001" };
                case 'O': return new[] { "01110", "10001", "10001", "10001", "10001", "10001", "01110" };
                case 'P': return new[] { "11110", "10001", "10001", "11110", "10000", "10000", "10000" };
                case 'Q': return new[] { "01110", "10001", "10001", "10001", "10101", "10010", "01101" };
                case 'R': return new[] { "11110", "10001", "10001", "11110", "10100", "10010", "10001" };
                case 'S': return new[] { "01111", "10000", "10000", "01110", "00001", "00001", "11110" };
                case 'T': return new[] { "11111", "00100", "00100", "00100", "00100", "00100", "00100" };
                case 'U': return new[] { "10001", "10001", "10001", "10001", "10001", "10001", "01110" };
                case 'V': return new[] { "10001", "10001", "10001", "10001", "10001", "01010", "00100" };
                case 'W': return new[] { "10001", "10001", "10001", "10101", "10101", "11011", "10001" };
                case 'X': return new[] { "10001", "10001", "01010", "00100", "01010", "10001", "10001" };
                case 'Y': return new[] { "10001", "10001", "01010", "00100", "00100", "00100", "00100" };
                case 'Z': return new[] { "11111", "00001", "00010", "00100", "01000", "10000", "11111" };
                case '0': return new[] { "01110", "10001", "10011", "10101", "11001", "10001", "01110" };
                case '1': return new[] { "00100", "01100", "00100", "00100", "00100", "00100", "01110" };
                case '2': return new[] { "01110", "10001", "00001", "00010", "00100", "01000", "11111" };
                case '3': return new[] { "11110", "00001", "00001", "01110", "00001", "00001", "11110" };
                case '4': return new[] { "00010", "00110", "01010", "10010", "11111", "00010", "00010" };
                case '5': return new[] { "11111", "10000", "10000", "11110", "00001", "00001", "11110" };
                case '6': return new[] { "01110", "10000", "10000", "11110", "10001", "10001", "01110" };
                case '7': return new[] { "11111", "00001", "00010", "00100", "01000", "01000", "01000" };
                case '8': return new[] { "01110", "10001", "10001", "01110", "10001", "10001", "01110" };
                case '9': return new[] { "01110", "10001", "10001", "01111", "00001", "00001", "01110" };
                case ':': return new[] { "00000", "00100", "00100", "00000", "00100", "00100", "00000" };
                case '-': return new[] { "00000", "00000", "00000", "11111", "00000", "00000", "00000" };
                case '+': return new[] { "00000", "00100", "00100", "11111", "00100", "00100", "00000" };
                case '(': return new[] { "00010", "00100", "01000", "01000", "01000", "00100", "00010" };
                case ')': return new[] { "01000", "00100", "00010", "00010", "00010", "00100", "01000" };
                case ',': return new[] { "00000", "00000", "00000", "00000", "00100", "00100", "01000" };
                case '.': return new[] { "00000", "00000", "00000", "00000", "00000", "00100", "00100" };
                case '/': return new[] { "00001", "00010", "00100", "01000", "10000", "00000", "00000" };
                case ' ': return new[] { "00000", "00000", "00000", "00000", "00000", "00000", "00000" };
                default:  return new[] { "00000", "00000", "00000", "00000", "00000", "00000", "00000" };
            }
        }

        // CODE FOURNI : permet uniquement d'afficher le message « prise obligatoire ».
        // Cette méthode utilise la liste de coups, une notion qui ne fait pas partie
        // des objectifs de la piste bleue. Vous n'avez pas à la modifier.
        private bool PriseObligatoireEnCours()
        {
            foreach (Coup coup in coupsLegaux)
            {
                if (coup.EstUnePrise)
                {
                    return true;
                }
            }

            return false;
        }
    }
}