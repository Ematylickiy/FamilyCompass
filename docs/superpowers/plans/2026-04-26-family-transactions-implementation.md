# Family Transactions Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build family-scoped transaction history and full transaction CRUD (create/edit/delete) with performer selection, filtering, totals, and pagination-ready API/UI.

**Architecture:** Move transactions from global scope to family-nested endpoints (`/families/{familyId}/transactions`) with membership checks at service layer. Add a dedicated family page in frontend that loads members, transactions, and totals in one flow. Keep categories hardcoded for MVP but validate server-side.

**Tech Stack:** ASP.NET Core + Application/Infrastructure layers + EF Core + PostgreSQL, React + TypeScript + react-router-dom.

---

### Task 1: Extend Transaction Domain and EF Mapping

**Files:**
- Modify: `backend/FamilyCompass.Domain/Entities/Transaction.cs`
- Modify: `backend/FamilyCompass.Infrastructure/Persistence/Transactions/TransactionConfiguration.cs`
- Modify: `backend/FamilyCompass.Infrastructure/Shared/Persistence/FamilyCompassDbContext.cs` (only if needed for navigation mapping)
- Create: `backend/FamilyCompass.Infrastructure/Migrations/<timestamp>_FamilyScopedTransactions.cs`

- [ ] **Step 1: Write failing migration generation command**

Run: `dotnet ef migrations add FamilyScopedTransactions --project backend/FamilyCompass.Infrastructure --startup-project backend/FamilyCompass.Api`

Expected: fail first if model is unchanged (no-op) or compile errors after partial edits.

- [ ] **Step 2: Add family/performer/audit fields to `Transaction` entity**

```csharp
public Guid FamilyId { get; private set; }
public Guid PerformedByUserId { get; private set; }
public Guid CreatedByUserId { get; private set; }
public DateTime UpdatedAt { get; private set; }
public Guid? UpdatedByUserId { get; private set; }

public static Transaction Create(
    Guid familyId,
    decimal amount,
    TransactionType type,
    string category,
    DateTime date,
    Guid performedByUserId,
    Guid createdByUserId,
    string? note)
{
    // validate here and normalize
}

public void Update(
    decimal amount,
    TransactionType type,
    string category,
    DateTime date,
    Guid performedByUserId,
    Guid updatedByUserId,
    string? note)
{
    // mutate and stamp UpdatedAt/UpdatedByUserId
}
```

- [ ] **Step 3: Update EF configuration for new required columns and indexes**

```csharp
builder.Property(t => t.FamilyId).IsRequired();
builder.Property(t => t.PerformedByUserId).IsRequired();
builder.Property(t => t.CreatedByUserId).IsRequired();
builder.Property(t => t.UpdatedAt).IsRequired();
builder.HasIndex(t => new { t.FamilyId, t.Date });
```

- [ ] **Step 4: Generate and inspect migration**

Run: `dotnet ef migrations add FamilyScopedTransactions --project backend/FamilyCompass.Infrastructure --startup-project backend/FamilyCompass.Api`

Expected: migration adds new columns, FK constraints (to `Families`/`Users` where applicable), and index on family/date.

- [ ] **Step 5: Apply migration locally**

Run: `dotnet ef database update --project backend/FamilyCompass.Infrastructure --startup-project backend/FamilyCompass.Api`

Expected: `Done.` with no SQL errors.

- [ ] **Step 6: Commit**

```bash
git add backend/FamilyCompass.Domain/Entities/Transaction.cs backend/FamilyCompass.Infrastructure/Persistence/Transactions/TransactionConfiguration.cs backend/FamilyCompass.Infrastructure/Migrations
git commit -m "feat: scope transactions to families in domain model"
```

### Task 2: Add Family-Scoped Transaction DTOs and Contracts

