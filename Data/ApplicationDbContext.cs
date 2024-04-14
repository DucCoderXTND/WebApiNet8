using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Portfolio> Portfolios { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Portfolio>(x => x.HasKey(p => new { p.AppUserId, p.StockId }));
            builder.Entity<Portfolio>()
            .HasOne(u=>u.AppUser)
            .WithMany(u=>u.Portfolios)
            .HasForeignKey(p=>p.AppUserId);

            builder.Entity<Portfolio>()
            .HasOne(s=>s.Stock)
            .WithMany(s=>s.Portfolios)
            .HasForeignKey(p=>p.StockId);


            //MĐ tạo và đưa dữ liệu về các quyền (roles) vào cơ sở dữ liệu bằng cách sử dụng Entity Framework Core.
            List<IdentityRole> roles = new List<IdentityRole>()
            {
                new IdentityRole() {Name = "Admin", NormalizedName="ADMIN"},
                new IdentityRole() {Name = "User", NormalizedName="USER"}
            };
            //MĐ thêm các quyền đã khởi tạo trong roles vào bảng IdentityRole in csdl
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}