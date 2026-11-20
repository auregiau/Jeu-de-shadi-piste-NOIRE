# Projet C# — Jeu de Shadi
## Piste bleue — parcours guidé

---

# 1. Objectif de la piste bleue

Vous allez programmer progressivement la logique du **Jeu de Shadi**.

L'objectif principal de ce projet est de consolider les notions de programmation que vous connaissez déjà :

- variables ;
- conditions `if` / `else` ;
- boucles ;
- tableaux ;
- méthodes et fonctions.

Le projet utilise également des éléments que vous n'avez pas encore étudiés, notamment **MonoGame**, les listes et quelques éléments de programmation orientée objet.

Vous n'avez pas à apprendre ces notions pour réussir la piste bleue.

Chaque fois qu'une partie du programme nécessite une notion qui n'a pas encore été étudiée :

- le code est déjà fourni ; ou
- une méthode d'aide vous est donnée ; ou
- l'appel exact à effectuer vous est indiqué.

Votre travail consiste donc à vous concentrer sur **la logique du jeu**.

---

# 2. Préparer le projet sous Windows

Le projet vous est fourni sous la forme d'un fichier ZIP.

Windows peut considérer un fichier téléchargé comme provenant d'un autre ordinateur et bloquer certains fichiers contenus dans le projet. Cela peut empêcher MonoGame de fonctionner correctement.

## 2.1 Débloquer le ZIP avant de l'extraire

Avant d'extraire le projet :

1. faites un clic droit sur le fichier ZIP ;
2. choisissez **Propriétés** ;
3. si l'option **Débloquer** est affichée en bas de la fenêtre, cochez-la ;
4. cliquez sur **Appliquer** ;
5. extrayez ensuite le fichier ZIP.

> Il est préférable de débloquer le ZIP **avant** de l'extraire.

## 2.2 Si vous avez déjà extrait le ZIP

Ouvrez PowerShell dans le dossier du projet et exécutez :

```powershell
Get-ChildItem -Recurse | Unblock-File
```

Cette commande débloque les fichiers contenus dans le projet.

## 2.3 Restaurer les outils MonoGame

Ouvrez un terminal dans le dossier contenant le fichier :

```text
JeuDeShadi.csproj
```

Puis exécutez :

```powershell
dotnet tool restore
```

Ensuite :

```powershell
dotnet restore
```

Vous pouvez vérifier que le projet compile avec :

```powershell
dotnet build
```

## 2.4 Premier lancement

Lancez ensuite le projet.

Au début du travail, vous devez déjà voir apparaître :

- une fenêtre ;
- un plateau de 10 × 10 cases.

Il est normal que le jeu ne fonctionne pas encore complètement : vous allez programmer sa logique progressivement.

---

# 3. Technologie utilisée pour l'affichage

Le projet est écrit en **C#** et utilise **.NET 9**.

L'affichage graphique est réalisé avec **MonoGame**.

MonoGame s'occupe notamment de :

- créer la fenêtre ;
- dessiner le plateau ;
- dessiner les pions ;
- lire la souris ;
- détecter les clics ;
- afficher les cases sélectionnées ;
- rafraîchir l'écran.

Vous n'avez pas à programmer ces éléments.

---

# 4. Avant de coder : comprendre l'organisation du code fourni

Le fichier principal est :

```text
Game1.cs
```

Vous allez voir dans ce fichier du code que vous ne connaissez pas encore.

Par exemple :

```csharp
public class Game1 : Game
```

ou :

```csharp
protected override void Update(...)
```

ou encore :

```csharp
private SpriteBatch _spriteBatch;
```

**Vous n'avez pas besoin de comprendre ces instructions pour réaliser la piste bleue.**

Elles appartiennent au cadre MonoGame et au code fourni.

Vous verrez plus tard, dans d'autres cours, les notions de classe, d'objet, de visibilité (`private`, `public`, `protected`) et d'héritage.

Pour ce projet, considérez simplement cette partie comme une **boîte noire déjà fonctionnelle**.

---

# 5. À propos du caractère `_`

Dans le code fourni, vous rencontrerez peut-être des noms comme :

```csharp
_graphics
_spriteBatch
_texturePion
```

Le caractère `_` ne signifie pas `private` et ne signifie pas `protected`.

Il s'agit uniquement d'une **convention de nommage** utilisée par certains développeurs.

