using System.Collections.Generic;
using System.Threading.Tasks;
using MartialArtsClubManagement.API.DTOs;
using MartialArtsClubManagement.API.Models.Entities;
using MartialArtsClubManagement.API.Models.Config;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace MartialArtsClubManagement.API.Services.Interfaces
{
    public interface ILopHocService
    {
        Task<IEnumerable<LopHocDto>> GetAllAsync();
        Task<LopHocDto?> GetByIdAsync(int id);
        Task<LopHocDto> CreateAsync(LopHocDto dto);
        Task<bool> UpdateAsync(int id, LopHocDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
