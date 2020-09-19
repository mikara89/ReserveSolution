using Player.Data.Models.Entites;
using Player.Data.Persistence;

namespace Player.Service.Repository
{
    public class PlayerRepository : RepositoryBase<PlayerEntity>, IPlayerRepository
    {
        public PlayerRepository(PlayerDBContext repositoryContext)
            : base(repositoryContext) 
        {
        }
    }
}
