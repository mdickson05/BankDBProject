﻿using BankWebService.Models;
using DatabaseLib;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BankWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly SqliteDataAccess _dataAccess;

        public TransactionController(IConfiguration configuration)
        {
            _dataAccess = new SqliteDataAccess(configuration);
        }

        // Deposit into an account
        [HttpPost("deposit")]
        public IActionResult Deposit([FromBody] TransactionRequest request)
        {
            try
            {
                if (request.RecipientAccount == null)
                {
                    return BadRequest("Recipient account is required for a deposit.");
                }

                _dataAccess.Deposit(request.RecipientAccount.Value, request.Amount);

                return Ok($"Successfully deposited {request.Amount} into account {request.RecipientAccount}.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Withdraw from an account
        [HttpPost("withdraw")]
        public IActionResult Withdraw([FromBody] TransactionRequest request)
        {
            try
            {
                if (request.SenderAccount == null)
                {
                    return BadRequest("Sender account is required for a withdrawal.");
                }

                _dataAccess.Withdraw(request.SenderAccount.Value, request.Amount);

                return Ok($"Successfully withdrew {request.Amount} from account {request.SenderAccount}.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Get transaction history for an account
        [HttpGet("history/{accountNumber}")]
        public IActionResult GetTransactionHistory(int accountNumber)
        {
            try
            {
                var transactions = _dataAccess.GetTransactionsByAccountId(accountNumber);
                if (transactions == null || transactions.Count == 0)
                {
                    return NotFound("No transactions found for this account.");
                }
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
    }
}