Dans la partie que vous allez programmer, nous utilisons volontairement des noms plus simples et plus explicites, par exemple :

```csharp
plateauDeJeu
joueurCourant
xDepart
yDepart
xArrivee
yArrivee
```

---

# 6. Les données principales du jeu

Deux variables sont particulièrement importantes pour votre travail.

## 6.1 `plateauDeJeu`

Le plateau est représenté par un tableau à deux dimensions :

```csharp
int[,] plateauDeJeu = new int[10, 10];
```

Vous pouvez l'imaginer comme une grille de 10 colonnes et 10 lignes.

Une case se lit ainsi :

```csharp
plateauDeJeu[x, y]
```

avec :

- `x` = colonne ;
- `y` = ligne.

La case `(0, 0)` se trouve en haut à gauche.

## 6.2 Valeur contenue dans une case

Trois constantes sont fournies :

```csharp
const int VIDE = 0;
const int BLANC = 1;
const int NOIR = 2;
```

Ainsi :

```csharp
plateauDeJeu[3, 5] = BLANC;
```

signifie qu'un pion blanc est placé sur la case `(3, 5)`.

Pour vider cette case :

```csharp
plateauDeJeu[3, 5] = VIDE;
```

## 6.3 `joueurCourant`

La variable :

```csharp
joueurCourant
```

contient la couleur du joueur qui doit jouer.

Sa valeur est donc :

```text
BLANC
```

ou :

```text
NOIR
```

Cette variable est fournie et accessible aux méthodes que vous allez compléter.

---

# 7. Comment l'affichage fonctionne avec votre code

Vous n'avez pas besoin de demander directement à MonoGame de dessiner un pion.

Le principe est le suivant :

```text
vous modifiez plateauDeJeu
            ↓
le code graphique lit plateauDeJeu
            ↓
MonoGame dessine le nouvel état du plateau
```

Par exemple, si votre code exécute :

```csharp
plateauDeJeu[4, 6] = BLANC;
```

le code graphique fourni dessine automatiquement un pion blanc à cette position.

C'est très important : **vous programmez les données et les règles, pas l'affichage**.

---

# 8. Comment la souris fonctionne

La gestion de la souris est également déjà programmée.

Le code fourni :

1. détecte la case sur laquelle le joueur clique ;
2. vérifie s'il s'agit d'un pion du joueur courant ;
3. demande à votre code quels déplacements sont possibles ;
4. affiche les destinations ;
5. détecte la destination choisie ;
6. appelle votre méthode qui applique le coup.

Vous n'avez donc aucune instruction MonoGame à écrire pour la souris.

---

# 9. Les notions plus avancées sont masquées

Le programme utilise en interne une liste de coups et une structure appelée `Coup`.

Vous n'avez pas encore étudié ces notions.

Vous n'avez donc pas à écrire du code comme :

```csharp
new Coup(...)
```

ou :

```csharp
coups.Add(...)
```

À la place, vous utiliserez des méthodes d'aide fournies.

Pour enregistrer un déplacement normal :

```csharp
AjouterDeplacement(xDepart, yDepart, xArrivee, yArrivee);
```

Pour enregistrer une prise :

```csharp
AjouterPrise(xDepart, yDepart, xArrivee, yArrivee);
```

Le code fourni s'occupe du reste.

---

# 10. Cycle général d'un tour

Le fonctionnement d'un tour peut être résumé ainsi :

```text
le joueur clique sur un pion
        ↓
le programme recherche les coups possibles
        ↓
les destinations sont affichées
        ↓
le joueur clique sur une destination
        ↓
votre code modifie plateauDeJeu
        ↓
MonoGame redessine automatiquement le plateau
        ↓
le joueur suivant peut jouer
```

---

# 11. Où devez-vous travailler ?

Dans `Game1.cs`, cherchez :

```csharp
// ZONE ÉLÈVE — PISTE BLEUE
```

Vous devez principalement modifier les méthodes contenues dans cette zone.

Ne modifiez pas les parties marquées :

```csharp
// MÉTHODES D'AIDE FOURNIES — NE PAS MODIFIER
```

et :

```csharp
// CODE FOURNI : AFFICHAGE ET INTERACTIONS
```

---

# 12. Les outils de test fournis

Pour la piste bleue, le projet contient plusieurs outils de vérification déjà programmés.

Vous n'avez pas besoin de comprendre le code MonoGame qui réalise ces tests. Vous devez simplement utiliser les touches indiquées.

