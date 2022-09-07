# S8_APP1

## Énoncé de la problématique

- Lors de vos activités de consultation en développement logiciel, la
compagnie Coup de Sonde décide de faire affaire avec vous afin d’appliquer
vos connaissances au développement d’un logiciel de sondage. Il est pri-
mordial de concevoir un logiciel intégrant les concepts de programmation
sécurisée, qui permettra de garantir la validité des résultats de votes. La com-
pagnie vous fournit les sondages en format texte.

- Le développement web a évolué dans plusieurs directions et une av-
enue populaire est le développement d’interface de programmation d’application
(API). Pour les entreprises, ceci amène des sources de revenus supplémen-
taires en vendant des accès à ces API, souvent même utilisés pour leur propre
développement d’applications. La compagnie énonce donc le requis ferme que
le logiciel de sondage possède un API évolutif auquel on pourra y intégrer des
applications externes et vendre des clefs d’API.

- La compagnie a un historique de développement avec les technologies
Microsoft, qui comprend entre autre les composants pour le développement
de services web (SOAP ou REST). Les API REST ont pris le pas aux API SOAP.
Aussi, l’avènement de .Net Core a causé un changement de perspective in-
téressant dans le développement logiciel en .Net, entre autres dans la gestion
des packages et de l’exécution sur des systèmes d’exploitation autres que Win-
dows. La compagnie voulant rester à l’avant-garde, l’application de sondage
devra donc comprendre un API REST en .Net Core, véhiculant l’information
dans le format JSON. Pour des fins de démonstration, utilisez l’outil Post-
man pour interagir avec votre API. Vous devrez composer une collection de
requêtes servant à démontrer que les différents cas d’utilisation sont couverts.

- Le langage C# et le langage phare du framework .Net de Microsoft.
C’est un langage géré, donc plusieurs pièges de programmation, tels que les
dépassements de tampons et d’entiers sont gérés de façon interne. Cependant,
d’autres pièges doivent être gérés par le programmeur. Pour ces raisons, la
compagnie vous demande donc que tout le développement se fasse en C#.

- Il est très important de s’assurer que le code qui sera produit soit testé
de façon convenable. La livraison de votre code devra contenir des tests uni-
taires développés à l’aide du framework xUnit et montrer une couverture de
code de 100%.

- Pour les langages non-gérés, Microsoft a intégré des mécanismes de
protection, tels que l’empêchement de l’exécution de la pile. Leur système
d’exploitation Windows intègre aussi des mécanismes de sécurité variés selon
les versions, mais celui qui est le plus intéressant pour les programmeurs est
sans doute la mise en mémoire (sur tas) aléatoire, ou Address Space Layout
Randomization (ASLR). D’autres systèmes d’exploitation intègrent une forme
ou une autre d’ASLR. Étant donné que les applications en .Net Core peuvent
être exécutées sur différents systèmes d’exploitation, vous devrez en recom-
mander un au point de vue des mécanismes de sécurité qui y sont intégrés.

- Vous sentez que votre client a fait quelque recherches en amont lorsqu’il
vous fait mention des ses requis au niveau de la sécurité. Le projet devra
adopter le processus du Security Development Lifecycle. Les pratiques asso-
ciées à chaque étape devront aussi être étudiées, telles que les analyses de la
menace, en utilisant la méthode STRIDE, et les vecteurs d’attaque à la concep-
tion et leur identification à l’aide de diagrammes de flux de données (DFD),
l’analyse statique à la réalisation et les pratiques de vérification comme les
tests dynamique et de fuzzing. Cependant, avant toute chose, il est pertinent
de bien cerner les besoins en sécurité que l’application nécessitera. Ils sont
aussi conscients qu’ils pourraient se faire voler leur propriété intellectuelle et
veulent qui vous recommandiez la configuration à utiliser avec Dotfuscator.

### Il vous faudra donc fournir les livrables suivants:
1. un API sécurisé par https et dont l’autorisation se fait par une clef d’accès;

2. une documentation d’API suivant le standard OpenAPI dont l’intégration
est faite en utilisant Swagger;

3. une collection de requêtes Postman démontrant les différentes fonction-
nalités de votre API, liée à votre schéma d’API OpenAPI;

4. une authentication des participants pour garantir l’unicité de leur par-
ticipation;

5. une batterie de tests automatisés xUnit en .Net 6.0 inclus dans la même
solution que le projet d’API qui prouvera une couverture complète du
code et l’intégration des mesures de mitigation proposées lors de l’analyse
de la surface d’attaque;

6. une analyse d’impact de la sécurité, comprenant une analyse des vecteurs
d’attaque;

7. inclure des mécanismes de sécurité lors de la programmation, tels que
l’empêchement de l’exécution de la pile, les options d’ajouts de robustesse
fournis par Visual Studio;

8. les techniques de protection du code par l’obfuscation et la configuration
à utiliser;

9. les recommandations au sujet de l’opération de l’API (architecture sys-
tème); et

10. un processus d’identification et de publication des bogues de sécurité.

Notez que pour alléger le développement, une simple persistence des données
de sondage dans un système de fichiers (questions et réponses) est requise.


## Rapport d’APP

## Consignes pour la préparation du dépôt du rapport et de la solution à la problématique

Documents à remettre avant le début de la première rencontre du deuxième tutorat, sur le site de dépôt de travaux.

Voici les documents à rendre, le tout dans un seul fichier zip, selon l’arborescence
suivante:
- répertoire rapport: un document PDF, nommé de par les CIP des membres
de l’équipe et séparés par des traits d’union (e.g. : abcd1234-efgh5678.pdf)
et comprenant la modélisation de la menace et la description des mesures
de mitigation mises en place pour chacun des vecteurs d’attaque selon la
disposition suivante:
- une page contenant une image du diagramme de flux de données prise
de l’outil de modélisation et la justification des frontières de confiance;
- une page par vecteur d’attaque incluant:

    * la description de l’attaque

    * l’impact sur l’application

    * la mesure de mitigation intégrée1

    * la référence dans le document de modélisation

- répertoire src: le code source (i.e. excluant les artéfacts de compilation)
contenu dans une solution Visual Studio 2019, laquelle contient:

- le project Visual Studio 2019 d’API web ASP.NET 5.0 incluant l’obfuscation
par Obsfuscar du code compilé en configuration Release

- la batterie de tests automatisés dans un projet Visual Studio 2019
xUnit .Net 5.0

- répertoire postman: la collection Postman exportée contenant les requêtes
pour les cas d’utilisation de l’application

- répertoire modélisation: l’exportation (en HTML) du résultat de l’outil de
modélisation

## Modalités d’évaluation du rapport d’APP
L’évaluation du rapport d’APP contribue à l’évaluation des éléments de compé-
tence de l’unité (voir section suivante). On évalue l’exactitude, la précision, la
complétude, la valeur de chaque élément de solution.
La qualité de la communication ne sera pas évaluée de façon sommative mais si
le rapport est fautif sur le plan de la qualité de l’écrit et de la présentation, il vous
sera retourné pour correction avant qu’il ne soit noté.
