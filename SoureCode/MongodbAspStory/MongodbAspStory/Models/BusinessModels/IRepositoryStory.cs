using MongodbAspStory.Models.DataModels;
using MongodbAspStory.Models.ViewModels;

namespace MongodbAspStory.Models.BusinessModels
{
    public interface IRepositoryStory: IRepositoryGeneric<Story, int>
    {
        List<StoryViewModel> GetStoryFull();
		StoryViewModel GetStoryFullById(int key);
	}
}