| Touche | Vérification |
|---|---|
| `R` | Recharger la position initiale |
### Le panneau d'aide et de test

La fenêtre du jeu contient maintenant deux zones :

```text
+----------------------------+----------------------+
|                            |                      |
|      PLATEAU DE JEU        |  PANNEAU D'AIDE      |
|                            |  ET DE TEST          |
|                            |                      |
+----------------------------+----------------------+
```

Le panneau de droite est entièrement fourni. Vous ne devez pas programmer son affichage.

Il vous sert à vérifier immédiatement votre travail. Lorsqu'un test est lancé, vous devez comparer les valeurs affichées avec les valeurs attendues.

| `F2` | Tester votre méthode `DirectionAvant()` |
| `F3` | Tester votre méthode `DansLesLimites()` |
| `F4` | Afficher ou masquer le test visuel de l'étape 4 |
| `F5` | Charger une situation simple permettant de tester une prise |

Lorsque vous utilisez `F2`, `F3`, `F4` ou `F5`, regardez le **panneau situé à droite du plateau**.

Ce panneau affiche :

- le joueur courant ;
- les touches de test disponibles ;
- le résultat détaillé du dernier test.

Vous n'avez rien à programmer pour afficher ce panneau : il fait partie du code fourni.

Le but de ces outils est simple : **à la fin de chaque étape, vous devez pouvoir vérifier immédiatement votre travail**.

---

# 13. Étape 1 — Initialiser le plateau

Méthode à compléter :

```csharp
void InitialiserPlateau()
```

Votre objectif est de placer les pions dans leur position de départ.

Règles :

- le plateau possède 10 lignes et 10 colonnes ;
- seules les cases où `(x + y)` est impair sont utilisées ;
- les Noirs occupent les lignes `0` à `3` ;
- les Blancs occupent les lignes `6` à `9`.

Vous devrez utiliser deux boucles imbriquées :

```csharp
for (...)
{
    for (...)
    {
        ...
    }
}
```

Pour savoir si une case est jouable, vous pouvez utiliser le modulo `%` :

```csharp
(x + y) % 2
```

Vous devrez ensuite affecter :

```csharp
NOIR
```

ou :

```csharp
BLANC
```

à la bonne case de `plateauDeJeu`.

### Ce que vous devez voir

Lancez le programme, ou appuyez sur `R` si le programme est déjà lancé.

Vous devez voir :

- **20 pions noirs** dans les quatre rangées du haut ;
- **20 pions blancs** dans les quatre rangées du bas ;
- les deux rangées centrales (`4` et `5`) vides ;
- les pions uniquement sur les cases jouables du damier.

Les Blancs se trouvent donc en bas et **les Blancs commencent la partie**.

Ne continuez pas tant que le placement initial n'est pas correct.

---

# 14. Étape 2 — Déterminer la direction d'un pion

Méthode à compléter :

```csharp
int DirectionAvant(int couleur)
```

Les Blancs avancent vers le haut du tableau :

```text
-1
```

Les Noirs avancent vers le bas :

```text
+1
```

Vous devez utiliser une condition et `return`.

Exemple de raisonnement :

```text
SI la couleur est BLANC
    retourner -1
SINON
    retourner 1
```

### Ce que vous devez voir

Lancez le programme puis appuyez sur `F2`. Regardez ensuite le panneau à droite du plateau.

### Ce que vous devez voir

Le panneau doit afficher quelque chose de proche de :

```text
TEST ETAPE 2
BLANC : -1  ATTENDU : -1
NOIR  : +1  ATTENDU : +1
RESULTAT : OK
```

Si `A CORRIGER` apparaît, vérifiez votre méthode avant de continuer.


Regardez le panneau à droite du plateau. Si votre méthode est correcte, vous devez voir :

```text
TEST ETAPE 2 : OK (Blanc=-1, Noir=+1)
```

Si le titre indique `A CORRIGER`, vérifiez votre `if`, votre `else` et vos `return`.

---

# 15. Étape 3 — Vérifier les limites du plateau

Méthode à compléter :

```csharp
bool DansLesLimites(int x, int y)
```

Cette méthode doit retourner `true` uniquement si la position existe dans le tableau.

Pour un plateau de 10 × 10 :

```text
x doit être compris entre 0 et 9
y doit être compris entre 0 et 9
```

Vous devez vérifier les quatre conditions.

