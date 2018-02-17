using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Aquarium.Settings;

namespace Aquarium
{
    public interface ILightManager
    {
        void Run();
        void Stop();
        void SetIntensity(int pin, int intensity);
        IEnumerable<LightState> GetLightTable();
    }
}