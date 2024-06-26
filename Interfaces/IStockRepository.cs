using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Stock;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface IStockRepository
    {
        Task<List<Stock>> GetAllAsync();
        Task<Stock?> GetByIdAsync(int id);
        Task<Stock> CreateAsync(Stock stock);
        Task<Stock?> DeleteAsync(int id);
        Task<Stock?> UpdateAsync(int id, UpdateStockDto stockDto);
        Task<bool> StockExist(int id);
        Task<List<Stock>> GetAllByQuery(QueryObject query);
        Task<Stock?> StockSymbolExist(string symbol);
    }
}