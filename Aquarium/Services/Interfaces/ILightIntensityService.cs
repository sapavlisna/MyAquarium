using System.Collections.Generic;
using Aquarium.Model;

namespace Aquarium.Services
{
    public interface ILightIntensityService
    {
        int GetValue(int pin);
        List<LightIntensity> GetValues();
        int SaveValues(IEnumerable<LightIntensity> lightIntensities);
        void Run();
        void Stop();
    }
}