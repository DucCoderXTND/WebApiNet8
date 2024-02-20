using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Extensions;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/portfolio")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepo;
        private readonly IPortfolioRepository _portfolioRepo;
        public PortfolioController(UserManager<AppUser> userManager, IStockRepository stockRepo, IPortfolioRepository portfolioRepo)
        {
            _stockRepo = stockRepo;
            _userManager = userManager;
            _portfolioRepo = portfolioRepo;
        }

        [HttpGet("Get stock by user")]
        [Authorize]
        public async Task<IActionResult> GetUserPortfolio()
        {
            var username = User.GetUserName();
            var appUser = await _userManager.FindByNameAsync(username);
            var userPortfolio = await _portfolioRepo.GetUserPortfolio(appUser);
            // var x= userPortfolio.Select(y=>y.ToStockDto());
            return Ok(userPortfolio);
        }
        [HttpPost("Post portfolio")]
        [Authorize]
        public async Task<IActionResult> Create(string stockSymbol)
        {
            var userName = User.GetUserName();
            var appUser = await _userManager.FindByNameAsync(userName);
            var stock = await _stockRepo.StockSymbolExist(stockSymbol);
            if (stock == null) return BadRequest("Stock not found");
            var userPortfolio = await _portfolioRepo.GetUserPortfolio(appUser);
            if (userPortfolio.Any(s => s.Symbol.ToLower() == stockSymbol.ToLower()))
                return BadRequest("Cannot add same symbol stock to portfolio");
            var portfolioModel = new Portfolio
            {
                AppUserId = appUser.Id,
                StockId = stock.Id
            };
            var newPortfolio = await _portfolioRepo.CreateAsync(portfolioModel);
            if (newPortfolio == null)
            {
                return StatusCode(500, "Could not create");
            }
            else
            {
                return Created();
            }
        }

        [HttpDelete("Delete portfolio")]
        [Authorize]
        public async Task<IActionResult> Delete(string stockSymbol)
        {
            var userName = User.GetUserName();
            var appUser = await _userManager.FindByNameAsync(userName);
            var userPortfolio = await _portfolioRepo.GetUserPortfolio(appUser);
            var filteredStock = userPortfolio.Where(s => s.Symbol.ToLower() == stockSymbol.ToLower()).ToList();
            if (filteredStock.Count == 1)
            {
                await _portfolioRepo.DeleteAsync(appUser, stockSymbol);
            }
            else
            {
                return BadRequest("Stock is not in your portfolio");
            }
            return Ok();
        }

    }
}