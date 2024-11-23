
namespace MongodbAspStory.Models.BusinessModels
{
    public interface IRepositoryGeneric<T,K>
    {
        List<T> GetAll();
        T GetById(K key);
        bool Insert(T entity);
        bool Update(T entity);
        bool Delete(K key);
		List<T> Paging(int page, int pageSize, out long totalPage);
		List<T> SearchPaging(string name, int page, int pageSize, out long totalPage);
	}
}
