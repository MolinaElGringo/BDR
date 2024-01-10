using BlazinRoleGame.Datads;

namespace BlazinRoleGame.Data
{

    public class UserService : ServiceBase<User>
    {
        public UserService(IConfiguration configuration) : base(configuration)
        {}
    }
    public class PersonneService : ServiceBase<Personne>
    {
        public PersonneService(IConfiguration configuration) : base(configuration)
        {}
    }

    public class MangaServices : ServiceBase<Manga>
    {
        public MangaServices(IConfiguration configuration) : base(configuration)
        {}
    }
}