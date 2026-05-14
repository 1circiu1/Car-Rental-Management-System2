using CarRental.Backend.Data;
using CarRental.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarRental.Backend.Services
{
    public class SupportTicketService
    {
        private readonly AppDbContext _context;

        public SupportTicketService(AppDbContext context)
        {
            _context = context;
        }

        public void CreateTicket(int userId, string subject, string category, string message)
        {
            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(message))
                return;

            var ticket = new SupportTicket
            {
                UserId = userId,
                Subject = subject,
                Category = category,
                Message = message,
                Status = "Open",
                CreatedAt = DateTime.Now
            };

            _context.SupportTickets.Add(ticket);
            _context.SaveChanges();
        }

        public List<SupportTicket> GetUserTickets(int userId)
        {
            return _context.SupportTickets
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToList();
        }

        public List<SupportTicket> GetRecentUserTickets(int userId, int count = 5)
        {
            return _context.SupportTickets
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Take(count)
                .ToList();
        }

        public int GetOpenTicketCount(int userId)
        {
            return _context.SupportTickets
                .Count(t => t.UserId == userId && t.Status == "Open");
        }
    }
}