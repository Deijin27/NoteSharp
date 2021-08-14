using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Notes.Services
{
    public interface IPermissionService
    {
        Task<PermissionStatus> CheckAndRequestStorageReadPermission();
        Task<PermissionStatus> CheckAndRequestStorageWritePermission();
    }
}