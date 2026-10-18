# Module Billing

Le **paiement** (Stripe Connect) et la **facturation** de chaque organisateur.

| | |
|---|---|
| Schéma PostgreSQL | `billing` |
| Possède | comptes connectés, paiements, remboursements, inbox des événements Stripe, factures, avoirs, compteurs de numérotation |
| Contrats synchrones | `IPaymentGateway` : créer et expirer une session de paiement · `IPaymentAccounts` : compte de l'organisateur actif ? |
| Émet | `PaymentSucceeded`, `RefundSucceeded`, `RefundFailed`, `InvoiceIssued`, `CreditNoteIssued`, `PaymentAccountActivated`, `DisputeOpened` |
| Consomme | `BookingConfirmed` (facture), `BookingExpired` (expiration de la session), `BookingCancelled` (remboursement, avoir), `LatePaymentRejected` (remboursement intégral), `CustomerDeleted` |

Points techniques :

- **charges directes** sur le compte Stripe de l'organisateur, via Checkout ;
- **webhook fiable** : signature vérifiée, événement inséré dans une inbox à clé unique (doublon ignoré), réponse
  immédiate, traitement asynchrone et idempotent ;
- la confirmation ne dépend **jamais** du navigateur, seulement du webhook ;
- **numérotation continue des factures** par compteur transactionnel (un rollback rend le numéro) ;
- deux passerelles : Stripe et une passerelle simulée (tests, développement sans clé).
