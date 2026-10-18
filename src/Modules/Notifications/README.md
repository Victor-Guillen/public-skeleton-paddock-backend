# Module Notifications

Les **emails** aux clients et au personnel, aux couleurs de chaque organisateur.

| | |
|---|---|
| Schéma PostgreSQL | `notifications` |
| Possède | journal des envois, gabarits |
| Contrat synchrone | — |
| Émet | — |
| Consomme | `TenantProvisioned`, `TenantSuspended`, `TenantActivated`, `BookingConfirmed`, `BookingCancelled`, `LatePaymentRejected`, `RefundSucceeded`, `RefundFailed`, `InvoiceIssued`, `CreditNoteIssued`, `PaymentAccountActivated`, `DisputeOpened`, `GroupAssignmentPublished` |

Points techniques : module 100 % réactif (aucun appel entrant), gabarits Liquid en marque blanche, journal d'envoi qui
garantit qu'un message rejoué n'envoie jamais deux fois le même email, Mailpit en développement.
