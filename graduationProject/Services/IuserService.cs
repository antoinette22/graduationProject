using graduationProject.DTOs;

namespace graduationProject.Services
{
    public interface IuserService
    {
        Task<searchDto> SearchUserProfile(string userName);
    }
}