**Files:**
- Modify: `backend/FamilyCompass.Application/Transactions/DTOs/CreateTransactionRequest.cs`
- Create: `backend/FamilyCompass.Application/Transactions/DTOs/UpdateTransactionRequest.cs`
- Modify: `backend/FamilyCompass.Application/Transactions/DTOs/TransactionResponse.cs`
- Create: `backend/FamilyCompass.Application/Transactions/DTOs/TransactionsQuery.cs`
- Create: `backend/FamilyCompass.Application/Transactions/DTOs/PagedTransactionsResponse.cs`

- [ ] **Step 1: Add failing compile state by updating interface references**

```csharp
// ITransactionService should reference Update + family-scoped methods
Task<PagedTransactionsResponse> GetByFamilyAsync(Guid familyId, Guid currentUserId, TransactionsQuery query, CancellationToken ct);
```

Expected: compile fails until DTOs are added.

- [ ] **Step 2: Define create/update request contracts with performer**

```csharp
public sealed record CreateTransactionRequest(
    decimal Amount,
    TransactionType Type,
    string Category,
    DateTime Date,
    Guid PerformedByUserId,
    string? Note);
```

```csharp
public sealed record UpdateTransactionRequest(
    decimal Amount,
    TransactionType Type,
    string Category,
    DateTime Date,
    Guid PerformedByUserId,
    string? Note);
```

- [ ] **Step 3: Define paged list response with totals**

```csharp
public sealed record PagedTransactionsResponse(
    IReadOnlyList<TransactionResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    decimal TotalIncome,
    decimal TotalExpense,
    decimal Balance);
```

- [ ] **Step 4: Build solution**

Run: `dotnet build backend/FamilyCompass.Api/FamilyCompass.Api.csproj`

Expected: build still fails on service/repository not updated yet (acceptable checkpoint).

- [ ] **Step 5: Commit**

```bash
git add backend/FamilyCompass.Application/Transactions/DTOs backend/FamilyCompass.Application/Transactions/Interfaces/ITransactionService.cs
git commit -m "feat: define family transaction contracts with pagination totals"
```

### Task 3: Implement Family Membership Checks + Transaction Service Logic

**Files:**
- Modify: `backend/FamilyCompass.Application/Transactions/Interfaces/ITransactionService.cs`
- Modify: `backend/FamilyCompass.Application/Transactions/Services/TransactionService.cs`
- Modify: `backend/FamilyCompass.Application/Transactions/Interfaces/ITransactionRepository.cs`
- Create: `backend/FamilyCompass.Application/Transactions/Exceptions/TransactionNotFoundException.cs`
- Create: `backend/FamilyCompass.Application/Transactions/Exceptions/InvalidTransactionCategoryException.cs`
- Reuse: `backend/FamilyCompass.Application/Families/Interfaces/IFamilyMembershipRepository.cs`

- [ ] **Step 1: Add category allowlists and validation helpers**

```csharp
private static readonly HashSet<string> ExpenseCategories = ["Продукты", "Транспорт", "ЖКХ", "Здоровье", "Развлечения", "Другое"];
private static readonly HashSet<string> IncomeCategories = ["Зарплата", "Подарок", "Подработка", "Кэшбек", "Другое"];
```

- [ ] **Step 2: Implement membership guard in service**

```csharp
private async Task EnsureFamilyMemberAsync(Guid familyId, Guid userId, CancellationToken ct)
{
    var membership = await membershipRepository.GetByFamilyAndUserIdAsync(familyId, userId, ct);
    if (membership is null) throw new InsufficientFamilyPermissionsException();
}
```

- [ ] **Step 3: Implement CRUD methods with family-scoped repository calls**

```csharp
public async Task<TransactionResponse> CreateAsync(Guid familyId, Guid currentUserId, CreateTransactionRequest request, CancellationToken ct)
{
    await EnsureFamilyMemberAsync(familyId, currentUserId, ct);
    await EnsurePerformerInFamilyAsync(familyId, request.PerformedByUserId, ct);
    ValidateRequest(request.Amount, request.Type, request.Category, request.Date);
    var entity = Transaction.Create(familyId, request.Amount, request.Type, request.Category, request.Date, request.PerformedByUserId, currentUserId, request.Note);
    var created = await repo.AddAsync(entity, ct);
    await unitOfWork.SaveChangesAsync(ct);
    return Map(created);
}
```

