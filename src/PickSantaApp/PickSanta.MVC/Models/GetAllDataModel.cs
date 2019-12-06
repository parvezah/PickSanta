using System.Collections;
using System.Collections.Generic;
using PickSanta.DB.Entities;

namespace PickSanta.MVC.Models
{
    public class GetAllDataModel
    {
        public IList<User> Users { get; set; }

        public IList<SantaMap> Maps { get; set; }
    }
}