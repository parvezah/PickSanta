using Microsoft.Azure.Cosmos.Table;
using PickSanta.DB.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PickSanta.DB
{
    public class TableAccess : IDataAccess, IDisposable
    {
        private CloudTable table;

        public TableAccess(string tableName, string connectionString)
        {
            this.table = Common.CreateTable(tableName, connectionString);
        }

        private T InsertEntity<T>(T entity) where T : TableEntity
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);

                // Execute the operation.
                TableResult result = table.ExecuteAsync(insertOrMergeOperation).Result;
                T insertedEntity = result.Result as T;

                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of InsertOrMerge Operation: " + result.RequestCharge);
                }

                return insertedEntity;
            }
            catch (StorageException e)
            {
                // TODO- Fix logging
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        private void DeleteEntity<T>(T deleteEntity) where T : TableEntity
        {
            try
            {
                if (deleteEntity == null)
                {
                    throw new ArgumentNullException("deleteEntity");
                }

                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
                TableResult result = table.ExecuteAsync(deleteOperation).Result;

                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of Delete Operation: " + result.RequestCharge);
                }

            }
            catch (StorageException e)
            {
                // TODO- Fix Logging
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        private T RetrieveEntityUsingPointQuery<T>(string partitionKey, string rowKey) where T : TableEntity, new()
        {
            try
            {
                TableOperation retrieveOperation = TableOperation.Retrieve<T>(partitionKey.ToLower(), rowKey.ToLower());
                TableResult result = table.ExecuteAsync(retrieveOperation).Result;
                T entity = result.Result as T;
                if (entity != null)
                {
                    Console.WriteLine($"{entity.ToString()}");
                }

                if (result.RequestCharge.HasValue)
                {
                    Console.WriteLine("Request Charge of Retrieve Operation: " + result.RequestCharge);
                }

                return entity;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        private IList<T> PartitionScan<T>(string partitionKey) where T : TableEntity, new()
        {
            var result = new List<T>();
            try
            {
                TableQuery<T> partitionScanQuery =
                    new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey.ToLower()));

                TableContinuationToken token = null;

                // Read entities from each query segment.
                do
                {
                    TableQuerySegment<T> segment = table.ExecuteQuerySegmentedAsync(partitionScanQuery, token).Result;
                    token = segment.ContinuationToken;
                    foreach (var entity in segment)
                    {
                        //var typeCastedEntity = entity as T;

                        Console.WriteLine($"Entity: {entity?.ToString()}");

                        //if(typeCastedEntity == null)
                        //{
                        //    throw new InvalidDataException($"Got an object which is of not type as expected {nameof(T)}");
                        //}

                        result.Add(entity);
                    }
                }
                while (token != null);
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }

            return result;
        }

        public void CreateSantaMap(string giftee, string santa)
        {
            var santaUser = this.GetUser(santa);
            var gifteeUser = this.GetUser(giftee);

            if(santaUser == null || gifteeUser == null)
            {
                throw new InvalidDataException($"One of the given values is null. {nameof(gifteeUser)}:{gifteeUser}, {nameof(santaUser)}:{santaUser}");
            }

            var santaMap = new SantaMap(giftee, santa);
            this.InsertEntity(santaMap);
        }

        public void CreateUser(string name, string email)
        {
            var user = new User(name, email, false);
            this.InsertEntity(user);
        }

        public IList<SantaMap> GetAllSantaMaps()
        {
            return this.PartitionScan<SantaMap>(SantaMap.PK);
        }

        public IList<User> GetAllUsers()
        {
            return this.PartitionScan<User>(User.PK);
        }

        public SantaMap GetGifteeForSanta(string santa)
        {
            return this.RetrieveEntityUsingPointQuery<SantaMap>(SantaMap.PK, santa.ToLower());
        }

        public User GetUser(string email)
        {
            return this.RetrieveEntityUsingPointQuery<User>(User.PK, email.ToLower());
        }

        public void RemoveUser(string email)
        {
            var user = this.GetUser(email);

            if(user != null)
            {
                this.DeleteEntity(user);
            }
        }

        public void ResetSantaMap(string santa)
        {
            var santaMap = this.GetGifteeForSanta(santa);

            if (santaMap != null)
            {
                this.DeleteEntity(santaMap);
            }
        }

        public void Dispose()
        {
            _ = table.DeleteIfExistsAsync().Result;
        }
    }
}
