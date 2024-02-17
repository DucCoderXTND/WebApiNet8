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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
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