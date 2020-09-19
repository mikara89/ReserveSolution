using Player.Data.Models.Entites;
using Player.Service.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Player.Service
{
    public class SeedInvitations
    {
        private readonly IInvetationRepository repository;
        public SeedInvitations(IInvetationRepository repository)
        {
            this.repository = repository;
        }

        public void Seed()
        {
            repository.Create(new InvetationEntity
            {
                Expiration = DateTime.Now + TimeSpan.FromMinutes(30),
                Id = "30dd879c-ee2f-11db-8314-0800200c9a66",
                IsCanceled = false,
                IsUsed = false,
                TeamId = Guid.NewGuid().ToString()
            });

            repository.SaveAsync();
        }
    }
}