Vous pouvez utiliser :

```csharp
&&
```

pour signifier **ET**.

### Ce que vous devez voir

Lancez le programme puis appuyez sur `F3`. Regardez ensuite le panneau à droite du plateau.

### Ce que vous devez voir

Le panneau doit confirmer les quatre essais :

```text
TEST ETAPE 3
(0,0)   ATTENDU VRAI : OK
(9,9)   ATTENDU VRAI : OK
(-1,3)  ATTENDU FAUX : OK
(10,4)  ATTENDU FAUX : OK
RESULTAT : OK
```

Si une ligne indique `ERREUR`, corrigez votre méthode `DansLesLimites()` avant de poursuivre.


Le projet vérifie automatiquement plusieurs positions, par exemple :

```text
(0,0)   -> dans le plateau
(9,9)   -> dans le plateau
(-1,3)  -> hors du plateau
(10,4)  -> hors du plateau
```

Si votre méthode fonctionne, le panneau à droite du plateau doit afficher :

```text
TEST ETAPE 3 : OK
```

---

# 16. Étape 4 — Chercher les pions du joueur courant

Méthode à compléter :

```csharp
void RecalculerCoupsLegaux()
```

Deux appels sont déjà fournis :

```csharp
CommencerRechercheCoups();
```

et :

```csharp
TerminerRechercheCoups();
```

Vous ne devez pas modifier ces appels.

Entre les deux, vous devez :

1. parcourir tout `plateauDeJeu` ;
2. vérifier si la case contient un pion de `joueurCourant` ;
3. si oui, appeler d'abord la méthode de test fournie :

```csharp
MarquerPionTrouvePourTest(x, y);
```

4. puis appeler :

```csharp
GenererCoupsPourPion(x, y);
```

`MarquerPionTrouvePourTest(...)` ne fait pas partie de la logique du jeu. Elle sert uniquement à vous montrer visuellement quels pions votre boucle a trouvés.

Vous utilisez donc ici principalement :

- deux boucles ;
- un `if` ;
- un appel de méthode.

### Ce que vous devez voir

1. Appuyez sur `R` pour revenir à la position initiale.
2. Appuyez sur `F4`.
3. Regardez le plateau **et** le panneau de droite.

### Ce que vous devez voir

Au début de la partie, les **20 pions blancs** doivent être surlignés en bleu.

Le panneau doit notamment afficher :

```text
TEST ETAPE 4
PIONS TROUVES : 20
ATTENDU AU DEPART : 20
```

Aucun pion noir ne doit être surligné.

Comme les Blancs commencent, **tous les pions blancs trouvés par votre code doivent être surlignés en bleu**.

Vous devez donc voir les 20 pions blancs surlignés, et aucun pion noir.

Appuyez à nouveau sur `F4` pour masquer ce mode de test.

Si certains pions manquent ou si des pions noirs sont surlignés, vérifiez vos deux boucles et votre condition.

---

# 17. Étape 5 — Rechercher les coups possibles d'un pion

Méthode concernée :

```csharp
void GenererCoupsPourPion(int x, int y)
```

Cette méthode est une étape importante du projet.

Jusqu'ici, vous savez :

- où se trouvent les pions ;
- quel joueur doit jouer ;
- dans quel sens un pion avance ;
- vérifier si des coordonnées sont encore dans le plateau.

Vous allez maintenant utiliser toutes ces informations pour répondre à une nouvelle question :

> **Sur quelles cases ce pion peut-il aller ?**

Pour un pion donné, deux types de coups sont possibles :

1. un **déplacement simple** ;
2. une **prise**.

La méthode `GenererCoupsPourPion(...)` doit donc regarder autour du pion et signaler chaque coup autorisé.

Vous ne devez pas gérer vous-même la liste des coups. Dès que vous trouvez un coup possible, utilisez simplement une des méthodes fournies :

```csharp
AjouterDeplacement(xDepart, yDepart, xArrivee, yArrivee);
```

ou :

```csharp
AjouterPrise(xDepart, yDepart, xArrivee, yArrivee);
```

---

## 17.1 Identifier le pion et son sens d'avancement

Le début de la méthode est déjà fourni :

```csharp
int couleurPion = plateauDeJeu[x, y];
int avant = DirectionAvant(couleurPion);
```

La variable `couleurPion` contient la couleur du pion placé en `(x, y)`.

La variable `avant` contient la direction verticale dans laquelle ce pion avance normalement :

