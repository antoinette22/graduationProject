

using graduationProject.core.Dtos;
using graduationProject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graduationProject.Services
{
    public interface IUserProfileService
    {
        //  Task<ResultDto> UpdateNameAsync(string email, UpdateNameDto dto);
        Task<ResultDto> UpdateEmailAsync(string userName, string newEmail);
        //  Task UpdatePasswordAsync(string email, UpdatePasswordDto dto);
        Task<ResultDto> forgetPasswordAsync(string email);
        Task<ResultDto> ResetPasswordAsync(resetPasswordDto model);
        Task<ResetTokenDto> VerifyCodeAsync(verifyCodeDto codeDto);



    }
}
