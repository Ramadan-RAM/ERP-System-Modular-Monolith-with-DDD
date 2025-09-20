using AutoMapper;
using ERPSys.SharedKernel.Persistence;
using HR.Application.DTOs.HR;
using HR.Application.HR.Interfaces;
using HR.Domain.HR.Entities;
using HR.Infrastructure.HR.DbContext;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.HR.Services
{
    public class JobTitleService : IJobTitleService
    {
        private readonly HRDbContext _context;
        private readonly IMapper _mapper;

        public JobTitleService(HRDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<List<JobTitleDTO>>> GetAllAsync()
        {
            var entities = await _context.JobTitles.ToListAsync();
            var dtos = _mapper.Map<List<JobTitleDTO>>(entities);
            return Result<List<JobTitleDTO>>.Success(dtos);
        }

        public async Task<Result<JobTitleDTO?>> GetByIdAsync(int id)
        {
            var entity = await _context.JobTitles.FindAsync(id);
            var dto = entity == null ? null : _mapper.Map<JobTitleDTO>(entity);
            return Result<JobTitleDTO?>.Success(dto);
        }

        public async Task<Result<JobTitleDTO>> CreateAsync(JobTitleDTO dto)
        {
            var entity = _mapper.Map<JobTitle>(dto);
            await _context.JobTitles.AddAsync(entity);
            await _context.SaveChangesAsync();
            var resultDto = _mapper.Map<JobTitleDTO>(entity);
            return Result<JobTitleDTO>.Success(resultDto);
        }

        public async Task<Result<bool>> UpdateAsync(int id, JobTitleDTO dto)
        {
            var existing = await _context.JobTitles.FindAsync(id);
            if (existing == null) return Result<bool>.Failure("JobTitle not found.");
            _mapper.Map(dto, existing);
            _context.JobTitles.Update(existing);
            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            var entity = await _context.JobTitles.FindAsync(id);
            if (entity == null) return Result<bool>.Failure("JobTitle not found.");
            _context.JobTitles.Remove(entity);
            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
    }

}
