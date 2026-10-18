# Module Booking

La **réservation** : de la place bloquée le temps du paiement jusqu'à la confirmation, l'expiration ou l'annulation.

| | |
|---|---|
| Schéma PostgreSQL | `booking` |
| Possède | inventaires (places, stock d'options), réservations, clés d'idempotence, annulations de masse |
| Contrats synchrones | `IBookingInventory` : places réservées et restantes · `IBookingQueries` |
| Lit | `IPlanningCatalog`, `ICustomerDirectory`, `IPaymentGateway` |
| Émet | `BookingPlaced`, `BookingConfirmed`, `BookingExpired`, `BookingCancelled`, `LatePaymentRejected` |
| Consomme | `TrackDayPublished`, `CourseSlotPublished` (création de l'inventaire), `*CapacityChanged`, `*Cancelled` (annulation de masse), `PaymentSucceeded` (confirmation), `RefundSucceeded`, `RefundFailed`, `CustomerDeleted` |

Points techniques :

- **zéro survente** : décrément atomique conditionnel (`UPDATE … WHERE held + confirmed + n <= capacity`) et
  contrainte `CHECK` en dernier rempart ;
- **idempotence** de `POST /me/bookings` par l'en-tête `Idempotency-Key` ;
- expiration de la réservation provisoire par **message programmé** ;
- paiement tardif : nouvelle réservation atomique si une place reste, sinon remboursement intégral ;
- barème d'annulation calculé avec une horloge injectée (`TimeProvider`).
