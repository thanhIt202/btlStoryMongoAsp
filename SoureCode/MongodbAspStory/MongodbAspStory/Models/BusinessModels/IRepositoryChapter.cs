using MongodbAspStory.Models.DataModels;
using MongodbAspStory.Models.ViewModels;

namespace MongodbAspStory.Models.BusinessModels
{
    public interface IRepositoryChapter : IRepositoryGeneric<Chapter, int>
    {
		List<ChapterViewModel> GetChapterFull();
		ChapterViewModel GetChapterFullById(int key);
	}
}
