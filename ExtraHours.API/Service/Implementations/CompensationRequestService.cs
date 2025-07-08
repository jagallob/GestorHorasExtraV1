using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ExtraHours.API.Data;
using ExtraHours.API.Model;
using ExtraHours.API.Service.Interface;
using System;
using System.Linq;

namespace ExtraHours.API.Service.Implementations
{
    public class CompensationRequestService : ICompensationRequestService
    {
        private readonly AppDbContext _context;
        public CompensationRequestService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CompensationRequest> CreateAsync(CompensationRequest request)
        {
            request.RequestedAt = DateTime.UtcNow;
            request.Status = "Pending";
            _context.compensationRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<IEnumerable<CompensationRequest>> GetAllAsync()
        {
            return await _context.compensationRequests
                .Include(cr => cr.Employee)
                .Include(cr => cr.ApprovedBy)
                .ToListAsync();
        }

        public async Task<CompensationRequest?> GetByIdAsync(int id)
        {
            return await _context.compensationRequests
                .Include(cr => cr.Employee)
                .Include(cr => cr.ApprovedBy)
                .FirstOrDefaultAsync(cr => cr.Id == id);
        }

        public async Task<IEnumerable<CompensationRequest>> GetByEmployeeIdAsync(long employeeId)
        {
            return await _context.compensationRequests
                .Where(cr => cr.EmployeeId == employeeId)
                .Include(cr => cr.Employee)
                .Include(cr => cr.ApprovedBy)
                .ToListAsync();
        }

        public async Task<CompensationRequest?> UpdateStatusAsync(int id, string status, string? justification, long? approvedById)
        {
            var request = await _context.compensationRequests.FindAsync(id);
            if (request == null) return null;
            request.Status = status;
            request.Justification = justification;
            request.ApprovedById = approvedById;
            request.DecidedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var request = await _context.compensationRequests.FindAsync(id);
            if (request == null) return false;
            _context.compensationRequests.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
