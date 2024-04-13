
using graduationProject.core.DbContext;
using graduationProject.core.Dtos;
using graduationProject.DTOs;
using graduationProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace graduationProject.Services
{
    public class UserProfileService: IUserProfileService
    {
        //private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMailingService _mailingService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly ApplicationDbContext _context;

        public UserProfileService(IAuthService authService, IMailingService mailingService, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory, UserManager<IdentityUser> userManager)
        {

            _authService = authService;
            _mailingService = mailingService;
            _httpContextAccessor = httpContextAccessor;
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
            _userManager = userManager;
            _context = context;
        }


        /*  public async Task UpdatePasswordAsync(string email, UpdatePasswordDto dto)
          {
              var user = await _unitOfWork.UserManager.FindByEmailAsync(email);
              if (user == null)
              {
                  return new ResultDto
                  {
                      IsSuccess = false,
                      Message = "User not found"
                  };
              }
              var result = await _unitOfWork.UserManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
              if (result.Succeeded)
              {
                  await _authService.RevokeToken(user.RefreshTokens.FirstOrDefault(r => r.IsActive).Token);
                  return new ResultDto
                  {
                      IsSuccess = true,
                      Message = "Password updated successfully"
                  };
              }
              return new ResultDto
              {
                  IsSuccess = false,
                  Message = "Password update failed"
              };
          }*/
        public async Task<ResultDto> UpdateEmailAsync(string email, string Newemail)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "User not found"
                };
            }
            if (user.Email == Newemail)
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Email is same as current email"
                };
            }
            if(_userManager.Users.Any(u => u.Email == Newemail))
            {
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Email already exists"
                };
            }

            var token =await _userManager.GenerateChangeEmailTokenAsync(user, Newemail);
            var encodedToken = Encoding.UTF8.GetBytes(token);
            var validToken = Convert.ToBase64String(encodedToken);
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var callBackUrl = urlHelper.Action("ConfirmNewEmail", "Auth", new { Id = user.Id,NewEmail=Newemail,Token = validToken }, _httpContextAccessor.HttpContext.Request.Scheme);
            var message = new MailRequestDto
            {
                MailTo = Newemail,
                Subject = "Confirm Your New Email",
                Content = $"<h1>Welcome to GP</h1> <p>Please confirm your email by <a href='{callBackUrl}'>Clicking here</a></p>",
            };
            try
            {
                var mailResult = await _mailingService.SendEmailAsync(message.MailTo, message.Subject, message.Content);
                return new ResultDto { IsSuccess = true, Message = "Confirmation Email Was Sent, Please confirm your new email" };
            }
            catch
            {
                return new ResultDto { Message = "Confirmation Email Failed to send" };
            }

        }
        public async Task<ResultDto> forgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "User not found"
                };
            Random rnd = new Random();
            var randomNum = (rnd.Next(100000, 999999)).ToString();
            string message = "Hi " + user.UserName + " Your password verification code is: " + randomNum;
            var result = await _mailingService.SendEmailAsync(user.Email, "Password Reset Code ", message, null);
            if (result)
            {
                var Vcode =new verificationCode
                {
                    Code = randomNum,
                    UserId = user.Id,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                };
               
               _context.verificationCodes.AddAsync(Vcode);
               await _context.SaveChangesAsync();
                
                return new ResultDto
                {
                    IsSuccess = true,
                    Message = "Verify code was sent to the email successfully !!",
                };
            }
            return new ResultDto
            {
                IsSuccess = false,
                Message = "Invalid Email"
            };

        }
        public async Task<ResultDto> ResetPasswordAsync(resetPasswordDto model)
        {

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user,model.Token, model.Password);
                if (result.Succeeded)
                {
                    return new ResultDto
                    {
                        IsSuccess = true,
                        Message = "Password Changed Successfully"
                    };
                }
                return new ResultDto
                {
                    IsSuccess = false,
                    Message = "Something wrong, password was not changed"
                };

            }
            return new ResultDto
            {
                IsSuccess = false,
                Message = "Email is incorrect or not found"
            };


        }
       
        public async Task<ResetTokenDto> VerifyCodeAsync(verifyCodeDto codeDto)
        {
            var user = await _userManager.FindByEmailAsync(codeDto.Email);
            if (user == null)
            {
                return new ResetTokenDto
                {
                    IsSuccess = false,
                    Message = "Email is incorrect or not found"
                };
            };
            var result = await _context.verificationCodes.FirstOrDefaultAsync(c=> c.UserId==user.Id && c.Code==codeDto.Code);/*(c => c.UserId == user.Id && c.Code == codeDto.Code);*/


            if (result != null && !result.IsEpired)
            {
               _context.verificationCodes.Remove(result);
                await _context.SaveChangesAsync();

                var restToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                return new ResetTokenDto
                {
                    IsSuccess = true,
                    Message = "Code Was Verified Successfully",
                    Token = restToken,
                    Email = user.Email
                };
            }
            return new ResetTokenDto
            {
                IsSuccess = false,
                Message = "Verification code is not correct"
            };
            }




   }
}
