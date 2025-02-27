﻿using AgenceImmobiliareApi.Data;

namespace AgenceImmobiliareApi.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepo CategoryRepo { get; }
        IRealEstateRepo RealEstateRepo { get; }
        IBlogArticleRepo BlogArticleRepo { get; }
        IImageRepo ImageRepo { get; }
        IUserContactRepo UserContactRepo { get; }
        IAddresseRepo AddresseRepo { get; }
        AppDbContext AppDbContext();

        Task Save();
    }
}
