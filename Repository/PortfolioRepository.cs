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
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDbContext _context;
        public PortfolioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Portfolio> CreateAsync(Portfolio newPortfolio)
        {
            await _context.Portfolios.AddAsync(newPortfolio);
            await _context.SaveChangesAsync();
            return newPortfolio;
        }

        public async Task<Portfolio?> DeleteAsync(AppUser appUser, string symbol)
        {
            var portfolioModel = await _context.Portfolios
                                .FirstOrDefaultAsync(s => s.AppUserId == appUser.Id && s.Stock.Symbol.ToLower() == symbol.ToLower());
            if(portfolioModel == null){
                return null;
            }
            _context.Portfolios.Remove(portfolioModel);
            await _context.SaveChangesAsync();
            return portfolioModel;
        }

        public async Task<List<Stock>> GetUserPortfolio(AppUser appUser)
        {
            return await _context.Portfolios
            .Where(p => p.AppUserId == appUser.Id)
            .Select(stock => new Stock
            {
                Id = stock.StockId,
                Symbol = stock.Stock.Symbol,
                CompanyName = stock.Stock.CompanyName,
                Purchase = stock.Stock.Purchase,
                LastDiv = stock.Stock.LastDiv,
                Industry = stock.Stock.Industry,
                MarketCap = stock.Stock.MarketCap
            }).ToListAsync();
        }
    }
}