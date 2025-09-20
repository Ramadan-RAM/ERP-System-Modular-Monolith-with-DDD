using AutoMapper;
using ERPSys.SharedKernel.Persistence;
using HR.Application.DTOs.HR;
using HR.Application.HR.Interfaces;
using HR.Domain.HR.Entities;
using HR.Infrastructure.HR.DbContext;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.HR.Services;
public class FingerprintDeviceService : IFingerprintDeviceService
{
    private readonly HRDbContext _context;
    private readonly IMapper _mapper;

    public FingerprintDeviceService(HRDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<FingerprintDeviceDTO>>> GetAllAsync()
    {
        try
        {
            var devices = await _context.FingerprintDevices
                .Include(d => d.Branch)
                .ToListAsync();

            var dtos = _mapper.Map<List<FingerprintDeviceDTO>>(devices);
            return Result<List<FingerprintDeviceDTO>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<FingerprintDeviceDTO>>.Failure(ex.Message);
        }
    }

    public async Task<Result<FingerprintDeviceDTO?>> GetByIdAsync(int id)
    {
        try
        {
            var device = await _context.FingerprintDevices
                .Include(d => d.Branch)
                .FirstOrDefaultAsync(d => d.Id == id);

            var dto = device == null ? null : _mapper.Map<FingerprintDeviceDTO>(device);
            return Result<FingerprintDeviceDTO?>.Success(dto);
        }
        catch (Exception ex)
        {
            return Result<FingerprintDeviceDTO?>.Failure(ex.Message);
        }
    }

    public async Task<Result<FingerprintDeviceDTO>> CreateAsync(FingerprintDeviceDTO dto)
    {
        try
        {
            var device = _mapper.Map<FingerprintDevice>(dto);
            _context.FingerprintDevices.Add(device);
            await _context.SaveChangesAsync();

            var createdDevice = await _context.FingerprintDevices
                .Include(d => d.Branch)
                .FirstOrDefaultAsync(d => d.Id == device.Id);

            var resultDto = _mapper.Map<FingerprintDeviceDTO>(createdDevice!);
            return Result<FingerprintDeviceDTO>.Success(resultDto);
        }
        catch (Exception ex)
        {
            return Result<FingerprintDeviceDTO>.Failure(ex.Message);
        }
    }

    public async Task<Result<bool>> UpdateAsync(int id, FingerprintDeviceDTO dto)
    {
        try
        {
            var device = await _context.FingerprintDevices.FindAsync(id);
            if (device == null) return Result<bool>.Success(false);

            device.Name = dto.DeviceName;
            device.IP = dto.IPAddress;
            device.Type = dto.Location;
            device.BranchId = dto.BranchId;
            device.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        try
        {
            var device = await _context.FingerprintDevices.FindAsync(id);
            if (device == null) return Result<bool>.Success(false);

            _context.FingerprintDevices.Remove(device);
            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(ex.Message);
        }
    }

    public async Task<Result<List<FingerprintDeviceDTO>>> GetByBranchIdAsync(int branchId)
    {
        try
        {
            var devices = await _context.FingerprintDevices
                .Include(d => d.Branch)
                .Where(d => d.BranchId == branchId)
                .ToListAsync();

            var dtos = _mapper.Map<List<FingerprintDeviceDTO>>(devices);
            return Result<List<FingerprintDeviceDTO>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<FingerprintDeviceDTO>>.Failure(ex.Message);
        }
    }
}
