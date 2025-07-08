using System.Collections.Generic;
using System.Threading.Tasks;
using ExtraHours.API.Model;

namespace ExtraHours.API.Service.Interface
{
    public interface ICompensationRequestService
    {
        Task<CompensationRequest> CreateAsync(CompensationRequest request);
        Task<IEnumerable<CompensationRequest>> GetAllAsync();
        Task<CompensationRequest?> GetByIdAsync(int id);
        Task<IEnumerable<CompensationRequest>> GetByEmployeeIdAsync(long employeeId);
        Task<CompensationRequest?> UpdateStatusAsync(int id, string status, string? justification, long? approvedById);
        Task<bool> DeleteAsync(int id);
    }
}
