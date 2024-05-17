using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyBusiness.TransactionMicroservice.DTO;
using MyBusiness.TransactionMicroservice.Services;

namespace MyBusiness.TransactionMicroservice.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] TransactionDTO transactionDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var createdTransaction = await _transactionService.CreateTransactionAsync(transactionDTO);
            return Ok(createdTransaction);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTransactions()
    {
        try
        {
            var transactions = await _transactionService.GetAllTransactionsAsync();
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{transactionId}")]
    public async Task<IActionResult> GetTransactionById(int transactionId)
    {
        try
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(transactionId);
            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTransaction([FromBody] TransactionDTO transactionDTO)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var existingTransaction = await _transactionService.GetTransactionByIdAsync(transactionDTO.TransactionId);
            if (existingTransaction == null)
            {
                return NotFound();
            }

            var updatedTransaction = await _transactionService.UpdateTransactionAsync(transactionDTO);

            return Ok(updatedTransaction);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("{transactionId}")]
    public async Task<IActionResult> DeleteTransaction(int transactionId)
    {
        try
        {
            var result = await _transactionService.DeleteTransactionAsync(transactionId);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
    }
}