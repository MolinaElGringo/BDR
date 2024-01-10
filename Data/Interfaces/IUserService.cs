using BlazinRoleGame.Data;

public interface IUserService{
    public Task<User[]> GetUsersAsync();
    public Task<User> AddUserAsync(User user);
    public Task<bool> DeleteUserAsync(User user);
    public Task<User> UpdateUserAsync(User user);
}