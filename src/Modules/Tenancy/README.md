# Module Tenancy

Les **organisateurs** (circuits, clubs, écoles) : le cœur du multi-tenant et de la marque blanche.

| | |
|---|---|
| Schéma PostgreSQL | `tenancy` |
| Possède | organisateurs, domaines, réglages (barème d'annulation, durée de la réservation provisoire), documents légaux versionnés, miroir du personnel, provisionnement Keycloak |
| Contrat synchrone | `ITenantDirectory` : organisateur par slug ou par domaine, réglages, versions légales en vigueur |
| Émet | `TenantProvisioned`, `TenantSuspended`, `TenantActivated`, `TenantDomainsChanged` |
| Consomme | `TenantDomainsChanged` (mise à jour des redirections OIDC dans Keycloak) |

Points techniques : résolution de l'organisateur par domaine (marque blanche), un realm Keycloak par organisateur
créé par l'Admin API, provisionnement en étapes persistées et rejouables.
