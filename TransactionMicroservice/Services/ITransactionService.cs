using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBusiness.TransactionMicroservice.DTO;

namespace MyBusiness.TransactionMicroservice.Services
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDTO>> GetAllTransactionsAsync();
        Task<TransactionDTO> GetTransactionByIdAsync(int transactionId);
        Task<TransactionDTO> CreateTransactionAsync(TransactionDTO transactionDTO);
        Task<TransactionDTO> UpdateTransactionAsync(TransactionDTO transactionDTO);
        Task<bool> DeleteTransactionAsync(int transactionId);
    }
}