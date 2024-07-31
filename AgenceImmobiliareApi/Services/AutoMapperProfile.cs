using AgenceImmobiliareApi.Models;
using AgenceImmobiliareApi.Models.DTOs;
using AutoMapper;

namespace AgenceImmobiliareApi.Services
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RealEstateCreateDto, RealEstate>();
            CreateMap<RealEstate, RealEstateCreateDto>();

            CreateMap<RealEstateUpdateDto, RealEstate>();
            CreateMap<RealEstate, RealEstateUpdateDto>();

            CreateMap<Category , CategoryCreateDto>(); 
            CreateMap<CategoryCreateDto , Category>();
            CreateMap<Category, CategoryUpdateDto>() ;
            CreateMap<CategoryUpdateDto, Category>();

            CreateMap<BlogArticle, BlogCreateDto>();
            CreateMap<BlogCreateDto, BlogArticle>();
            CreateMap<BlogArticle, BlogUpdateDto>();
            CreateMap<BlogUpdateDto, BlogArticle>();

            CreateMap<UserContact, UserContactCreateDto>();
            CreateMap<UserContactCreateDto, UserContact>();
        }
    }
}
