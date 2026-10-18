# Module Customers

Les **clients et pilotes** d'un organisateur. Un même pilote qui réserve chez deux organisateurs a deux comptes
distincts.

| | |
|---|---|
| Schéma PostgreSQL | `customers` |
| Possède | profils clients, véhicules (garage) |
| Contrat synchrone | `ICustomerDirectory` : profil complet, véhicule |
| Émet | `CustomerDeleted` |
| Consomme | — |

Points techniques : minimisation des données personnelles, suppression de compte par anonymisation (les factures
restent intactes), aucune donnée personnelle dans les logs.
