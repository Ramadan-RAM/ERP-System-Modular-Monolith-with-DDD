using AutoMapper;
using ERPSys.SharedKernel.Persistence;
using HR.Application.DTOs.HR;
using HR.Application.HR.Interfaces;
using HR.Application.Interfaces.HR;
using HR.Infrastructure.HR.DbContext;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.HR.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly HRDbContext _context;
        private readonly IMapper _mapper;

        public DepartmentService(HRDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<List<DepartmentDTO>>> GetAllAsync()
        {
            var entities = await _context.Departments.ToListAsync();
            var dtos = _mapper.Map<List<DepartmentDTO>>(entities);
            return Result<List<DepartmentDTO>>.Success(dtos);
        }

        public async Task<Result<DepartmentDTO?>> GetByIdAsync(int id)
        {
            var entity = await _context.Departments.FindAsync(id);
            var dto = entity == null ? null : _mapper.Map<DepartmentDTO>(entity);
            return Result<DepartmentDTO?>.Success(dto);
        }

        public async Task<Result<DepartmentDTO>> CreateAsync(DepartmentDTO dto)
        {
            var entity = _mapper.Map<Department>(dto);
            await _context.Departments.AddAsync(entity);
            await _context.SaveChangesAsync();
            var createdDto = _mapper.Map<DepartmentDTO>(entity);
            return Result<DepartmentDTO>.Success(createdDto);
        }

        public async Task<Result<bool>> UpdateAsync(int id, DepartmentDTO dto)
        {
            var existing = await _context.Departments.FindAsync(id);
            if (existing == null) return Result<bool>.Success(false);
            _mapper.Map(dto, existing);
            _context.Departments.Update(existing);
            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var entity = await _context.Departments.FindAsync(id);
            if (entity == null) return Result<bool>.Success(false);
            _context.Departments.Remove(entity);
            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }

}

