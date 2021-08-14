using System.Threading.Tasks;

namespace Notes.Services
{
    public interface IBackupService
    {
        Task<bool> GetPermissionAndCreateBackup();
        Task GetPermissionAndRestoreBackup();
    }
}