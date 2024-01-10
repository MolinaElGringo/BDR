
public interface ISupportCRUD<Titem>
{
    Task<List<Titem>> GetAllAsync();
    Task<List<Titem>> GetById(Titem entity);
    Task<List<Titem>> AddAsync(Titem entity);
    Task<List<Titem>> DeleteAsync(Titem entity);
    Task<List<Titem>> UpdateAsync(Titem entity);

}

