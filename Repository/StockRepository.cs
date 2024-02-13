using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public StockRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Stock> CreateAsync(Stock stock)
        {
            await _dbContext.Stocks.AddAsync(stock);
            await _dbContext.SaveChangesAsync();
            return stock;
        }

        public async Task<Stock?> DeleteAsync(int id)
        {
            var stock = await _dbContext.Stocks.FirstOrDefaultAsync(x => x.Id == id);
            if (stock == null)
            {
                return null;
            }
            _dbContext.Stocks.Remove(stock);
            await _dbContext.SaveChangesAsync();
            return stock;
        }

        public async Task<List<Stock>> GetAllAsync()
        {
            return await _dbContext.Stocks.ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            // return await _dbContext.Stocks.FirstOrDefaultAsync(s => s.Id == id);

            var stockModel = await (from s in _dbContext.Stocks
                                    where s.Id == id
                                    select s).FirstOrDefaultAsync();
            if (stockModel == null)
            {
                return null;
            }
            return stockModel;
        }


    }
}