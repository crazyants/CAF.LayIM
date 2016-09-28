using CAF.IM.Core.Domain;

namespace CAF.IM.Services
{
    public interface ISettingsManager
    {
        ApplicationSettings Load();
        void Save(ApplicationSettings settings);
    }
}
