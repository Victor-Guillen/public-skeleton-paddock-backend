# Module Operations

Le **jour J** : accueil des pilotes, groupes de niveau, chronos et exports.

| | |
|---|---|
| Schéma PostgreSQL | `operations` |
| Possède | listes de participants, numéros de course, billets et check-in, groupes, répartitions, chronos, exports |
| Contrat synchrone | `IRosterQueries` |
| Émet | `GroupAssignmentPublished`, `ParticipantCheckedIn` |
| Consomme | `TrackDayPublished`, `CourseSlotPublished` (groupes, liste vide), `BookingConfirmed`, `BookingCancelled` (projection de la liste), `CustomerDeleted` |

Points techniques :

- liste des participants = **projection** alimentée par les événements de Booking ;
- billets PDF avec QR code signé, **check-in idempotent** (double scan → « déjà enregistré ») ;
- **algorithme de brassage déterministe** (service de domaine pur) : brassage initial, rebrassage par les chronos,
  rééquilibrage, verrous posés par l'organisateur ;
- import CSV des chronos avec rapport de lignes rejetées, exports CSV et XLSX en streaming.
