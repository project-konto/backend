using KontoApi.Application.DTOs;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using Microsoft.AspNetCore.Mvc;

namespace KontoApi.Api.Controllers;

public class TransactionController(ITransactionRepository repository) : BaseController
{
	private readonly ITransactionRepository repository = repository;

	[HttpPost]
	public IActionResult Create([FromBody] CreateTransactionRequest transaction)
	{
		// TODO
		return Ok();
	}

	[HttpGet("{id}")]
	public IActionResult Get(Guid id)
	{
		// TODO 
		return Ok();
	}

	[HttpGet]
	public IActionResult Get()
	{
		// TODO
		return Ok(new List<TransactionResponse>());
	}
}