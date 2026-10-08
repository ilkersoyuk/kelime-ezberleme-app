using System.Threading.Tasks;
using KelimeEzberApp.Models;

namespace KelimeEzberApp.Services;

public interface IStorageService
{
    Task<UserProfile> LoadProfileAsync();
    Task SaveProfileAsync(UserProfile profile);
    Task ExportBackupAsync(UserProfile profile);
    Task<UserProfile?> ImportBackupAsync(string jsonContent);
    Task ResetDataAsync();
}
