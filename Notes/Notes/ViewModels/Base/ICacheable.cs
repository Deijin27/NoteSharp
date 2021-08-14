using System.Threading.Tasks;

namespace Notes.ViewModels.Base
{
    interface ICacheable
    {
        Task Cache();
    }
}
