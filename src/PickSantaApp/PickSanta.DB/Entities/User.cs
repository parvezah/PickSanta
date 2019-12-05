using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace PickSanta.DB.Entities
{
    public class User : TableEntity
    {
        internal static string PK = "User";
        public User() 
        {
            this.PartitionKey = PK.ToLower();
        }

        public User(string name, string email, bool hasRolled)  : this()
        {
            this.RowKey = email.ToLower();
            this.Email = email;
            this.Name = name;
            this.HasRolled = hasRolled;
            //this.Id = Guid.NewGuid().ToString();
        }
        
        public string Name { get; set; }

        public string Email { get; set; }

        public bool HasRolled { get; set; }

        //public string Id { get; set; }
    }
}