- [ ] **Step 4: Add optional future-date rejection**

```csharp
if (date.Date > DateTime.UtcNow.Date)
    throw new ArgumentException("Transaction date cannot be in the future.");
```

- [ ] **Step 5: Build backend**

Run: `dotnet build backend/FamilyCompass.Api/FamilyCompass.Api.csproj`

Expected: pass once repository/controller are updated, otherwise continue to Task 4.

- [ ] **Step 6: Commit**

```bash
git add backend/FamilyCompass.Application/Transactions
git commit -m "feat: add family-aware transaction service with membership checks"
```

### Task 4: Implement Repository Queries and Pagination/Totals

**Files:**
- Modify: `backend/FamilyCompass.Infrastructure/Persistence/Transactions/TransactionRepository.cs`
- Modify: `backend/FamilyCompass.Application/Transactions/Interfaces/ITransactionRepository.cs`

- [ ] **Step 1: Add family-scoped query method signatures**

```csharp
Task<(IReadOnlyList<Transaction> Items, int TotalCount, decimal TotalIncome, decimal TotalExpense)> GetByFamilyAsync(Guid familyId, TransactionsQuery query, CancellationToken ct);
Task<Transaction?> GetByIdAsync(Guid familyId, Guid transactionId, CancellationToken ct);
Task<Transaction> AddAsync(Transaction transaction, CancellationToken ct);
Task RemoveAsync(Transaction transaction, CancellationToken ct);
```

- [ ] **Step 2: Implement filtered query**

```csharp
var q = db.Transactions.AsNoTracking().Where(t => t.FamilyId == familyId);
if (query.From is not null) q = q.Where(t => t.Date >= query.From.Value);
if (query.To is not null) q = q.Where(t => t.Date <= query.To.Value);
if (query.PerformedByUserId is not null) q = q.Where(t => t.PerformedByUserId == query.PerformedByUserId.Value);
```

- [ ] **Step 3: Compute totals from filtered query before pagination**

```csharp
var totalIncome = await q.Where(t => t.Type == TransactionType.Income).SumAsync(t => t.Amount, ct);
var totalExpense = await q.Where(t => t.Type == TransactionType.Expense).SumAsync(t => t.Amount, ct);
```

- [ ] **Step 4: Paginate and sort by date/created**

```csharp
var items = await q.OrderByDescending(t => t.Date).ThenByDescending(t => t.CreatedAt)
    .Skip((query.Page - 1) * query.PageSize)
    .Take(query.PageSize)
    .ToListAsync(ct);
```

- [ ] **Step 5: Build backend**

Run: `dotnet build backend/FamilyCompass.Api/FamilyCompass.Api.csproj`

Expected: success.

- [ ] **Step 6: Commit**

```bash
git add backend/FamilyCompass.Infrastructure/Persistence/Transactions backend/FamilyCompass.Application/Transactions/Interfaces/ITransactionRepository.cs
git commit -m "feat: support filtered paged family transaction queries"
```

### Task 5: Replace Transactions Controller with Family-Nested Endpoints + Members Endpoint

**Files:**
- Modify: `backend/FamilyCompass.Api/Transactions/TransactionsController.cs`
- Create: `backend/FamilyCompass.Api/Families/Contracts/FamilyMemberResponse.cs`
- Modify: `backend/FamilyCompass.Api/Families/FamiliesController.cs` (add `GET {familyId}/members`)
- Modify: `backend/FamilyCompass.Api/Program.cs` (DI updates if required)

- [ ] **Step 1: Introduce family-nested routes**

```csharp
[Route("api/v1/families/{familyId:guid}/transactions")]
public class TransactionsController : ControllerBase
{
    [HttpGet]
    public async Task<IResult> Get(Guid familyId, [FromQuery] TransactionsQueryRequest query, CancellationToken ct) { ... }
}
```

