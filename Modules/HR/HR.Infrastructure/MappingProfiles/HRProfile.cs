using AutoMapper;
using HR.Domain.HR.Entities;
using HR.Application.DTOs.HR;

namespace HR.Infrastructure.MappingProfiles
{
    public class HRProfile : Profile
    {
        public HRProfile()
        {
            // ✅ From Entity → DTO
            CreateMap<Employee, EmployeeDTO>()
                .ForMember(dest => dest.FullName,
                           opt => opt.MapFrom(src => $"{src.FirstName} {src.MiddelName} {src.LastName}"))
                .ForMember(dest => dest.Department,
                           opt => opt.MapFrom(src => src.JobTitle != null && src.JobTitle.Department != null
                                ? src.JobTitle.Department.Name
                                : string.Empty))
                .ForMember(dest => dest.JobTitle,
                           opt => opt.MapFrom(src => src.JobTitle != null ? src.JobTitle.Title : string.Empty))
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.PicturePath))
                .ForMember(dest => dest.CVUrl, opt => opt.MapFrom(src => src.CVPath));

            // ✅ From DTO → Entity (ignore BaseEntity props)
            CreateMap<EmployeeDTO, Employee>()
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.PicturePath, opt => opt.MapFrom(src => src.PictureUrl))
                .ForMember(dest => dest.CVPath, opt => opt.MapFrom(src => src.CVUrl))
                .ForMember(dest => dest.FirstName, opt => opt.Ignore())
                .ForMember(dest => dest.MiddelName, opt => opt.Ignore())
                .ForMember(dest => dest.LastName, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    var names = (src.FullName ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    dest.FirstName = names.ElementAtOrDefault(0) ?? "";
                    dest.MiddelName = names.ElementAtOrDefault(1) ?? "";
                    dest.LastName = names.ElementAtOrDefault(2) ?? "";
                });

            // ✅ Department & JobTitle
            CreateMap<Department, DepartmentDTO>().ReverseMap();

            CreateMap<JobTitle, JobTitleDTO>()
                .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
                .ForMember(dest => dest.DepartmentName,
                           opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : string.Empty))
                .ReverseMap();
                //.ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                //.ForMember(dest => dest.UpdatedOn, opt => opt.Ignore());

            // ✅ Payslip
            CreateMap<Payslip, PayslipDTO>()
                .ForMember(dest => dest.EmployeeName,
                           opt => opt.MapFrom(src => $"{src.Employee.FirstName} {src.Employee.MiddelName} {src.Employee.LastName}"));
            CreateMap<PayslipDTO, Payslip>()
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore());

            // ✅ EmployeeLeave
            CreateMap<EmployeeLeave, EmployeeLeaveDTO>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => $"{src.Employee.FirstName} {src.Employee.MiddelName} {src.Employee.LastName}"))
                .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => src.LeaveType.ToString()));

            CreateMap<EmployeeLeaveDTO, EmployeeLeave>()
                .ForMember(dest => dest.LeaveType, opt => opt.MapFrom(src => Enum.Parse<LeaveType>(src.LeaveType)));
                //.ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                //.ForMember(dest => dest.UpdatedOn, opt => opt.Ignore());

            // ✅ AttendanceRecord
            CreateMap<AttendanceRecord, AttendanceRecordDTO>()
                .ForMember(dest => dest.EmployeeFullName,
                           opt => opt.MapFrom(src => src.Employee != null ? $"{src.Employee.FirstName} {src.Employee.MiddelName} {src.Employee.LastName}" : ""));

            // ✅ Branch
            CreateMap<Branch, BranchDTO>().ReverseMap()
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore());

            // ✅ FingerprintDevice
            CreateMap<FingerprintDevice, FingerprintDeviceDTO>()
                .ForMember(dest => dest.DeviceName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.IPAddress, opt => opt.MapFrom(src => src.IP))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch != null ? src.Branch.Name : string.Empty));

            CreateMap<FingerprintDeviceDTO, FingerprintDevice>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DeviceName))
                .ForMember(dest => dest.IP, opt => opt.MapFrom(src => src.IPAddress))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.CreatedOn, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedOn, opt => opt.Ignore());
        }
    }
}
