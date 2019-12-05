using PickSanta.DB.Entities;
using System;
using System.Collections.Generic;
using User = PickSanta.DB.Entities.User;

namespace PickSanta.DB
{
    public interface IDataAccess : IDisposable
    {
        IList<User> GetAllUsers();

        User GetUser(string email);

        void CreateUser(string name, string email);

        void RemoveUser(string email);

        void ResetSantaMap(string santa);

        IList<SantaMap> GetAllSantaMaps();

        SantaMap GetGifteeForSanta(string santa);

        void CreateSantaMap(string giftee, string santa);
    }
}
