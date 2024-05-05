using graduationProject.DTOs;
using graduationProject.Models;
using Microsoft.Extensions.Hosting;

namespace graduationProject.Mapping
{
    public static class GetOfferedUserDtoMap
    {
        public static async Task<List<GetOfferedUserDto>> MapToGetOfferedUserDtoMap(this IEnumerable<Post> posts)
        {
            var response = new List<GetOfferedUserDto>();

            foreach (var post in posts)
            {
                var dto = new GetOfferedUserDto()
                {
                    Id = post.Id,
                    Content = post.Content,
                    Rrice = post.offers.FirstOrDefault()?.Rrice ??0, // handle if offers are null or empty
                    Description = post.offers.FirstOrDefault()?.Description,
                    NationalId = post.offers.FirstOrDefault()?.NationalId,
                    ProfitRate = post.offers.FirstOrDefault()?.ProfitRate ?? 0 // handle if offers are null or empty
                };
                response.Add(dto);
            }

            return response;
        }
    }
    }

