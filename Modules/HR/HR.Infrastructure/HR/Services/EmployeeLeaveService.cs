// File: Infrastructure/HR/Services/EmployeeLeaveService.cs
using AutoMapper;
using ERPSys.SharedKernel.Persistence;
using HR.Application.DTOs.HR;
using HR.Application.HR.Interfaces;
using HR.Domain.HR.Entities;
using HR.Infrastructure.HR.DbContext;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.HR.Services
{
    public class EmployeeLeaveService : IEmployeeLeaveService
    {
        private readonly HRDbContext _context;
        private readonly IMapper _mapper;

        public EmployeeLeaveService(HRDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<List<EmployeeLeaveDTO>>> GetAllAsync()
        {
            var leaves = await _context.Leaves.Include(e => e.Employee).ToListAsync();
            var dtos = leaves.Select(l =>
            {
                var dto = _mapper.Map<EmployeeLeaveDTO>(l);
                dto.EmployeeName = $"{l.Employee?.FirstName} {l.Employee?.LastName}";
                return dto;
            }).ToList();
            return Result<List<EmployeeLeaveDTO>>.Success(dtos);
        }

        public async Task<Result<EmployeeLeaveDTO?>> GetByIdAsync(int id)
        {
            var leave = await _context.Leaves.Include(e => e.Employee).FirstOrDefaultAsync(e => e.Id == id);
            if (leave == null) return Result<EmployeeLeaveDTO?>.Success(null);
            var dto = _mapper.Map<EmployeeLeaveDTO>(leave);
            dto.EmployeeName = $"{leave.Employee?.FirstName} {leave.Employee?.LastName}";
            return Result<EmployeeLeaveDTO?>.Success(dto);
        }

        public async Task<Result<EmployeeLeaveDTO>> CreateAsync(EmployeeLeaveDTO dto)
        {
            var entity = _mapper.Map<EmployeeLeave>(dto);
            await _context.Leaves.AddAsync(entity);
            await _context.SaveChangesAsync();
            var resultDto = _mapper.Map<EmployeeLeaveDTO>(entity);
            return Result<EmployeeLeaveDTO>.Success(resultDto);
        }

        public async Task<Result<bool>> UpdateAsync(int id, EmployeeLeaveDTO dto)
        {
            var existing = await _context.Leaves.FindAsync(id);
            if (existing == null) return Result<bool>.Success(false);
            _mapper.Map(dto, existing);
            _context.Leaves.Update(existing);
            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var entity = await _context.Leaves.FindAsync(id);
            if (entity == null) return Result<bool>.Success(false);
            _context.Leaves.Remove(entity);
            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }
}
