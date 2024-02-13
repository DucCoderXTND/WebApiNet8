using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Comment;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CommentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Comment> CreateAsync(Comment comment)
        {
            await _dbContext.Comments.AddAsync(comment);
            await _dbContext.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment?> DeleteAsync(int id)
        {
            var commentExists = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (commentExists == null)
            {
                return null;
            }
            _dbContext.Comments.Remove(commentExists);
            await _dbContext.SaveChangesAsync();
            return commentExists;
        }

        public async Task<List<Comment>> GetAllAsync()
        {
            return await _dbContext.Comments.ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            var comment = await _dbContext.Comments.FindAsync(id);
            if (comment == null)
            {
                return null;
            }
            return comment;
        }

        public async Task<Comment?> UpdateAsync(int id, UpdateCommentRequestDto commentDto)
        {
            var commentExists = await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id);
            if (commentExists == null)
            {
                return null;
            }
            commentExists.Title = commentDto.Title;
            commentExists.Content = commentDto.Content;
            await _dbContext.SaveChangesAsync();
            return commentExists;

        }
    }
}