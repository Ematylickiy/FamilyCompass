# Family Transactions Design

## Context

The project already supports:
- authenticated users;
- family creation/listing/deletion;
- a basic transactions module that is currently global and not scoped to a family.

Goal: add a family-focused transactions workflow where the user can open a specific family, see its transaction history, and manage transactions.

Confirmed product decision for MVP permissions:
- policy `A`: any family member can create, edit, and delete any transaction inside that family.

## Scope

### In scope

1. Family selection and navigation to a dedicated family transactions screen.
2. Family transaction history view.
3. Create transaction with fields:
   - amount;
   - type (income/expense);
   - category (hardcoded for now);
   - date;
   - performed-by user (selected from family members if multiple users);
   - optional note.
4. Edit transaction.
5. Delete transaction.
6. Family membership-based access control on all transaction actions.
7. UX and product additions requested as "ideas":
   - filters by period/type/member/category;
   - totals for selected period (income/expense/balance);
   - delete confirmation;
   - prevent double-submit;
   - optimistic update behavior where safe;
   - basic pagination support for longer history;
   - optional business validation: prevent future dates.

### Out of scope (next phases)

- persisted categories table and category CRUD;
- advanced roles/permissions (`B`/`C`);
- attachments, recurring transactions, budgets, export/import.

## Architecture Choice

Selected approach: **balanced family-nested API**.

Why:
- explicit family context in URL;
- cleaner ownership and authorization boundaries;
- aligns with existing families module;
- easy extension to stricter permission policies later.

## Backend Design

### API Endpoints

Transactions (family-scoped):
- `GET /api/v1/families/{familyId}/transactions`
- `POST /api/v1/families/{familyId}/transactions`
- `PUT /api/v1/families/{familyId}/transactions/{transactionId}`
- `DELETE /api/v1/families/{familyId}/transactions/{transactionId}`

Family members (for performed-by selector):
- `GET /api/v1/families/{familyId}/members`

Optional query params for history:
- `from`, `to`, `type`, `category`, `performedByUserId`, `page`, `pageSize`.

### Transaction Data Model

Required business fields:
- `FamilyId` (`Guid`);
- `Amount` (`decimal`, `> 0`);
- `Type` (`income|expense`);
- `Category` (`string`, hardcoded set validated server-side);
- `Date` (`DateTimeOffset` or `DateOnly` + mapping choice consistent across API);
- `PerformedByUserId` (`Guid`);
- `Note` (`string?`, optional).

Audit and evolution fields (added now to avoid migration pain):
- `CreatedByUserId` (`Guid`);
- `CreatedAt` (`DateTimeOffset`);
- `UpdatedByUserId` (`Guid?`);
- `UpdatedAt` (`DateTimeOffset?`).

### Authorization Rules (MVP A)

For each endpoint:
1. Resolve current user from claims.
2. Validate user is a member of `familyId`.
3. Execute operation.

Responses:
- `401` when unauthenticated/invalid token claims;
- `403` when user is not a member of the family;
- `404` when family or transaction does not exist in scoped context.

### Validation Rules

- `amount > 0`;
- `category` is one of hardcoded values;
- `performedByUserId` belongs to the same family;
- `date` valid format;
- optional rule: reject future `date` (enabled in MVP as requested idea);
- `note` length capped (for example 500 or existing project style).

### Categories (Hardcoded for MVP)

Expense:
- `Продукты`, `Транспорт`, `ЖКХ`, `Здоровье`, `Развлечения`, `Другое`

Income:
- `Зарплата`, `Подарок`, `Подработка`, `Кэшбек`, `Другое`

Backend keeps authoritative allowlist; frontend mirrors for UX.

## Frontend Design

### Navigation and Pages

1. `HomePage` family cards get an `Открыть` action.
2. New route/page: `/families/:familyId` (FamilyTransactionsPage).

FamilyTransactionsPage includes:
- family header (name, role, back);
- filters panel;
- period totals card;
- create/edit transaction form;
- transaction history list with pagination controls.

### Transaction Form

Fields:
- amount input;
- type selector;
- category selector (depends on selected type);
- date picker;
- member selector (`performedByUserId`);
- optional note.

Behavior:
- one form supports create and edit modes;
- disable submit while request is in progress;
- show server errors inline.

### Transaction List

Each item shows:
- signed amount (+/-);
- type/category/date;
- performer name;
- note if present;
- edit/delete actions.

Delete action:
- confirmation modal/dialog before API call.

### Filtering and Totals

Filters state:
- period (`from`, `to`);
- `type`;
- `category`;
- `performedByUserId`.

Totals panel computed from filtered dataset or API summary response:
- total income;
- total expense;
- balance.

For MVP simplicity:
- compute totals on frontend from loaded page only, OR
- return totals with paged response.

Recommended: include totals in API response for correctness under pagination.

### Optimistic UI Strategy

- create: append temporary/local state only after API success (safer);
- edit: optimistic local patch with rollback on error;
- delete: optimistic remove with rollback on error.

If rollback complexity becomes high, fallback to "action pending" + refetch.

## Data Flow

1. User opens family page by `familyId`.
2. Frontend loads:
   - family members;
   - transactions list with default filters/page;
   - totals (same query context).
3. Create/edit/delete calls update list and totals state.
4. Filter changes trigger re-fetch with debounced query sync.

## Error Handling

Backend:
- structured errors with stable messages/codes.

Frontend:
- global banner for page-level failures;
- field-level messages for validation;
- clear empty states for no transactions.

## Testing Strategy

Backend:
- service tests for membership checks and validations;
- controller tests for status codes and route behavior;
- repository query tests for family scoping and filters.

Frontend:
- component tests:
  - form validation;
  - edit/delete flows;
  - filters and totals rendering;
- integration test for page happy-path with mocked API.

Manual smoke:
- open family;
- create transaction;
- edit transaction;
- delete transaction;
- filter transactions;
- verify totals update;
- verify non-member cannot access.

## Incremental Delivery Plan (Implementation Order)

1. Backend model + migration updates (family scope, performer, audit).
2. Backend endpoints (family-scoped list/create/update/delete + members list).
3. Frontend API layer updates.
4. New family transactions page + route + "open family" action.
5. Create/edit form and delete confirmation.
6. Filters, totals, pagination.
7. Final polish and tests.

## Risks and Mitigations

- Risk: duplicated category lists frontend/backend diverge.
  - Mitigation: shared constants contract or backend endpoint for categories later.

- Risk: optimistic updates may desync totals.
  - Mitigation: rollback strategy or post-action refetch.

- Risk: pagination + local totals mismatch.
  - Mitigation: server-computed totals for active filter.

## Acceptance Criteria

1. User can choose a family and open its transactions screen.
2. User sees only transactions belonging to selected family.
3. User can create a transaction with all required fields.
4. User can edit and delete transactions.
5. User can select performer from family members.
6. Filters work for history and totals.
7. Totals are shown for selected period/filter.
8. Non-family members cannot access or mutate family transactions.
9. Categories are hardcoded and validated on both frontend/backend.
10. UI prevents accidental duplicate submits and confirms deletion.