```text
BLANC -> -1 -> vers le haut du tableau
NOIR  -> +1 -> vers le bas du tableau
```

Par exemple, si un pion blanc se trouve en :

```text
(4, 6)
```

alors la ligne située devant lui est :

```text
6 + (-1) = 5
```

---

## 17.2 Comprendre `decalagesX`

Le code fournit également :

```csharp
int[] decalagesX = { -1, 1 };
```

Il s'agit simplement d'un tableau de deux entiers :

```text
-1 -> une colonne vers la gauche
+1 -> une colonne vers la droite
```

Un pion se déplace en diagonale. Il faut donc essayer ces deux possibilités.

À partir d'un pion placé en `(4, 6)`, les colonnes possibles sont :

```text
4 + (-1) = 3
4 + 1    = 5
```

---

# 18. Étape 5A — Chercher les déplacements simples

Un déplacement simple consiste à avancer d'**une case en diagonale vers l'avant**.

Prenons un pion blanc situé en :

```text
(4, 6)
```

Comme un pion blanc monte dans le tableau, les deux cases à examiner sont :

```text
(3, 5)    et    (5, 5)
```

Visuellement :

```text
      .   .
       \ /
        B
```

`B` représente le pion blanc et les deux `.` représentent les destinations à tester.

Pour chacune des deux diagonales, vous devez :

1. calculer la colonne d'arrivée ;
2. calculer la ligne d'arrivée ;
3. vérifier que cette position existe dans le plateau ;
4. vérifier que la case est vide ;
5. si tout est correct, ajouter le déplacement.

### Pourquoi vérifier les limites avant le tableau ?

Imaginez un pion placé sur la colonne `0`.

Si vous essayez d'aller encore à gauche, vous obtenez :

```text
xArrivee = -1
```

La case :

```csharp
plateauDeJeu[-1, y]
```

n'existe pas et provoquerait une erreur.

Vous devez donc d'abord appeler :

```csharp
DansLesLimites(xArrivee, yArrivee)
```

et seulement ensuite regarder le contenu de la case.

### Pseudo-code à conserver

```text
POUR CHAQUE decalageX du tableau decalagesX

    xArrivee = x + decalageX
    yArrivee = y + avant

    SI (xArrivee, yArrivee) est dans les limites

        SI la case d'arrivée est VIDE
            appeler AjouterDeplacement(x, y, xArrivee, yArrivee)
        FIN SI

    FIN SI

FIN POUR
```

Votre travail consiste maintenant à traduire ce raisonnement en C#.

## Ce que vous devez voir

1. Appuyez sur `R` pour revenir à la position initiale.
2. Cliquez sur un pion blanc situé sur la rangée avant des Blancs.

Si votre code fonctionne, les cases sur lesquelles le pion peut se déplacer doivent être indiquées graphiquement.

Un pion placé loin du bord doit normalement proposer **deux diagonales** si elles sont libres.

Un pion placé près d'un bord peut n'avoir qu'**une seule destination**.

Si aucune destination n'apparaît alors qu'une case diagonale est libre, vérifiez :

- la boucle sur `decalagesX` ;
- le calcul de `xArrivee` ;
- le calcul de `yArrivee` ;
- l'appel à `DansLesLimites(...)` ;
- le test `plateauDeJeu[xArrivee, yArrivee] == VIDE`.

---

# 19. Étape 5B — Chercher les prises

Une prise est un peu plus complexe qu'un déplacement simple.

Cette fois, le pion ne se déplace pas d'une seule case : il **saute par-dessus un pion adverse**.

Il faut donc examiner deux positions :

1. la case du milieu, qui doit contenir un adversaire ;
2. la case d'arrivée, qui doit être vide.

Exemple :

```text
B N .
```

- `B` est votre pion ;
- `N` est un pion adverse ;
- `.` est la case d'arrivée libre.

Le pion `B` peut sauter par-dessus `N` et arriver sur `.`.

## 19.1 Pourquoi tester avant et arrière ?

Pour les déplacements simples, le pion avance uniquement dans son sens normal.

Pour une prise, le Jeu de Shadi autorise une capture vers l'avant **ou vers l'arrière**.

Le code fournit donc :

```csharp
int[] directionsY = { avant, -avant };
```

Ce tableau contient les deux directions verticales à essayer.

Vous devez combiner :

```text
gauche / droite
```

