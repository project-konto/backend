using KontoApi.Application.DTOs;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using Microsoft.AspNetCore.Mvc;

namespace KontoApi.Api.Controllers;

public class TransactionController: BaseController
{
    private readonly ITransactionRepository _repository;

    public TransactionController(ITransactionRepository repository)
    {
        _repository = repository;
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateTransactionRequest transaction)
    {
        // TO DO
        return Ok();
    }

    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        // TO DO 
/        return Ok();
    }

    [HttpGet]
    public IActionResult Get()
    {
        // TO DO
        return Ok(new List<TransactionResponse>());    
    }
    
}