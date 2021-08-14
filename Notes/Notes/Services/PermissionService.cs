using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Notes.Services
{
    public class PermissionService : IPermissionService
    {
        public async Task<PermissionStatus> CheckAndRequestStorageWritePermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            }

            // Additionally could prompt the user to turn on in settings

            return status;
        }

        public async Task<PermissionStatus> CheckAndRequestStorageReadPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageRead>();
            }

            // Additionally could prompt the user to turn on in settings

            return status;
        }
    }
}