avec :

```text
avant / arrière
```

C'est pourquoi vous aurez besoin de **deux boucles**.

## 19.2 Calculer la case du milieu et la case d'arrivée

Pour chaque combinaison, calculez d'abord la position du pion que l'on souhaite éventuellement capturer :

```text
xMilieu = x + decalageX
yMilieu = y + directionY
```

Puis la position située juste derrière :

```text
xArrivee = x + 2 * decalageX
yArrivee = y + 2 * directionY
```

Vous devez ensuite vérifier :

1. que les coordonnées sont dans le plateau ;
2. que la case du milieu contient un pion adverse ;
3. que la case d'arrivée est vide.

Pour savoir si la case du milieu contient un adversaire, utilisez la méthode fournie :

```csharp
EstUnAdversaire(xMilieu, yMilieu, couleurPion)
```

Vous n'avez pas à programmer cette méthode.

### Pseudo-code à conserver

```text
POUR CHAQUE decalageX du tableau decalagesX

    POUR CHAQUE directionY du tableau directionsY

        calculer xMilieu et yMilieu
        calculer xArrivee et yArrivee

        SI la case du milieu ET la case d'arrivée sont dans les limites

            SI la case du milieu contient un adversaire
               ET la case d'arrivée est VIDE

                appeler AjouterPrise(x, y, xArrivee, yArrivee)

            FIN SI

        FIN SI

    FIN POUR

FIN POUR
```

Votre travail consiste à traduire ce pseudo-code en C#.

## Ce que vous devez voir

Pour ne pas devoir jouer plusieurs tours avant de tester une capture, une situation de test est fournie.

1. Appuyez sur `F5`.
2. Le programme place un pion blanc en `(3,6)` et un pion noir en `(4,5)`.
3. Cliquez sur le pion blanc.

Le panneau de droite rappelle :

```text
TEST PRISE
CLIQUEZ LE PION BLANC
EN (3,6)
LA PRISE DOIT ALLER
VERS (5,4)
```

Si votre détection des prises fonctionne, la destination `(5,4)` doit être proposée.

Si elle n'apparaît pas, vérifiez dans cet ordre :

- vos deux boucles ;
- `xMilieu` et `yMilieu` ;
- `xArrivee` et `yArrivee` ;
- les appels à `DansLesLimites(...)` ;
- l'appel à `EstUnAdversaire(...)` ;
- le test de la case d'arrivée vide ;
- l'appel à `AjouterPrise(...)`.

---

# 20. La prise obligatoire est déjà gérée

Le Jeu de Shadi impose une prise lorsqu'au moins une capture est disponible.

Vous n'avez pas à programmer la gestion de la liste des coups pour cette règle.

Le code fourni applique automatiquement le principe suivant :

```text
SI au moins une prise existe
    conserver uniquement les prises
SINON
    conserver les déplacements simples
FIN SI
```

Votre responsabilité est donc uniquement de **détecter correctement les prises** et d'appeler :

```csharp
AjouterPrise(...)
```

Le code fourni s'occupe du reste.

---

# 21. Étape 6 — Appliquer un coup

Méthode à compléter :

```csharp
void AppliquerCoup(
    int xDepart,
    int yDepart,
    int xArrivee,
    int yArrivee,
    bool estUnePrise)
```

Jusqu'à présent, vous avez appris à **trouver** les coups possibles.

Mais lorsqu'un joueur clique réellement sur une destination, il faut maintenant modifier le plateau.

C'est le rôle de `AppliquerCoup(...)`.

La partie graphique a déjà transformé le clic du joueur en cinq informations simples :

- `xDepart` : colonne de départ ;
- `yDepart` : ligne de départ ;
- `xArrivee` : colonne d'arrivée ;
- `yArrivee` : ligne d'arrivée ;
- `estUnePrise` : `true` si le coup capture un pion, sinon `false`.

Vous n'avez donc pas à gérer la souris ni l'objet `Coup`.

Votre seul travail est de **modifier `plateauDeJeu`**.

## 21.1 Mémoriser la couleur du pion

Avant de vider la case de départ, mémorisez la couleur du pion :

```text
couleurPion = contenu de la case de départ
```

Pourquoi ?

Parce qu'après avoir écrit :

```text
case de départ = VIDE
```

la couleur du pion ne sera plus disponible à cet endroit.

### Pseudo-code

```text
couleurPion = plateauDeJeu[xDepart, yDepart]
```