- [ ] **Step 2: Add create/update/delete actions mapped to service exceptions**

```csharp
catch (InsufficientFamilyPermissionsException) { return Results.Forbid(); }
catch (TransactionNotFoundException) { return Results.NotFound(); }
catch (ArgumentException ex) { return Results.BadRequest(new { error = ex.Message }); }
```

- [ ] **Step 3: Add `GET /families/{familyId}/members` response contract**

```csharp
public sealed record FamilyMemberResponse(Guid UserId, string Email, string Role);
```

- [ ] **Step 4: Build backend**

Run: `dotnet build backend/FamilyCompass.Api/FamilyCompass.Api.csproj`

Expected: success.

- [ ] **Step 5: Commit**

```bash
git add backend/FamilyCompass.Api/Transactions/TransactionsController.cs backend/FamilyCompass.Api/Families backend/FamilyCompass.Api/Program.cs
git commit -m "feat: expose family-scoped transactions and members endpoints"
```

### Task 6: Frontend API and Types for Family Transactions

**Files:**
- Modify: `frontend/src/features/transactions/types.ts`
- Modify: `frontend/src/features/transactions/api.ts`
- Modify: `frontend/src/features/transactions/hooks/useTransactions.ts`
- Create: `frontend/src/features/families/membersApi.ts` (or extend existing families api)

- [ ] **Step 1: Replace flat transaction contracts with family-scoped types**

```ts
export interface Transaction {
  id: string;
  familyId: string;
  amount: number;
  type: TransactionType;
  category: string;
  date: string;
  performedByUserId: string;
  performedByUserName: string;
  note?: string | null;
}
```

- [ ] **Step 2: Add paged response + filters contract**

```ts
export interface TransactionsQuery {
  page: number;
  pageSize: number;
  from?: string;
  to?: string;
  type?: TransactionType;
  category?: string;
  performedByUserId?: string;
}
```

- [ ] **Step 3: Update API client methods**

```ts
getByFamily: (familyId: string, query: TransactionsQuery) =>
  client.get<PagedTransactionsResponse>(`families/${familyId}/transactions`, { params: query }),
create: (familyId: string, data: CreateTransactionRequest) =>
  client.post<Transaction>(`families/${familyId}/transactions`, data),
update: (familyId: string, id: string, data: UpdateTransactionRequest) =>
  client.put<Transaction>(`families/${familyId}/transactions/${id}`, data),
remove: (familyId: string, id: string) =>
  client.delete(`families/${familyId}/transactions/${id}`),
```

- [ ] **Step 4: Add members API**

```ts
getMembers: (familyId: string) => client.get<FamilyMember[]>(`families/${familyId}/members`)
```

- [ ] **Step 5: Run frontend typecheck**

Run: `npm --prefix frontend run build`

Expected: may fail until page wiring in Task 7 is complete.

- [ ] **Step 6: Commit**

```bash
git add frontend/src/features/transactions frontend/src/features/families
git commit -m "feat: add family-scoped transactions frontend API contracts"
```

### Task 7: Add Family Transactions Page with Create/Edit/Delete

**Files:**
- Create: `frontend/src/pages/FamilyTransactionsPage.tsx`
- Create: `frontend/src/pages/FamilyTransactionsPage.module.css`
- Modify: `frontend/src/App.tsx`
- Modify: `frontend/src/pages/HomePage.tsx`
- Modify: `frontend/src/features/transactions/components/TransactionsList.tsx`
- Create: `frontend/src/features/transactions/components/TransactionForm.tsx`

- [ ] **Step 1: Add route and navigation from home**

```tsx
<Route path="/families/:familyId" element={<FamilyTransactionsPage />} />
```

```tsx
<Button type="button" variant="secondary" onClick={() => navigate(`/families/${family.id}`)}>
  Открыть
</Button>
```

- [ ] **Step 2: Implement page data loading**

