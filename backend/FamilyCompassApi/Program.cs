var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<ITransactionService, TransactionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/v1/transactions",
        (ITransactionService service) =>
            Results.Ok(service.GetAll()))
    .WithName("GetTransactions");

app.MapPost("/api/v1/transactions",
        (ITransactionService service, CreateTransactionRequest request) =>
            Results.Created($"/api/v1/transactions/{Guid.NewGuid()}", service.Create(request)))
    .WithName("CreateTransaction");

app.MapDelete("/api/v1/transactions/{id:guid}",
        (ITransactionService service, Guid id) =>
            service.Delete(id) ? Results.NoContent() : Results.NotFound())
    .WithName("DeleteTransaction");

app.Run();

// app.UseHttpsRedirection();

public class Transaction
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Note { get; set; }
};

public record CreateTransactionRequest(
    decimal Amount,
    string Type,
    string Category,
    DateTime Date,
    string? Note
);

public interface ITransactionService
{
    List<Transaction> GetAll();
    Transaction Create(CreateTransactionRequest request);
    bool Delete(Guid id);
}

public class TransactionService : ITransactionService
{
    private readonly List<Transaction> _transactions = [];

    public List<Transaction> GetAll() => _transactions;

    public Transaction Create(CreateTransactionRequest request)
    {
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = request.Amount,
            Type = request.Type,
            Category = request.Category,
            Date = request.Date,
            Note = request.Note,
        };

        _transactions.Add(transaction);
        return transaction;
    }

    public bool Delete(Guid id)
    {
        var transaction = _transactions.FirstOrDefault(t => t.Id == id);
        if (transaction is null) return false;
        _transactions.Remove(transaction);
        return true;
    }
}