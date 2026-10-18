# Module Planning

L'**offre** et le **calendrier** : ce qu'un client peut réserver, quand et à quel prix.

| | |
|---|---|
| Schéma PostgreSQL | `planning` |
| Possède | circuits, formules de roulage, de stage et de baptême, options, journées de roulage, ressources (moniteurs, véhicules), créneaux |
| Contrats synchrones | `IPlanningCatalog` : « élément réservable » (statut, fenêtre, prix, options, règles d'âge, de permis, de catégorie et de bruit) · `IPlanningCalendar` |
| Lit | `IBookingInventory` (avant de réduire une capacité), `IPaymentAccounts` (pas de publication sans compte de paiement actif) |
| Émet | `TrackDayPublished`, `CourseSlotPublished`, `TrackDayCapacityChanged`, `CourseSlotCapacityChanged`, `TrackDayCancelled`, `CourseSlotCancelled` |
| Consomme | — |

Points techniques : machine à états des journées, création de créneaux en série avec rapport de conflits,
**contrainte d'exclusion PostgreSQL** (`EXCLUDE USING gist` sur un `tstzrange`) : un moniteur ou un véhicule ne peut
pas être réservé sur deux créneaux qui se chevauchent.
