using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Aquarium.Config.Model;

namespace Aquarium
{
    public interface ILightControllService
    {
        void Run();
        void Stop();
        void SetIntensity(int pin, int intensity);
        IEnumerable<LightState> GetLightTable();
    }
}