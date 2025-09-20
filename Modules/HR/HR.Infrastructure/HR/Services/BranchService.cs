using AutoMapper;
using ERPSys.SharedKernel.Persistence;
using HR.Application.DTOs.HR;
using HR.Application.Interfaces.HR;
using HR.Domain.HR.Entities;
using HR.Infrastructure.HR.DbContext;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.HR.Services
{
    // Infrastructure/HR/Services/BranchService.cs
    public class BranchService : IBranchService
    {
        private readonly HRDbContext _context;
        private readonly IMapper _mapper;

        public BranchService(HRDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<List<BranchDTO>>> GetAllAsync()
        {
            var entities = await _context.Branches.ToListAsync();
            var dtos = _mapper.Map<List<BranchDTO>>(entities);
            return Result<List<BranchDTO>>.Success(dtos);
        }

        public async Task<Result<BranchDTO?>> GetByIdAsync(int id)
        {
            var entity = await _context.Branches.FindAsync(id);
            var dto = entity == null ? null : _mapper.Map<BranchDTO>(entity);
            return Result<BranchDTO?>.Success(dto);
        }

        public async Task<Result<BranchDTO>> CreateAsync(BranchDTO dto)
        {
            var entity = _mapper.Map<Branch>(dto);
            _context.Branches.Add(entity);
            await _context.SaveChangesAsync();
            var createdDto = _mapper.Map<BranchDTO>(entity);
            return Result<BranchDTO>.Success(createdDto);
        }

        public async Task<Result<bool>> UpdateAsync(int id, BranchDTO dto)
        {
            var entity = await _context.Branches.FindAsync(id);
            if (entity == null)
                return Result<bool>.Failure("Branch not found.");

            entity.Name = dto.Name;
            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var entity = await _context.Branches.FindAsync(id);
            if (entity == null)
                return Result<bool>.Failure("Branch not found.");

            _context.Branches.Remove(entity);
            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }

}