```tsx
const { familyId } = useParams();
const [members, setMembers] = useState<FamilyMember[]>([]);
const [query, setQuery] = useState<TransactionsQuery>({ page: 1, pageSize: 20 });
useEffect(() => { void loadMembersAndTransactions(); }, [familyId, query]);
```

- [ ] **Step 3: Implement create/edit shared form with double-submit guard**

```tsx
const [submitting, setSubmitting] = useState(false);
if (submitting) return;
setSubmitting(true);
try { await onSubmit(payload); } finally { setSubmitting(false); }
```

- [ ] **Step 4: Add delete confirmation and optimistic UI with rollback**

```tsx
const confirmed = window.confirm('Удалить транзакцию?');
if (!confirmed) return;
const snapshot = items;
setItems((prev) => prev.filter((x) => x.id !== id));
try { await removeTransaction(id); } catch { setItems(snapshot); }
```

- [ ] **Step 5: Add performer display and edit button to list items**

```tsx
<span>{t.performedByUserName}</span>
<Button type="button" variant="ghost" onClick={() => onEdit(t)}>Редактировать</Button>
```

- [ ] **Step 6: Build frontend**

Run: `npm --prefix frontend run build`

Expected: success.

- [ ] **Step 7: Commit**

```bash
git add frontend/src/pages frontend/src/features/transactions frontend/src/App.tsx
git commit -m "feat: add family transactions page with create edit delete flows"
```

### Task 8: Add Filters, Totals, and Pagination Controls

**Files:**
- Modify: `frontend/src/pages/FamilyTransactionsPage.tsx`
- Create: `frontend/src/features/transactions/components/TransactionFilters.tsx`
- Create: `frontend/src/features/transactions/components/TransactionsTotals.tsx`
- Modify: `frontend/src/features/transactions/components/TransactionsList.tsx`

- [ ] **Step 1: Add filters UI and bind to query**

```tsx
<TransactionFilters
  value={query}
  members={members}
  onChange={(next) => setQuery((prev) => ({ ...prev, ...next, page: 1 }))}
/>
```

- [ ] **Step 2: Render totals from API response**

```tsx
<TransactionsTotals
  totalIncome={data.totalIncome}
  totalExpense={data.totalExpense}
  balance={data.balance}
/>
```

- [ ] **Step 3: Add pagination controls**

```tsx
const totalPages = Math.max(1, Math.ceil(totalCount / query.pageSize));
<Button onClick={() => setQuery((q) => ({ ...q, page: q.page - 1 }))} disabled={query.page <= 1}>Назад</Button>
<Button onClick={() => setQuery((q) => ({ ...q, page: q.page + 1 }))} disabled={query.page >= totalPages}>Вперёд</Button>
```

- [ ] **Step 4: Build frontend**

Run: `npm --prefix frontend run build`

Expected: success.

- [ ] **Step 5: Commit**

```bash
git add frontend/src/features/transactions/components frontend/src/pages/FamilyTransactionsPage.tsx
git commit -m "feat: add transaction filters totals and pagination UI"
```

### Task 9: Verification and Regression Checks

**Files:**
- Modify: `docs/superpowers/specs/2026-04-26-family-transactions-design.md` (only if implementation decisions changed)
- Optional Create: `docs/superpowers/notes/2026-04-26-family-transactions-smoke.md`

- [ ] **Step 1: Run backend build**

Run: `dotnet build backend/FamilyCompass.Api/FamilyCompass.Api.csproj`

Expected: success.

- [ ] **Step 2: Run frontend build**

Run: `npm --prefix frontend run build`

Expected: success.

- [ ] **Step 3: Smoke test with running app**

Run:
- `dotnet run --project backend/FamilyCompass.Api/FamilyCompass.Api.csproj`
- `npm --prefix frontend run dev`

Manual checks:
- open family;
- create/edit/delete transaction;
- member selector works;
- filters and totals update;
- non-member gets forbidden response.

- [ ] **Step 4: Commit final polish**

```bash
git add .
git commit -m "chore: finalize family transaction flow and verification"
```