---

## 21.2 Déplacer le pion

Déplacer un pion revient simplement à modifier deux cases du tableau :

```text
case de départ = VIDE
case d'arrivée = couleurPion
```

Le code graphique lira ensuite le nouveau contenu de `plateauDeJeu` et redessinera automatiquement le pion au bon endroit.

Vous ne devez donc faire aucun appel MonoGame.

### Pseudo-code

```text
plateauDeJeu[xDepart, yDepart] = VIDE
plateauDeJeu[xArrivee, yArrivee] = couleurPion
```

---

## 21.3 Supprimer le pion capturé

Cette partie ne doit être exécutée que si :

```csharp
estUnePrise == true
```

Le pion adverse se trouve exactement au milieu entre la case de départ et la case d'arrivée.

Vous pouvez donc calculer :

```text
xMilieu = (xDepart + xArrivee) / 2
yMilieu = (yDepart + yArrivee) / 2
```

Puis vider cette case.

### Pseudo-code

```text
SI estUnePrise est vrai
    xMilieu = (xDepart + xArrivee) / 2
    yMilieu = (yDepart + yArrivee) / 2
    plateauDeJeu[xMilieu, yMilieu] = VIDE
FIN SI
```

## Ce que vous devez voir pour tester la prise

1. Appuyez sur `F5`.
2. Cliquez sur le pion blanc en `(3,6)`.
3. Cliquez sur la destination `(5,4)`.

Si votre code fonctionne :

- le pion blanc doit quitter `(3,6)` ;
- il doit apparaître en `(5,4)` ;
- le pion noir situé en `(4,5)` doit disparaître.

C'est un test très important : il vérifie à la fois le déplacement et la suppression du pion capturé.

---

## 21.4 Vérifier si le coup fait gagner

Un pion blanc gagne s'il atteint la ligne :

```text
0
```

Un pion noir gagne s'il atteint la dernière ligne du plateau :

```text
TailleGrille - 1
```

Si le pion vient de gagner, utilisez la méthode fournie :

```csharp
DeclarerVictoire(couleurPion);
```

Sinon, utilisez :

```csharp
TerminerTour(couleurPion);
```

Cette méthode fournie change de joueur et recalcule les coups possibles.

### Pseudo-code

```text
SI le pion blanc est arrivé sur la ligne 0
    déclarer la victoire du BLANC
SINON SI le pion noir est arrivé sur la dernière ligne
    déclarer la victoire du NOIR
SINON
    terminer le tour
FIN SI
```

## Ce que vous devez voir

Lorsque vous jouez un déplacement normal :

- le pion doit changer de case ;
- la case de départ doit devenir vide ;
- le joueur courant doit changer dans le panneau de droite.

Lors d'une prise :

- le pion doit se déplacer ;
- le pion capturé doit disparaître ;
- le joueur doit ensuite changer si la partie n'est pas terminée.

---

# 22. Ce que vous devez savoir expliquer à la fin

À la fin de la piste bleue, vous devez être capable d'expliquer avec vos propres mots :

- pourquoi le plateau est représenté par un tableau à deux dimensions ;
- ce que représentent `x` et `y` ;
- comment les pions sont placés au début ;
- comment une boucle permet de parcourir le plateau ;
- comment une condition détermine si un déplacement est possible ;
- comment une méthode retourne une valeur ;
- comment une prise est détectée ;
- comment un déplacement modifie `plateauDeJeu` ;
- pourquoi l'affichage change automatiquement après la modification du tableau.

Vous n'avez pas besoin d'expliquer le fonctionnement interne de MonoGame, de `List<Coup>` ou de la structure `Coup`.

---

# 23. Conseils de travail

Travaillez une étape à la fois.

Après chaque étape :

1. compilez ;
2. lancez le programme ;
3. observez le résultat ;
4. testez plusieurs situations ;
5. corrigez avant de continuer.

La touche :

```text
R
```

permet de recommencer rapidement une partie pendant les tests.

La touche :

```text
Échap
```

permet de quitter le programme.

---

# 24. Rappel : ce qui est évalué dans la piste bleue

Le but n'est pas de savoir programmer MonoGame.

Le but est de montrer que vous savez utiliser ensemble :

```text
variables
+
conditions
+
boucles
+
tableaux
+
méthodes
```

Le projet graphique sert uniquement à donner un résultat concret et visible à votre code.
