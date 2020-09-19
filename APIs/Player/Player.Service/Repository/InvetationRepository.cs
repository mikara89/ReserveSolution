using Player.Data.Models.Entites;
using Player.Data.Persistence;

namespace Player.Service.Repository
{
    public class InvetationRepository : RepositoryBase<InvetationEntity>, IInvetationRepository
    {
        public InvetationRepository(PlayerDBContext repositoryContext)
            : base(repositoryContext) 
        {
        }
    }
}
