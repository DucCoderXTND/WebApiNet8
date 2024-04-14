using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Stock;
using api.Helpers;
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
            return await _dbContext.Stocks.Include(c => c.Comments).ToListAsync();
        }

        public async Task<List<Stock>> GetAllByQuery(QueryObject query)
        {
            // Phương thức ThenInclude cho phép bạn bao gồm dữ liệu từ các quan hệ liên kết ngoại trừ các bảng liên kết
            var stock = _dbContext.Stocks.Include(s => s.Comments).ThenInclude(a=>a.AppUser).AsQueryable();
            if (!string.IsNullOrWhiteSpace(query.CompanyName))
            {
                stock = stock.Where(s => s.CompanyName.Contains(query.CompanyName));
            }
            if (!string.IsNullOrWhiteSpace(query.Symbol))
            {
                stock = stock.Where(s => s.Symbol.Contains(query.Symbol));
            }
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    stock = query.IsDecsending ? stock.OrderByDescending(s => s.Symbol) : stock.OrderBy(s =>s.Symbol);
                }
            }
            return await stock.ToListAsync();
        }

        public async Task<Stock?> GetByIdAsync(int id)
        {
            // return await _dbContext.Stocks.FirstOrDefaultAsync(s => s.Id == id);

            var stockModel = await (from s in _dbContext.Stocks
                                    where s.Id == id
                                    select s).Include(c => c.Comments).FirstOrDefaultAsync();
            if (stockModel == null)
            {
                return null;
            }
            return stockModel;
        }

        public async Task<bool> StockExist(int id)
        {
            return await _dbContext.Stocks.AnyAsync(x => x.Id == id);

        }

        public async Task<Stock?> StockSymbolExist(string symbol)
        {
            return await _dbContext.Stocks.FirstOrDefaultAsync(x=>x.Symbol == symbol);
        }

        public async Task<Stock?> UpdateAsync(int id, UpdateStockDto stockDto)
        {
            var stockModel = await _dbContext.Stocks.FirstOrDefaultAsync(x => x.Id == id);
            if (stockModel == null)
            {
                return null;
            }
            stockModel.Symbol = stockDto.Symbol;
            stockModel.CompanyName = stockDto.CompanyName;
            stockModel.Purchase = stockDto.Purchase;
            stockModel.LastDiv = stockDto.LastDiv;
            stockModel.Industry = stockDto.Industry;
            stockModel.MarketCap = stockDto.MarketCap;
            await _dbContext.SaveChangesAsync();
            return stockModel;
        }
    }
}