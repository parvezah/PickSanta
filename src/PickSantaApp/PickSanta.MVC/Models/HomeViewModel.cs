using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PickSanta.DB.Entities;

namespace PickSanta.MVC.Models
{
    public class HomeViewModel
    {
        public User User { get; set; }

        public SantaMap Map { get; set; }
    }
}
