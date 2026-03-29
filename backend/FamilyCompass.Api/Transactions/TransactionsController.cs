using FamilyCompass.Application.Transactions.DTOs;
using FamilyCompass.Application.Transactions.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FamilyCompass.Api.Transactions;

[ApiController]
[Route("api/v1/transactions")]
public class TransactionsController(ITransactionService service) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(service.GetAll());
    }

    [HttpPost]
    public IActionResult Create(CreateTransactionRequest request)
    {
        var created = service.Create(request);
        return Created($"/api/v1/transactions/{created.Id}", created);
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        return service.Delete(id)
            ? NoContent()
            : NotFound();
    }
}